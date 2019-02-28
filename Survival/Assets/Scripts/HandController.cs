using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public static bool IsActivate = false;

    [SerializeField]
    private Hand _currentHand;      // 현재 장착된 Hand형 타입 무기

    private bool _isAttack = false;
    private bool _isSwing = false;

    private RaycastHit _hitInfo;

    // Update is called once per frame
    void Update()
    {
        if(IsActivate)
            TryAttack();
    }

    private void TryAttack()
    {
        if (Input.GetButton("Fire1"))
        {
            if (!_isAttack)
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        _isAttack = true;
        _currentHand.TheAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(_currentHand.AttackDelayA);
        _isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(_currentHand.AttackDelayB);
        _isSwing = false;

        yield return new WaitForSeconds(_currentHand.AttackDelay - _currentHand.AttackDelayA - _currentHand.AttackDelayB);
        _isAttack = false;
    }

    IEnumerator HitCoroutine()
    {
        while (_isSwing)
        {
            if (CheckObject())  // 충돌
            {
                _isSwing = false;
                Debug.Log(_hitInfo.transform.name);
            }
            yield return null;
        }
    }

    private bool CheckObject()      // 충돌(공격 성공)여부 확인
    {
        if (Physics.Raycast(transform.position, transform.forward, out _hitInfo, _currentHand.Range))
            return true;
        return false;
    }

    public void ChangeHand(Hand hand)
    {
        if(WeaponManager.CurrentWeapon != null)
            WeaponManager.CurrentWeapon.gameObject.SetActive(false);

        _currentHand = hand;
        WeaponManager.CurrentWeapon = _currentHand.GetComponent<Transform>();
        WeaponManager.AnimCurrentWeapon = _currentHand.TheAnimator;
        _currentHand.transform.localPosition = Vector3.zero;
        _currentHand.gameObject.SetActive(true);

        IsActivate = true;
    }
}
