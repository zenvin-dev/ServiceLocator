using System;

namespace Zenvin.Services.Exceptions
{
	public class ServiceLocatorNotInitializedException : Exception
	{
		public ServiceLocatorNotInitializedException () : base ("ServiceLocator has not yet been initialized.") { }
	}
}
