using UnityEngine;

public class MusicPlaying : MonoBehaviour
{
    [SerializeField] private AudioClip[] _musics;

    private AudioSource _sourceMusic;
    private int index;

    private void Start()
    {
        _sourceMusic = GetComponent<AudioSource>();
        _sourceMusic.loop = false;
        _sourceMusic.playOnAwake = false;
    }

    private void Update()
    {
        index = Random.Range(0, _musics.Length);
        if (!_sourceMusic.isPlaying && _musics.Length > 0)
        {
            _sourceMusic.clip = _musics[index];
            _sourceMusic.Play();
        }
    }
}
