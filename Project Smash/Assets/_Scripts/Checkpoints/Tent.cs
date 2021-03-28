using PSmash.Core;
using PSmash.SceneManagement;
using System;
using UnityEngine;

namespace PSmash.Checkpoints
{
    /// <summary>
    /// Only passes the IInterface to a event for the Tent Menu to answer to its call
    /// The interface is used as a way to detect interactable collider, like the Tent's case
    /// </summary>
    public class Tent : MonoBehaviour, IInteractable
    {
        public static event Action OnTentMenuOpen;

        public void Interact()
        {
            if (OnTentMenuOpen != null)
            {
                OnTentMenuOpen();
                FindObjectOfType<SavingWrapper>().Save();
            }
        }
    }

}
