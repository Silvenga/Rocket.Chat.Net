$coverageXml = "opencoverCoverage.xml"
$xunit = (Resolve-Path "packages\xunit.runner.console.*\tools\xunit.console.exe").ToString()
$openCover = (Resolve-Path "packages\OpenCover.*\tools\OpenCover.Console.exe").ToString()
$coveralls = (Resolve-Path "packages\coveralls.net.*\tools\csmacnz.coveralls.exe").ToString()

$testDdl1 = "test\Rocket.Chat.Net.Tests\bin\Debug\Rocket.Chat.Net.Tests.dll"
$testDdl2 = "test\Rocket.Chat.Net.Bot.Tests\bin\Debug\Rocket.Chat.Net.Bot.Tests.dll"

$targetargs = "$testDdl1 $testDdl2 -noshadow -notrait category=Sandbox"
$filter = "+[Rocket.Chat.Net]Rocket.Chat.Net.* +[Rocket.Chat.Bot]Rocket.Chat.Net.Bot.*"

$command = "$openCover -register:user `"-target:$xunit`" `"-targetargs:$targetargs`" `"-filter:$filter`" -mergebyhash -output:$coverageXml"

$command
iex $command

& $coveralls --opencover -i $coverageXml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID