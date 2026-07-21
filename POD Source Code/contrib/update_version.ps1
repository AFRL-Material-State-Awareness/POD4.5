param ($SolutionDir, $ProjectDir)

try {
    $SolutionDir = $SolutionDir.Trim("'")
	$ProjectDir = $ProjectDir.Trim("'")
	$version = Get-Content -path (Join-Path -Path $SolutionDir -ChildPath VERSION.txt) -Raw
	$assemblyversion = 'AssemblyVersion("' + $version + '")'
	$assemblyfileversion = 'AssemblyFileVersion("' + $version + '")'
	$outpath = $ProjectDir + '\Properties\AssemblyInfo.cs'
	$updatedtext = (((Get-Content -path (Join-Path -Path $ProjectDir -ChildPath Properties\AssemblyInfo.cs) -Raw) -replace 'AssemblyVersion\(\"[0-9.a-zA-Z]+\"\)',$assemblyversion -replace 'AssemblyFileVersion\(\"[0-9.a-zA-Z]+\"\)',$assemblyfileversion))
	[System.Io.File]::WriteAllText($outpath, $updatedtext, [System.Text.Encoding]::UTF8)
	return
}
catch {
	Write-Host $_
	Write-Host $_.ScriptStackTrace
	return 1
}
