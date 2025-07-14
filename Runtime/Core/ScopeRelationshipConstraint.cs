namespace Zenvin.Services.Core
{
	/// <summary>
	/// Enum to define the constraint applied to a parent reference when building a new scope.
	/// </summary>
	public enum ScopeRelationshipConstraint
	{
		/// <summary>
		/// The parent key is only a logical connection; the scope does not depend on its parent existing.
		/// </summary>
		Loose,
		/// <summary>
		/// The scope does not require its parent to exist when it is added, but once the parent exists the scope will depend on it.
		/// </summary>
		Hardened,
		/// <summary>
		/// The scope will not be added, if the parent scope does not exist, and will depend on its parent scope after being added.
		/// </summary>
		Required,
	}
}
