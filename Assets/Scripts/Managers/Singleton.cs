using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
	protected static T _instance;

	public static T Instance
	{
		get
		{
			if (applicationIsQuitting) {
				Debug.LogWarning( "Singleton of " + typeof(T) + " already destroyed on app quit." +
				                 " Don't create again." );
				return null;
			}

			if (_instance == null)
			{
				Debug.Log ("Singleton of " + typeof(T) + " is null.");
				_instance = (T) FindObjectOfType(typeof(T));

				if (FindObjectsOfType(typeof(T)).Length > 1)
				{
					Debug.LogError ("Error: More than 1 of a singleton.  This should not happen!");
					return _instance;
				}

				if (_instance == null)
				{
					GameObject go = new GameObject();
					_instance = go.AddComponent<T>();
					go.name = typeof(T).Name;

					Debug.Log ("Singleton of " + typeof(T) + " is created." + " Instance ID: " + go.GetInstanceID());
				}
				else
				{
					Debug.Log ("Singleton of " + typeof(T) + " assigned existing instance.");
				}
			}

			return _instance;
		}
	}

	private static bool applicationIsQuitting = false;

	void OnDestroy() {
		Debug.Log ("Singleton " + typeof(T) + " destroyed." + " Instance ID: " + gameObject.GetInstanceID());
		if (_instance == this.GetComponent<T> ())
		{
			applicationIsQuitting = true;
		}

		DoDestroy ();
	}

	// Manual destroy.  Wont set off application quit detection.
	public void Destroy()
	{
		if (_instance == this.GetComponent<T> ())
		{
			_instance = null;
		}

		Destroy (this.gameObject);
	}

	// Use this for initialization
	void Awake () {
		Debug.Log ("Singleton " + typeof(T) + " initializing." + " Instance ID: " + gameObject.GetInstanceID());
		DontDestroyOnLoad(gameObject);

		if (_instance == null ) {
			_instance = this.GetComponent<T> ();
		}
		else if (_instance != this.GetComponent<T> ())
		{
			Debug.Log ("Singleton " + typeof(T) + " already exists.  Destroy.");
			Destroy (gameObject);
		}

		DoAwake ();
	}

	protected virtual void DoAwake () {
	}

	protected virtual void DoDestroy () {
	}

    public static bool IsDestroy()
    {
        return applicationIsQuitting;
    }
}
