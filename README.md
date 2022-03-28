README.md
============ GeoDataRest Project ============

Hello,

This project was made with Visual Studio 2019 as WebApi ASP Net application using REST 

It's a REST api to ingest geography data to a SQL Server database instance and provide simple ingesting and listing endpoints

* Be sure to have installed CLR Types for SQL Server 2012/2014 

We have a 3 GET methods and 2 POST methods to :

* GET to List all elements
* GET to get the weekly average trips for a region
* GET to get the weekly average trips for a boxed bounding box coordinates 
* POST for a single element to be ingested
* POST for a CSV file to be uploaded

These are some sample API Calls : 

* To list all data
https://localhost:44379/api/GeoData/Get 

* To get the average weekly trips for a boxed bounding region (Near Turin area)
https://localhost:44379/api/GeoData/GetBoxAverage/?lat1=7&long1=46&lat2=8&long2=44

* To get the average weekly trips for a region ("Turin")
https://localhost:44379/api/GeoData/GetRegionAverage/?region=Turin

* To upload a CSV file
https://localhost:44379/api/GeoData/PostCSV
(In Postman: Click on Body, form-data, add a "file" key and choose "File" in the right dropdown to select a file in the value area)

* To add a single element (endpoint ingest)
https://localhost:44379/api/GeoData/Post
(In Postman: Click on Body, raw, and then post the following sample in the text area:
{
        "Region": "Turin",
        "Origin_coord": "POINT (7.672837913286881 44.9957109242058)",
		"Destination_coord":  "POINT (7.720368637535126 45.06782385393849)",
        "Datetime": "2018-05-21 02:54:04",
        "Datasource": "baba_car"
}
	
	
============ Project Structure ============

The project follows the classic entity framework project model

We have a :

* Connection class for the datasource (*Edit this to connect to your database)
* A Model for the object data (GeoData)
* A Data access class (GeoDataData)
* A controller (GeodataController)

WebApiConfig was modified to provide multiple actions for the controller ( routeTemplate: "api/{controller}/{action}/{id}" ) 

Global.asax was modified to add SQL Server support to Geography data types

And nugets for cors, EntityFramework, WebApi and Microsoft.SqlServer.Types were added to




