Codecov C# Example
==================

| [https://codecov.io][1] | [@codecov][2] | [hello@codecov.io][3] |
| ----------------------- | ------------- | --------------------- |

## Installation

You need to add the [OpenCover][5] nuget package to your Visual Studio solution which is used to generate code coverage analysis:
   
```
PM> Install-Package OpenCover
```

Secondly, you need to either write a PowerShell script (if you intend to generate code coverage and upload the result interactively) or you need to add a few entries in you CI config file (if you intend to let your CI generate the coverage).


## Generate the coverage file

Let's assume your C# solution has the following folder structure:

- C:\_build\MyProject\ --> this is your solution's root folder. For instance, this is where you have saved your visual studio solution (.sln) file.
- C:\_build\MyProject\packages\ --> this is where all nuget packages are downloaded
- C:\_build\MyProject\MyProject Unit Tests\ --> this is where your C# unit testing project is located (.csproj)

To generate code coverage analysis, execute the following comment from a command prompt in your solution's root:

```PowerShell
.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user -target:"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe" -targetargs:"/noresults /noisolation /testcontainer:"".\MyProject Unit Tests\bin\Debug\MyProject.UnitTests.dll" -filter:"+[MyProject]*  -[MyProject]MyProject.Properties.*" -excludebyattribute:*.ExcludeFromCodeCoverage* -hideskipped:All -output:.\MyProject_coverage.xml
```

This command makes a few assumptions which you will most likely have to adjust to meet your needs:

1. Use Visual Studio 2013
2. Use OpenCover v4.6.519, which is the latest version as of this writing, to generate code coverage. Of course you will need to adjust the path to the OpenCover package when new versions are available.
3. Unit tests are compiled in a DLL called .\MyProject Unit Tests\bin\Debug\MyProject.UnitTests.dll (replace 'Debug' with 'Release if you compile in 'Release' mode).
4. Code coverage will include all classes, methods and properties under the [MyProject] namespace except [MyProject]MyProject.Properties. These properties, typically, include your project version number, compiler options, etc. and therefore should be excluded from the code coverage report.
5. Any class decorated with the 'ExcludeFromCodeCoverage' attribute is automatically excluded from the coverage analysis.
6. Code coverage analysis is written to a XML file called MyProject_coverage.xml  

Now that you have generated your code coverage analysis, you must download Codecov's "upload script" and use it to upload your code coverage file to CodeCov.


##PowerShell script

The PowerShell script contains two lines: one to download Codecov's script and the second one to upload your coverage file to Codecov.io
 
```PowerShell
(New-Object System.Net.WebClient).DownloadFile("https://codecov.io/bash", ".\CodecovUploader.sh")
.\CodecovUploader.sh -f "MyProject_coverage.xml -t <your upload token> -X gcov
```

Of course, you need to replace the `<your upload token>` with your private Codecov upload token.

TIP: for added convenience, you can paste the command from the 'Generate the Coverage file' at the top of this PowerShell script in order to create one convenient script that will both generate the coverage file and upload it to Codecov.io


## Continuous Integration
 
If you use a CI, such as AppVeyor for example, you need to modify the 'after_test' section of your .yml to instruct your CI to generate the coverage file, download Codecov's uploader and finally to upload your coverage file to codecov.io. Here's what the 'after_test' section should look like:

``` YAML
after_test: 
    - .\packages\OpenCover.4.6.519\OpenCover.Console.exe -register:user -target:"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\MSTest.exe" -targetargs:"/noresults /noisolation /testcontainer:"".\MyProject Unit Tests\bin\Debug\MyProject.UnitTests.dll" -filter:"+[MyProject]*  -[MyProject]MyProject.Properties.*" -excludebyattribute:*.ExcludeFromCodeCoverage* -hideskipped:All -output:.\MyProject_coverage.xml
    - "SET PATH=C:\\Python34;C:\\Python34\\Scripts;%PATH%"
    - pip install codecov
    - codecov -f "MyProject_coverage.xml"
```

We recently incorporated codecov.io into [DotNetAnalyzers/StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers). Based on this experience, I recommend the following changes to the documentation.

### Combine test and code coverage stages

The steps to do this are elusive, but relatively straightforward when you see them.

1. Add the `-returntargetcode` argument to OpenCover.Console
1. Change the `after_test` section to `test_script`

:thought_balloon: It might make sense to make a note that the code coverage gathering process may be run as a separate build step for users who are unable to or do not wish to run their complete automated testing through OpenCover. Users will want to integrate Codecov *either* in their `test_script` section *or* in their `after_test` section, but not both.

### Include information for XUnit users

Users working with XUnit will need to modify their OpenCover call as follows:

1. The `-target` argument becomes the following:

  ```
  -target:"%xunit20%\xunit.console.x86.exe"
  ```

1. The `-targetargs` argument becomes the following (you may want to replace our target assembly path with the one in your original sample; I wasn't familiar enough with the escape sequences for spaces to do it myself):

  ```
  -targetargs:"C:\projects\stylecopanalyzers\StyleCop.Analyzers\StyleCop.Analyzers.Test\bin\Debug\StyleCop.Analyzers.Test.dll -noshadow -appveyor"
  ```
  
  
### Important notes
- `BuildOptions` should not include `"portable"` option on the dotnetcore test project's `project.json` file.


### Sample project

- [DotNetAnalyzers/StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)


[1]: https://codecov.io/
[2]: https://twitter.com/codecov
[3]: mailto:hello@codecov.io
[4]: https://github.com/codecov/codecov-bash
[5]: https://www.nuget.org/packages/OpenCover/
