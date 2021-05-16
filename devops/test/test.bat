cd ..\..\src\LogoFX.Client.Mvvm.Model.Specs
dotnet test
cd bin
livingdoc test-assembly LogoFX.Client.Mvvm.Model.Specs.dll -t TestExecution.json --work-item-url-template https://github.com/LogoFX/logofx-client-mvvm-model/issues/{id} --work-item-prefix WI
cd ..\..\..\devops