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

    private Transform _autoAimTarget;
    private Vector3 _autoAimCameraPoint;
    private bool _isAutoAiming = false;
    private float _autoAimSmoothTime = 0.05f;
    private float _autoAimYawVelocity;
    private float _autoAimPitchVelocity;

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

        if (_isAutoAiming && _autoAimTarget != null)
        {
            HandleAutoAimRotation();
        }
        else
        {
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
        }
        CameraRotation();
    }


    private Vector3 GetAimTargetPosition()
    {
        return _autoAimTarget.position + Vector3.up * 1f;
    }

    private Vector3 GetCameraAimPoint()
    {
        return cameraLookAt.position + cameraLookAt.right * 0.5f;;
    }

    private void HandleAutoAimRotation()
    {
        Vector3 targetPosition = GetAimTargetPosition();
        Vector3 cameraAimPoint = GetCameraAimPoint();

        Vector3 directionToTarget = (targetPosition - cameraAimPoint).normalized;

        if (directionToTarget == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        Vector3 targetEuler = targetRotation.eulerAngles;

        float targetYaw = targetEuler.y;
        float targetPitch = targetEuler.x;

        targetPitch = ClampAngle(targetPitch, _cameraBottomClamp, _cameraTopClamp);

        // Плавный поворот по горизонтали (yaw)
        _cinemachineTargetYaw = Mathf.SmoothDampAngle(
            _cinemachineTargetYaw,
            targetYaw,
            ref _autoAimYawVelocity,
            _autoAimSmoothTime 
        );

        // Плавный поворот по вертикали (pitch)
        _cinemachineTargetPitch = Mathf.SmoothDampAngle(
            _cinemachineTargetPitch,
            targetPitch,
            ref _autoAimPitchVelocity,
            _autoAimSmoothTime * 0.1f
        );
    }

    public void SetAutoAimTarget(Transform target)
    {
        if (target == null)
        {
            _autoAimTarget = null;
            _isAutoAiming = false;
        }
        else
        {
            _autoAimTarget = target;
            _isAutoAiming = true;
        }
    }

    private void CameraRotation()
    {
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _cameraBottomClamp, _cameraTopClamp);

        // Поворот тела
        var euler = transform.eulerAngles;
        euler.y = _cinemachineTargetYaw;
        transform.eulerAngles = euler;

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

    public void SetFullRotationLock(bool isLock)
    {
        isRotationLock = isLock;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void SetRotationLock(bool isLock) => isRotationLock = isLock;


    void OnDrawGizmos()
    {
        if (_autoAimTarget != null)
        {
            Vector3 targetPos = GetAimTargetPosition();
            Vector3 cameraPos = GetCameraAimPoint();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(targetPos, 0.2f); // центр цели

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(cameraPos, 0.1f); // точка отсчёта камеры

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(cameraPos, targetPos); // вектор прицеливания
        }
    }
}
