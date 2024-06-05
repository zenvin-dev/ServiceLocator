using System;
using Zenvin.ServiceLocator;

public partial class ServiceLocatorTests {
	public readonly struct TestContext : IServiceContext {
		public readonly int Id;

		public TestContext (int id) {
			Id = id;
		}

		bool IEquatable<IServiceContext>.Equals (IServiceContext other) {
			return other is TestContext context && context.Id == Id;
		}
	}
}
