using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AiRigControl : MonoBehaviour
{
    [SerializeField] private Rig _rig;
    [SerializeField, Range(0, 1)] private float _startWeightRig = 1f;
    [SerializeField] private GameObject _target;

    private bool _isLook = false;
    private float _currentWeightRig = 1f;
    private Vector3 _targetStartLookPosition;
    private Vector3 _currentTargetLookPosition;

    private void Start()
    {
        if (_target == null || _rig == null) return;

        _rig.weight = _currentWeightRig = _startWeightRig;
    }

    private void Update()
    {
        if (_target == null || _rig == null) return;

        _rig.weight = _currentWeightRig;

        if (_isLook)
            _target.transform.position = _currentTargetLookPosition;
    }

    public void SetWeight(float weight) => _currentWeightRig = weight;
    public float GetWeight() => _currentWeightRig;

    public void SetPositionLook(Vector3 position)
    {
        _isLook = true;
        SetWeight(1f);
        _currentTargetLookPosition = position + transform.up * 1.5f;
    }
    public void ResetPositionLook()
    {
        _isLook = false;
        SetWeight(0f);
        _currentTargetLookPosition = _targetStartLookPosition + transform.position;
    }
}
