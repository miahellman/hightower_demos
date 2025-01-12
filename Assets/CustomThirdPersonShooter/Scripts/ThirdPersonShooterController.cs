using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;


public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private GameObject firePoint;

    [Header("Aim Camera")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    [Header("Look Sensitivity")]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [Header("Layer Mask + Debugging")]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    [Header("Particles")]
    [SerializeField] private GameObject hitVFX;
    [SerializeField] private GameObject missVFX;
    [SerializeField] private GameObject shootVFX;

    private StarterAssetsInputs starterAssetsInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;
    private BulletCounter bulletCounter;


    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
        bulletCounter = GetComponent<BulletCounter>();
    }

    private void Update()
    {
        #region SHOOT HITSCAN
        //get fire point position
        Vector3 firePos = firePoint.transform.position;

        //get player aim at center of screen, not in front of player character
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }
        #endregion

        #region AIMING
        //if aiming is enabled, activate the aim camera and set the sensitivity to the aim sensitivity
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            //aim animation
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));

            #region LOOK AT MOUSE POS WHILE AIMING
            //make player character look where the player is aiming
            thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;

            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 15f);

            //turn on aimRig
            aimRig.weight = Mathf.Lerp(aimRig.weight, 1f, Time.deltaTime * 10f);
            #endregion

            //if shooting is enabled, instantiate the hit or miss VFX
            if (starterAssetsInputs.shoot && bulletCounter.canShoot)
            {
                CinemachineShake.Instance.ShakeCamera(1f, 0.1f);

                Instantiate(shootVFX, firePos, Quaternion.identity);
                bulletCounter.Shoot();

                //if hit something
                if (hitTransform != null)
                {
                    //if hit has bullet target component
                    if (hitTransform.CompareTag("Enemy"))
                    {
                        Instantiate(hitVFX, mouseWorldPosition, Quaternion.identity);
                        //destroy hit object
                        Destroy(hitTransform.gameObject);

                    }
                    else //(no bullet target component)
                    {
                        Instantiate(missVFX, mouseWorldPosition, Quaternion.identity);
                    }
                }
                starterAssetsInputs.shoot = false;
            }
        } 
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            //no aim animation
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

            //turn off aimRig
            aimRig.weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 10f);
        }
        #endregion


    }

}
