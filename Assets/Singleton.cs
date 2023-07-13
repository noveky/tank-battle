using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T _instance;

	private static object _lock = new object();

	//public void GetSingleton() { }
	public T GetSingleton() { return instance; }

	public static bool hasInstance { get => _instance != null; }

	public static bool isGeneratingInstance = false;

	public static T instance
	{
		get
		{
			/*if (applicationIsQuitting)
			{
				return null;
			}*/

			lock (_lock)
			{
				if (_instance == null)
				{
					_instance = (T)FindObjectOfType(typeof(T));

					if (FindObjectsOfType(typeof(T)).Length > 1)
					{
						Debug.Log("[0], Instance = " + _instance);
						return _instance;
					}

					if (_instance == null)
					{
						isGeneratingInstance = true;
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) " + typeof(T).ToString();
						isGeneratingInstance = false;

						//DontDestroyOnLoad(singleton);
					}
				}

				return _instance;
			}
		}
	}

	/*private static bool applicationIsQuitting = false;

	public void OnDestroy()
	{
		applicationIsQuitting = true;
	}*/
}
