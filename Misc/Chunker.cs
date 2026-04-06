using System.Text;
using System.Text.RegularExpressions;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;

namespace ManualChunkingRagDemo;

public enum ChunkingStrategy
{
    FixedSize,
    SentenceWindow,
    HeadingAware
}

public sealed record TextChunk(
    string Id,
    string Text,
    string? Section,
    int Start,
    int End);

public static class Program
{
    public static async Task Main()
    {
        // ---------------------------------------------------------------------
        // 1) Load a document
        // ---------------------------------------------------------------------
        // Keep the demo simple: use a .txt or markdown-exported file.
        // Replace with your own loader if you want to read PDF, DOCX, etc.
        var filePath = "sample-doc.md";
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return;
        }

        string documentText = await File.ReadAllTextAsync(filePath);

        // ---------------------------------------------------------------------
        // 2) Pick a chunking strategy
        // ---------------------------------------------------------------------
        ChunkingStrategy strategy = ChunkingStrategy.SentenceWindow;

        List<TextChunk> chunks = strategy switch
        {
            ChunkingStrategy.FixedSize =>
                Chunkers.FixedSize(documentText, chunkSize: 800, overlap: 120),

            ChunkingStrategy.SentenceWindow =>
                Chunkers.SentenceWindow(documentText, sentencesPerChunk: 5, overlapSentences: 2),

            ChunkingStrategy.HeadingAware =>
                Chunkers.HeadingAwareMarkdown(documentText, maxSectionChunkSize: 1200),

            _ => throw new NotSupportedException($"Unknown strategy: {strategy}")
        };

        Console.WriteLine($"Strategy: {strategy}");
        Console.WriteLine($"Chunk count: {chunks.Count}");
        Console.WriteLine();

        // Show the first few chunks so people can SEE the chunking result.
        foreach (var chunk in chunks.Take(3))
        {
            Console.WriteLine($"--- {chunk.Id} | Section: {chunk.Section ?? "(none)"} ---");
            Console.WriteLine(chunk.Text);
            Console.WriteLine();
        }

        // ---------------------------------------------------------------------
        // 3) Ask a question
        // ---------------------------------------------------------------------
        string question = "What are the main security recommendations in this document?";

        // ---------------------------------------------------------------------
        // 4) Retrieve top chunks (very simple scoring for demo purposes)
        // ---------------------------------------------------------------------
        var topChunks = Retriever.GetTopChunks(question, chunks, topK: 4);

        Console.WriteLine("Top retrieved chunks:");
        foreach (var hit in topChunks)
        {
            Console.WriteLine($"  {hit.chunk.Id} | score={hit.score:F3} | section={hit.chunk.Section ?? "(none)"}");
        }

        Console.WriteLine();

        // ---------------------------------------------------------------------
        // 5) Build a grounded prompt manually
        // ---------------------------------------------------------------------
        string groundedPrompt = PromptBuilder.BuildGroundedPrompt(question, topChunks.Select(x => x.chunk));

        // ---------------------------------------------------------------------
        // 6) Create a Microsoft Agent Framework agent and ask it to answer
        // ---------------------------------------------------------------------
        // Env vars:
        //   PROJECT_ENDPOINT = https://<your-project>.services.ai.azure.com/api/projects/<project-name>
        //   MODEL_DEPLOYMENT = your deployment name (for example: gpt-4o-mini)
        string? projectEndpoint = Environment.GetEnvironmentVariable("PROJECT_ENDPOINT");
        string deployment = Environment.GetEnvironmentVariable("MODEL_DEPLOYMENT") ?? "gpt-4o-mini";

        if (string.IsNullOrWhiteSpace(projectEndpoint))
        {
            Console.WriteLine("Set PROJECT_ENDPOINT first.");
            Console.WriteLine("Example: https://<your-project>.services.ai.azure.com/api/projects/<project-name>");
            return;
        }

        var projectClient = new AIProjectClient(new Uri(projectEndpoint), new DefaultAzureCredential());

        AIAgent agent = projectClient.AsAIAgent(
            model: deployment,
            name: "ManualChunkingRagAgent",
            instructions: """
                You are a grounded RAG assistant.
                Answer ONLY from the supplied context.
                If the answer is not in the context, say you do not have enough information.
                When possible, cite the chunk IDs that support your answer.
                """);

        string answer = await agent.RunAsync(groundedPrompt);

        Console.WriteLine("ANSWER");
        Console.WriteLine("======");
        Console.WriteLine(answer);
    }
}

public static class Chunkers
{
    public static List<TextChunk> FixedSize(string text, int chunkSize, int overlap)
    {
        if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
        if (overlap < 0 || overlap >= chunkSize) throw new ArgumentOutOfRangeException(nameof(overlap));

        var chunks = new List<TextChunk>();
        int start = 0;
        int index = 0;

        while (start < text.Length)
        {
            int length = Math.Min(chunkSize, text.Length - start);
            int end = start + length;

            string chunkText = text[start..end].Trim();
            if (!string.IsNullOrWhiteSpace(chunkText))
            {
                chunks.Add(new TextChunk(
                    Id: $"fixed-{index:D4}",
                    Text: chunkText,
                    Section: null,
                    Start: start,
                    End: end));
                index++;
            }

            if (end >= text.Length)
                break;

            start = end - overlap;
        }

        return chunks;
    }

    public static List<TextChunk> SentenceWindow(string text, int sentencesPerChunk, int overlapSentences)
    {
        if (sentencesPerChunk <= 0) throw new ArgumentOutOfRangeException(nameof(sentencesPerChunk));
        if (overlapSentences < 0 || overlapSentences >= sentencesPerChunk) throw new ArgumentOutOfRangeException(nameof(overlapSentences));

        // Very simple sentence split for demo use.
        // For production, use a stronger tokenizer / sentence segmenter.
        var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        var chunks = new List<TextChunk>();
        int step = sentencesPerChunk - overlapSentences;
        int index = 0;
        int runningOffset = 0;

        for (int i = 0; i < sentences.Count; i += step)
        {
            var window = sentences.Skip(i).Take(sentencesPerChunk).ToList();
            if (window.Count == 0) break;

            string chunkText = string.Join(" ", window).Trim();
            if (string.IsNullOrWhiteSpace(chunkText)) continue;

            int start = text.IndexOf(window.First(), runningOffset, StringComparison.Ordinal);
            if (start < 0) start = runningOffset;

            int end = start + chunkText.Length;
            runningOffset = Math.Max(runningOffset, start);

            chunks.Add(new TextChunk(
                Id: $"sent-{index:D4}",
                Text: chunkText,
                Section: null,
                Start: start,
                End: Math.Min(end, text.Length)));

            index++;
        }

        return chunks;
    }

    public static List<TextChunk> HeadingAwareMarkdown(string markdown, int maxSectionChunkSize)
    {
        if (maxSectionChunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxSectionChunkSize));

        var lines = markdown.Split('\n');
        var sections = new List<(string heading, StringBuilder content)>();

        string currentHeading = "Introduction";
        var currentContent = new StringBuilder();

        foreach (var rawLine in lines)
        {
            string line = rawLine.TrimEnd();

            // Treat markdown headings as section boundaries
            if (Regex.IsMatch(line, @"^\#{1,6}\s+"))
            {
                if (currentContent.Length > 0)
                {
                    sections.Add((currentHeading, currentContent));
                    currentContent = new StringBuilder();
                }

                currentHeading = Regex.Replace(line, @"^\#{1,6}\s+", "").Trim();
            }
            else
            {
                currentContent.AppendLine(line);
            }
        }

        if (currentContent.Length > 0)
        {
            sections.Add((currentHeading, currentContent));
        }

        var chunks = new List<TextChunk>();
        int index = 0;
        int absoluteOffset = 0;

        foreach (var (heading, contentBuilder) in sections)
        {
            string content = contentBuilder.ToString().Trim();
            if (string.IsNullOrWhiteSpace(content))
                continue;

            // If section is small, keep it whole.
            if (content.Length <= maxSectionChunkSize)
            {
                chunks.Add(new TextChunk(
                    Id: $"head-{index:D4}",
                    Text: content,
                    Section: heading,
                    Start: absoluteOffset,
                    End: absoluteOffset + content.Length));
                index++;
            }
            else
            {
                // If section is large, sub-chunk it.
                int localStart = 0;
                int subIndex = 0;

                while (localStart < content.Length)
                {
                    int len = Math.Min(maxSectionChunkSize, content.Length - localStart);
                    string piece = content.Substring(localStart, len).Trim();

                    if (!string.IsNullOrWhiteSpace(piece))
                    {
                        chunks.Add(new TextChunk(
                            Id: $"head-{index:D4}-{subIndex:D2}",
                            Text: piece,
                            Section: heading,
                            Start: absoluteOffset + localStart,
                            End: absoluteOffset + localStart + piece.Length));
                        subIndex++;
                    }

                    localStart += maxSectionChunkSize;
                }

                index++;
            }

            absoluteOffset += content.Length + 1;
        }

        return chunks;
    }
}

public static class Retriever
{
    public static List<(TextChunk chunk, double score)> GetTopChunks(
        string question,
        IReadOnlyList<TextChunk> chunks,
        int topK)
    {
        var qTerms = Tokenize(question);

        return chunks
            .Select(chunk =>
            {
                var cTerms = Tokenize(chunk.Text);
                double overlap = qTerms.Intersect(cTerms).Count();
                double normalization = Math.Sqrt(qTerms.Count * Math.Max(cTerms.Count, 1));
                double score = normalization == 0 ? 0 : overlap / normalization;

                // Tiny boost if the section title has relevant words
                if (!string.IsNullOrWhiteSpace(chunk.Section))
                {
                    var sTerms = Tokenize(chunk.Section!);
                    score += 0.15 * qTerms.Intersect(sTerms).Count();
                }

                return (chunk, score);
            })
            .OrderByDescending(x => x.score)
            .Take(topK)
            .ToList();
    }

    private static HashSet<string> Tokenize(string text)
    {
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "the", "a", "an", "and", "or", "to", "of", "for", "in", "on", "is", "are",
            "what", "which", "how", "with", "this", "that", "it", "be", "as", "at",
            "by", "from", "your", "you"
        };

        var words = Regex.Matches(text.ToLowerInvariant(), @"[a-z0-9]+")
            .Select(m => m.Value)
            .Where(w => !stopWords.Contains(w))
            .ToHashSet();

        return words;
    }
}

public static class PromptBuilder
{
    public static string BuildGroundedPrompt(string question, IEnumerable<TextChunk> chunks)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Use ONLY the context below to answer the question.");
        sb.AppendLine("If the answer is not present, say you do not have enough information.");
        sb.AppendLine();

        sb.AppendLine("CONTEXT:");
        foreach (var chunk in chunks)
        {
            sb.AppendLine($"[{chunk.Id}] Section: {chunk.Section ?? "(none)"}");
            sb.AppendLine(chunk.Text);
            sb.AppendLine();
        }

        sb.AppendLine("QUESTION:");
        sb.AppendLine(question);
        sb.AppendLine();

        sb.AppendLine("Answer clearly and include the supporting chunk IDs.");
        return sb.ToString();
    }
}