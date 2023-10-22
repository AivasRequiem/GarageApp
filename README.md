# GarageApp
App for searching autoservices

Install SQL Server (https://www.microsoft.com/uk-ua/sql-server/sql-server-downloads) and then run following commands:
dotnet tool install --global dotnet-ef
dotnet ef migrations add YOURMIGRATIONNAME
dotnet ef database update
