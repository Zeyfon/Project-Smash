using PSmash.Inventories;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleUI : MonoBehaviour
{
    [SerializeField] CollectibleItem collectible = null;
    [SerializeField] Image image = null;


    public CollectibleItem GetMyCollectible()
    {
        return collectible;
    }
    // Start is called before the first frame update
    void Awake()
    {
        SetCollectibleImage();
    }

    void SetCollectibleImage()
    {
        image.sprite = collectible.GetSprite();

    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (collectible != null)
        {
            SetCollectibleImage();
        }
        else
        {
            print("Not setting skill");
        }
    }
#endif

}
