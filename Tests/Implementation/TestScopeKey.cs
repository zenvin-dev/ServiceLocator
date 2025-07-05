using System;
using Zenvin.Services.Core;

namespace Zenvin.Services.Tests.Implementation
{
	internal readonly struct TestScopeKey : IScopeKey
	{
		public readonly int Key;

		public TestScopeKey (int key)
		{
			Key = key;
		}

		bool IEquatable<IScopeKey>.Equals (IScopeKey other)
		{
			return other is TestScopeKey key && key.Key == Key;
		}
	}
}
