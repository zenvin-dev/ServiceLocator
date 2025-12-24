using System;
using UnityEngine;
using Zenvin.Services.Core;
using Zenvin.Services.Utility;
using ILogger = Zenvin.Services.Utility.ILogger;

namespace Zenvin.Services.Unity
{
	[DisallowMultipleComponent]
	[AddComponentMenu("Zenvin/Services/Service Proxy")]
	public sealed class ServiceProxyComponent : MonoBehaviour
	{
		public enum RegisterMode
		{
			Factory,
			Lazy,
			LazyPersistent,
		}

		[Flags]
		public enum RegisterContext
		{
			Editor = 1,
			DebugBuild = 2,
			FullBuild = 4,
		}

		[SerializeField] private Component serviceInstance;
		[SerializeField] private RegisterMode registerMode = RegisterMode.Factory;
		[SerializeField] private RegisterContext registerContext = RegisterContext.Editor | RegisterContext.DebugBuild | RegisterContext.FullBuild;


		internal ServiceScopeBuilder RegisterInstance (ServiceScopeBuilder builder, ILogger logger = null)
		{
			if (builder == null)
				return null;

			if (serviceInstance == null || serviceInstance == this || serviceInstance.gameObject != gameObject)
				return logger.LogErrorPassing("", builder);

			if (!GetShouldRegister ())
				return builder;

			var contractType = serviceInstance is IService service ? service.ContractType : serviceInstance.GetType ();
			if (contractType == null)
				return builder;

			return registerMode switch
			{
				RegisterMode.Factory => builder.RegisterFactory (contractType, serviceInstance),
				RegisterMode.Lazy => builder.RegisterLazy (contractType, serviceInstance, false),
				RegisterMode.LazyPersistent => builder.RegisterLazy (contractType, serviceInstance, true),
				_ => builder
			};
		}

		private bool GetShouldRegister ()
		{
			var editor = Application.isEditor;
			var debug = Debug.isDebugBuild;

			if ((registerContext & RegisterContext.DebugBuild) != 0 && !editor && debug)
				return true;
			if ((registerContext & RegisterContext.FullBuild) != 0 && !editor && !debug)
				return true;
			if ((registerContext & RegisterContext.Editor) != 0 && editor)
				return true;

			return false;
		}
	}
}
