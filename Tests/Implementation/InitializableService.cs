using System;
using Zenvin.Services.Core;

namespace Zenvin.Services.Tests.Implementation
{
	internal class InitializableService : IInitializable
	{
		public Action InitCallback { get; set; }


		void IInitializable.Initialize (IScopeKey scope)
		{
			InitCallback?.Invoke ();
		}
	}
}
