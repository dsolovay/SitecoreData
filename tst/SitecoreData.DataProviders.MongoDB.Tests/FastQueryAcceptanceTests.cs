﻿
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SitecoreData.DataProviders.MongoDB.Tests
{
    [TestFixture]
    class FastQueryAcceptanceTests : MongoTestsBase
    {
        private const string fast_sitecore_NotReal = 
            "fast:/sitecore/NotReal";
        private const string fast_sitecore_Layout = 
            "fast:/sitecore/Layout";
        private const string fast_sitecore_Layout_asterisk = 
            "fast:/sitecore/Layout/*";
        private const string id_with_curly_braces = 
            "fast:/sitecore/Layout//*[@@id='{46D2F427-4CE5-4E1F-BA10-EF3636F43534}']";
        private const string id_without_curly_braces = 
            "fast:/sitecore/Layout//*[@@id='46D2F427-4CE5-4E1F-BA10-EF3636F43534']";
        private const string name_selector = 
            "fast:/sitecore/Layout//*[@@name='Print']";
        private const string key_selector = 
            "fast:/sitecore/Layout//*[@@key='print']";
        private const string templatename_selector = 
            "fast:/sitecore/Layout/Sublayouts//*[@@templatename='Sublayout']";
        private const string templateid_selector = 
            "fast:/sitecore/Layout//*[@@templateid='B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B']";
        private const string templateid_with_braces = 
            "fast:/sitecore/Layout//*[@@templateid='{B6F7EEB4-E8D7-476F-8936-5ACE6A76F20B}']";
        private const string templatekey_selector = 
            "fast:/sitecore/Layout/Sublayouts//*[@@templatekey='sublayout']";
        private const string parentid_selector = 
            "fast:/sitecore/Layout//*[@@parentid='{E18F4BC6-46A2-4842-898B-B6613733F06F}']";

        public enum Database
        {
            Mongo, SqlServer
        }

        [SetUp]
        public void SetUp()
        {
            TransferUtil.TransferPath("/sitecore/layout", _sourceDatabase, _targetDatabase, null);
        }

        [Test]

        [TestCase(Database.Mongo, fast_sitecore_NotReal, Result = 0)]
        [TestCase(Database.SqlServer, fast_sitecore_NotReal, Result = 0)]

        [TestCase(Database.Mongo, fast_sitecore_Layout, Result = 1)]
        [TestCase(Database.SqlServer, fast_sitecore_Layout, Result = 1)]

        [TestCase(Database.Mongo, fast_sitecore_Layout_asterisk, Result = 8)]
        [TestCase(Database.SqlServer, fast_sitecore_Layout_asterisk, Result = 8)]

        [TestCase(Database.Mongo, id_with_curly_braces, Result = 1)]
        [TestCase(Database.SqlServer, id_with_curly_braces, Result = 1)]

        [TestCase(Database.Mongo, id_without_curly_braces, Result = 1)]
        [TestCase(Database.SqlServer, id_without_curly_braces, Result = 1)]

        [TestCase(Database.Mongo, name_selector, Result = 1)]
        [TestCase(Database.SqlServer,name_selector,Result = 1)]

        [TestCase(Database.Mongo, key_selector, Result = 1)]
        [TestCase(Database.SqlServer, key_selector, Result = 1)]

        [TestCase(Database.Mongo, templatename_selector, Result = 2)]
        [TestCase(Database.SqlServer, templatename_selector, Result = 2)]

        [TestCase(Database.Mongo, templateid_selector, Result = 3)]
        [TestCase(Database.SqlServer, templateid_selector, Result = 3)]

        [TestCase(Database.Mongo, templateid_with_braces, Result = 3)]
        [TestCase(Database.SqlServer, templateid_with_braces, Result = 3)]

        [TestCase(Database.Mongo, templatekey_selector, Result = 2)]
        [TestCase(Database.SqlServer, templatekey_selector, Result = 2)]

        [TestCase(Database.Mongo, parentid_selector, Result = 3)]
        [TestCase(Database.SqlServer, parentid_selector, Result = 3)]

        public int TestCountOfReturnedItemsForQuery(Database testdb, string query)
        {
            var db = GetTestDatabase(testdb);
            Item[] items = db.SelectItems(query);
            return items.Length;
        }
        //TODO @@masterid, 
        private Sitecore.Data.Database GetTestDatabase(Database testdb)
        {
            switch (testdb)
            {
                case Database.Mongo:
                    return _targetDatabase;
                case Database.SqlServer:
                    return _sourceDatabase;
                default:
                    throw new ArgumentOutOfRangeException("testdb");
            }
        }


    }
}
