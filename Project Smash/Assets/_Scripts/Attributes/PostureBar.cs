using UnityEngine;
using System.Collections;

namespace PSmash.Attributes
{
    public class PostureBar : MonoBehaviour
    {
        [SerializeField] Transform guardBar = null;
        [SerializeField] float effectTime = 0.5f;
        [SerializeField] AudioClip guardBarRecoveredSound = null;
        [SerializeField] Transform effectTransform = null;

        EnemyPosture posture;

        private void Awake()
        {
            posture = transform.parent.transform.GetComponentInChildren<EnemyPosture>();
            if(posture == null)
                posture = GetComponentInParent<EnemyPosture>();
            if(posture == null)
                Destroy(gameObject);
        }

        private void Update()
        {
            transform.rotation = Quaternion.identity;
            guardBar.localScale = new Vector2(posture.GetPosture() / posture.GetInitialPosture(), transform.localScale.y);
        }

        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }

        public void FullyRegenPosture()
        {
            StartCoroutine(FullyRegenBarVisualEffect());
        }

        IEnumerator FullyRegenBarVisualEffect()
        {
            SpriteRenderer renderer = effectTransform.GetComponent<SpriteRenderer>();
            float alpha = renderer.color.a;
            while(alpha < 1)
            {
                alpha += Time.deltaTime / effectTime;
                if (alpha > 1) alpha = 1;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }
            yield return new WaitForSeconds(1f);
            alpha = 1;
            while(alpha != 0)
            {
                alpha -= Time.deltaTime / effectTime;
                if (alpha < 0) alpha = 0;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }
        }

        public void DisablePostureBar()
        {
            Transform transforms = GetComponentInChildren<Transform>();
                foreach(Transform transform in transforms)
            {
                transform.gameObject.SetActive(false);
            }
        }

        public void EnablePostureBar()
        {
            Transform transforms = GetComponentInChildren<Transform>();
            foreach (Transform transform in transforms)
            {
                transform.gameObject.SetActive(true);
            }
        }
    }
}


