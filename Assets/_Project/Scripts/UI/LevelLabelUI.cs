using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class LevelLabelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _label;

    [SerializeField] private string _prefix = "Level ";


    private void Awake()
    {
        if (_label == null)
        {
            _label = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (_label == null)
        {
            return;
        }

        int level = 1;


        if (GameManager.Instance != null)
        {
            level = Mathf.Max(1, GameManager.Instance.CurrentLevel);
        }
        else
        {
            Scene scene = SceneManager.GetActiveScene();
            level = Mathf.Max(1, scene.buildIndex);
        }

        _label.text = _prefix + level.ToString();

    }

}
