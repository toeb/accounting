call "%VS120COMNTOOLS%\VsDevCmd.bat"
cd Accounting
msbuild
vstest.console Accounting.Tests\bin\Debug\Accounting.Tests.dll Application.Web.Test\bin\Debug\Application.Web.Test.dll