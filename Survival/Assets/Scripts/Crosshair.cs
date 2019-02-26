using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    private float _gunAccuracy;     // 크로스헤어에 따른 총의 정확도
    public float GunAccuracy
    {
        get { return _gunAccuracy; }
        set
        {
            if(_animator.GetBool("Walking"))
                _gunAccuracy = 0.06f;
            else if(_animator.GetBool("Crouching"))
                _gunAccuracy = 0.015f;
            else if(_gunController.IsFineSightMode)
                _gunAccuracy = 0.0005f;
            else
                _gunAccuracy = 0.035f;
        }
    }
    [SerializeField]
    private GameObject _goCrosshairHud;     // 크로스헤어 비활성화를 위한 부모 객체
    [SerializeField]
    private GunController _gunController;

    public void PlayWalkingAnimation(bool flag)
    {
        _animator.SetBool("Walking", flag);
    }

    public void PlayRunningAnimation(bool flag)
    {
        _animator.SetBool("Running", flag);
    }

    public void PlayCrouchingAnimation(bool flag)
    {
        _animator.SetBool("Crouching", flag);
    }

    public void PlayFineSightAnimation(bool flag)
    {
        _animator.SetBool("FineSight", flag);
    }

    public void PlayFireAnimation()
    {
        if(_animator.GetBool("Walking"))
            _animator.SetTrigger("Walk_Fire");
        else if(_animator.GetBool("Crouching"))
            _animator.SetTrigger("Crouch_Fire");
        else
            _animator.SetTrigger("Idle_Fire");
    }
}
