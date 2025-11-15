using UnityEngine;

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private float _musicVolume = 0.5f;

    private AudioSource _audioSource;
    private static AudioManager _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _audioSource.clip = _musicClip;
        _audioSource.volume = _musicVolume;
        _audioSource.loop = true;
    }

    private void Start()
    {
        if (!_audioSource.isPlaying && _musicClip != null)
        {
            _audioSource.Play();
        }
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null) { return; }
        _audioSource.clip = newClip;
        _audioSource.Play();
    }

    public void SetVolume(float volume)
    {
        _audioSource.volume = Mathf.Clamp01(volume);
    }
}
