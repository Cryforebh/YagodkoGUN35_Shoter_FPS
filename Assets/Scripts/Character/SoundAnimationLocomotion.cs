using UnityEngine;

public class SoundAnimationLocomotion : MonoBehaviour
{
    [SerializeField] private AudioSource _sourceLeg;
    [SerializeField] private AudioSource _sourceBody;
    [SerializeField] private AudioClip[] _walkLegsSounds;
    [SerializeField] private AudioClip[] _sprintLegsSounds;
    [SerializeField] private AudioClip _jumpSound;
    [SerializeField] private AudioClip _jumpEndSound;
    [SerializeField] private AudioClip _deadSound;


    private bool _isJump = false;

    private void Start()
    {
        _sourceLeg.playOnAwake = false;
    }

    public void AnimationWalkSound(string nameStageAnim)
    {
        if (_isJump) return;

        if (nameStageAnim == "LegLeft")
        {
            if (_sourceLeg.isPlaying)
                _sourceLeg.Stop();
            _sourceLeg.clip = _walkLegsSounds[0];
            _sourceLeg.Play();
        }
        if (nameStageAnim == "LegRight")
        {
            if (_sourceLeg.isPlaying)
                _sourceLeg.Stop();
            _sourceLeg.clip = _walkLegsSounds[1];
            _sourceLeg.Play();
        }
    }

    public void AnimationSprintSound(string nameStageAnim)
    {
        if (_isJump) return;

        if (nameStageAnim == "LegLeft")
        {
            if (_sourceLeg.isPlaying)
                _sourceLeg.Stop();
            _sourceLeg.clip = _sprintLegsSounds[0];
            _sourceLeg.Play();
        }
        if (nameStageAnim == "LegRight")
        {
            if (_sourceLeg.isPlaying)
                _sourceLeg.Stop();
            _sourceLeg.clip = _sprintLegsSounds[1];
            _sourceLeg.Play();
        }
    }

    public void PlaySoundJumpStart()
    {
        if (_jumpSound && !_isJump)
        {
            _isJump = true;
            if (_sourceLeg.isPlaying)
                _sourceLeg.Stop();
            _sourceLeg.clip = _jumpSound;
            _sourceLeg.Play();
        }
    }

    public void PlaySoundJumpEnd()
    {
        _isJump = false;
        if (!_jumpEndSound) return;
        if (_sourceLeg.isPlaying)
            _sourceLeg.Stop();
        _sourceLeg.clip = _jumpEndSound;
        _sourceLeg.Play();
    }

    public void PlaySoundDead()
    {
        if (!_deadSound) return;
        if (_sourceBody.isPlaying)
            _sourceBody.Stop();
        _sourceBody.clip = _deadSound;
        _sourceBody.Play();
    }

    private void PlayingSetup(AudioSource source, AudioClip[] audioClips)
    {

        if (!source)
        {
            Debug.LogWarning("Звук не установлен!");
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
