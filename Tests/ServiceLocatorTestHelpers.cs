using System;
using Zenvin.Services.Core;

namespace Zenvin.Services.Tests
{
	internal static class ServiceLocatorTestHelpers
	{
		public static void BuildEmptyScope (ServiceScopeBuilder builder) { }

		public static void BuildNonEmptyScope (ServiceScopeBuilder builder, params object[] values)
		{
			foreach (var value in values)
			{
				builder.RegisterInstance (value);
			}
		}

		public static void BuildNonEmptyScope (ServiceScopeBuilder builder, params (Type contract, object instance)[] values)
		{
			foreach (var value in values)
			{
				builder.RegisterInstance (value.contract, value.instance);
			}
		}

		public static void BuildNonEmptyScope (ServiceScopeBuilder builder, IScopeKey parent, params (Type contract, object instance)[] values)
		{
			builder.SetParent (parent);
			foreach (var value in values)
			{
				builder.RegisterInstance (value.contract, value.instance);
			}
		}
	}
}
