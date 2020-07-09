using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGuard : MonoBehaviour
{
    [SerializeField] int damage = 30;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
    public int GetParryDamage()
    {
        return damage;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
