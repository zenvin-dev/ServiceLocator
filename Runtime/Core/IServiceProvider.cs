using System;

namespace Zenvin.Services.Core
{
	internal interface IServiceProvider : IDisposable
	{
		bool IsValid { get; }

		object Get ();
	}
}
