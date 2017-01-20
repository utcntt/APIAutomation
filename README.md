# APIAutomation
An API automation testing framework based on NUnit.

## Features
1. Send synchronously/asynchronously every kind of  Http requests and verify every Http responses.
2. Easy to add Model classes, serialize/deserialize Json/Xml.
3. Log feature: NUnit log, text log, html log.
4. Easy to intergrate with any CI.
5. The framework supports to work with large number of test cases.
6. Bug/Feature ID supporting.
7. Regression test supporting.

## Technologies
1. NUnit
2. .Net Core.
3. Newton.Json

## Prerequisites
1. [.Net Core](https://docs.microsoft.com/en-us/dotnet/articles/core/)
2. [Visual Studio](https://code.visualstudio.com/)
3. [A Git client tool](https://git-scm.com/)

## How to write test cases
1. Clone the framework
2. Create a .NetCore Command Line project 
3. Add references to APICore and TestLibrary. [Example](https://github.com/utcntt/APIAutomation/blob/master/test/QnAMaker.Test/project.json)
4. Create an 'appsettings.json' file that contains all settings for the project. [Example](https://github.com/utcntt/APIAutomation/blob/master/test/QnAMaker.Test/appsettings.json)
5. Develop a Configuration class that inherits from TestLibrary.TestConfig. [Example](https://github.com/utcntt/APIAutomation/blob/master/test/QnAMaker.Test/QnAMarkerConfiguration.cs)
6. Develop a SetUp class. [Example](https://github.com/utcntt/APIAutomation/blob/master/test/QnAMaker.Test/SetUp.cs) 
7. Develop Model classes
8. Develop API wrapper classes that generates APIMethod instances from request data.
9. Develop XmlHelper class if your APIs use XML.
10. Write test cases.

## How to execute test cases
1. Quick execution: use TestExplorer of Visual Studio.
2. Command line/batch execution
  * Windows: go to the project folder 
```
dotnet test project.json
```
- Execution with log(none|failed|full):
```

dotnet test project.json -params "log=full"
```

- Execution with a Bug ID:
```
ddotnet test project.json -where "Bug=123"
```

- Regresion test Execution:
```
dotnet test project.json -params "regression=true"
```

##License: 

MIT License

Copyright (c) 2016 Pham Van Truong

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

##Author: Truong Pham (utcntt[at]gmail[dot]com)
