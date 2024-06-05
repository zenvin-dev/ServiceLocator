using NUnit.Framework;
using Zenvin.ServiceLocator;

public partial class ServiceLocatorTests {
	[Test]
	public void GlobalServiceRegistrationSucceeds () {
		// Arrange
		var service = new TestService ();
		var success = true;

		// Act
		ServiceLocator.Global.Register (service);
		ServiceLocator.Global.Get (out service, delegate { success = false; });

		// Assert
		Assert.NotNull (service);
		Assert.IsTrue (success);
	}

	[Test]
	public void ContextualizedServiceRegistrationSucceeds () {
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
}
