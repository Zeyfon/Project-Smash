using UnityEngine;

namespace PSmash.Menus
{
    /// <summary>
    /// Its only function is to Instantiante and keep organize all the menus the game will use.
    /// </summary>
    public class Menus : MonoBehaviour
    {
        [SerializeField] GameObject status = null;        
        //[SerializeField] GameObject craftingSystem = null;
        [SerializeField] GameObject tentMenu = null;
        void Awake()
        {
            Instantiate(status, transform);
            //Instantiate(craftingSystem, transform);
            Instantiate(tentMenu, transform);
        }
    }
}

