meadow app build -c Debug
meadow runtime disable
meadow app deploy -c Debug
#meadow file write --files .\bin\Debug\netstandard2.1\Chibi.Ui.dll
meadow runtime enable
meadow listen