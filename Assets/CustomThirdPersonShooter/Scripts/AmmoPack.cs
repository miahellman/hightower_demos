using UnityEngine;

public class AmmoPack : MonoBehaviour
{
    private BulletCounter bulletCounter;

    private void Start()
    {
        bulletCounter = GetComponent<BulletCounter>();
    }

    //if player collides with ammo pack, add +1 to magazines and destroy ammo pack
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AmmoPack"))
        {
            bulletCounter.currentMagazines++;
            Destroy(other.gameObject);
        }
    }
}
