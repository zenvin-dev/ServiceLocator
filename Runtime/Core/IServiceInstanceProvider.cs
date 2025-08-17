using System;

namespace Zenvin.Services.Core
{
	internal interface IServiceInstanceProvider : IDisposable
	{
		bool IsValid { get; }

		void Initialize (IScopeKey scope) { }
		void InitializeLate (IScopeKey scope) { }
		object Get ();
	}
}
