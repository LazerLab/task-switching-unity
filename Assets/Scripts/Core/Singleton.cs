/*
 * Author(s): Isaiah Mann
 * Description: Defines a singleton MonoBehaviour object (only one can exist at a time)
 * Usage: [no notes]
 */
using UnityEngine;

namespace VSDataCollector
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		const string INSTANCE_KEY = "Instance";

		public static T Get
		{
			get
			{
				if(_instance == null)
				{
					_instance = createInstance();
				}
				return _instance;
			}
		}

		static T _instance;

		// Creates a new instance in the game world
		static T createInstance() 
		{
			GameObject singletonGameObject = new GameObject();
			DontDestroyOnLoad(singletonGameObject);
			singletonGameObject.name = string.Format("{0}_{1}", typeof(T).ToString(), INSTANCE_KEY);
			return singletonGameObject.AddComponent<T>();
		}

	}
}
