cd /root/source/Work/LoadingProductWeb/LoadingProduct/LoadingProductWeb
dotnet publish -c Release
cd ./bin/Release/netcoreapp2.1/publish
rm /home/LoadingProduct/* -rf
cp -rp * /home/LoadingProduct/
systemctl restart loadingproduct.service
systemctl status loadingproduct.service
