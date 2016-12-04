using System;
using System.Configuration;
using System.Linq;
using FakeItEasy;
using MongoDB.Driver;
using NUnit.Framework;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SitecoreData.DataProviders.MongoDB.Tests
{
	[TestFixture]
	[Category("MongoDB Data Transfer Tests")]
	internal class DataTransferTests : MongoTestsRefreshDbPerTest
	{

		[TearDown]
		public void ClearDataBase()
		{
			_db.Drop();
		}

		[Test]
		public void CanCreateTestDatabase()
		{
			Assert.That(_db, Is.Not.Null);
		}

		[Test]
		public void MongoDbHasItemsCollection()
		{
			TransferUtil.TransferPath("/sitecore/layout", _sqlServerSourceDb, _mongoTargetDb, null);
			Assert.That(_db.GetCollectionNames().Contains("items"), Is.True);
		}

		[Test]
		public void CanTransferData()
		{
			TransferUtil.TransferPath("/sitecore/layout", _sqlServerSourceDb, _mongoTargetDb, null);
			Assert.That(_db.GetCollection("items").Count(), Is.GreaterThanOrEqualTo(60));
		}

		[Test]
		public void CanTransferDeepHierarchy()
		{
			TransferUtil.TransferPath("/sitecore/layout/renderings", _sqlServerSourceDb, _mongoTargetDb, null);
			Assert.That(_db.GetCollection("items").Count(), Is.GreaterThanOrEqualTo(13));
		}

		[Test]
		public void CanAccessSitecoreLayoutItem()
		{
			Item item = Factory.GetDatabase("master").GetItem("/sitecore/layout");
			Assert.That(item.ID.ToString(), Is.EqualTo("{EB2E4FFD-2761-4653-B052-26A64D385227}"));
		}

		[Test]
		public void CallbackMethodCalled()
		{
			var action = A.Fake<Action<string>>();
			using (var scope = Fake.CreateScope())
			{
				TransferUtil.TransferPath("/sitecore/layout/devices/print", _sqlServerSourceDb, _mongoTargetDb, action);
				using (scope.OrderedAssertions())
				{
					A.CallTo(() => action.Invoke("/sitecore")).MustHaveHappened(Repeated.Exactly.Once);
					A.CallTo(() => action.Invoke("/sitecore/layout")).MustHaveHappened(Repeated.Exactly.Once);
					A.CallTo(() => action.Invoke("/sitecore/layout/Devices")).MustHaveHappened(Repeated.Exactly.Once);
					A.CallTo(() => action.Invoke("/sitecore/layout/Devices/Print")).MustHaveHappened(
						Repeated.Exactly.Once);
				}
			}
		}

		[Test]
		public void TranferUtil_WhenTransferingRoot_PopulatesFields()
		{
			TransferUtil.TransferPath("/sitecore/content", _sqlServerSourceDb, _mongoTargetDb, null);
			Item root = _mongoTargetDb.GetItem("/sitecore");
			Assert.IsNotEmpty(root[Sitecore.FieldIDs.Created]);
		}
	}
}