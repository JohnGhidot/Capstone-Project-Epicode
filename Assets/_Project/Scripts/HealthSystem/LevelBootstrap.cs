using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LevelBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private bool _resetHealthOnStart = true;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_resetHealthOnStart == true)
        {
            if (_playerHealth == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _playerHealth = player.GetComponent<PlayerHealth>();
                }
            }

            if (_playerHealth != null)
            {
                _playerHealth.ResetToFull();
            }
        }
    }
}
