using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] bool pressButton = false;
    [SerializeField] float openingTime = 5;
    [SerializeField] AudioClip doorOpenedSound = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pressButton)
        {
            PerformAction();
            pressButton = false;
        }
    }

    public void PerformAction()
    {
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        yield return new WaitForSeconds(1);
        GetComponentInChildren<ParticleSystem>().Play();
        GetComponent<AudioSource>().Play();
        float timer = 0;
        float rate = 3 / openingTime;
        Transform spriteTransform = transform.GetChild(0);
        while (timer < openingTime)
        {
            spriteTransform.position = new Vector3(spriteTransform.position.x, spriteTransform.position.y + (Time.deltaTime * rate));
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().PlayOneShot(doorOpenedSound,1);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<ParticleSystem>().Stop();
    }
}
