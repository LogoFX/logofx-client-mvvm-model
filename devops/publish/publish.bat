cd ../build
call build.bat
cd ../test
call test.bat
cd ./pack
call ./pack.bat
cd ../publish
call ./copy.bat LogoFX.Client.Mvvm.Model 2.2.0-rc2 %1