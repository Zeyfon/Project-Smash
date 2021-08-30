using UnityEngine;
using UnityEngine.UI;

namespace PSmash.UI.CraftingSytem
{
    public class Link : MonoBehaviour
    {
        [SerializeField] Image image = null;
        bool isLinkSet = false;


        /// <summary>
        /// This method is to update the color of the link.
        /// Also will check each time if the link has become white
        /// if it had, it will no longer update the color and will remain white
        /// </summary>
        /// <param name="state"></param>
        internal void UpdateColor(string state)
        {
            if (!isLinkSet)
                return;

            if (state == "White")
            {
                image.color = Color.white;
                isLinkSet = true;
            }
            else 
            {
                image.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            }
        }
    }
}

