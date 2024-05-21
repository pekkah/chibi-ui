meadow app build -c Release
meadow runtime disable
meadow app deploy -c Release
#meadow file write --files .\bin\Debug\netstandard2.1\Chibi.Ui.dll
meadow runtime enable
meadow listen