using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;
using PSmash.Attributes;

public class EnemyRespawner : MonoBehaviour
{
    [Serializable]
    public class Respawner
    {
        public GameObject gameObject;
        public Vector2 position;
    }

    Dictionary<int, Respawner> respawner = new Dictionary<int, Respawner>();
    // Start is called before the first frame update
    void Start()
    {


        List<Respawner> newList = new List<Respawner>();
        for(int i = 0; i <transform.childCount; i++)
        {
            print(transform.GetChild(i).gameObject.name);

            Respawner enemiesInfo = new Respawner
            {
                gameObject = transform.GetChild(i).gameObject,
                position = transform.GetChild(i).position
            };
            newList.Add(enemiesInfo);
        }
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    print(transform.GetChild(i).gameObject.name);
        //    enemiesInfo.gameObject = transform.GetChild(i).gameObject;
        //    enemiesInfo.position = transform.GetChild(i).position;
        //    respawner.Add(i, enemiesInfo);
        //}
    }

    private void OnEnable()
    {
        Tent.OnTentMenuOpen += RespawnAllEnemies;
    }

    private void RespawnAllEnemies()
    {
        for(int i =0; i < transform.childCount; i++)
        {
            //print(transform.GetChild(i).GetChild(3));
            transform.GetChild(i).GetComponentInChildren<EnemyHealth>().Respawn();
        }
    }
}
