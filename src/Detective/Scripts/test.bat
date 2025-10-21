
@echo off

dotnet build UnitTests
dotnet test UnitTests --no-build
dotnet test IntegrationTests
dotnet test IntegrationTests

if exist allure-results-merged rmdir /s /q allure-results-merged
mkdir allure-results-merged

xcopy /s /y /Q UnitTests\bin\allure-results\* allure-results-merged\
xcopy /s /y IntegrationTests\bin\allure-results\* allure-results-merged\

allure generate allure-results-merged -o allure-report --clean

allure open allure-report
