using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Chest : MonoBehaviour
{
    [SerializeField] float shakeTime = 0.2f;
    [SerializeField] float speed = 2;
    [SerializeField] float shakeAmount = 2;
    [SerializeField] GameObject dropItem = null;
    // Start is called before the first frame update
    int hitCounter = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit()
    {
        StartCoroutine(Shake());
        //SoundEffect();
        hitCounter++;
        if (hitCounter >= 3)
        {
            DropItem();
            Destroy(gameObject);
        }
    }

    IEnumerator Shake()
    {
        Vector3 initialPosition = transform.position;
        float timer = 0;
        while(timer < shakeTime)
        {
            timer += Time.deltaTime;
            transform.position = new Vector3(Mathf.Sin(Time.time * speed) * shakeAmount+initialPosition.x,transform.position.y,transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = initialPosition;
    }
    
    void DropItem()
    {
        GameObject dropItemClone = Instantiate(dropItem, transform.position, Quaternion.identity);
        Rigidbody2D rbClone = dropItemClone.GetComponent<Rigidbody2D>();
        if (rbClone != null)
        {
            rbClone.velocity = new Vector2(1, 7);
            return;
        } 
    }
}
