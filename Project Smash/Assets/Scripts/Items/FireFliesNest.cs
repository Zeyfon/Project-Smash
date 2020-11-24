using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;
using System;
using UnityEngine.Experimental.Rendering.Universal;

namespace PSmash.Items
{

    public class FireFliesNest : MonoBehaviour
    {
        [SerializeField] bool isLighted = false;
        [SerializeField] GameObject FireFlies = null;
        [SerializeField] Sprite unlightedNestSprite = null;
        [SerializeField] SpriteRenderer spriteRenderer = null;
        [SerializeField] Light2D lights = null;
        [SerializeField] Sprite lightedNest = null;

        GameObject carryOnFireFliesClone;
        // Start is called before the first frame update
        private void Awake()
        {
            if (!isLighted)
            {
                lights.enabled = false;
                spriteRenderer.sprite = unlightedNestSprite;
            }
        }

        //Will check first if it is lighted
        //Then will check if the player already has some FireFlies with him.
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;

            if (CanIBeLighted(collision))
            {
                lights.enabled = true;
                spriteRenderer.sprite = lightedNest;
                isLighted = true;

            }

            if (CanIResupplyFireFlies(collision))
            {
                ResupplyFireFlies(collision);
            }
        }

        private void ResupplyFireFlies(Collider2D collision)
        {
            FireFlies fireFlies = collision.GetComponentInChildren<FireFlies>();
            if (fireFlies != null) Destroy(fireFlies.gameObject);
            StartCoroutine(InstantiateFireFlies(collision.transform));
        }

        private bool CanIResupplyFireFlies(Collider2D collision)
        {
            if (isLighted)
            {
                print("Will resupply");
                return true;
            }
            else
            {
                print("Cannot resupply");
                return false;
            }

        }

        bool CanIBeLighted(Collider2D collision)
        {

            if (!isLighted && collision.GetComponentInChildren<FireFlies>())
            {
                print("I Can be Lighted");
                return true;
            }
            else
            {
                print(" I Cannot be Ligthed");
                return false;
            }

        }

        IEnumerator InstantiateFireFlies(Transform targetTransform)
        {
            carryOnFireFliesClone = Instantiate(FireFlies, targetTransform.position + new Vector3(0, 1, 0), Quaternion.identity, targetTransform);
            yield return null;
            carryOnFireFliesClone.GetComponent<FollowEntity>().SetData(targetTransform, true);
        }
    }
}