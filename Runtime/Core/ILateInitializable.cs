namespace Zenvin.Services.Core
{
	public interface ILateInitializable
	{
		void InitializeLate (IScopeKey scope);
	}
}
