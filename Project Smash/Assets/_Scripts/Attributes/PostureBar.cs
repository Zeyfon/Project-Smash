using UnityEngine;
using System.Collections;

namespace PSmash.Attributes
{
    public class PostureBar : MonoBehaviour
    {
        [SerializeField] bool isBarFixedLength = false;
        [SerializeField] Transform bar = null;
        [SerializeField] Transform background = null;
        [SerializeField] float postureValueForFixedLength = 100;
        [SerializeField] float xScale = 1.5f;
        [SerializeField] float effectTime = 0.5f;
        [SerializeField] AudioClip guardBarRecoveredSound = null;
        [SerializeField] Transform effectTransform = null;

        EnemyPosture posture;

        private void Awake()
        {          
            posture = GetComponentInParent<EnemyPosture>();
            if(posture == null)
                Destroy(gameObject);
        }

        private void Start()
        {
            float xScale;

            if (!isBarFixedLength)
            {
                xScale = posture.GetInitialPosture() * this.xScale / postureValueForFixedLength;
            }
            else
            {
                xScale = this.xScale;
            }
            xScale += 0.05f;
            background.localScale = new Vector2(xScale, background.localScale.y);
        }
        private void Update()
        {
            float xLocalScale;

            transform.rotation = Quaternion.identity;
            if (!isBarFixedLength)
            {
                xLocalScale = (posture.GetPosture() * xScale / postureValueForFixedLength);
            }
            else
            {
                xLocalScale = (posture.GetPosture() / posture.GetInitialPosture()) * xScale;
            }
            bar.localScale = new Vector2(xLocalScale, bar.localScale.y);
        }
    }
}


