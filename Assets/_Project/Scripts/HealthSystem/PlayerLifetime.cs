using UnityEngine;

[DisallowMultipleComponent]
public class PlayerLifetime : MonoBehaviour
{
    private static bool _exists;

    private void Awake()
    {
        if (_exists == true)
        {
            Destroy(gameObject);
            return;
        }

        _exists = true;
        DontDestroyOnLoad(gameObject);
    }
}
