using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Items
{
    public class UsableItem : MonoBehaviour
    {
        protected Health owner;

        /// <summary>
        /// Function called by all the characters that can throw a projectile
        /// This is in order to know who spawned this so the Trigger events look for the correct target
        /// </summary>
        /// <param name="owner"></param>
        public void SetOwner(Health owner)
        {
            this.owner = owner;
        }
    }
}

