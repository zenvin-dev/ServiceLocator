using NUnit.Framework;
using System;
using Zenvin.Services.Core;
using Zenvin.Services.Providers;

namespace Zenvin.Services.Tests
{
	public class InstanceProviderTests
	{
		[Test]
		public void ReferenceProviderShouldAlwaysReturnSameInstance ()
		{
			// Arrange
			var instance = new object ();
			var provider = new ReferenceProvider (instance);

			// Act
			var ins0 = Helper_Get (provider);
			var ins1 = Helper_Get (provider);

			// Assert
			Assert.AreEqual (ins0, ins1);
		}

		[Test]
		public void LazyProviderShouldAlwaysReturnInstance ()
		{
			// Arrange
			var provider = new LazyProvider<object> (() => new object ());

			// Act
			var ins = Helper_Get (provider);

			// Assert
			Assert.IsNotNull (ins);
		}

		[Test]
		public void LazyProviderShouldAlwaysReturnSameInstance ()
		{
			// Arrange
			var provider = new LazyProvider<object> (() => new object ());

			// Act
			var ins0 = Helper_Get (provider);
			var ins1 = Helper_Get (provider);

			// Assert
			Assert.AreEqual (ins0, ins1);
		}

		[Test]
		public void FactoryProviderShouldAlwaysReturnNewInstance ()
		{
			// Arrange
			var provider = new FactoryProvider<Guid> (Guid.NewGuid);

			// Act
			var ins0 = Helper_Get (provider);
			var ins1 = Helper_Get (provider);

			// Assert
			Assert.AreNotEqual (ins0, ins1);
		}


		private static object Helper_Get (IServiceInstanceProvider provider)
		{
			return provider.Get ();
		}
	}
}
