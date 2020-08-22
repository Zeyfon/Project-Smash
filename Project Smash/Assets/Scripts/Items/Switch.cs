using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] Door door = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            PressButton();
        }
    }

    void PressButton()
    {
        GetComponent<Animator>().Play("PressSwitch",0);
        GetComponent<AudioSource>().Play();
    }


    //Anim Event
    void IsPressedState()
    {
        BoxCollider2D[] colliders = GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
        spriteRenderer.enabled = false;
        door.PerformAction();
    }
}
