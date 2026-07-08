using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenvin.Services.Core;

namespace Zenvin.Services.Unity
{
	public abstract class BootstrapperModule : MonoBehaviour
	{

		internal protected virtual UniTask Postprocess () => UniTask.CompletedTask;
		internal protected virtual void Execute (ServiceScopeBuilder builder) { }
		internal protected virtual UniTask Preprocess () => UniTask.CompletedTask;
	}
}
