
GSTMagazin est une application web "**ASP.NET Core**"  complète conçue pour rationaliser et améliorer la gestion des magasins de détail. Que vous exploitiez une petite boutique ou une grande chaîne de magasins, GSTMagazin offre des outils intuitifs pour simplifier les opérations quotidiennes et augmenter la productivité.  


Note : Ceci est personnalisé en fonction de la demande du client, sans gestion des vendeurs et des commandes. Le magasinier fait ce que le vendeur devrait faire.  

### Technologies Utilisées:


- ![C#](https://img.shields.io/badge/Language-C%23-239120.svg) 
- ![ASP.NET Core](https://img.shields.io/badge/Framework-ASP.NET%20Core-blue.svg) 
- ![Entity Framework Core](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-green.svg) 
- ![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red.svg)  
- ![Chart.js](https://img.shields.io/badge/Visualization-Chart.js-orange.svg) 
- ![jQuery](https://img.shields.io/badge/Library-jQuery-lightgrey.svg) 
- ![JSON](https://img.shields.io/badge/Data-JSON-yellow.svg)   
- ![HTML](https://img.shields.io/badge/Markup-HTML-blue.svg) 
- ![CSS](https://img.shields.io/badge/Style-CSS-blueviolet.svg) 
- ![JavaScript](https://img.shields.io/badge/Language-JavaScript-yellowgreen.svg) 
- ![Bootstrap](https://img.shields.io/badge/Framework-Bootstrap-purple.svg) 




Pour une démo ou un test, n'hésitez pas à me contacter.

Visitez le site web ici : https://gstmag.azurewebsites.net/

-----------
# Project Setup Instructions

## 1. Configure the Database Connection String
Update the connection string in the `appsettings.json` file to match your SQL Server instance:

```json
"ConnectionStrings": {
  "StringChainCnx": "Server=tcp:{your server database},1433;Initial Catalog={your database};Persist Security Info=False;User ID={Your User Id};Password={your password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

*Note: Make sure you have all necessary NuGet packages installed and the correct SDK version. You can verify this in the `Technexa.csproj` file.*

## 2. Apply Database Migrations
Open a terminal in your project folder and run the following command to apply migrations and create the database schema:

```bash
dotnet ef database update
```

## 3. Add Initial Directeur  Information
Log in to your Microsoft SQL Server and manually add an initial directeur record to the database. Use a strong username and password for security.

## 4. Build and Run the Project
Run the following commands in your terminal to build and launch the project:

```bash
dotnet build
dotnet run
```




_______


## Page de Stock
![Stock Page](screnshots/Stock.png)

## Stock avec Recherche
![Stock with Search](screnshots/StockAvecChercherPar.png)

## Stock avec Actions
![Stock with Actions](screnshots/StockwithActions.png)

## Page d'Analyse
![Analysis Page](screnshots/Analyse.png)

## Analyse avec Options de Filtrage
![Analysis with Filter Options](screnshots/AnalyseFiltrageoptions.png)

## Plan des Achats
![Purchase Plans](screnshots/PlansAchats.png)

## Plan des Ventes
![Sales Plans](screnshots/plansventes.png)

## Les Articles
![The Articles](screnshots/LesArticles.png)

--------

GSTMagazin is a comprehensive web application designed to streamline and enhance retail store management. Whether you're operating a small boutique or a large retail chain, GSTMagazin offers intuitive tools to simplify daily operations and boost productivity.

## Note : This is customized based on the client's request, without seller and order handling. The storekeeper does what the seller should do. 


### Technologies Used:  

- ![C#](https://img.shields.io/badge/Language-C%23-239120.svg) 
- ![ASP.NET Core](https://img.shields.io/badge/Framework-ASP.NET%20Core-blue.svg) 
- ![Entity Framework Core](https://img.shields.io/badge/ORM-Entity%20Framework%20Core-green.svg) 
- ![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red.svg)  
- ![Chart.js](https://img.shields.io/badge/Visualization-Chart.js-orange.svg) 
- ![jQuery](https://img.shields.io/badge/Library-jQuery-lightgrey.svg) 
- ![JSON](https://img.shields.io/badge/Data-JSON-yellow.svg)   
- ![HTML](https://img.shields.io/badge/Markup-HTML-blue.svg) 
- ![CSS](https://img.shields.io/badge/Style-CSS-blueviolet.svg) 
- ![JavaScript](https://img.shields.io/badge/Language-JavaScript-yellowgreen.svg) 
- ![Bootstrap](https://img.shields.io/badge/Framework-Bootstrap-purple.svg) 

For demo or test, feel free to contact me.

 visit the  website in : https://gstmag.azurewebsites.net/


