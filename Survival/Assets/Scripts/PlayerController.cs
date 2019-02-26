using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController:MonoBehaviour
{
    // 스피드 조정 변수
    private float _applySpeed;
    [SerializeField]
    private float _walkSpeed;
    [SerializeField]
    private float _runSpeed;
    [SerializeField]
    private float _crouchSpeed;     // 앉았을 때 속도

    [SerializeField]
    private float _jumpForce;

    // 상태 변수
    private bool _isRunning = false;
    private bool _isCrouching = false;
    private bool _isWalking = false;
    private bool _isOnGround = true;

    // 숙이기 정도 변수
    [SerializeField]
    private float _crouchPosY;
    private float _originPosY;
    private float _applyCrouchPosY;

    // 카메라 관련 변수
    [SerializeField]
    private float _camSensitivity;      // 카메라 민감도
    [SerializeField]
    private float _camRotationLimit;        // 카메라 제한 각도
    private float _currentCamRotationX = 0;

    // 기타 컴포넌트
    [SerializeField]
    private Camera _camera;
    private Rigidbody _myRigid;
    private CapsuleCollider _capsuleCollider;
    private GunController _theGunController;
    private Crosshair _crosshair;

    // Start is called before the first frame update
    void Start()
    {
        _myRigid = GetComponent<Rigidbody>();
        _crosshair = FindObjectOfType<Crosshair>();   
        _capsuleCollider = GetComponent<CapsuleCollider>();
        _theGunController = FindObjectOfType<GunController>();

        _applySpeed = _walkSpeed;
        _originPosY = _camera.transform.localPosition.y;
        _applyCrouchPosY = _originPosY;
    }

    // Update is called once per frame
    void Update()
    {
        IsOnGround();
        TryJump();
        TryRun();
        TryCrouch();
        Move();
        CheckMove();
        RotateCamera();
        RotateCharacter();
    }

    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isOnGround)
        {
            Jump();
        }
    }

    private void IsOnGround()       // 점프를 위해 땅에 닿아있는지 여부
    {
        _isOnGround = Physics.Raycast(transform.position, Vector3.down, _capsuleCollider.bounds.extents.y + 0.15f);
        _crosshair.PlayRunningAnimation(!_isOnGround);
        Debug.Log(_isOnGround);
    }

    private void Jump()
    {
        if (_isCrouching)
            Crouch();   // 앉은 상태에서 점프하면 점프 후 일어섬

        _myRigid.velocity = transform.up * _jumpForce;
    }

    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Run();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopRun();
        }
    }

    private void Run()
    {
        if (_isCrouching)
            Crouch();      // 앉은 상태에서 달리면 일어선 후 달림

        _theGunController.CancelFineSight();

        _isRunning = true;
        _crosshair.PlayRunningAnimation(_isRunning);
        _applySpeed = _runSpeed;
    }

    private void StopRun()
    {
        _isRunning = false;
        _crosshair.PlayRunningAnimation(_isRunning);
        _applySpeed = _walkSpeed;
    }

    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    private void Crouch()
    {
        _isCrouching = !_isCrouching;
        _crosshair.PlayCrouchingAnimation(_isCrouching);

        if (_isCrouching)
        {
            _applySpeed = _crouchSpeed;
            _applyCrouchPosY = _crouchPosY;
        }
        else
        {
            _applySpeed = _walkSpeed;
            _applyCrouchPosY = _originPosY;
        }

        //_camera.transform.localPosition = new Vector3(_camera.transform.localPosition.x, _applyCrouchPosY, _camera.transform.localPosition.z);
        StartCoroutine(CrouchCoroutine());
    }

    IEnumerator CrouchCoroutine()       // 앉을 때 카메라가 자연스럽게 움직이도록 만들기 위함
    {
        float posY = _camera.transform.localPosition.y;
        int count = 0;

        while (posY != _applyCrouchPosY)
        {
            count++;
            // 카메라 원래위치 -> 이동해야하는 위치까지 0.3의 공비로 증가
            posY = Mathf.Lerp(posY, _applyCrouchPosY, 0.3f);
            _camera.transform.localPosition = new Vector3(0, posY, 0);

            if (count > 15)
                break;

            yield return null;
        }
        _camera.transform.localPosition = new Vector3(0, _applyCrouchPosY, 0);
    }

    private void Move()
    {
        float moveDirX = Input.GetAxisRaw("Horizontal");        // 좌우
        float moveDirZ = Input.GetAxisRaw("Vertical");          // 전후

        Vector3 moveHorizontal = transform.right * moveDirX;
        Vector3 moveVertical = transform.forward * moveDirZ;
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * _applySpeed;

        _myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void CheckMove()
    {
        if(!_isRunning && !_isCrouching && _isOnGround)
        {
            float moveDirX = Input.GetAxisRaw("Horizontal");        // 좌우
            float moveDirZ = Input.GetAxisRaw("Vertical");          // 전후

            //if(Vector3.Distance(_lastPos, transform.position) >= 0.01f)
            if(moveDirX >= 0.01f || moveDirX <= -0.01f || moveDirZ >= 0.01f || moveDirZ <= -0.01f)
            {
                _isWalking = true;
            }

            else
                _isWalking = false;

            _crosshair.PlayWalkingAnimation(_isWalking);
        }
    }

    private void RotateCamera() // 상하 카메라 회전
    {
        float rotationX = Input.GetAxisRaw("Mouse Y");
        float cameraRotationX = rotationX * _camSensitivity;
        _currentCamRotationX -= cameraRotationX;
        _currentCamRotationX = Mathf.Clamp(_currentCamRotationX, -_camRotationLimit, _camRotationLimit);

        _camera.transform.localEulerAngles = new Vector3(_currentCamRotationX, 0f, 0f);
    }

    private void RotateCharacter()  // 좌우 캐릭터 회전
    {
        float rotationY = Input.GetAxis("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, rotationY, 0f) * _camSensitivity;
        _myRigid.MoveRotation(_myRigid.rotation * Quaternion.Euler(characterRotationY));
    }
}
