using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static bool IsWeaponChanged;     // 무기 중복 교체 실행 방지
    public static Transform CurrentWeapon; 
    public static Animator AnimCurrentWeapon;

    [SerializeField]
    private string _currentWeaponType;      // 현재 무기 타입

    [SerializeField]
    private float _delayChangeWeapon;       // 무기 교체 딜레이
    [SerializeField]
    private float _delayChangeWeaponFinish;     // 무기 교체가 완전히 끝난 시점

    // 무기 종류 관리
    [SerializeField]
    private Gun[] _guns;
    [SerializeField]
    private Hand[] _hands;

    // 무기 접근을 용이하게 하기 위한 무기 관리
    private Dictionary<string, Gun> _dictionaryGun = new Dictionary<string, Gun>();
    private Dictionary<string, Hand> _dictionaryHand = new Dictionary<string, Hand>();

    // 필요한 컴포넌트
    [SerializeField]
    private GunController _gunController;
    [SerializeField]
    private HandController _handController;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < _guns.Length; i++)
        {
            _dictionaryGun.Add(_guns[i].GunName, _guns[i]);
        }
        for(int i = 0; i < _hands.Length; i++)
        {
            _dictionaryHand.Add(_hands[i].HandName, _hands[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsWeaponChanged)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))        // 맨손으로 교체
                StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));
            else if(Input.GetKeyDown(KeyCode.Alpha2))       // 총으로 교체
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachinegun1"));
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string type, string name)
    {
        IsWeaponChanged = true;
        AnimCurrentWeapon.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(_delayChangeWeapon);

        CancelPreWeaponAction();
        ChangeWeapon(type, name);

        yield return new WaitForSeconds(_delayChangeWeaponFinish);

        _currentWeaponType = type;
        IsWeaponChanged = false;
    }

    private void CancelPreWeaponAction()
    {
        switch(_currentWeaponType)
        {
            case "GUN":
                _gunController.CancelFineSight();
                _gunController.CancelReload();
                GunController.IsActivate = false;
                break;
            case "HAND":
                HandController.IsActivate = false;
                break;
        }
    }

    private void ChangeWeapon(string type, string name)
    {
        if(type == "GUN")
            _gunController.ChangeGun(_dictionaryGun[name]);
        else if(type == "HAND")
            _handController.ChangeHand(_dictionaryHand[name]);
    }
}
