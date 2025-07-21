using System;
using Zenvin.Services.Core;
using Object = UnityEngine.Object;

namespace Zenvin.Services.Unity
{
	public static class UnityProviderExtensions
	{
		public static ServiceScopeBuilder RegisterFactory<TInstance> (this ServiceScopeBuilder builder, TInstance prefab)
			where TInstance : Object
		{

			return RegisterFactory (builder, typeof (TInstance), prefab);
		}

		public static ServiceScopeBuilder RegisterFactory<TContract, TInstance> (this ServiceScopeBuilder builder, TInstance prefab)
			where TInstance : Object, TContract
		{
			return RegisterFactory (builder, typeof (TContract), prefab);
		}

		public static ServiceScopeBuilder RegisterFactory (this ServiceScopeBuilder builder, Type contractType, Object prefab)
		{
			if (prefab != null)
			{
				var provider = new UnityObjectFactoryProvider<Object> (prefab);
				builder.RegisterProviderRaw (contractType, provider);
			}
			return builder;
		}
	}
}
