## How to build

```dotnet restore```

```dotnet run {service}```

The service is running under a local mongodb url, so if you want to change it you must open the ```MongoContext``` class e write down your needs :)

PS: Pull requests for json's parse are welcome! 

## Types of service

**bec**: BEC website's crawler

**buscape**: Buscapé website's crawler

**list**: Create a list of bidding from BEC (1 month length)