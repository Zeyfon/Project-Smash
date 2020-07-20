using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] AudioClip bombBreakingSound = null;
    [SerializeField] GameObject bombExplotion;
    [SerializeField] LayerMask whatIsGround;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        //if (collision.collider.CompareTag("Ground"))
        //{


        //    return;
        //}
        if (collision.collider.CompareTag("Enemy"))
        {
            RaycastHit2D hit = Physics2D.Raycast(collision.transform.position, Vector2.down, 6, whatIsGround);
            if (hit)
            {
                //Debug.Log("Enemt hit  " + hit.normal);
                BombBreak(hit.normal,hit.point);
                return;
            }
            Debug.LogWarning("Bomb didn't find ground on Enemy");
            Debug.Break();
        }
        if (collision.collider.CompareTag("Burnable"))
        {
            RaycastHit2D hit = Physics2D.Raycast(collision.transform.position, Vector2.down, 6, whatIsGround);
            if (hit)
            {
                //Debug.Log("Enemt hit  " + hit.normal);
                BombBreak(hit.normal, hit.point);
                return;
            }
        }
        else
        {
            ContactPoint2D contact = collision.contacts[0];
            print(contact.collider.gameObject + "  " + contact.normal);
            BombBreak(contact.normal, contact.point);
        }

        //Debug.LogWarning("Bomb collided with something else");
        //Debug.Break();
    }

    void BombBreak(Vector2 spawnOrientation, Vector2 spawnPosition)
    {
        audioSource.PlayOneShot(bombBreakingSound, 1);
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        Quaternion childRotation = Quaternion.Euler(0,0, Mathf.Atan2(spawnOrientation.x, spawnOrientation.y)*Mathf.Rad2Deg*-1);
        print(childRotation);
        Instantiate(bombExplotion, spawnPosition, childRotation);
        //bombClone.GetComponent<Rigidbody2D>().SetRotation(childRotation);
        StartCoroutine(WaitingSoundToEnd());
    }
    IEnumerator WaitingSoundToEnd()
    {
        while (audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
