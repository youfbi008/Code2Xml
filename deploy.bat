rd nuspec /s /q
mkdir nuspec
copy *.nuspec nuspec /y
cd nuspec
mkdir lib
copy ..\Code2Xml.Console\bin\Release\*.dll lib\
copy ..\Code2Xml.Console\bin\Release\*.pdb lib\
del lib\Paraiba.*
FOR %%f IN (*.nuspec) DO (
	nuget pack %%f
)
FOR %%f IN (*.nupkg) DO (
	nuget push %%f
)
cd ..\
