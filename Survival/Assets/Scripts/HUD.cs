using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField]
    private GunController _gunController;
    private Gun _currentGun;

    [SerializeField]
    private GameObject _goBulletHud;    // 필요 시 HUD 호출, 그 외 HUD 비활성화
    [SerializeField]
    private Text[] _txtBullet;      // 총알 개수 반영 

    // Update is called once per frame
    void Update()
    {
        CheckBullet();        
    }

    private void CheckBullet()
    {
        _currentGun = _gunController.CurrentGun;
        _txtBullet[0].text = _currentGun.CarryBulletCount.ToString();
        _txtBullet[1].text = _currentGun.ReloadBulletCount.ToString();
        _txtBullet[2].text = _currentGun.CurrentBulletCount.ToString();
    }
}
