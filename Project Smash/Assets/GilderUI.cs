using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Movement;
using UnityEngine.UI;

public class GilderUI : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] Image foregroundImage = null;
    PlayerMovement movement;
    // Start is called before the first frame update
    void Awake()
    {
        movement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movement.GetGlideTimer() < movement.GetMaxGliderTimer())
        {
            canvasGroup.alpha = 1;
        }
        else
        {
            canvasGroup.alpha = 0;
        }
        transform.rotation = Quaternion.identity;
        foregroundImage.fillAmount = movement.GetGlideTimer() / movement.GetMaxGliderTimer();
    }
}
