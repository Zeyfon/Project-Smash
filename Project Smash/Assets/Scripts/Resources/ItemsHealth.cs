using System.Collections;
using UnityEngine;

namespace PSmash.Resources
{
    public class ItemsHealth : MonoBehaviour
    {
        [SerializeField] ParticleSystem particles = null;
        [SerializeField] GameObject lights;
        // Start is called before the first frame update

        public void Burn()
        {
            Debug.Log("Crate Received Damage");
            StartCoroutine(StartBurning());
        }

        IEnumerator StartBurning()
        {
            yield return new WaitForSeconds(2);
            GetComponent<Animator>().SetTrigger("Burn");
        }

        void FlameEffects()
        {
            particles.Play();
            lights.SetActive(true);
        }

        void DestroyGameObject()
        {
            Destroy(gameObject);
        }

    }
}

