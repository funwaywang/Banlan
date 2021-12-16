set z7z="C:\program files\7-zip\7z.exe"

set zipfile="./bin/Banlan_1.21.1216.zip"
del %zipfile%

%z7z% a -tzip %zipfile% ../Src/bin/Release/Banlan.exe 
%z7z% a -tzip %zipfile% ../Src/bin/Release/Banlan.exe.config
%z7z% a -tzip %zipfile% ../Src/bin/Release/*.dll
%z7z% a -tzip %zipfile% ../Swatches

