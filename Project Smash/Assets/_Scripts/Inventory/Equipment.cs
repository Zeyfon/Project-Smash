using PSmash.Checkpoints;
using UnityEngine;
using GameDevTV.Saving;
using System.Collections.Generic;

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

        int subweaponIndex = 0;
        int currentIndex = 0;
        //Dictionary<Weapon, float> extraDamageForWeapons = new Dictionary<Weapon, float>();

        //INITIALIZE/////////////////////


        void Start()
        {

            RestockToolNumbers();
            if (onCurrentToolEquippedChange != null)
                onCurrentToolEquippedChange(currentIndex);
            if(onSubWeaponChange != null)
                onSubWeaponChange(equippedSubweapon);
            if(equippedSubweapon == null)
            {
               equippedSubweapon =  Inventory.GetPlayerInventory().GetSubweapon();
                //print(equippedSubweapon.GetDisplayName());
            }
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += RestockToolNumbersAndUpdateUI;
        }

        private void OnDisable()
        {
            Tent.OnTentMenuOpen -= RestockToolNumbersAndUpdateUI;
        }


        /////////////////////////////////////////PUBLIC//////////////

        public Weapon GetMainWeapon()
        {
            return Inventory.GetPlayerInventory().GetMainWeapon();
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
        public Subweapon GetEquippedSubweapon()
        {
            return equippedSubweapon;
        }

        public int GetThisSubweaponLevel(Subweapon subweapon)
        {
            return subweapon.GetMyLevel();
        }

        public int GetSubWeaponAnimationValue()
        {
            //print(subWeapon.GetAnimatorInt());
            return equippedSubweapon.GetAnimatorInt();
        }

        public float GetMainWeaponAttackImpulse()
        {
            return Inventory.GetPlayerInventory().GetMainWeapon().GetWeaponAttackImpulse();
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
            List<Subweapon> subweapons= GetSubweapons(Inventory.GetPlayerInventory().GetWeapons());
            if (subweaponIndex >= subweapons.Count)
                subweaponIndex = 0;
            equippedSubweapon = subweapons[subweaponIndex];
            print("Player changed subweapon to " + equippedSubweapon.GetDisplayName());
            onSubWeaponChange(equippedSubweapon);
        }

        List<Subweapon> GetSubweapons(List<Weapon> weapons)
        {
            List<Subweapon> subweapons = new List<Subweapon>();
            foreach (Weapon weapon in weapons)
            {
                if (weapon is Subweapon)
                {
                    subweapons.Add(weapon as Subweapon);
                }
            }
            return subweapons;
        }

        public float GetExtraWeaponDamage(Weapon equippedWeapon)
        {
            return Inventory.GetPlayerInventory().GetExtraWeaponDamage(equippedWeapon);
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
            //Capture the Tools with the current quantity and current max quantity numbers
            foreach(ToolSlot slot in slots)
            {
                List<int> numbers = new List<int>();
                numbers.Add(slot.number);
                numbers.Add(slot.maxNumber);
                toolsForSerialization.Add(slot.item.GetID(), numbers);
            }

            //Capture the current selecte tool by its index
            toolsForSerialization.Add("currentIndex", currentIndex);

            if (equippedSubweapon != null)
            {
                toolsForSerialization.Add("Subweapon", equippedSubweapon.GetID());
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
                }

                if(name == "Subweapon")
                {
                    foreach (Weapon weapon in Inventory.GetPlayerInventory().GetWeapons())
                    {
                        string weaponID = (string)equippedItemsForSerialization[name];
                        if (weaponID == weapon.GetID())
                        {
                            this.equippedSubweapon = weapon as Subweapon;
                            //print("Restoring Subweapon  " + this.equippedSubweapon.GetDisplayName());
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

