using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class EnemyID : MonoBehaviour
    {
        public enum EnemyType
        {
            Reaper,
            Ranger,
            Smasher
        }

        [SerializeField] EnemyType enemyType;

        public EnemyType GetEnemyType()
        {
            return enemyType;
        }
    }

}
