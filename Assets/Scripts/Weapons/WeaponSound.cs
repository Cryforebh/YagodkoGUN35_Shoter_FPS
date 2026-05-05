using UnityEngine;

public class WeaponSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] _audioClips;
    [SerializeField] private AudioClip[] _audioClipsRecoil;
    [SerializeField] private AudioClip[] _audioClipsEquip;

    private AudioSource[] _allSource;
    private AudioSource _source;
    private AudioSource _sourceRecoil;

    private void Start()
    {
        _allSource = GetComponentsInChildren<AudioSource>();
        foreach (AudioSource source in _allSource)
        {
            if (source.name == "SoundRecoil")
                _sourceRecoil = source;
            switch (source.name)
            {
                case "SoundRecoil":
                    _sourceRecoil = source;
                    break;
                case "SoundAttack":
                    _source = source;
                    break;
                default:
                    Debug.LogError("AudioSource не найден, либо имя обьекта нет в реестре!");
                    break;
            }
        }
        _source.playOnAwake = false;
    }

    public void PlaySoundAttack()
    {
        PlayingSetup(_source, _audioClips);
    }

    public void PlaySoundRecoil()
    {
        PlayingSetup(_sourceRecoil, _audioClipsRecoil);
    }

    public void PlaySoundEquip()
    {
        PlayingSetup(_sourceRecoil, _audioClipsEquip);
    }

    private void PlayingSetup(AudioSource source, AudioClip[] audioClips)
    {

        if (!source)
        {
            //Debug.LogWarning("Звук не установлен!");
            return;
        }
        if (source.isPlaying)
            source.Stop();
        var index = IndexSetup(audioClips);
        if (index < 0) return;
        source.clip = audioClips[index];
        source.Play();
    }

    private int IndexSetup(AudioClip[] audioClips)
    {
        if (audioClips.Length > 0)
            return Random.Range(0, audioClips.Length);
        else
        {
            Debug.LogError("Звуков не найдено!");
            return -1;
        }
    }
}
