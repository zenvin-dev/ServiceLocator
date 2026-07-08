using NUnit.Framework;
using UnityEngine;
using Zenvin.Services.Unity;
using Zenvin.Services.Unity.Bootstrapping;

namespace Zenvin.Services.Tests
{
	public class BatchedModuleCollectionTests
	{
		private BatchedModuleCollection coll;
		private GameObject container;


		[SetUp]
		public void Setup ()
		{
			container = new GameObject ("Module Container") { hideFlags = HideFlags.DontSave };
		}

		[TearDown]
		public void Teardown ()
		{
			coll?.Dispose ();
			Object.DestroyImmediate (container);
		}


		[Test]
		public void Batching_BatchesSameLoadOrder ()
		{
			// Arrange
			coll = new (
				new Bootstrapper.OrderedModule (CreateModule (), 0),
				new Bootstrapper.OrderedModule (CreateModule (), 1),
				new Bootstrapper.OrderedModule (CreateModule (), 1)
			);

			// Act
			coll.Update ();

			// Assert
			Assert.AreEqual (2, coll.BatchCount);
			Assert.AreEqual (3, coll.ModuleCount);
			Assert.AreEqual (1, coll[0].Value.List.Count);
			Assert.AreEqual (2, coll[1].Value.List.Count);
		}

		[Test]
		public void Batching_IgnoresNullReferences ()
		{
			// Arrange
			coll = new (
				new Bootstrapper.OrderedModule (null, 0),
				new Bootstrapper.OrderedModule (CreateModule (), 0)
			);

			// Act
			coll.Update ();

			// Assert
			Assert.AreEqual (1, coll.BatchCount);
			Assert.AreEqual (1, coll.ModuleCount);
		}

		[Test]
		public void Batching_RemovesDuplicates ()
		{
			// Arrange
			var module = CreateModule ();
			coll = new (
				new Bootstrapper.OrderedModule (module, 0),
				new Bootstrapper.OrderedModule (module, 1),
				new Bootstrapper.OrderedModule (CreateModule (), 2)
			);

			// Act
			coll.Update ();

			// Assert
			Assert.AreEqual (2, coll.BatchCount);
			Assert.AreEqual (2, coll.ModuleCount);
			Assert.Contains (module, coll[0].Value.List);
		}

		[Test]
		public void Batching_SortsCorrectly ()
		{
			// Arrange
			coll = new (
				new Bootstrapper.OrderedModule (CreateModule (), 5),
				new Bootstrapper.OrderedModule (CreateModule (), 1)
			);

			// Act
			coll.Update ();

			// Assert
			Assert.AreEqual (2, coll.BatchCount);
			Assert.Greater (coll[1].Value.Order, coll[0].Value.Order);
		}


		private BootstrapperModule CreateModule () => container.AddComponent<BootstrapperModuleTestImplementation> ();
	}
}
