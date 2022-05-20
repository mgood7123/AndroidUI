cd K:\AndroidUI-SkiaSharp\
dotnet tool restore

if ($?) {
	if (-not(Test-Path BUILD_NUMBER.txt)) {
		echo 3000 > BUILD_NUMBER.txt;
	}

	$build_number = [int] (cat BUILD_NUMBER.txt)
	$build_number++
	echo $build_number > BUILD_NUMBER.txt

	dotnet cake --target=nuget --buildall=true --skipexternals=all --buildnumber=$build_number

	if ($?) {
		dotnet remove C:\Users\small\source\repos\WindowsProject1\AndroidUI\AndroidUI.csproj package SkiaSharp

		dotnet add C:\Users\small\source\repos\WindowsProject1\AndroidUI\AndroidUI.csproj package SkiaSharp --version 2.88.0-preview.$build_number --source=K:\AndroidUI-SkiaSharp\output\nugets

		if ($?) {
			dotnet run --project C:\Users\small\source\repos\WindowsProject1\AndroidUITest\AndroidUITest.csproj
		}
	}
}