using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

public class BulletCounter : MonoBehaviour
{
    //bullet counter
    [Header("Bullets and Magazines")]
    [SerializeField] private int maxBullets = 6;
    private int currentBullets;
    [SerializeField] private int startingMagazines = 3; //add in objects to collide with that add +1 to magazines
    [HideInInspector] public int currentMagazines;
    [HideInInspector] public bool canShoot;

    [Header("Reloading")]
    [SerializeField] private float reloadTime = 3f;
    private float currentReloadTime;
    private bool canReload = false;
    private bool isReloading = false;

    [Header("UI")]
    [SerializeField] private TMP_Text bulletCounterUI;
    [SerializeField] private TMP_Text reloadTextUI;

    //get third person shoot controller
    private ThirdPersonShooterController thirdPersonShooterController;
    private StarterAssetsInputs starterAssetsInputs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thirdPersonShooterController = GetComponent<ThirdPersonShooterController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        currentBullets = maxBullets;
        currentMagazines = startingMagazines;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBullets > 0)
        {
            canShoot = true;
            canReload = false;
        }
        else
        {
            canShoot = false;
            canReload = true;
        }

        if (canReload && starterAssetsInputs.reload)
        {
            StartCoroutine(ReloadTimer());
        }

        //update ui with current bullets and magazines
        bulletCounterUI.text = currentBullets.ToString() + " / " + currentMagazines.ToString();
        //update reload text
        if (currentMagazines <= 0 && currentBullets <= 0)
        {reloadTextUI.text = "OUT OF AMMO";} 
        else if (!isReloading){ reloadTextUI.text = canReload ? "RELOAD" : ""; }
        else{ reloadTextUI.text = isReloading ? "Reloading..." : ""; }
        //if reload hit while out of ammo isReloading is false
        if (currentMagazines <= 0 && currentBullets <= 0 && isReloading) { isReloading = false; }
    }


    void ReloadGun()
    {
        canReload = false;
        
        if (isReloading)
        {
            if (currentMagazines > 0)
            {
                currentBullets = maxBullets;
                currentMagazines--;
                isReloading = false;
            }
        }
    }

    public void Shoot()
    {
        if (canShoot)
        {
            currentBullets--;
        }
    }
    
    private IEnumerator ReloadTimer()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        ReloadGun();
    }

}
