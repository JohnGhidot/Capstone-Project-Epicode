using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class HealthBarDriver : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _label;

    public void SetBoth(int current, int max)
    {
        if (_slider != null)
        {
            if (max < 1) { max = 1; }
            _slider.minValue = 0;
            _slider.maxValue = max;
            _slider.value = Mathf.Clamp(current, 0, max);
        }
        UpdateLabel();
    }

    public void SetCurrent(int current)
    {
        if (_slider != null)
        {
            int max = Mathf.RoundToInt(_slider.maxValue);
            _slider.value = Mathf.Clamp(current, 0, max);
        }
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (_label == null) { return; }
        if (_slider == null) { return; }

        int cur = Mathf.RoundToInt(_slider.value);
        int max = Mathf.RoundToInt(_slider.maxValue);
        _label.text = $"HP: {cur}/{max}";
    }
}
