#	run: PowerShell.exe -File StringsExtractor.ps1
#	rebuild project after new file generated

function GetFiles($path = $pwd, [string[]]$exclude, [Array] $files)
{
    foreach ($item in Get-ChildItem $path)
    {
        if ($exclude | Where {$item -like $_}) { continue }

		if($item.Extension.Equals(".cs"))
		{
			$files += $item.FullName
		}

        if (Test-Path $item.FullName -PathType Container)
        {
			$emptyfiles = @()
            $files += GetFiles $item.FullName $exclude $emptyfiles
        }
    }

	return $files
} 

function ExtractStrings([Array] $files)
{
	foreach ($file in $files)
    {
		$content = Get-Content $file
		$matches = [regex]::Matches($content, '(".*?")')
		foreach($matche in $matches)
		{
			if($matche.Success -and $matche.Value.Contains("%"))
			{
				$matche.Value
			}
		}
	}
}

$files = @()
$excludeList = @(".hg",".git","bin","obj")
$files = GetFiles "..\..\Com.Drew" $excludeList $files
$allStrings = ExtractStrings $files
$allStrings | out-file ".\TestStrings.txt"