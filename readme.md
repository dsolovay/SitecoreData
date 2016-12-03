# SitecoreData

Experiments with [Sitecore](http://www.sitecore.net) running on [RavenDB](http://ravendb.net) and [MongoDB](http://www.mongodb.org). Primary goals:

* Universal DataProvider wrapper for simplifying writing data providers for Sitecore.
* Read only data provider for serialized Sitecore items, intented for quick Sitecore prototyping and testing (or think TDS).
* Fully production ready data providers, including support for event queue, history, blob storage, the full monty!
* "Event Sourcing" stylede data providers (basically all CRUD operations is only inserts which enables some 
interesting data replication benefits).

## Performance ##

See [/doc/Measurements.xlsx](https://github.com/pbering/SitecoreData/blob/master/doc/Measurements.xlsx) for current performance data.

## Notes ##

Inspiration for the MongoDB implementation came from this blog post: (http://hermanussen.eu/sitecore/wordpress/2012/05/making-sitecore-faster-with-mongodb/), big thanks 
to Robin Hermanussen for getting me inspired to trying out MongoDB!

This is tested with Sitecore 6.4.1 rev. 110324 and Sitecore 6.5.0 rev. 120427.

## Installation ##

* Create fresh install of Sitecore.
* Clone SitecoreData project.
* Place Sitecore.Kernel.dll in \lib\Sitecore\
* Copy ConnectionStrings.config from Sitecore install to SitecoreData\Website project.
* Add connection string entries for MongoDB databases:

```xml
<add name="nosqlmongoweb" connectionString="mongodb://localhost:27017/web" />
<add name="nosqlmongomaster" connectionString="mongodb://localhost:27017/master" />
<add name="nosqlmongocore" connectionString="mongodb://localhost:27017/core" />
<add name="nosqlmongotest" connectionString="mongodb://localhost:27017/test" />
```
 
* Install Mongo, launch mongod.exe (this must be done after each server restart, or the mongo service must be configured.
* Copy default_deploycommand.xml to deploycommand.xml, and change path to point to Sitecore instance website folder.
* Deploy (Use build configuration "Debug with Publish" and a deploycommand.xml pointing to above IIS website)
* Run /Transfer.aspx, copying web to nosqlmongoweb, and so forth for core and master.
* Recycle app pool.  Mongo databases are now available in desktop DB chooser.

## Contributing ##

Please do :)