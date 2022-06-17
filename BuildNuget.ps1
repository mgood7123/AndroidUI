cd K:\AndroidUI-SkiaSharp\
dotnet tool restore

if ($?) {
	dotnet run --project=utils/SkiaSharpGenerator/SkiaSharpGenerator.csproj -- generate --config binding/libSkiaSharp.json --skia externals/skia --output binding/Binding/SkiaApi.generated.cs

	if ($?) {
		dotnet run --project=utils/SkiaSharpGenerator/SkiaSharpGenerator.csproj -- verify --config binding/libSkiaSharp.json --skia externals/skia

		if ($?) {
			if (-not(Test-Path BUILD_NUMBER.txt)) {
				echo 3000 > BUILD_NUMBER.txt;
			}

			$build_number = [int] (cat BUILD_NUMBER.txt)
			$build_number++
			echo $build_number > BUILD_NUMBER.txt

			dotnet cake --target=nuget --buildall=true --buildnumber=$build_number

			if ($?) {
				dotnet remove K:\Windows\WindowsProject1\AndroidUI\AndroidUI.csproj package SkiaSharp

				dotnet add K:\Windows\WindowsProject1\AndroidUI\AndroidUI.csproj package SkiaSharp --version 2.88.1-preview.$build_number --source=K:\AndroidUI-SkiaSharp\output\nugets

				if ($?) {
					dotnet run --project K:\Windows\WindowsProject1\AndroidUITest\AndroidUITest.csproj
				}
			}
		}
	}
}