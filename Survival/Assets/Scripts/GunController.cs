using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun _currentGun;    // 현재 장착된 총
    public Gun CurrentGun
    {
        get { return _currentGun; }
        set { _currentGun = value; }
    }
    private float _currentFireRate;     // 연사 속도 계산
    private AudioSource _audioSource;   // 효과음
    [SerializeField]
    private GameObject _hitEffectPrefab;    // 피격 효과

    // 상태 변수
    private bool _isReload;
    private bool _isFineSightMode;
    public bool IsFineSightMode
    {
        get { return _isFineSightMode; }
        set { _isFineSightMode = value; }
    }
    private Vector3 _originPos;     // 본래 포지션 값
    private RaycastHit _hitInfo;    // 충돌 정보를 받아옴

    // 필요한 컴포넌트
    [SerializeField]
    private Camera _camera;
    private Crosshair _crosshair;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _originPos = Vector3.zero;
        _crosshair = FindObjectOfType<Crosshair>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateFireRate();
        TryFire();
        TryReload();
        TryFineSight();
    }

    private void CalculateFireRate()    // 연사시간 계산
    {
        if (_currentFireRate > 0)
            _currentFireRate -= Time.deltaTime;
    }

    private void TryFire()
    {
       if (Input.GetButton("Fire1") && _currentFireRate <= 0 && !_isReload)
       {
           Fire();
       }   
    }

    private void Fire()
    {
        if(!_isReload)
        {
            if (_currentGun.CurrentBulletCount > 0)     // 탄알집에 총알이 있으면 발사 
                Shoot();
            else        // 총알 없으면 장전
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void Shoot()    // 실질적인 발사
    {
        _crosshair.PlayFireAnimation();
        _currentFireRate = _currentGun.FireRate;    // 연사속도 재계산
        PlaySE(_currentGun.FireSound);
        _currentGun.MuzzleFlash.Play();
        _currentGun.CurrentBulletCount--;
        Hit();

        // 총기 반동 코루틴
        StopAllCoroutines();
        StartCoroutine(RetroActionCoroutine());
    }

    private void Hit()
    {
        Vector3 missRange = new Vector3(Random.Range(-_crosshair.GunAccuracy - _currentGun.Accuracy, _crosshair.GunAccuracy + _currentGun.Accuracy), Random.Range(-_crosshair.GunAccuracy - _currentGun.Accuracy, _crosshair.GunAccuracy + _currentGun.Accuracy), 0);

        if(Physics.Raycast(_camera.transform.position, _camera.transform.forward + missRange, out _hitInfo, _currentGun.Range))
        {
            GameObject clone = Instantiate(_hitEffectPrefab, _hitInfo.point, Quaternion.LookRotation(_hitInfo.normal));    // 피격되는 사물의 방향으로 로테이션이 이루어짐
            Destroy(clone, 1f);
        }
    }

    private void PlaySE(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !_isReload && _currentGun.CurrentBulletCount < _currentGun.ReloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if(_currentGun.CarryBulletCount > 0)    // 가지고 있는 총알이 있어야만 장전이 이루어짐
        {
            _isReload = true;

            _currentGun.TheAnimator.SetTrigger("Reload");

            _currentGun.CarryBulletCount += _currentGun.CurrentBulletCount;
            _currentGun.CurrentBulletCount = 0;

            yield return new WaitForSeconds(_currentGun.ReloadTime);     // 장전하고 있는 시간동안은 발사 불가능

            // 장전이 끝난 후에 총알이 채워짐
            if (_currentGun.CarryBulletCount >= _currentGun.ReloadBulletCount)
            {
                _currentGun.CurrentBulletCount = _currentGun.ReloadBulletCount;
                _currentGun.CarryBulletCount -= _currentGun.ReloadBulletCount;
            }
            else
            {
                _currentGun.CurrentBulletCount = _currentGun.CarryBulletCount;
                _currentGun.CarryBulletCount = 0;
            }
            _isReload = false;
        }
        else
        {
            // 총알이 없어서 재장전이 안되는 상황 (틱틱소리 넣을 부분)
        }
    }

    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !_isReload)
        {
            FineSight();
        }
    }

    public void CancelFineSight()
    {
        if (_isFineSightMode)
            FineSight();
    }

    private void FineSight()
    {
        _isFineSightMode = !_isFineSightMode;
        _crosshair.PlayFineSightAnimation(_isFineSightMode);
        _currentGun.TheAnimator.SetBool("FineSightMode", _isFineSightMode);

        if (_isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());       // 정조준 모드로
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());     // 정조준 모드 해제
        }
    }

    IEnumerator FineSightActivateCoroutine()
    {
        while(_currentGun.transform.localPosition != _currentGun.FineSightOriginPos)
        {
            _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _currentGun.FineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine()
    {
        while (_currentGun.transform.localPosition != _originPos)
        {
            _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _originPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine()
    {
        Vector3 recoilBack = new Vector3(_currentGun.RetroActionForce, _originPos.y, _originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(_currentGun.RetroActionFineSightForce, _originPos.y, _originPos.z);

        if (!_isFineSightMode)
        {
            _currentGun.transform.localPosition = _originPos;

            // 반동 시작
            while (_currentGun.transform.localPosition.x <= _currentGun.RetroActionForce - 0.02f)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while(_currentGun.transform.localPosition != _originPos)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _originPos, 0.1f);
                yield return null;
            }
        }
        else    // 조준 시
        {
            _currentGun.transform.localPosition = _currentGun.FineSightOriginPos;

            // 반동 시작
            while (_currentGun.transform.localPosition.x <= _currentGun.RetroActionFineSightForce - 0.02f)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            // 원위치
            while (_currentGun.transform.localPosition != _currentGun.FineSightOriginPos)
            {
                _currentGun.transform.localPosition = Vector3.Lerp(_currentGun.transform.localPosition, _currentGun.FineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }
}
