using UnityEngine;

public class HenrysAudioManager : MonoBehaviour
{
    public static HenrysAudioManager mInstance;

    // AudioClips
    [field: SerializeField] public AudioClip Jump_SFX { get; protected set; }
    [field: SerializeField] public AudioClip Slide_SFX { get; protected set; }
    [field: SerializeField] public AudioClip Coin_SFX { get; protected set; }
    [field: SerializeField] public AudioClip Run_SFX { get; protected set; }
    [field: SerializeField] public AudioClip TakeDamage_SFX { get; protected set; }
    [field: SerializeField] public AudioClip TakeDamage2_SFX { get; protected set; }
    [field: SerializeField] public AudioClip BackgroundMusic_SFX { get; protected set; }

    // AudioSource
    private AudioSource audioSource;

    private void Awake()
    {
        mInstance = this;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void PlaySound(AudioClip clip, Transform positionToPlay)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Audio clip is null!");
        }
    }

    public void PlayJumpSound(GameObject _go, Transform positionToPlay)
    {
        PlaySound(Jump_SFX, positionToPlay);
    }

    public void PlaySlideSound(GameObject _go, Transform positionToPlay)
    {
        PlaySound(Slide_SFX, positionToPlay);
    }

    public void PlayCoinSound(GameObject _go, Transform positionToPlay)
    {
        PlaySound(Coin_SFX, positionToPlay);
    }

    public void PlayRunSound(GameObject _go, Transform positionToPlay)
    {
        PlaySound(Run_SFX, positionToPlay);
    }

    public void PlayTakeDamageSound(GameObject _go, Transform positionToPlay)
    {
        PlaySound(TakeDamage_SFX, positionToPlay);
    }

    public void PlayTakeDamage2Sound(GameObject _go, Transform positionToPlay)
    {
        PlaySound(TakeDamage2_SFX, positionToPlay);
    }

    public void PlayBackgroundMusic(GameObject _go, Transform positionToPlay)
    {
        PlaySound(BackgroundMusic_SFX, positionToPlay);
    }

}
