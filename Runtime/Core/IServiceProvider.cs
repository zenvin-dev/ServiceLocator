using System;

namespace Zenvin.Services.Core
{
	internal interface IServiceProvider : IDisposable
	{
		bool IsValid { get; }

		void Initialize (IScopeKey scope);
		object Get ();
	}
}
