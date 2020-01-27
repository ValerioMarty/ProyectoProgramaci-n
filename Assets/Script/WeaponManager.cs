using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    private Weapon[] weapons;


    [SerializeField]
    private int selectedWeapon;

    public int SelectedWeapon
    {
        get => selectedWeapon;
        set
        {
            if (value < 0)
            {
                value = 1;
            }
            else if (value > 1)
            {
                value = 0;
            }
            selectedWeapon = value;
        }
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        weapons = new Weapon[2];
        weapons[1] = GetComponent<BombManager>();
        weapons[0] = GetComponent<GrenadeLauncherManager>();
    }

    private void Start()
    {


    }

    private void Update()
    {

        if (Input.mouseScrollDelta.y > 0)
        {
            SelectedWeapon++;
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            SelectedWeapon--;
        }

        //el problema es que para el grenade launcher quiero getbutton y para el primary de las bombas quiero el getbuttondown

        switch (selectedWeapon)
        {
            case 0:
                if (Input.GetButton("Fire1"))
                {
                    weapons[SelectedWeapon].PrimaryFire();
                }
                break;
            case 1:
                if (Input.GetButtonDown("Fire1"))
                {
                    weapons[SelectedWeapon].PrimaryFire();
                }
                break;
            default:
                break;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            weapons[1].SecondaryFire();
        }

    }


}
