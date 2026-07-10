using Cysharp.Threading.Tasks;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Pool;

namespace Zenvin.Services.Unity.Bootstrapping
{
	[DisallowMultipleComponent]
	public abstract class Bootstrapper : MonoBehaviour
	{
		public enum BootstrapTrigger
		{
			Awake,
			Start,
			Manual,
		}

		public enum BootstrapState
		{
			Idle,
			Running,
			Finished,
			Failed,
		}

		[SerializeField] private BatchedModuleCollection modules;
		[SerializeField] private BootstrapperHook hook = null;
		[SerializeField] private BootstrapTrigger trigger = BootstrapTrigger.Awake;
		[Space]
		[SerializeField] private ThreadPriority defaultPriority = ThreadPriority.BelowNormal;
		[SerializeField] private ThreadPriority initPriority = ThreadPriority.High;
		[Space]
		[SerializeField] private bool verboseLogging = false;


		public BootstrapState State { get; private set; } = BootstrapState.Idle;
		private protected virtual bool CanExecute => true;


		private void Awake ()
		{
			if (trigger == BootstrapTrigger.Awake)
			{
				_ = ExecuteInternal ();
			}
		}

		private void Start ()
		{
			if (trigger == BootstrapTrigger.Start)
			{
				_ = ExecuteInternal ();
			}
		}


		public async UniTask Execute ()
		{
			if (trigger == BootstrapTrigger.Manual)
			{
				_ = ExecuteInternal ();
			}
		}


		private protected abstract void Initialize (BatchedModuleCollection modules);


		private async UniTask ExecuteInternal ()
		{
			if (State != BootstrapState.Idle)
				return;
			if (!CanExecute)
				return;

			Debug.Log ("[Service Bootstrap] Executing bootstrapper");

			State = BootstrapState.Running;
			Application.backgroundLoadingPriority = initPriority;
			try
			{
				modules.Update (true);
				LogModules ();

				if (modules != null && modules.ModuleCount > 0)
				{
					await PreprocessModules ();
					Initialize (modules);
					await PostprocessModules ();
				}

				State = BootstrapState.Finished;
				if (hook != null)
				{
					await hook.Complete ();
				}

				Debug.Log ("[Service Bootstrap] Finished bootstrap process");
			}
			catch (Exception e)
			{
				Debug.LogError ($"[Service Bootstrap] An error occurred during execution: {e.Message}");

				State = BootstrapState.Failed;
				if (hook != null)
				{
					await hook.Fail (e);
				}
			}
			Application.backgroundLoadingPriority = defaultPriority;
		}

		private async UniTask PreprocessModules ()
		{
			Debug.Log ($"[Service Bootstrap] Preprocessing modules");

			using var _ = ListPool<UniTask>.Get (out var taskBatch);
			foreach (var batch in modules.IterateBatches ())
			{
				taskBatch.Clear ();
				foreach (var module in batch)
				{
					taskBatch.Add (module.Preprocess ());
				}
				await UniTask.WhenAll (taskBatch);
			}

			Debug.Log ($"[Service Bootstrap] Finished preprocessing modules");
		}

		private async UniTask PostprocessModules ()
		{
			Debug.Log ($"[Service Bootstrap] Postprocessing modules");

			using var _ = ListPool<UniTask>.Get (out var taskBatch);
			foreach (var batch in modules.IterateBatches ())
			{
				taskBatch.Clear ();
				foreach (var module in batch)
				{
					taskBatch.Add (module.Postprocess ());
				}
				await UniTask.WhenAll (taskBatch);
			}

			Debug.Log ($"[Service Bootstrap] Finished postprocessing modules");
		}

		private void LogModules ()
		{
			if (!verboseLogging)
				return;
			if (modules == null)
				return;

			var sb = new StringBuilder ();
			sb.AppendLine ("[Service Bootstrap] Module execution order:");

			int i = 0;
			foreach (var batch in modules.IterateBatches ())
			{
				sb.Append ("\tBatch ");
				sb.Append (i++);
				sb.AppendLine ();

				foreach (var module in batch)
				{
					sb.Append ("\t\t");
					sb.Append (module.GetType ().FullName);
					sb.Append (" ('");
					sb.Append (module.name);
					sb.Append ("', ");
#if UNITY_6000_5_OR_NEWER
					sb.Append (module.GetEntityId ().ToString());
#else
					sb.Append (module.GetInstanceID ());
#endif
					sb.Append (")");
					sb.AppendLine ();
				}
			}

			Debug.Log (sb.ToString ());
		}


		[Serializable]
		internal sealed class OrderedModule
		{
			[SerializeField] private BootstrapperModule module;
			[SerializeField] private int order;

			internal BootstrapperModule Module => module;
			internal int Order => order;
			internal bool Valid => module != null;


			internal OrderedModule (BootstrapperModule module, int order)
			{
				this.module = module;
				this.order = order;
			}
		}
	}
}
