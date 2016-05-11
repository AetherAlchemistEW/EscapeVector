using UnityEngine;
using System.Collections;

//this script sets up our obstacles, this is our asteroids but also our pickups
public class Obstacle : MonoBehaviour 
{
	public int forceApplied = 100;
	public bool isAsteroid = false;
	public int pickupType;
	// Use this for initialization
	void Start () 
	{
		//give them force and spin
		GetComponent<Rigidbody>().AddForce(Vector3.right * forceApplied);
		if(isAsteroid && this.tag == "Asteroid")//spin randomly, this gives asteroids believability 
		{
			GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(0,50), Random.Range(0,50), Random.Range(0,50)) * forceApplied);
		}
		else //spin in a measured fashion, this is for pickups.
		{
			GetComponent<Rigidbody>().AddTorque(Vector3.up * 75);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	//destroys them when they leave view
	void OnBecameInvisible()
	{
		Destroy(this.gameObject);
	}
}
