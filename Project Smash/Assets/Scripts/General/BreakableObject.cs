﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{

    [SerializeField] AudioClip breakingObjectSound = null;
    [SerializeField] Sprite brokenSprite = null;

    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObjectBreaks();
    }

    public void ObjectBreaks()
    {
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = brokenSprite;
        audioSource.clip = breakingObjectSound;
        audioSource.Play();    
    }

}
