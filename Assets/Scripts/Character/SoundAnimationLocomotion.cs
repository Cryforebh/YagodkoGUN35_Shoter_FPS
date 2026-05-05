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
}
