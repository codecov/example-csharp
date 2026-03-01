# [Codecov](https://codecov.io) C# Example

[![codecov](https://codecov.io/gh/codecov/example-csharp/branch/master/graph/badge.svg)](https://codecov.io/gh/codecov/example-csharp)
[![AppVeyor](https://img.shields.io/appveyor/ci/stevepeak/example-csharp.svg)](https://ci.appveyor.com/project/stevepeak/example-csharp/branch/master)
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fcodecov%2Fexample-csharp.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fcodecov%2Fexample-csharp?ref=badge_shield)

## Guide
### AppVeyor Setup
Add to your `appveyor.yml` file.
```yml
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
- codecov -f "MyProject_coverage.xml
```
### Producing Coverage Reports
Coverage is generated using [OpenCover](https://github.com/OpenCover/opencover). You can obtain it from [NuGet](https://www.nuget.org/packages/opencover) or [Chocolatey](https://chocolatey.org/packages/opencover.portable). If we run the following command in PowerShell to install OpenCover via Chocolatey, 

```powershell
choco install opencover.portable
```

the OpenCover commandline will become available.

Generation of coverage report is slighly different depending on the .NET platform of your test projects.

#### .NET Framework project

##### xUnit

First install the xUnit console runner via [Nuget](https://www.nuget.org/packages/xunit.runner.console/2.3.0-beta1-build3642) or [Chocolatey](https://chocolatey.org/packages/XUnit). If we run the following in PowerShell to install xUnit via Chocolatey

```powershell
choco install xunit
```

and execute the following in your solution's root,

```powershell
OpenCover.Console.exe -register:user -target:"xunit.console.x86.exe" -targetargs:".\MyUnitTests\bin\Debug\MyUnitTests.dll -noshadow" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
```

Then a coverage report will be generated.

##### MSTest

Execute the following in your solution's root,

```powershell
OpenCover.Console.exe -register:user -target:"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\MSTest.exe" -targetargs:"/testcontainer:"".\MyUnitTests\bin\Debug\MyUnitTests.dll" -filter:"+[UnitTestTargetProject*]* -[MyUnitTests*]*" -output:".\MyProject_coverage.xml"
```

where you may need to change the `-target` flag to point to the correct version of MSTest.


#### .NET Core project

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

### Bash
In bash run the following to upload the report

```bash
curl -s https://codecov.io/bash > codecov
chmod +x codecov
./codecov -f "MyProject_coverage.xml" -t <your upload token>
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

## Caveats
### Private Repo
Repository tokens are required for (a) all private repos, (b) public repos not using Travis-CI, CircleCI, GitHub Actions, or AppVeyor. Find your repository token at Codecov and provide via appending `-t <your upload token>` to you where you upload reports e.g. `.\codecov -f "MyProject_coverage.xml" -t <your upload token>`

## Links
- [Community Boards](https://community.codecov.io)
- [Support](https://codecov.io/support)
- [Documentation](https://docs.codecov.io)


## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fcodecov%2Fexample-csharp.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fcodecov%2Fexample-csharp?ref=badge_large)
