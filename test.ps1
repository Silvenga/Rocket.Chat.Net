$coverageXml = "opencoverCoverage.xml"
$xunit = (Resolve-Path "packages\xunit.runner.console.*\tools\xunit.console.exe").ToString()
$testDdl = "test\Rocket.Chat.Net.Tests\bin\Debug\Rocket.Chat.Net.Tests.dll"
$openCover = (Resolve-Path "packages\OpenCover.*\tools\OpenCover.Console.exe").ToString()
$coveralls = (Resolve-Path "packages/coveralls.net.*/tools/csmacnz.coveralls.exe").ToString()

$targetargs = "`"$testDdl`" -noshadow -verbose -notrait category=Sandbox"
$filter = "-filter:`"+[Rocket.Chat.Net]* -[Rocket.Chat.Net]Rocket.Chat.Net.Models.* -[*]JetBrains.*`""

& $openCover -register:user -target:$xunit -targetargs:"$targetargs" -filter:"$filter" -output:$coverageXml
& $coveralls --opencover -i $coverageXml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessa$env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB