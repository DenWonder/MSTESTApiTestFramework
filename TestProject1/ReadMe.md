Use https://dillinger.io/ for improve readme file

# Test Task for Software Test Automation

## _Tools and Technologies:_
 - Platform: .Net 6;
 - Programming language: C#;
 - Test frameworK: MSTest;

## Used API:
 - https://dummyjson.com/docs/carts

## How to Use :
1) First of all, you need to check, if GIT is installed at your computer. 
If you are using WINDOWS OS, just run CMD and enter the following: 
```sh
git --version
```
If you are using linux-based OS, open terminal and enter the following 
```sh
git --version
```
In case, if you haven't installed GIT at your computer - please, visit GIT website and install it, followin official instructions;

2) GIT CLONE current repo; Open directory for this project, then
Open git bash console and enter the following:
```sh
git clone https://github.com/DenWonder/MSTESTApiTestFramework.git
```
3) CHECK IF YOU HAVE .NET tools at your computer (You need for .net 6) Install it, if you dont;
   https://dotnet.microsoft.com/en-us/download/dotnet/6.0
4) Launch project in IDE, wait for a while, and start "BUILD PROJECT" command;
5) After project successfully gets all dependenses and build - run "Run Unit Tests" command;
5) Check results in Unit Test console;

## How to use the code:

 - If you want to change any URL's, that used in project, go to variables.cs file and edit it (yes, all URLs declarated only once in whole programm to make it easy to change it, if we have to);
 - If you want to change API Requests fileds name - go to variables.cs file and edit it;
 - If you want to use a specific user for all tests - go to "Initialize.cs" file, found "Initialize_Real_User_for_Tests" method and change existedUserId variable value to your user ID value;
 - If you want to change a value of acceptable time of server response - go to variables.cs file and change value of AcceptableServerResponseTime variable (integer value, count of milliseconds);
 - If you want to add tests for new api endpoint - create URL variable at variables.cs,add new file for unit tests of current endpoint and enjoy!;
 - Driver/ApiDriver.cs - use for interaction with api. Requests launcher;
 - Helpers/ApiHelper.cs - use for construct api requests and call request launcher with parameters (request type, headers, body);
 - Helpers/AuthHelper.cs - use for construct headers parameters of api requests;
 - Helpers/CartsHelper.cs - use for generate request body parameters for carts endpoint;
 - Helpers/DeserializeHelper.cs - all deserializers, use for transformation response data to useful Objects;
 - Models/ - list of useful models (local representation of api models, like a dto);
