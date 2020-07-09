using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] AudioClip bombBreakingSound = null;
    [SerializeField] GameObject bombExplotion;
    [SerializeField] LayerMask whatIsGround;
    AudioSource audioSource;

    bool isEnemyHit = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.CompareTag("Player")) return;
        Quaternion childRotation;

        if (collision.collider.CompareTag("Ground"))
        {        
            audioSource.PlayOneShot(bombBreakingSound, 1);
            GetComponent<Collider2D>().enabled = false;
            ContactPoint2D contact = collision.contacts[0];
            print(contact.collider.gameObject + "  " + contact.normal);
            childRotation = new Quaternion(contact.normal.x, contact.normal.y,0, 0);
            transform.GetChild(0).gameObject.SetActive(false);
            Instantiate(bombExplotion, contact.point, childRotation);
            StartCoroutine(WaitingSoundToEnd());
            return;
        }

            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 1), Vector2.down, 6, whatIsGround);
            if (hit)
            {
                GetComponent<Collider2D>().enabled = false;
                audioSource.PlayOneShot(bombBreakingSound, 1);
                Debug.Log("Enemt hit  " + hit.normal);
                isEnemyHit = true;
                childRotation = new Quaternion(hit.normal.x, hit.normal.y, 0, 0);
                transform.GetChild(0).gameObject.SetActive(false);
                Instantiate(bombExplotion, hit.point, childRotation);
                StartCoroutine(WaitingSoundToEnd());
                return;
            }
        
        Debug.Log(collision.gameObject.tag);
        Debug.Break();

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
