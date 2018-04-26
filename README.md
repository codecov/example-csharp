# Codecov C# Example

| [https://codecov.io](https://codecov.io/) | [@codecov](https://twitter.com/codecov) | [hello@codecov.io](mailto:hello@codecov.io) |
| ----------------------- | ------------- | --------------------- |

[![AppVeyor](https://img.shields.io/appveyor/ci/stevepeak/example-csharp.svg)](https://ci.appveyor.com/project/stevepeak/example-csharp/branch/master)
[![codecov](https://codecov.io/gh/codecov/example-csharp/branch/master/graph/badge.svg)](https://codecov.io/gh/codecov/example-csharp)

## Solution

Start by restoring the nuget packages and building the solution.

## Generate the Coverage File

Coverage is generated using [OpenCover](https://github.com/OpenCover/opencover). You can obtain it from [NuGet](https://www.nuget.org/packages/opencover) or [Chocolatey](https://chocolatey.org/packages/opencover.portable). If we run the following command in PowerShell to install OpenCover via Chocolatey, 

```powershell
choco install opencover.portable
```

the OpenCover commandline will become available.

Generation of coverage report is slighly different depending on the .NET platform of your test projects.

### .NET Framework project

#### xUnit

First install the xUnit console runner via [Nuget](https://www.nuget.org/packages/xunit.runner.console/2.3.0-beta1-build3642) or [Chocolatey](https://chocolatey.org/packages/XUnit). If we run the following in PowerShell to install xUnit via Chocolatey

```powershell
choco install xunit
```

and execute the following in your solution's root,

```powershell
OpenCover.Console.exe -register:user -target:"xunit.console.x86.exe" -targetargs:".\MyUnitTests\bin\Debug\MyUnitTests.dll -noshadow" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
```

Then a coverage report will be generated.

#### MSTest

Execute the following in your solution's root,

```powershell
OpenCover.Console.exe -register:user -target:"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe" -targetargs:"/testcontainer:"".\MyUnitTests\bin\Debug\MyUnitTests.dll" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
```

where you may need to change the `-target` flag to point to the correct version of MSTest.


### .NET Core project

If you don't yet have .NET Core SDK installed, install it

```powershell
choco install dotnetcore-sdk
```

In case of .NET Core projects, there is no difference between `MSTest` and `xUnit` for coverage report generation.

Make sure all covered projects generate full pdb file (not only test projects), either through `<DebugType>full</DebugType>` in the `.csproj` file or by using a Visual Studio: Project Properties > Build > Advanced > Debugging information. By default, projects created by `dotnet` or by Visual Studio use a portable format for pdb files. Support for portable pdb format [hasn't been released in OpenCover yet](https://github.com/OpenCover/opencover/issues/610). If you fail to set full pdb, the `OpenCover` will print out a message notifying you that it has no results along with common causes.

The .NET Core test assembly can't be run by a `xunit.console.x86.exe`, because that tool works only with .NET Framework assemblies. The tests are run by `dotnet test` (possibly `dotnet xunit` if you [add dotnet-xunit](https://xunit.github.io/docs/getting-started-dotnet-core.html#create-project) CLI tool to your project).

Execute the following command in your solution's root:

```powershell
OpenCover.Console.exe -register:user -target:"C:/Program Files/dotnet/dotnet.exe" -targetargs:test -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml" -oldstyle
```

where `-oldstyle` switch is necessary, because .NET Core uses `System.Private.CoreLib` instead of `mscorlib` and thus `OpenCover` can't use  `mscorlib` for code instrumentation. You may also need to change the location of `dotnet.exe` to depending on the installed location.

## Uploading Report

Many options exit for uploading reports to Codecov. Three commonly used uploaders for .NET are

1. [Codecov-exe](https://github.com/codecov/codecov-exe) (C# source code)
2. [Bash](https://github.com/codecov/codecov-bash)
3. [Python](https://github.com/codecov/codecov-python)

For OS X and Linux builds, the recommended uploader is bash. For windows builds, all three uploaders work, but Codecov-exe does not require any dependencies. For example, the bash uploader and python uploader would require bash or python to be installed. This may or may not be an option.

### Codecov-exe

First install Codecov-exe via [Nuget](https://www.nuget.org/packages/Codecov/) or [Chocolatey](https://chocolatey.org/packages/codecov). If we run the following in PowerShell to install it via Chocolatey

```powershell
choco install codecov
```

and then run the following in PowerShell

```
.\codecov -f "MyProject_coverage.xml" -t <your upload token>
```

the report will be uploaded.

### Bash

In bash run the following to upload the report

```bash
curl -s https://codecov.io/bash > codecov
chmod +x codecov
./codecov -f "MyProject_coverage.xml" -t <your upload token>
```

### Python
 
First installed python (if you don't have it already). A simple way to install python is [Chocolatey](https://chocolatey.org/packages/python)

```powershell
choco install python
```

Next run the following in PowerShell

```
pip install codecov
.\codecov -f "MyProject_coverage.xml" -t <your upload token>
```

### Continous Integration

The previous examples assumed local development. More commonly, you'll use a CI service like [AppVeyor](https://www.appveyor.com/) or [TeamCity](https://www.jetbrains.com/teamcity/). For TeamCity builds please see the [documentation](https://github.com/codecov/codecov-exe#teamcity). For AppVeyor builds using xUnit, your yaml file would look something like

#### Codecov-exe using Chocolatey

```yaml
image: Visual Studio 2015

before_build:
- nuget restore
- choco install opencover.portable
- choco install codecov

build:
  project: CodecovProject.sln
  verbosity: minimal

test_script:
- OpenCover.Console.exe -register:user -target:"%xunit20%\xunit.console.x86.exe" -targetargs:".\MyUnitTests\bin\Debug\MyUnitTests.dll -noshadow" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
- codecov -f "MyProject_coverage.xml"
```

#### Codecov-exe using NuGet

Using this method you can cache your packages.config file.

```yaml
image: Visual Studio 2015

before_build:
- nuget restore

build:
  project: CodecovProject.sln
  verbosity: minimal

test_script:
- .\packages\<ADD PATH>\OpenCover.Console.exe -register:user -target:"%xunit20%\xunit.console.x86.exe" -targetargs:".\MyUnitTests\bin\Debug\MyUnitTests.dll -noshadow" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
- .\packages\<ADD PATH>\codecov.exe -f "MyProject_coverage.xml"
```

#### Python

```yaml
image: Visual Studio 2015

before_build:
- nuget restore
- choco install opencover.portable

build:
  project: CodecovProject.sln
  verbosity: minimal

test_script:
- OpenCover.Console.exe -register:user -target:"%xunit20%\xunit.console.x86.exe" -targetargs:".\MyUnitTests\bin\Debug\MyUnitTests.dll -noshadow" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
- "SET PATH=C:\\Python34;C:\\Python34\\Scripts;%PATH%"
- pip install codecov
- codecov -f "MyProject_coverage.xml"
```

## Cake.Codecov

If you use [Cake](http://cakebuild.net/) (C# Make) for your build automation, there is a [Cake.Codecov](http://cakebuild.net/dsl/codecov/) addin available. Cake also has built in support for [OpenCover](http://cakebuild.net/dsl/opencover/). It makes using OpenCover and Codecov-exe really easy!

## Sample Project

An example C# project using AppVeyor, xUnit, OpenCover, and Codecov-exe is [DotNetAnalyzers/StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers).

We are happy to help if you have any questions. Please contact email our Support at [support@codecov.io](mailto:support@codecov.io)
