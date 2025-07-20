using System;

namespace Zenvin.Services.Core
{
	internal interface IServiceInstanceProvider : IDisposable
	{
		bool IsValid { get; }

		void Initialize (IScopeKey scope);
		object Get ();
	}
}
