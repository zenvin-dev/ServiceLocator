using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Zenvin.Services.Unity.Bootstrapping
{
	[DisallowMultipleComponent]
	[RequireComponent (typeof (Bootstrapper))]
	public abstract class BootstrapperHook : MonoBehaviour
	{
		public virtual UniTask Complete () => UniTask.CompletedTask;
		public virtual UniTask Fail (Exception e) => UniTask.CompletedTask;
	}
}
