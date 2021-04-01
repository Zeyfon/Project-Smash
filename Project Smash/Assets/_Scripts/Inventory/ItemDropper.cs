
using System.Collections.Generic;
using UnityEngine;


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
            //print("Random Drops from " + gameObject.name);
            //int level = GetComponent<BaseStats>().GetLevel();
            int level = 1;
            var drops = dropLibrary.GetRandomDrops(level);
            if(HasDrops(drops))
                DropItems(drops);
        }

        bool HasDrops(IEnumerable<DropLibrary.Dropped> drops)
        {
            foreach(DropLibrary.Dropped drop in drops)
            {
                if (drop.item != null)
                {
                   // print("Has Drops");
                    //print(drop.item.name);
                    return true;
                }
            }

            //print("Does not have drops");
            return false;
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
            //print("Randomness not applied");
            return transform.position;

        }
    }

}
