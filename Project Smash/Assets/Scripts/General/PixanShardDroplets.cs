using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixanShardDroplets : MonoBehaviour
{
    [SerializeField] AudioClip itemCollected=null;
    AudioSource audioSource;
    ParticleSystem ps;
    List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
    static int test = 0;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

        StartCoroutine(PlayerCollider());
        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator PlayerCollider()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ps.trigger.SetCollider(10, player.GetComponent<Collider2D>());
    }
    void Update()
    {
        if (!ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
    private void OnParticleTrigger()
    {
        int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside);
        for (int i = 0; i < numInside; i++)
        {
            ParticleSystem.Particle p = inside[i];
            p.remainingLifetime = 0;
            inside[i] = p;
            audioSource.PlayOneShot(itemCollected);
            ItemCounter.itemCounter += 1;
        }
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside);  
    }
}
