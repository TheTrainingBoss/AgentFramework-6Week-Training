## Running the samples in Python

- Make sure to create a virtual Environment first under the `Python Demos` folder

```powershell
    python -m venv .venv
```

- Activate the virtual Environment in your terminal

```powershell
  .\venv\Scripts\Activate
```
This is under Windows, if you are using Mac the activation uses the `source` command

- Once the Virtual Environment `.venv` is created install all the needed packages using the following command:

```powershell
    pip install -r requirements.txt
```

- Make sure you are using Python interpreter version 3.10, 3.11, 3.12 or 3.13, preferrably 3.13, but do not use version 3.14 yet as some libraries have not been updated yet.
- Finally, just run each demo by issuing the following command from the terminal:

```powershell
    python <name of the python file>
```