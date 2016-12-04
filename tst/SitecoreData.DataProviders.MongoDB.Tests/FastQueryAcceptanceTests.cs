
using System.Configuration;
using System.IO;
using System.Security;
using MongoDB.Driver;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace SitecoreData.DataProviders.MongoDB.Tests
{

  [TestFixture]
  class FastQueryAcceptanceTests
  {

    protected MongoDatabase _db;
    protected Sitecore.Data.Database _SqlDatabase;
    protected Sitecore.Data.Database _mongoTestDb;



    private void InitializeMongoConnection()
    {
      var connectionString = ConfigurationManager.ConnectionStrings["nosqlmongotest"].ConnectionString;
      var server = MongoServer.Create(connectionString);
      var databaseName = MongoUrl.Create(connectionString).DatabaseName;
      _db = server.GetDatabase(databaseName);
    }

    public enum Database
    {
      Mongo, SqlSv
    }

    [TestFixtureSetUp]
    public void SetUp()
    {
      Sitecore.Context.IsUnitTesting = true;
      InitializeMongoConnection();
      _SqlDatabase = Factory.GetDatabase("master");
      _mongoTestDb = Factory.GetDatabase("nosqlmongotest");
      CreateTestTemplate(_SqlDatabase);
      CreateTestContent(_SqlDatabase);
      TransferUtil.TransferPath("/sitecore/templates", _SqlDatabase, _mongoTestDb, null);
      TransferUtil.TransferPath("/sitecore/content", _SqlDatabase, _mongoTestDb, null);
    }

    [TestFixtureTearDown]
    public void ClearDataBase()
    {
      RemoveTestContent(_SqlDatabase);
      RemoveTestTemplate(_SqlDatabase);
      RemoveTestContent(_mongoTestDb);
      RemoveTestTemplate(_mongoTestDb);
      _db.Drop();
    }


    [TestCase("fast:/sitecore/NotReal", Database.Mongo, Result = 0)]
    [TestCase("fast:/sitecore/NotReal", Database.SqlSv, Result = 0)]

    [TestCase("fast:/sitecore/content/home/test data", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content/home/test data", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content/home/Test Data", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content/home/Test Data", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content/home/#Test Data#", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content/home/#Test Data#", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content/home/Test Data/*", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content/home/Test Data/*", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content/home/Test Data//*", Database.Mongo, Result = 5)]
    [TestCase("fast:/sitecore/content/home/Test Data//*", Database.SqlSv, Result = 5)]

    [TestCase("fast:/sitecore/content//*[@@id='{526D0CDD-6C4D-4137-AE7A-C73C35DCEC47}']", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content//*[@@id='{526D0CDD-6C4D-4137-AE7A-C73C35DCEC47}']", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content//*[@@id='526D0CDD-6C4D-4137-AE7A-C73C35DCEC47']", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content//*[@@id='526D0CDD-6C4D-4137-AE7A-C73C35DCEC47']", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content//*[@@name='Item 2']", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content//*[@@name='Item 2']", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content//*[@@key='item 2']", Database.Mongo, Result = 1)]
    [TestCase("fast:/sitecore/content//*[@@key='item 2']", Database.SqlSv, Result = 1)]

    [TestCase("fast:/sitecore/content/home/test data//*[@@templatename='Sample Item']", Database.Mongo, Result = 4)]
    [TestCase("fast:/sitecore/content/home/test data//*[@@templatename='Sample Item']", Database.SqlSv, Result = 4)]

    [TestCase("fast:/sitecore/Layout//*[@@templateid='B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B']", Database.Mongo, Result = 4)]
    [TestCase("fast:/sitecore/Layout//*[@@templateid='B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B']", Database.SqlSv, Result = 4)]

    [TestCase("fast:/sitecore/Layout//*[@@templateid='{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}']", Database.Mongo, Result = 4)]
    [TestCase("fast:/sitecore/Layout//*[@@templateid='{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}']", Database.SqlSv, Result = 4)]

    [TestCase("fast:/sitecore/content/home/test data//*[@@templatekey='sample item']", Database.Mongo, Result = 4)]
    [TestCase("fast:/sitecore/content/home/test data//*[@@templatekey='sample item']", Database.SqlSv, Result = 4)]

    [TestCase("fast:/sitecore/content/home/test data//*[@@parentid='{946244AC-1BE6-4A4F-A958-88440341A8DB}']", Database.Mongo, Result = 2)]
    [TestCase("fast:/sitecore/content/home/test data//*[@@parentid='{946244AC-1BE6-4A4F-A958-88440341A8DB}']", Database.SqlSv, Result = 2)]

    [TestCase("fast:/sitecore/Layout//*[@@masterid='{00E66E02-DF20-4F3A-B8AA-5239108DC2BB}']", Database.Mongo, Result = 4, Ignore = true, IgnoreReason = "BranchId not implemented yet.")]
    [TestCase("fast:/sitecore/Layout//*[@@masterid='{00E66E02-DF20-4F3A-B8AA-5239108DC2BB}']", Database.SqlSv, Result = 4)]

    [TestCase("fast:/sitecore/Layout//*[@query string='p=1']", Database.Mongo, Result = 1, Ignore = true, IgnoreReason = "Field name logic not yet implmented.")]
    [TestCase("fast:/sitecore/Layout//*[@query string='p=1']", Database.SqlSv, Result = 1)]

    public int TestCountOfReturnedItemsForQuery(string query, Database testdb)
    {
      var db = GetTestDatabase(testdb);

      Item[] items = db.SelectItems(query);

      return items.Length;

    }

    private void RemoveTestContent(Sitecore.Data.Database db)
    {
      using (new SecurityDisabler())
      {
        db.GetItem("/sitecore/content/home/test data").Delete();
      }
    }

    private void CreateTestContent(Sitecore.Data.Database db)
    {
      string itemxml = File.ReadAllText(System.Environment.CurrentDirectory + @"\..\..\xml\TestItems.xml");
      Item home = db.GetItem("/sitecore/content/home");
      using (new SecurityDisabler())
      {
        home.Paste(itemxml, false, PasteMode.Merge);
      }
    }

    private void RemoveTestTemplate(Sitecore.Data.Database db)
    {
      using (var disabler = new Sitecore.SecurityModel.SecurityDisabler())
      {
        var template = db.GetItem("/sitecore/templates/user defined/user defined item");
        template.Delete();
      }
    }

    private void CreateTestTemplate(Sitecore.Data.Database db)
    {
      using (var disabler = new Sitecore.SecurityModel.SecurityDisabler())
      {
        string templatexml = File.ReadAllText(System.Environment.CurrentDirectory + @"\..\..\xml\TestTemplate.xml").ToString();
        db.GetItem("/sitecore/templates/user defined").Paste(templatexml, false, PasteMode.Merge);
      }
    }

    private Sitecore.Data.Database GetTestDatabase(Database testdb)
    {
      switch (testdb)
      {
        case Database.Mongo:
          return _mongoTestDb;
        case Database.SqlSv:
          return _SqlDatabase;
        default:
          throw new ArgumentOutOfRangeException("testdb");
      }
    }




  }
}
