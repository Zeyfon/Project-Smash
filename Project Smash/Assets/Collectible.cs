using GameDevTV.Saving;
using PSmash.Inventories;
using PSmash.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, ISaveable
{
    [SerializeField] CollectibleItem item = null;

    bool alreadyTaken = false;

    public CollectibleItem GetCollectible()
    {
        return item;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            alreadyTaken = true;
            AddColletibleToInventory();
            DisableMyself();
            FindObjectOfType<SavingWrapper>().Save();
        }
    }

    private void DisableMyself()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
    }

    void AddColletibleToInventory()
    {
        Inventory.GetPlayerInventory().AddCollectibleToInventory(item);
    }

    public object CaptureState()
    {
        return alreadyTaken;
    }

    public void RestoreState(object state, bool isLoadLastScene)
    {
        alreadyTaken = (bool)state;
        if (alreadyTaken)
            DisableMyself();
    }
}
