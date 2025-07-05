namespace Zenvin.Services.Core
{
	public interface IScopeContextProvider
	{
		IScopeKey GetActiveScope ();
	}
}
