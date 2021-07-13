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
        [SerializeField] Weapon subWeapon;

        public delegate void ToolChange(int index);
        public event ToolChange onCurrentToolEquippedChange;

        public delegate void SubWeaponChange(Weapon item);
        public static event SubWeaponChange onSubWeaponChange;

        List<Weapon> subWeapons = new List<Weapon>();
        int currentIndex = 0;
        Weapon mainWeapon;
        //INITIALIZE/////////////////////
        private void Awake()
        {
            GetWeapons();
            RestockToolNumbers();
        }

        /// <summary>
        /// Get all the ScriptableObjects "Weapons" for the player to use
        /// </summary>
        void GetWeapons()
        {
            foreach (Weapon item in Resources.LoadAll<Weapon>(""))
            {
                if (item.GetID() == "22fb0e72-60f1-4908-81f3-ed22b0195c88")
                    mainWeapon = item;
                subWeapons.Add(item);
            }

        }

        void Start()
        {
            onCurrentToolEquippedChange(currentIndex);
            onSubWeaponChange(subWeapon);
        }

        private void OnEnable()
        {
            Tent.OnCheckpointDone += RestockToolNumbersAndUpdateUI;
        }

        private void OnDisable()
        {
            Tent.OnCheckpointDone -= RestockToolNumbersAndUpdateUI;
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
        public void SetSubWeapon(Weapon subWeapon)
        {
            this.subWeapon = subWeapon; 
        }

        /// <summary>
        /// Pass the current equipped subweapon. This can be null
        /// </summary>
        /// <returns></returns>
        public Weapon GetSubWeapon()
        {
            return subWeapon;
        }

        public int GetSubWeaponAnimationValue()
        {
            //print(subWeapon.GetAnimatorInt());
            return subWeapon.GetAnimatorInt();
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

            if (subWeapon != null)
            {
                toolsForSerialization.Add("Mace", subWeapon.GetID());
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
                    foreach (Weapon subWeapon in subWeapons)
                    {
                        string subWeaponName = (string)equippedItemsForSerialization[name];
                        if (subWeaponName == subWeapon.GetID())
                        {
                            this.subWeapon = subWeapon;
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
            onSubWeaponChange(subWeapon);
        }
    }
}

