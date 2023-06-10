cd ./api
dotnet publish -c Release -o bin/publish
cd ..
swa deploy --env production --api-location .\api\bin\publish --api-language dotnet