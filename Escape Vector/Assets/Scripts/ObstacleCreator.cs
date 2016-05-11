using UnityEngine;
using System.Collections;

//this handles the spawning of all our asteroids, pickups, and decorative asteroids (LEGACY) which pass by outside of the game area (just to give the game a bit more life)
public class ObstacleCreator : MonoBehaviour 
{
	//variable declaration
	//items
	public GameObject[] Asteroids;
	public GameObject[] Deco_Asteroids;
	public GameObject[] Pickups;
	private GameObject[] spawns;
	//systems variables
	private float timer = 0f;
	private float flowPoint;
	private float spawnThreshold = 2;
	private int setpieceTimer = 0;
	private int setpieceFrequency = 0;
	//set piece variables
	//public GameObject[] debris;

	//for increased difficulty and avoiding staying in one spot
	private GameObject[] spawnsToSort;
	private int s = 0;

	//for manipulating difficulty easier
	public int Dif_WaveFrequency = 3;
	public int Dif_WaveDepth = 5;
	public int Dif_Base = 5;
	public int Dif_Steepness = 2;
	public int Dif_RandomLow = 5;
	public int Dif_RandomHigh = 15;
	public int baseSpeed = 150;
	public int pickupFrequency = 10;

	// Use this for initialization
	void Start () 
	{
		//populate arrays
		spawns = GameObject.FindGameObjectsWithTag("Spawns");
		spawnsToSort = GameObject.FindGameObjectsWithTag("Spawns");
		setpieceFrequency = Random.Range(10,15);
		Sort(spawnsToSort);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//spawn timer calculations, modeled on a flow graph by skewing a sine wave and adding some randomness
		flowPoint = ((Time.timeSinceLevelLoad*Dif_Steepness)+Mathf.Sin(Time.timeSinceLevelLoad*Dif_WaveFrequency)/Dif_WaveDepth)+Dif_Base;
		//if boosted, double the flowpoint.

		//Debug.Log (flowPoint);
		timer += Time.smoothDeltaTime;
		if(timer >= spawnThreshold)	//spawn based on a timer
		{
			Spawn(Random.Range(0,pickupFrequency));
			spawnThreshold = Random.Range(Dif_RandomLow,Dif_RandomHigh)/flowPoint;
			//Debug.Log(spawnThreshold);
			timer = 0;
			setpieceTimer ++;
		}

		if(setpieceTimer == setpieceFrequency)	//seperate timer for major obstacles, legacy, kept in case I decide to bring it back
		{
			setpieceTimer = 0;
			setpieceFrequency = Random.Range(15,30);
			SpawnSetpiece(Random.Range(1,5));
		}
	}

	void Spawn (int type)
	{
		//instantiate stuff
		//int i = Mathf.RoundToInt(Random.Range(-2,2));
		//int j = Mathf.RoundToInt(Random.Range(-2,2));
		if(type == 0) //spawning a pickup
		{
			GameObject clone = Instantiate (Pickups[Random.Range(0,Pickups.Length)], spawns[Random.Range(0,spawns.Length)].transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<Obstacle>().forceApplied = Mathf.RoundToInt(baseSpeed * (flowPoint+2));
			//spawn additional asteroid in shuffled array
			GameObject clone2 = Instantiate (Asteroids[Random.Range(0,Asteroids.Length)], spawnsToSort[s].transform.position, Quaternion.identity) as GameObject;
			clone2.transform.localScale += new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f));
			clone2.GetComponent<Obstacle>().forceApplied = Mathf.RoundToInt(baseSpeed * (flowPoint));
			clone2.GetComponent<Obstacle>().isAsteroid = true;
			s++;
			if(s >= spawnsToSort.Length)
			{
				s = 0;
				Sort(spawnsToSort);
			}
		}
		else //spawning an asteroid
		{
			GameObject clone = Instantiate (Asteroids[Random.Range(0,Asteroids.Length)], spawns[Random.Range(0,spawns.Length)].transform.position, Quaternion.identity) as GameObject;
			clone.transform.localScale += new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f));
			clone.GetComponent<Obstacle>().forceApplied = Mathf.RoundToInt(baseSpeed * (flowPoint));
			clone.GetComponent<Obstacle>().isAsteroid = true;
		}
	}

	void SpawnSetpiece(int type)	//for spawning decorative elements, LEGACY, kept in case I decide to bring it back
	{
		GameObject clone = null;
		switch (type)
		{
			case 0:		//debris
			//clone = Instantiate(debris[Random.Range (0,debris.Length)],spawns[Random.Range(0,spawns.Length)].transform.position, Quaternion.identity) as GameObject;	
				break;
			case 1:		//rogue asteroid
			clone = Instantiate(Deco_Asteroids[Random.Range(0,Deco_Asteroids.Length)],spawns[Random.Range(0,spawns.Length)].transform.position + new Vector3(0,20,0), Quaternion.identity) as GameObject;
				break;
			case 2:
			clone = Instantiate(Deco_Asteroids[Random.Range(0,Deco_Asteroids.Length)],spawns[Random.Range(0,spawns.Length)].transform.position + new Vector3(0,-20,0), Quaternion.identity) as GameObject;
				break;
			case 3:		//rogue asteroid
			clone = Instantiate(Deco_Asteroids[Random.Range(0,Deco_Asteroids.Length)],spawns[Random.Range(0,spawns.Length)].transform.position + new Vector3(0,0,20), Quaternion.identity) as GameObject;
				break;
			case 4:
			clone = Instantiate(Deco_Asteroids[Random.Range(0,Deco_Asteroids.Length)],spawns[Random.Range(0,spawns.Length)].transform.position + new Vector3(0,0,-20), Quaternion.identity) as GameObject;
				break;
			default:
				break;
		}
		clone.GetComponent<Obstacle>().forceApplied = Mathf.RoundToInt(baseSpeed * (flowPoint));
		clone.GetComponent<Obstacle>().isAsteroid = true;
	}

	void Sort(GameObject[] sortList)	//shuffles my list of spawn points, this makes it easier for me to spawn at a random point
	{
		for(int i = 0; i < sortList.Length-1; i++)
		{
			int index = Random.Range(0, sortList.Length);
			GameObject temp = sortList[i];
			sortList[i] = sortList[index];
			sortList[index] = temp;
		}
	}
}
