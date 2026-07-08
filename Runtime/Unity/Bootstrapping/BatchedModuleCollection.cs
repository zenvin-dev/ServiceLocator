using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static Zenvin.Services.Unity.Bootstrapping.Bootstrapper;

namespace Zenvin.Services.Unity.Bootstrapping
{
	[Serializable]
	internal sealed class BatchedModuleCollection : IDisposable
	{
		private bool calculated;
		private List<ModuleBatch> batches;

		[SerializeField] private List<OrderedModule> originalModules;


		public int BatchCount { get; private set; }
		public int ModuleCount { get; private set; }
		internal ModuleBatch? this[int index] => batches?[index];


		public BatchedModuleCollection () { }
		internal BatchedModuleCollection (params OrderedModule[] modules) => originalModules = new (modules ?? Array.Empty<OrderedModule> ());


		public void Dispose ()
		{
			calculated = false;
			BatchCount = 0;
			ModuleCount = 0;

			if (batches == null)
				return;

			foreach (var batch in batches)
			{
				batch.Dispose ();
			}
			batches.Clear ();
		}


		public void Update (bool force = false)
		{
			if (calculated && !force)
				return;

			if (calculated)
				Dispose ();

			batches ??= new ();
			if (originalModules == null || originalModules.Count == 0)
				return;

			using var _ = HashSetPool<BootstrapperModule>.Get (out var visited);
			foreach (var module in originalModules)
				InsertModule (module, visited);
		}

		public IEnumerable<IReadOnlyCollection<BootstrapperModule>> IterateBatches ()
		{
			Update (false);
			foreach (var batch in batches)
			{
				yield return batch.List;
			}
		}

		public IEnumerable<BootstrapperModule> IterateFlat ()
		{
			Update (false);
			foreach (var batch in batches)
			{
				foreach (var module in batch.List)
				{
					yield return module;
				}
			}
		}


		private void InsertModule (OrderedModule module, HashSet<BootstrapperModule> visited)
		{
			if (module == null)
				return;
			if (!module.Valid)
				return;
			if (!visited.Add (module.Module))
				return;

			ModuleCount++;

			var index = FindBatchIndex (module.Order, out var addToGroup);
			if (addToGroup)
			{
				batches[index].List.Add (module.Module);
				return;
			}

			var batch = new ModuleBatch (module.Order);
			batch.List.Add (module.Module);
			batches.Insert (index, batch);

			BatchCount++;
		}

		private int FindBatchIndex (int order, out bool addToGroup)
		{
			addToGroup = false;
			for (int i = 0; i < batches.Count; i++)
			{
				var batchOrder = batches[i].Order;

				if (batchOrder > order)
					return i;
				if (batchOrder < order)
					continue;

				addToGroup = true;
				return i;
			}
			return batches.Count;
		}


		internal readonly struct ModuleBatch : IDisposable
		{
			public readonly List<BootstrapperModule> List;
			public readonly int Order;


			public ModuleBatch (int order)
			{
				List = ListPool<BootstrapperModule>.Get ();
				Order = order;
			}

			public void Dispose ()
			{
				List.Clear ();
				ListPool<BootstrapperModule>.Release (List);
			}
		}
	}
}
