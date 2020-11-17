using PSmash.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreCluster : MonoBehaviour, IDamagable
{
    [SerializeField] int health = 30;
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] AudioClip damageSound = null;
    [SerializeField] GameObject drop = null;
    public void TakeDamage(Transform attacker, int damage)
    {
        print("Ore Damaged");
        health -= damage;
        audioSource.PlayOneShot(damageSound);
        if (health <= 0)
        {
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(DestroyObject());
        }
    }

    IEnumerator DestroyObject()
    {
        audioSource.Play();
        for(int i = 0; i<2; i++)
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
