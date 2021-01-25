using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Stats;

namespace PSmash.LevelUpSystem
{
    [CreateAssetMenu(fileName = "New Skill", menuName = "Skills")]
    public class Skill : ScriptableObject
    {
        public Sprite sprite;
        public Stat stat;
        public float value;
        public string description;
    }
}

