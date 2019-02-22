using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private string _gunName;
    public string GunName
    {
        get { return _gunName; }
        set { _gunName = value; }
    }
    [SerializeField]
    private float _range;   // 사정거리
    public float Range
    {
        get { return _range; }
        set { _range = value; }
    }
    [SerializeField]
    private float _accuracy;     // 정확도
    public float Accuracy
    {
        get { return _accuracy; }
        set { _accuracy = value; }
    }
    [SerializeField]
    private float _fireRate;       // 연사속도
    public float FireRate
    {
        get { return _fireRate; }
        set { _fireRate = value; }
    }
    [SerializeField]
    private float _reloadTime;     // 재장전 속도
    public float ReloadTime
    {
        get { return _reloadTime; }
        set { _reloadTime = value; }
    }
    [SerializeField]
    private int _damage;
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }
    [SerializeField]
    private float _retroActionForce;    // 반동 세기
    public float RetroActionForce
    {
        get { return _retroActionForce; }
        set { _retroActionForce = value; }
    }
    [SerializeField]
    private float _retroActionFineSightForce;       // 정조준 시 반동 세기
    public float RetroActionFineSightForce
    {
        get { return _retroActionFineSightForce; }
        set { _retroActionFineSightForce = value; }
    }
    [SerializeField]
    private Vector3 _fineSightOriginPos;        // 정조준 위치
    public Vector3 FineSightOriginPos
    {
        get { return _fineSightOriginPos; }
        set { _fineSightOriginPos = value; }
    }

    [SerializeField]
    private int _reloadBulletCount;      // 총알 재장전 개수
    public int ReloadBulletCount
    {
        get { return _reloadBulletCount; }
        set { _reloadBulletCount = value; }
    }
    [SerializeField]
    private int _currnetBulletCount;     // 현재 탄알집에 남아있는 총알 개수
    public int CurrentBulletCount
    {
        get { return _currnetBulletCount; }
        set { _currnetBulletCount = value; }
    }
    [SerializeField]
    private int _maxBulletCount;        // 최대 소유 가능 총알 개수
    [SerializeField]
    private int _carryBulletCount;      // 현재 소유하고 있는 총알 개수
    public int CarryBulletCount
    {
        get { return _carryBulletCount; }
        set { _carryBulletCount = value; }
    }

    [SerializeField]
    private Animator _theAnimator;
    public Animator TheAnimator
    {
        get { return _theAnimator; }
        set { _theAnimator = value; }
    }
    [SerializeField]
    private ParticleSystem _muzzleFlash;
    public ParticleSystem MuzzleFlash
    {
        get { return _muzzleFlash; }
        set { _muzzleFlash = value; }
    }
    [SerializeField]
    private AudioClip _fireSound;
    public AudioClip FireSound
    {
        get { return _fireSound; }
        set { _fireSound = value; }
    }
}
