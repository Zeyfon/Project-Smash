using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] Sprite lightBombSprite = null;
    [SerializeField] Sprite thruthBombSprite = null;
    GameObject currentBomb;
    Image image;

    // Start is called before the first frame update
    void Start()
    {       
        image = transform.GetChild(0).GetComponent<Image>();
    }

    private void OnEnable()
    {
        EventManager.ItemChanged += ChangeItem;
    }

    private void OnDisable()
    {
        EventManager.ItemChanged -= ChangeItem;
    }

    public void ChangeItem(int item)
    {
        switch (item)
        {
            case (int)ItemList.lightBomb:
                image.sprite = lightBombSprite;
                break;
            case (int)ItemList.thruthBomb:
                image.sprite = thruthBombSprite;
                break;
            default:
                break;
        }
    }

}
