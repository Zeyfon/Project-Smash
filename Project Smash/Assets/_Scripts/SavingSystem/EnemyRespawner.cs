using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Saving
{
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
            for (int i = 0; i < transform.childCount; i++)
            {
                print(transform.GetChild(i).gameObject.name);

                Respawner enemiesInfo = new Respawner
                {
                    gameObject = transform.GetChild(i).gameObject,
                    position = transform.GetChild(i).position
                };
                newList.Add(enemiesInfo);
            }
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += RespawnAllEnemies;
        }

        private void RespawnAllEnemies()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponentInChildren<EnemyHealth>().Respawn();
            }
        }
    }

}

