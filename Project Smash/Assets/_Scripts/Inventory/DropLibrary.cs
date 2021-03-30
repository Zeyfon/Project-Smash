using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Inventories;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = ("PSmash/Inventory/DropLibrary"))]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] DropConfig[] potentialDrops;
        [SerializeField] int[] minDrops;
        [SerializeField] int[] maxDrops;
        [SerializeField] float[] dropChancePercentage;


        [System.Serializable]
        class DropConfig
        {
            public Item item;
            public float[] relativeChance;
            public int[] maxNumber;
            public int[] minNumber;
            public int GetRandomNumber(int level)
            {
                int min = GetByLevel(minNumber, level);
                int max = GetByLevel(maxNumber, level);
                return Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public Item item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if (!ShouldRandomDrop(level))
            {
                yield break;
            }
            for (int i = 0; i < GetRandomNumberOfDrops(level); i++)
            {
                yield return GetRandomDrop(level);
            }
        }

        bool ShouldRandomDrop(int level)
        {
            float randomRoll = Random.Range(0, 100);
            return randomRoll < GetByLevel(dropChancePercentage, level);
        }

        int GetRandomNumberOfDrops(int level)
        {
            int min = GetByLevel(minDrops, level);
            int max = GetByLevel(maxDrops, level);
            return Random.Range(min, max + 1);
        }

        Dropped GetRandomDrop(int level)
        {
            DropConfig dropConfig = SelectRandomItem(level);
            Dropped drop = new Dropped();
            drop.item = dropConfig.item;
            drop.number = dropConfig.GetRandomNumber(level);
            return drop;
        }

        DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0;
            foreach (DropConfig drop in potentialDrops)
            {
                chanceTotal += GetByLevel(drop.relativeChance, level);
                if (chanceTotal > randomRoll)
                {
                    return drop;

                }
            }
            return null;
        }

        float GetTotalChance(int level)
        {
            float totalChance = 0;
            foreach (DropConfig drop in potentialDrops)
            {
                totalChance += GetByLevel(drop.relativeChance, level);
            }
            return totalChance;
        }

        static T GetByLevel<T>(T[] values, int level)
        {
            if (values.Length == 0)
            {
                return default;
            }

            if (level > values.Length)
            {
                return values[values.Length - 1];
            }
            if (level <= 0)
            {
                return default;
            }
            return values[level - 1];
        }
    }
}
