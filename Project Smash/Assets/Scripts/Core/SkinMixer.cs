using Spine;
using Spine.Unity;
using UnityEngine;

public class SkinMixer : MonoBehaviour
{
    [Tooltip("The names must be exactly as the names of the skin " +
             "or will throw a warnin")]
    [SerializeField] string[] skinNames = null;
    SkeletonMecanim skeletonMecanim;
    // Start is called before the first frame update

    private void Awake()
    {
        skeletonMecanim = GetComponent<SkeletonMecanim>();
    }
    void Start()
    {

        var skeleton = skeletonMecanim.skeleton;
        var skeletonData = skeleton.Data;
        var combinedSkin = new Skin("merleCombined");
        foreach(string skinName in skinNames)
        {
            if (skeletonData.FindSkin(skinName) == null) Debug.Log(skinName + "  was not found. Check spelling");
            combinedSkin.AddSkin(skeletonData.FindSkin(skinName));
        }
        skeleton.SetSkin(combinedSkin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
