# **NikkudApi**

## Goal of this repository

Give input, which are hebrew words, and return hebrew words back with diatricts (nikkud).

## How does it work

Send GET call with input and get input back with diatricts.
Code will open headless browser with puppeteersharp and use several websites who add nikkud for free. Results will then be returned.

## How to improve
- Calls take a long time to process. [Huggingface](https://huggingface.co/spaces/malper/unikud) uses a python library and will be faster than opening a browser each time. Check out Python.NET or IronPython to integrate library inside web api. 
- reject calls based on several criteria
    - *null*
    - non hebrew words
- implement error handling
    - convert text into json to handle error handling
