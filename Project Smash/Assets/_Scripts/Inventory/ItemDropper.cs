using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Stats;

namespace PSmash.Inventories
{
    public class ItemDropper : MonoBehaviour
    {

        [Tooltip("How far can the pikcups be scattered from the dropper")]
        [SerializeField] float scatterDistance = 1;
        [SerializeField] DropLibrary dropLibrary;
        [SerializeField] Pickup pickup;

        const int ATTEMPTS = 30;

        public void RandomDrop()
        {
            print("Random Drops from " + gameObject.name);
            //int level = GetComponent<BaseStats>().GetLevel();
            int level = 1;
            var drops = dropLibrary.GetRandomDrops(level);
            if (drops == null)
                return;
            DropItems(drops);
            //foreach (var drop in drops)
            //{
            //    DropItem(drop.item, drop.number);
            //}
        }

        void DropItems(IEnumerable<DropLibrary.Dropped> drops)
        {
            SpawnPickup(drops, GetDropLocation());
        }

        void SpawnPickup(IEnumerable<DropLibrary.Dropped> drops, Vector2 spawnLocation)
        {
            Pickup clonePickup = Instantiate(pickup,spawnLocation, Quaternion.identity);
            clonePickup.Setup(drops);
        }

        //void DropItems(Item item, int number)
        //{
        //    SpawnPickup(item, GetDropLocation(), number);
        //}

        //void SpawnPickup(Item item, Vector2 spawnLocation, int number)
        //{
        //    Instantiate((item as CraftingItem).GetGameObject(), spawnLocation, Quaternion.identity);
        //}

        Vector2 GetDropLocation()
        {
            for (int i = 0; i <= ATTEMPTS; i++)
            {
                Vector2 randomPoint = (Vector2)transform.position+ new Vector2(0,1) + Random.insideUnitCircle * scatterDistance;
                RaycastHit2D hit = Physics2D.Raycast(randomPoint,Vector2.down, 2);
                if (hit)
                {
                    return hit.point;
                }
            }
            print("Randomness not applied");
            return transform.position;

        }
    }

}
