using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField]
    private string _handName;   // 너클이나 맨손을 구분
    public string HandName
    {
        get { return _handName; }
        set { _handName = value; }
    }
    [SerializeField]
    private float _range;       // 공격 범위
    public float Range
    {
        get { return _range; }
        set { _range = value; }
    }
    [SerializeField]
    private int _damage;        // 공격력
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }
    [SerializeField]
    private float _workSpeed;      // 작업 속도
    [SerializeField]
    private float _attackDelay;   // 공격 딜레이
    public float AttackDelay
    {
        get { return _attackDelay; }
        set { _attackDelay = value; }
    }
    [SerializeField]
    private float _attackDelayA;    // 공격 활성화 시점
    public float AttackDelayA
    {
        get { return _attackDelayA; }
        set { _attackDelayA = value; }
    }
    [SerializeField]
    private float _attackDelayB;    // 공격 비활성화 시점
    public float AttackDelayB
    {
        get { return _attackDelayB; }
        set { _attackDelayB = value; }
    }

    [SerializeField]
    private Animator _theAnimator;
    public Animator TheAnimator
    {
        get { return _theAnimator; }
        set { _theAnimator = value; }
    }
}
