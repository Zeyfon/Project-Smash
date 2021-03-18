using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;

namespace PSmash.Attributes
{
    public class EnvironmentObjectsManager : MonoBehaviour
    {
        List<IRespawn> respawns = new List<IRespawn>();
    // Start is called before the first frame update
        void Start()
        {
            foreach(IRespawn respawn in GetComponentsInChildren<IRespawn>())
            {
                respawns.Add(respawn);
            }
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += RespawnObjects;
        }

        void RespawnObjects()
        {
            foreach(IRespawn respawn in respawns)
            {
                respawn.Respawn();
            }
        }
    }

}

