using UnityEngine;

public class DoorEnter : MonoBehaviour
{
    [SerializeField] private DoorTrigger _doorTrigger;
    [SerializeField] private AudioClip _doorClip;
    [SerializeField] private bool _isPrivate = false;

    private Animator _animator;
    private AudioSource _source;
    private bool _previousTriggerState;
    private int _isOpenHash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _isOpenHash = Animator.StringToHash("isOpen");
        _previousTriggerState = _doorTrigger.IsInTrigger();
        _doorTrigger.SetTriggerPrivate(_isPrivate);

        _source = GetComponent<AudioSource>();
        _source.clip = _doorClip;
        _source.loop = false;
        _source.playOnAwake = false;
    }

    private void Update()
    {
        bool currentTriggerState = _doorTrigger.IsInTrigger();

        if (currentTriggerState != _previousTriggerState)
        {
            _animator.SetBool(_isOpenHash, currentTriggerState);
            if (_doorClip != null)
            {
                if (_source.isPlaying)
                    _source.Stop();
                _source.Play();
            }
            _previousTriggerState = currentTriggerState;
        }
    }
}
