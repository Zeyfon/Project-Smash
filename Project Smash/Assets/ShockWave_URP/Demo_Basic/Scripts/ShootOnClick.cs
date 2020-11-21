/*
This script is will shoot a Ray on click.
If they Ray hits the shield it will trigger an effect to occur.
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShootOnClick : MonoBehaviour 
{


	//the projectile that will be used
	public GameObject projectile;

	//the force of the projectile
	public float force = 500f;

	_Controller _controller;
	private void Awake()
	{
		_controller = new _Controller();
	}

    private void OnEnable()
    {
		_controller.Player.Enable();
		_controller.Player.ButtonA.started += ctx => ButtonAPressed();
    }

	void ButtonAPressed()
    {
		//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Transform t = Camera.main.transform;
		GameObject p = Instantiate(projectile, t.position, t.rotation) as GameObject;
		//p.GetComponent<Rigidbody>().AddForce(ray.direction * force);
	}

    // Update is called once per frame
 //   void Update () 
	//{


	//	On Click
	//	if (Input.GetMouseButtonDown(0))
	//	{

	//	    Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	//		Transform t = Camera.main.transform;
	//		GameObject p =  Instantiate(projectile,t.position,t.rotation) as GameObject;
	//		p.GetComponent<Rigidbody>().AddForce(ray.direction * force);
		
	//	}
	
	//}
}
