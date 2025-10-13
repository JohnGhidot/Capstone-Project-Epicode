using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerHealth))]
public class DebugDamageOnKey : MonoBehaviour
{
    [SerializeField] private KeyCode _key = KeyCode.V;
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private bool _logToConsole = true;

    private PlayerHealth _playerHealth;

    private void Awake()
    {
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_key) == true)
        {
            if (_playerHealth != null && _playerHealth.IsDead == false)
            {
                _playerHealth.TakeDamage(_damageAmount, gameObject);

                if (_logToConsole == true)
                {
                    Debug.Log($"[DebugDamageOnKey] Danno: {_damageAmount}. HP: {_playerHealth.CurrentHP}/{_playerHealth.MaxhHP}");
                }
            }
        }
    }
}

