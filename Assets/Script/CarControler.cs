using System.Collections;
using UnityEngine;

public class CarControler : MonoBehaviour
{
    [SerializeField] private string _hAxisInputName = "Horizontal", _accelerateInputName = "Accelerate", _driftInputName = "Drift";
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Rigidbody _rb;

    [Header("Speed Zones")]
    private float _speedReductionFactor = 1f; // 1 = pas de réduction
    private float _originalMaxSpeed;

    [Header("Oil Drift")]
    [SerializeField] private float _oilDriftDuration = 2f;
    private bool _isOilDrifting;
    private float _oilDriftTimer;

    // Vitesses
    private float _currentSpeed;
    [SerializeField] private float _speedMaxBasic = 3, _speedMaxTurbo = 10;
    private float _currentMaxSpeed; // Utilisé pour gérer turbo/drift

    // Acceleration
    [SerializeField] private float _accelerationFactor = 0.1f;
    private float _accelerationLerpInterpolator;
    [SerializeField] private AnimationCurve _accelerationCurve;

    // Rotation
    private float _rotationInput;
    [SerializeField] private float _rotationSpeed = 0.5f, _maxAngle = 360;
    private float _originalRotationSpeed;

    // Drift
    [SerializeField] private float _driftRotationSpeed = 2f;
    [SerializeField] private float _minSpeedForDrift = 2f;
    [SerializeField] private float _driftBoostMultiplier = 1.5f;
    [SerializeField] private float _driftBoostDuration = 1f;
    private bool _isDrifting = false;
    private bool _canMove = false;
    

    private bool _isAccelerating, _isTurbo;
    private float _terrainSpeedVariator = 1f;

    [SerializeField] public Transform _carColliderAndMesh;

    private void Start()
    {
        _originalRotationSpeed = _rotationSpeed;
        _originalMaxSpeed = _speedMaxBasic;
        _currentMaxSpeed = _originalMaxSpeed;
    }

    void Update()
    {
        if (!_canMove) 
    {
        _isAccelerating = false;
        return;
    }
        // Inputs
        _rotationInput = Input.GetAxis(_hAxisInputName);

        if (Input.GetButtonDown(_accelerateInputName))
          
             _isAccelerating = true;
             FindAnyObjectByType<RaceCountdown>().TryGetBoost(_accelerateInputName);
        if (Input.GetButtonUp(_accelerateInputName)) _isAccelerating = false;

        // Drift
        if (Input.GetButtonDown(_driftInputName) && _currentSpeed > _minSpeedForDrift && Mathf.Abs(_rotationInput) > 0.5f)
            StartDrift();
        
        if (Input.GetButtonUp(_driftInputName) && _isDrifting)
            EndDrift();

        
        HandleOilDrift();
        if (!_canMove) return;

    }

    private void FixedUpdate()
    {
         if (!_canMove) 
    {
        _rb.linearVelocity = Vector3.zero; 
        return;
    }
        HandleAcceleration();
        HandleRotation();
        ApplyMovement();
       
    }
    public void ApplySpeedReduction(float reductionFactor)
{
    _speedReductionFactor = reductionFactor;
    _currentMaxSpeed = _originalMaxSpeed * _speedReductionFactor;
}

public void ResetSpeedReduction()
{
    _speedReductionFactor = 1f;
    _currentMaxSpeed = _originalMaxSpeed;
}


    private void HandleAcceleration()
    {
         if (_isAccelerating)
        _accelerationLerpInterpolator += _accelerationFactor;
    else
        _accelerationLerpInterpolator -= _accelerationFactor * 2f;

    _accelerationLerpInterpolator = Mathf.Clamp01(_accelerationLerpInterpolator);

    float targetSpeed = _accelerationCurve.Evaluate(_accelerationLerpInterpolator) * 
                      _currentMaxSpeed * 
                      _terrainSpeedVariator;

    if (_isDrifting)
        targetSpeed *= 0.8f;

    _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.fixedDeltaTime * 5f);
    }

    private void HandleRotation()
    {
            float rotationAmount = _rotationSpeed * _rotationInput * Time.fixedDeltaTime;
    
   
    if (_isOilDrifting)
    {
        rotationAmount *= 1.5f; 
    }
    else if (_isDrifting)
    {
        rotationAmount *= 1.2f; 
    }

    transform.Rotate(0, rotationAmount, 0);

    
    if (IsOnSlope(out var hit, out var angle, out var angleZ))
    {
        _carColliderAndMesh.eulerAngles = new Vector3(-angle, _carColliderAndMesh.eulerAngles.y, angleZ);
    }
    }

    private void ApplyMovement()
    {
        Vector3 forward = IsOnSlope(out var hit, out var angle, out var angleZ) 
            ? Vector3.ProjectOnPlane(transform.forward, hit.normal).normalized 
            : transform.forward;

        _rb.MovePosition(_rb.position + forward * _currentSpeed * Time.fixedDeltaTime);
    }

    private void StartDrift()
    {
        _isDrifting = true;
        _rotationSpeed = _driftRotationSpeed;
    }

    private void EndDrift()
    {
       if (_isDrifting) ResetDrift();
    if (_currentSpeed > _minSpeedForDrift)
        ActivateTurbo(_driftBoostMultiplier, _driftBoostDuration);
    }

    public void ActivateTurbo(float multiplier, float duration)
    {
        StartCoroutine(TurboRoutine(multiplier, duration));
    }

    private IEnumerator TurboRoutine(float multiplier, float duration)
    {
        _isTurbo = true;
        _currentMaxSpeed = _speedMaxTurbo * multiplier;

        yield return new WaitForSeconds(duration);

        _isTurbo = false;
        _currentMaxSpeed = _speedMaxBasic;
    }

    private bool IsOnSlope(out RaycastHit hit, out float angle, out float angleZ)
    {
        if (Physics.Raycast(transform.position + transform.forward * 0.5f, Vector3.down, out hit, 0.8f))
        {
            angle = Vector3.Angle(Vector3.up, hit.normal);
            angleZ = Vector3.Angle(Vector3.right, hit.normal);
            return angle != 0 && angle <= _maxAngle;
        }

        hit = new RaycastHit();
        angle = angleZ = 0f;
        return false;
    }
    public void ForceDrift(float driftPower)
{
    if (_isDrifting) return; // Ne s'applique pas si déjà en drift
    
    _isOilDrifting = true;
    _oilDriftTimer = _oilDriftDuration;
    _rotationSpeed *= driftPower;
}

private void HandleOilDrift()
{
    if (_isOilDrifting)
    {
        _oilDriftTimer -= Time.deltaTime;
        if (_oilDriftTimer <= 0) ResetDrift();
    }
}
private void ResetDrift()
{
    _isDrifting = false;
    _isOilDrifting = false;
    _rotationSpeed = _originalRotationSpeed;
    Debug.Log("Drift reset - Rotation speed: " + _rotationSpeed);
}
public void SetCanMove(bool canMove)
{
    _canMove = canMove;
    
    if (!canMove)
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
}
public void ResetSpeed()
{
    _currentSpeed = 0;
    _accelerationLerpInterpolator = 0;
}
public void FullReset()
{
    _currentSpeed = 0f;
    _accelerationLerpInterpolator = 0f;
    _rb.linearVelocity = Vector3.zero;
    _rb.angularVelocity = Vector3.zero;
    _isAccelerating = false;
}
}