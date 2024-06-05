namespace Zenvin.ServiceLocator {
	public interface IService {
		/// <summary> 
		/// Called when the <see cref="IService"/> instance is registered with an <see cref="IServiceLocator"/>. 
		/// </summary>
		void OnRegister ();
		/// <summary> 
		/// Called when the <see cref="IService"/> instance is unregistered from an <see cref="IServiceLocator"/>.
		/// </summary>
		/// <param name="replaced">
		/// Will be <see langword="true"/> when the instance is unregistered because another instance of the same type replaced it.
		/// </param>
		void OnUnregister (bool replaced);
		/// <summary>
		/// Called when the <see cref="IService"/> instance is cleared from an <see cref="IServiceLocator"/> through <see cref="IServiceLocator.Reset()"/>.
		/// </summary>
		void OnClear ();
	}
}