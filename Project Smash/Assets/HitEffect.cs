using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class HitEffect : MonoBehaviour
    {
        ParticleSystem ps;
        // Start is called before the first frame update
        void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            while (ps.isPlaying)
            {
                return;
            }
            Destroy(gameObject);
        }
    }
}

