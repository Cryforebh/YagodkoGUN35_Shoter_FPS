using UnityEngine;

public class CharacterAiming : MonoBehaviour
{
    [SerializeField] private float _cameraBottomClamp = -30;
    [SerializeField] private float _cameraTopClamp = 40;
    [SerializeField] private float _cameraAngleOverride;
    [SerializeField] private float _speedRotation = 1f;
    [SerializeField] private float _speedAimRotation = 0.5f;

    public Transform cameraLookAt;
    public bool isRotationLock = false;

    [HideInInspector] public float xAxis;
    [HideInInspector] public float yAxis;
    [HideInInspector] public bool isAiming;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private float _currentSpeedAimRotation;
    private ActiveWeapon _activeWeapon;
    private Animator _animator;
    private int _isAimingParam = Animator.StringToHash("isAiming");

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _animator = GetComponent<Animator>();
        _activeWeapon = GetComponent<ActiveWeapon>();
        _currentSpeedAimRotation = _speedAimRotation;
    }

    private void Update()
    {
        if (isRotationLock)
            return;

        isAiming = Input.GetMouseButton(1);
        _animator.SetBool(_isAimingParam, isAiming);

        WeaponBase weaponBase = _activeWeapon.GetActiveWeapon();

        if (weaponBase)
            _currentSpeedAimRotation = isAiming ? weaponBase.speedAimRotate * _speedAimRotation : _speedAimRotation;

        if (weaponBase is RaycastWeapon weapon)
        {
            weapon.recoil.recoilModifier = isAiming ? 0.3f : 1.0f;
        }

        CalculateRotationInput();
        CameraRotation();
    }

    private void CameraRotation()
    {
        // clamp our rotations so our values are limited 360 degrees
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _cameraBottomClamp, _cameraTopClamp);

        // Поворот тела
        var euler = transform.eulerAngles;
        euler.y = _cinemachineTargetYaw;
        transform.eulerAngles = euler;

        // Cinemachine will follow this target
        cameraLookAt.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void CalculateRotationInput()
    {
        xAxis = Input.GetAxis("Mouse X");
        yAxis = Input.GetAxis("Mouse Y");

        var speedRotate = isAiming ? _currentSpeedAimRotation : _speedRotation;

        //Не следует умножать ввод с мыши на Time.deltaTime;
        float deltaTimeMultiplier = 1.0f * speedRotate;

        _cinemachineTargetYaw += xAxis * deltaTimeMultiplier;
        _cinemachineTargetPitch -= yAxis * deltaTimeMultiplier;
    }

    public void AddCinemachineTargetYaw(float targetYaw) => _cinemachineTargetYaw += targetYaw;
    public void AddCinemachineTargetPitch(float targetPitch) => _cinemachineTargetPitch -= targetPitch;

    public void SetRotationLock(bool isLock)
    {
        isRotationLock = isLock;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
}
