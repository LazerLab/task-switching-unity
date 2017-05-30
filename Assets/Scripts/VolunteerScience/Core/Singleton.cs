/*
 * Author(s): Isaiah Mann
 * Description: Defines a singleton MonoBehaviour object (only one can exist at a time)
 * Usage: [no notes]
 */
using UnityEngine;

namespace VolunteerScience
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

        public static bool HasInstance
        {
            get
            {
                return _instance != null;
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

        protected virtual void Awake()
        {
            if(_instance == null)
            {
                _instance = this as T;
            }
        }

	}
}
