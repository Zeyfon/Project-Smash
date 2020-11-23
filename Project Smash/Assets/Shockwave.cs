using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] float scaleSpeed = 2;
    [SerializeField] float timer = 1;
    [SerializeField] bool testing = false;
    Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += (Time.deltaTime * scaleSpeed)*transform.localScale;
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (testing) TestingActions();
            else NormalActions();
        }
    }
    void NormalActions()
    {
        StartCoroutine(FadeOut());
    }
    
    IEnumerator FadeOut()
    {
        float fadeOutTime = 0.5f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float alpha = 1;
        while(alpha != 0)
        {
            alpha -=Time.deltaTime / fadeOutTime;
            if (alpha < 0) alpha = 0;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
    void TestingActions()
    {
        transform.localScale = scale;
        timer = 1;
    }
}
