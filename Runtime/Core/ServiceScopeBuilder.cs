using System;
using UnityEngine;
using Zenvin.Services.Providers;

namespace Zenvin.Services.Core
{
	public sealed class ServiceScopeBuilder
	{
		private readonly ServiceScope scope;
		private readonly bool isGlobal;

		private bool wasBuilt;


		private ServiceScopeBuilder ()
		{
			scope = new ServiceScope ();
		}

		internal ServiceScopeBuilder (bool isGlobal = false) : this ()
		{
			this.isGlobal = isGlobal;
		}


		internal ServiceScope Build ()
		{
			wasBuilt = true;
			return scope;
		}

		private void AssertWasNotBuilt ()
		{
			if (wasBuilt)
			{
				throw new InvalidOperationException (
					"The ServiceScopeBuilder already finished building its associated ServiceScope instance and may not be modified anymore."
				);
			}
		}
	}
}
