call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\VsDevCmd.bat"
cd Accounting
msbuild
vstest.console Accounting.Tests\bin\Debug\Accounting.Tests.dll Application.Web.Test\bin\Debug\Application.Web.Test.dll