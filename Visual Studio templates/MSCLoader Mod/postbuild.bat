:: Has to run in a batch file because Visual Studio apparently doesnt support variables...

:: %1 - TargetPath
:: %2 - TargetDir
:: %3 - TargetName
:: %4 - ConfigurationName

echo [ Running post build actions for configuration %4! ]

:: CHANGE THESE
set ModFolder=YOURMODFOLDERPATH
set ABFolder=YOURASSETBUNDLEFOLDERPATH

if %ModFolder%==YOURMODFOLDERPATH (
    echo [ Set your Mod Folder path in postbuild.bat - not set yet, not copying to mods folder! ]
)

if %ABFolder%==YOURASSETBUNDLEFOLDERPATH (
    echo [ Set your Assetbundle Folder path in postbuild.bat - not set yet, not copying to Assetbundle folder! ]
)

if %4=="Mini" goto :mini
if %4=="mini" goto :mini
if %4=="MINI" goto :mini

if NOT %ModFolder%==YOURMODFOLDERPATH (
    copy %1 "%ModFolder%" /y
    echo [ Copied dll into mods folder! ]

    if %4=="Debug" (
        copy %2%3.pdb "%ModFolder%" /y
        echo [ Copied pdb file into mods folder! ]
    
        cd "%ModFolder%"
        if exist debug.bat (
            call debug.bat
        ) else (
            echo [ debug.bat not found in %ModFolder%. You probably dont have MSCLoader debugging enabled! ]
        )
    )
)

echo [ Done! ]
exit

:mini
echo [ Building mini dll, hence not copying to mods folder! ]
if NOT %ABFolder%==YOURASSETBUNDLEFOLDERPATH (
    copy %1 "%ABFolder%" /y
    echo [ Copied to ABFolder for mini build! ]
    echo [ Done! ]
)
exit