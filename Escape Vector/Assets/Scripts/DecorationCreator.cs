using UnityEngine;
using System.Collections;

//creates decorative elements like the planets, purely cosmetic, procedural.
public class DecorationCreator : MonoBehaviour 
{
	//variable declaration
	private float xPos;
	private float yPos;
	private float zPos;
	
	public Material[] planetMaterials;
	public GameObject[] planets;
	public GameObject[] asteroids;
	public GameObject[] debris;

	private GameObject clone;
	private int objectSize;
	private Vector3 spawnPos;
	private int spawnType;
	private float timer;

	// Use this for initialization
	void Start () 
	{
		//spawn a set of planets when the game starts, 
		SpawnDecorations(1);
		SpawnDecorations(4);
		SpawnDecorations(7);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//spawn more objects after 300 seconds 
		timer += Time.smoothDeltaTime;
		if(Mathf.RoundToInt(timer) == 300)
		{
			SpawnDecorations(7);
			timer = 0;
		}
	}

	void SpawnDecorations (int wave)
	{
		//generate random positions for the decorations (planets) which are outside of a range around the play space where they could interfere with gameplay.
		xPos = Random.Range(-250,-750)*wave;
		yPos = Random.Range(-200,200);
		zPos = Random.Range(-600,-150);
		spawnPos = new Vector3(xPos, yPos, zPos);
		/*spawnType = Random.Range(0,0);		This is legacy from when I trialed different decorations, like debris, still here in case I update with more visuals
		switch (spawnType)
		{
			case 0:*/
			//procedural generation for the planets
			clone = Instantiate(planets[Random.Range(0,planets.Length)], spawnPos, Quaternion.identity) as GameObject;
			//add some random variation to the object
			objectSize = Random.Range(50,120);
			clone.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
			clone.GetComponent<Renderer>().material = planetMaterials[Random.Range(0,planetMaterials.Length)];
			clone.GetComponent<Renderer>().material.color = new Vector4(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),1);
			clone.transform.Rotate(new Vector3(Random.Range(0,360),Random.Range(0,360),Random.Range(0,360)));
		/*		break;
			case 1:
			objectSize = Random.Range(100,200);
			//clone = Instantiate(asteroids[Random.Range(0,asteroids.Length-1)], spawnPos, Quaternion.identity) as GameObject;
			//add some random variation to the object
			//clone.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
			//clone.rigidbody.AddForce(Vector3.left * 500);
				break;
			case 2:
			objectSize = Random.Range(100,200);
			//clone = Instantiate(debris[Random.Range(0,debris.Length-1)], spawnPos, Quaternion.identity) as GameObject;
			//add some random variation to the object
			//clone.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
				break;
		}*/

		//repeats the process for the other side of the gameplay area
		xPos = Random.Range(-250,-750)*wave;
		yPos = Random.Range(-200,200);
		zPos = Random.Range(150,600);
		spawnPos = new Vector3(xPos, yPos, zPos);
		/*spawnType = Random.Range(0,0);
		switch (spawnType)
		{
		case 0:*/
			clone = Instantiate(planets[Random.Range(0,planets.Length)], spawnPos, Quaternion.identity) as GameObject;
			//add some random variation to the object
			objectSize = Random.Range(50,120);
			clone.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
			clone.GetComponent<Renderer>().material = planetMaterials[Random.Range(0,planetMaterials.Length)];
			clone.GetComponent<Renderer>().material.color = new Vector4(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),1);
		/*	break;
		case 1:
			objectSize = Random.Range(100,200);
			//clone = Instantiate(asteroids[Random.Range(0,asteroids.Length-1)], spawnPos, Quaternion.identity) as GameObject;
			//add some random variation to the object
			//clone.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
			//clone.rigidbody.AddForce(Vector3.left * 500);
			break;
		case 2:
			objectSize = Random.Range(100,200);
			//clone = Instantiate(debris[Random.Range(0,debris.Length-1)], spawnPos, Quaternion.identity) as GameObject;
			//add some random variation to the object
			//clone.transform.localScale = new Vector3(objectSize, objectSize, objectSize);
			break;
		}*/
	}
}
