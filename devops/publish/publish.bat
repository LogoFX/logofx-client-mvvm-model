SET package_name=LogoFX.Client.Mvvm.Model
SET package_version=2.2.1
cd ../../src
nuget restore
cd ../devops/build
call build.bat
cd ../test
call test.bat
cd ./pack
call ./pack.bat
cd ../publish
call ./copy.bat %package_name% %package_version% %1
cd ../install
call ./uninstall-global-single.bat %package_name% %package_version%