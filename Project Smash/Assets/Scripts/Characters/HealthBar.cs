using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    //Its purpose is for identification with the GameObject.FindObjectOfType<HealthBar>();
    public void SubstractDamage(float valueScale)
    {
        transform.localScale = new Vector2(valueScale, transform.localScale.y);
    }
}
