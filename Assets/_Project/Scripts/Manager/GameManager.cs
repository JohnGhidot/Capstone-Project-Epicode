using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Progression")]
    [SerializeField] private int _startLevel = 1;
    [SerializeField] private bool _autoApplyOnSceneLoaded = true;

    [Header("Scenes Level Mapping")]
    [SerializeField] private bool _useBuildIndexAsLevel = false;
    [SerializeField] private int _levelOffsetForBuildIndex = 0;

    [Header("Player Options")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private bool _resetPlayerHealthOnSceneLoaded = true;

    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CurrentLevel = Mathf.Max(1, _startLevel);
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
        if (_useBuildIndexAsLevel)
        {
            int mapped = scene.buildIndex + _levelOffsetForBuildIndex;
            CurrentLevel = Mathf.Max(1, mapped);
        }

        if (_resetPlayerHealthOnSceneLoaded)
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

        if (_autoApplyOnSceneLoaded)
        {
            ApplyDifficultyToAllEnemies();
        }
    }

    public void SetLevelAndReapply(int newLevel)
    {
        CurrentLevel = Mathf.Max(1, newLevel);
        ApplyDifficultyToAllEnemies();
    }

    public void AdvanceLevelAndReapply()
    {
        CurrentLevel = Mathf.Max(1, CurrentLevel + 1);
        ApplyDifficultyToAllEnemies();
    }

    public void ApplyDifficultyToAllEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>(true);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].SetupForLevel(CurrentLevel);                
            }
        }
    }

    public void ApplyDifficultyTo(Enemy enemy)
    {
        if (enemy == null)
        {
            return;
        }

        enemy.SetupForLevel(CurrentLevel);
    }
}