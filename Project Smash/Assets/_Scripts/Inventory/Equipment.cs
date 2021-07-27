using PSmash.Checkpoints;
using UnityEngine;
using GameDevTV.Saving;
using System.Collections.Generic;
using System;
using PSmash.SceneManagement;

namespace PSmash.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        [SerializeField] ToolSlot[] slots;
        [Tooltip("Acquired subweapon to use alternatively to the punches")]
        Subweapon equippedSubweapon;

        public delegate void ToolChange(int index);
        public event ToolChange onCurrentToolEquippedChange;

        public delegate void SubWeaponChange(Weapon item);
        public static event SubWeaponChange onSubWeaponChange;

        List<Subweapon> subWeapons = new List<Subweapon>();
        int subweaponIndex = 0;
        int currentIndex = 0;
        Weapon mainWeapon;
        //INITIALIZE/////////////////////

        void Start()
        {

            GetWeapons();
            RestockToolNumbers();
            if (onCurrentToolEquippedChange != null)
                onCurrentToolEquippedChange(currentIndex);
            if(onSubWeaponChange != null)
                onSubWeaponChange(equippedSubweapon);
        }

        private void OnEnable()
        {
            Tent.OnCheckpointDone += RestockToolNumbersAndUpdateUI;
        }

        private void OnDisable()
        {
            Tent.OnCheckpointDone -= RestockToolNumbersAndUpdateUI;
        }

        void GetWeapons()
        {
            bool isSubweaponSet = false;
            foreach (Weapon item in GetComponentInChildren<Inventory>().GetWeapons())
            {
                if(item.GetID() == "22fb0e72-60f1-4908-81f3-ed22b0195c88")
                {
                    mainWeapon = item;
                }
                if(!isSubweaponSet && item is Subweapon)
                    equippedSubweapon = item as Subweapon;
                if (item is Subweapon)
                {
                    subWeapons.Add(item as Subweapon);
                    print(item.displayName);
                }

            }

        }


        /////////////////////////////////////////PUBLIC//////////////

        public Weapon GetMainWeapon()
        {
            return mainWeapon;
        }

        /// <summary>
        /// Set the current subweapon to whatever weapon is passed
        /// </summary>
        /// <param name="subWeapon"></param>
        public void SetSubWeapon(Subweapon subWeapon)
        {
            this.equippedSubweapon = subWeapon; 
        }

        /// <summary>
        /// Pass the current equipped subweapon. This can be null
        /// </summary>
        /// <returns></returns>
        public Subweapon GetSubWeapon()
        {
            return equippedSubweapon;
        }

        public int GetSubWeaponAnimationValue()
        {
            //print(subWeapon.GetAnimatorInt());
            return equippedSubweapon.GetAnimatorInt();
        }

        public float GetMainWeaponAttackImpulse()
        {
            return mainWeapon.GetWeaponAttackImpulse();
        }

        public float GetSubWeaponAttackImpulse()
        {
            return equippedSubweapon.GetWeaponAttackImpulse();
        }


        public ToolSlot GetCurrentEquipmentSlot()
        {
            return slots[currentIndex];
        }

        /// <summary>
        /// Change the current equipped tool. Accepts 1 and -1
        /// </summary>
        /// <param name="movementDirection"></param>
        public void UpdateCurrentEquippedTool(float movementDirection)
        {
            //print(currentIndex);
            if (movementDirection == 1)
            {
                currentIndex++;
                //print("Moving right" + currentIndex);
                if (currentIndex > slots.Length - 1)
                {
                    currentIndex = 0;
                }
                //print(currentIndex);
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = slots.Length - 1;
                }
            }
            onCurrentToolEquippedChange(currentIndex);
        }


        public void ItemUsed(ToolSlot slot)
        {
            slot.number -= 1;
            onCurrentToolEquippedChange(currentIndex);
        }

        /// <summary>
        /// Get all the tools information. ToolItem, currentNumber, maxNumber
        /// </summary>
        /// <returns></returns>
        public ToolSlot[] GetTools()
        {
            return slots;
        }


        [System.Serializable]
        public class ToolSlot
        {
            public ToolItem item;
            public int number;
            public int maxNumber;
        }

        /// <summary>
        /// Expand the capacity of the paramiter item.
        /// </summary>
        /// <param name="item"></param>
        public void UpgradeStock(Item item)
        {
            foreach (ToolSlot slot in slots)
            {
                if (item == slot.item)
                {
                    slot.maxNumber += 1;
                    //print("The " + item.name + "  increase to  " + slot.maxNumber);
                    RestockToolNumbers();
                    onCurrentToolEquippedChange(currentIndex);
                    return;
                }
            }
        }

        public void SwitchSubWeapon()
        {
            subweaponIndex++;

            if (subweaponIndex >= subWeapons.Count)
                subweaponIndex = 0;

            print(subweaponIndex + "  "  + subWeapons.Count);
            equippedSubweapon = subWeapons[subweaponIndex];
            onSubWeaponChange(equippedSubweapon);
        }

        ///////////////////////////PRIVATE//////////////////////

        /// <summary>
        /// Restock and Update UI
        /// </summary>
        void RestockToolNumbersAndUpdateUI()
        {
            RestockToolNumbers();
            onCurrentToolEquippedChange(currentIndex);
        }

        /// <summary>
        /// Set the tool currentNumber to maxNumber
        /// </summary>
        void RestockToolNumbers()
        {
            foreach (ToolSlot slot in slots)
            {
                slot.number = slot.maxNumber;
            }
        }


        //////////////////////////////////SAVE SYSTEM////////////////////////

        public object CaptureState()
        {
            ///Save the quantity of each tool
            Dictionary<string, object> toolsForSerialization = new Dictionary<string, object>();
            foreach(ToolSlot slot in slots)
            {
                List<int> numbers = new List<int>();
                numbers.Add(slot.number);
                numbers.Add(slot.maxNumber);
                toolsForSerialization.Add(slot.item.GetID(), numbers);
            }

            toolsForSerialization.Add("currentIndex", currentIndex);

            if (equippedSubweapon != null)
            {
                toolsForSerialization.Add("Mace", equippedSubweapon.GetID());
            }
            return toolsForSerialization;
        }

        public void RestoreState(object state, bool isLoadingLastSavedScene)
        {
            ///
            var equippedItemsForSerialization = (Dictionary<string, object>)state;
            //print("X");
            foreach (string name in equippedItemsForSerialization.Keys)
            {
                var item = (ToolItem)Item.GetFromID(name);
                //print("y");
                if (item != null /*&& !isLoadLastScene*/)
                {
                    //print("W");
                    foreach(ToolSlot slot in slots)
                    {
                        //print("Z");
                        if(item == slot.item)
                        {
                            List<int> tests = (List<int>)equippedItemsForSerialization[name];
                            //print("Restoring " + item.name);
                            slot.number = tests[0];
                            slot.maxNumber = tests[1];
                        }
                    }
                }
                
                if(name == "currentIndex")
                {
                    currentIndex =(int)equippedItemsForSerialization[name];
                    //print("Restoring Tool index  " + currentIndex);
                }

                if(name == "Mace")
                {
                    foreach (Subweapon subWeapon in subWeapons)
                    {
                        string subWeaponName = (string)equippedItemsForSerialization[name];
                        if (subWeaponName == subWeapon.GetID())
                        {
                            this.equippedSubweapon = subWeapon;
                            //print("I have the mace");
                        }
                    }
                }
                
            }
            if (isLoadingLastSavedScene)
            {
                RestockToolNumbersAndUpdateUI();
            }
            onCurrentToolEquippedChange(currentIndex);
            onSubWeaponChange(equippedSubweapon);
        }
    }
}

