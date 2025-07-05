using NUnit.Framework;
using System;
using System.IO;
using Zenvin.Services.Core;
using Zenvin.Services.Exceptions;
using Zenvin.Services.Tests.Implementation;
using static Zenvin.Services.Tests.ServiceLocatorTestHelpers;

namespace Zenvin.Services.Tests
{
	public class ServiceLocatorTests
	{
		[Test]
		public void IsInitializedWithEmptyGlobalScope ()
		{
			// Act
			var init = ServiceLocator.Initialize (BuildEmptyScope).WasInitialized;

			// Assert
			Assert.IsTrue (init, "ServiceLocator should have been initialized");
		}

		[Test]
		public void CannotBeInitializedTwice ()
		{
			// Act
			ServiceLocator.Initialize (BuildEmptyScope);
			var init = ServiceLocator.Initialize (BuildEmptyScope).WasInitialized;

			// Assert
			Assert.IsFalse (init, "ServiceLocator should not be initialized twice");
		}

		[Test]
		public void ShouldHaveScopeProvider ()
		{
			// Arrange
			var provider = new TestScopeProvider ();

			// Act
			var init = ServiceLocator.Initialize (BuildEmptyScope)
				.WithScopeContextProvider(provider)
				.WasInitialized;

			// Assert
			Assert.IsTrue (init);
			Assert.AreEqual (provider, ServiceLocator.ScopeContextProvider);
		}

		[Test]
		public void AddScopeShouldAddFilledScope ()
		{
			// Arrange
			var key = new TestScopeKey (0);

			// Act
			var init = ServiceLocator.Initialize (BuildEmptyScope).WasInitialized;
			var added = ServiceLocator.AddScope (key, builder => BuildNonEmptyScope (builder, "Test"));

			// Assert
			Assert.IsTrue (init, "ServiceLocator should have been initialized");
			Assert.IsTrue (added, "Filled scope should have been added");
			Assert.AreEqual (2, ServiceLocator.ActiveScopeCount, "ServiceLocator should contain two scopes");
		}

		[Test]
		public void AddScopeShouldNotAddEmptyScope ()
		{
			// Arrange
			var key = new TestScopeKey (0);
			var init = ServiceLocator.Initialize (BuildEmptyScope).WasInitialized;

			// Act
			var added = ServiceLocator.AddScope (key, BuildEmptyScope);

			// Assert
			Assert.IsTrue (init, "ServiceLocator should have been initialized");
			Assert.IsFalse (added, "Empty scope should not have been added");
			Assert.AreEqual (1, ServiceLocator.ActiveScopeCount, "ServiceLocator should only contain the global scope");
		}

		[Test]
		public void RemoveScopeShouldRemoveScope ()
		{
			// Arrange
			var key = new TestScopeKey (0);
			var init = ServiceLocator.Initialize (BuildEmptyScope).WasInitialized;
			var added = ServiceLocator.AddScope (key, builder => BuildNonEmptyScope (builder, "Test"));

			// Act
			var removed = ServiceLocator.RemoveScope (key);

			// Assert
			Assert.IsTrue (init, "ServiceLocator should have been initialized");
			Assert.IsTrue (added, "Filled scope should have been added");
			Assert.IsTrue (removed, "Scope should have been removed");
			Assert.AreEqual (1, ServiceLocator.ActiveScopeCount, "ServiceLocator should only contain the global scope");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromGlobalScopeDirectAndInstanceType ()
		{
			// Arrange
			const string inValue = "rlkgnilrfgn";
			ServiceLocator.Initialize (builder => BuildNonEmptyScope (builder, inValue));

			// Act
			var get = ServiceLocator.TryGet (out string outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromGlobalScopeDirectAndContractType ()
		{
			// Arrange
			var inValue = new StringReader ("");
			ServiceLocator.Initialize (builder => BuildNonEmptyScope (builder, (typeof (IDisposable), inValue)));

			// Act
			var get = ServiceLocator.TryGet<IDisposable, StringReader> (out var outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromCustomScopeDirectAndInstanceType ()
		{
			// Arrange
			const string inValue = "dxgbkcg";
			var key = new TestScopeKey (1);
			ServiceLocator.Initialize (BuildEmptyScope);
			ServiceLocator.AddScope (key, builder => BuildNonEmptyScope (builder, inValue));

			// Act
			var get = ServiceLocator.TryGet (key, out string outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromCustomScopeDirectAndContractType ()
		{
			// Arrange
			var inValue = new StringReader ("");
			var key = new TestScopeKey (1);
			ServiceLocator.Initialize (BuildEmptyScope);
			ServiceLocator.AddScope (key, builder => BuildNonEmptyScope (builder, (typeof (IDisposable), inValue)));

			// Act
			var get = ServiceLocator.TryGet<IDisposable, StringReader> (key, out var outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromGlobalScopeCascadingAndInstanceType ()
		{
			// Arrange
			const string inValue = "dxgbkcg";
			var key0 = new TestScopeKey (0);
			ServiceLocator.Initialize (BuildEmptyScope);
			ServiceLocator.AddScope (key0, builder => BuildNonEmptyScope (builder, key0, inValue));

			// Act
			var get = ServiceLocator.TryGet (key0, out string outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromGlobalScopeCascadingAndContractType ()
		{
			// Arrange
			var inValue = new StringReader ("");
			var key0 = new TestScopeKey (0);
			ServiceLocator.Initialize (BuildEmptyScope);
			ServiceLocator.AddScope (key0, builder => BuildNonEmptyScope (builder, key0, (typeof (IDisposable), inValue)));

			// Act
			var get = ServiceLocator.TryGet<IDisposable, StringReader> (key0, out var outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromCustomScopeCascadingAndInstanceType ()
		{
			// Arrange
			const string inValue = "dxgbkcg";
			var key0 = new TestScopeKey (0);
			var key1 = new TestScopeKey (1);
			ServiceLocator.Initialize (BuildEmptyScope);
			ServiceLocator.AddScope (key0, builder => BuildNonEmptyScope (builder, 0));
			ServiceLocator.AddScope (key1, builder => BuildNonEmptyScope (builder, key0, inValue));

			// Act
			var get = ServiceLocator.TryGet (key1, out string outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void TryGetReturnsExpectedValueFromCustomScopeCascadingAndContractType ()
		{
			// Arrange
			var inValue = new StringReader ("");
			var key0 = new TestScopeKey (0);
			var key1 = new TestScopeKey (1);
			ServiceLocator.Initialize (BuildEmptyScope);
			ServiceLocator.AddScope (key0, builder => BuildNonEmptyScope (builder, "sfjhfshkjb"));
			ServiceLocator.AddScope (key1, builder => BuildNonEmptyScope (builder, key0, (typeof (IDisposable), inValue)));

			// Act
			var get = ServiceLocator.TryGet<IDisposable, StringReader> (key1, out var outValue);

			// Assert
			Assert.IsTrue (get, "TryGet should have been successful");
			Assert.AreEqual (inValue, outValue, "In- and Out-Values should be equal");
		}

		[Test]
		public void GetThrowsWhenUnresolvedInstanceTypeImplicit ()
		{
			// Arrange
			ServiceLocator.Initialize (BuildEmptyScope);

			// Act & Assert
			Assert.Throws<ServiceException> (() => ServiceLocator.Get<string> ());
		}

		[Test]
		public void GetThrowsWhenUnresolvedInstanceTypeExplicit ()
		{
			// Arrange
			ServiceLocator.Initialize (BuildEmptyScope);

			// Act & Assert
			Assert.Throws<ServiceException> (() => ServiceLocator.Get<string> (true));
		}

		[Test]
		public void GetThrowsWhenUnresolvedContractTypeImplicit ()
		{
			// Arrange
			ServiceLocator.Initialize (BuildEmptyScope);

			// Act & Assert
			Assert.Throws<ServiceException> (() => ServiceLocator.Get<IDisposable, StringReader> ());
		}

		[Test]
		public void GetThrowsWhenUnresolvedContractTypeExplicit ()
		{
			// Arrange
			ServiceLocator.Initialize (BuildEmptyScope);

			// Act & Assert
			Assert.Throws<ServiceException> (() => ServiceLocator.Get<IDisposable, StringReader> (true));
		}

		[Test]
		public void ServiceLocationStartsAtActiveScopeByDefault ()
		{
			// Arrange
			var key0 = new TestScopeKey (0);
			var provider = new TestScopeProvider () { CurrentKey = key0 };
			const string expectedValue = "expected";
			const string fakeValue = "fake";
			ServiceLocator.Initialize (builder => BuildNonEmptyScope (builder, fakeValue))
				.WithScopeContextProvider(provider);
			ServiceLocator.AddScope (key0, builder => BuildNonEmptyScope (builder, expectedValue));

			// Act
			var get = ServiceLocator.TryGet (out string value);

			// Assert
			Assert.AreEqual (ServiceLocator.ScopeContextProvider, provider);
			Assert.AreEqual (expectedValue, value);
		}


		[TearDown]
		public void Teardown ()
		{
			ServiceLocator.Reset ();
		}
	}
}
