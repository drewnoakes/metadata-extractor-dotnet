doxygen\doxygen.exe doc\Com.Drew.Doxyfile
if(Test-Path -Path 'doc\html' ){
	7z a -tzip doc.zip doc\html\
	Push-AppveyorArtifact doc.zip
}
if(Test-Path -Path 'doc\Com.Drew.warnings.txt' ){
	7z a -tzip doc.warnings.zip doc\Com.Drew.warnings.txt
	Push-AppveyorArtifact doc.warnings.zip
}
exit 0