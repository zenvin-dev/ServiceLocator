using System;

namespace Zenvin.Services.Core
{
	internal interface IServiceProvider : IDisposable
	{
		object Get ();
	}
}
