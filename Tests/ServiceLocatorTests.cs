using NUnit.Framework;
using Zenvin.ServiceLocator;

public partial class ServiceLocatorTests {
	[Test]
	public void GlobalServiceRegistrationSucceeds () {
		ServiceLocator.ResetAll ();

		// Arrange
		var locator = new ServiceLocator ();
		var service = new TestService ();
		var success = true;

		// Act
		locator.Register (service);
		locator.Get (out service, delegate { success = false; });

		// Assert
		Assert.NotNull (service);
		Assert.IsTrue (success);
	}

	[Test]
	public void ContextualizedServiceRegistrationSucceeds () {
		ServiceLocator.ResetAll ();

		// Arrange
		var service = new TestService ();
		var context = new TestContext (1);
		var success = true;

		// Act
		ServiceLocator.ForContext (context).Register (service);
		ServiceLocator.ForContext (context).Get (out service, delegate { success = false; });

		// Assert
		Assert.NotNull (service);
		Assert.IsTrue (success);
	}

	[Test]
	public void ContextAccessActivatesProxy () {
		ServiceLocator.ResetAll ();

		// Arrange
		var context = new TestContext (1);

		// Act
		var locator = ServiceLocator.ForContext(context);

		// Assert
		Assert.IsInstanceOf<ServiceLocator.Proxy> (locator);
	}
}
