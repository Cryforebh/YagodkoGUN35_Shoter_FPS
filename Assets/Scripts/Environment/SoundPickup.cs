using UnityEngine;

public class SoundPickup : MonoBehaviour
{
    [SerializeField] private AudioClip _sound;

    private AudioSource _source;
    private bool _isPlaying = false;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.clip = _sound;
        _source.playOnAwake = false;
        _source.loop = false;
    }

    public void PlaySound()
    {
        if (_source.isPlaying)
        {
            _isPlaying = false;
            _source.Stop();
        }
        if (_source && _sound)
        {
            _source.clip = _sound;
            _source.Play();
            _isPlaying = true;
        }
    }

    public bool IsEnd() => _isPlaying && !_source.isPlaying;
}
