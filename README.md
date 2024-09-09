# e-com

### High level architecture

[see here](./documentation/architecture.md)

### How to run

Ideally, one could run 

```
docker-compose build --no-cache
```

and then

```
docker-compose up
```

but there are some problems with opening TCP ports in containers and making containers work together

Instead, it's possible to build solution and run projects one by one.  
Navigate to the root folder
```
dotnet build .\e-com.sln
dotnet run --project .\hosts\lighthouse\lighthouse.csproj
dotnet run --project .\hosts\paas\btm.paas.csproj
dotnet run --project .\hosts\web\btm.web.app\btm.web.app.csproj
```

Open mapped url (from console) in browser and navigate to '/payment'.  
On the page you can input payment value, and press deposit. In the text area under the form, you can see payment statuses.  
This example, was created to demonstrate Actor model approach in processing high load transactions.  
When used in clusters together with AKS or EKS, can be very efficient, since actors can scale out very easily.