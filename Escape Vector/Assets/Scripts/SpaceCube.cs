using UnityEngine;
using System.Collections;

//this controls the grid, the grid needs to handle all of our collisions because we use discreete space but lerp the character between the spaces
public class SpaceCube : MonoBehaviour 
{
	public bool occupied = false;
	private GameSpace master;
	private GameObject player;
	public GameObject explosion;
	public GameObject Wave;
	public ParticleSystem boostParticle;
	// Use this for initialization
	void Start () 
	{
		//sets up the gameobjects it needs to message
		master = GameObject.Find("GameSpace").GetComponent<GameSpace>();
		player = GameObject.Find("Ship");
		boostParticle = GameObject.Find("BoostedParticle").GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnTriggerEnter(Collider hit)
	{
		if(occupied)	//if the player is in the space
		{
			//hit.audio.Play();
			hit.GetComponent<FMOD_oneShot>().SendMessage("Play");
			if(hit.GetComponent<Obstacle>().isAsteroid)		//collided with an asteroid
			{
				player.GetComponent<Animation>().PlayQueued("Hit");
				if(!master.shielded)
				{
					master.health -= 1;
					master.SendMessage("UpdateLives");
				}
				//spawn explosion
				//play hurt animation
				//play hit sound
				if(master.health <= 0)
				{
					master.SendMessage("GameOver");
				}
			}
			else  //collided with a pickup of some sort
			{
				//visual feedback
				master.scoreDisplay.GetComponent<TextMesh>().color = Color.white;
				int type = hit.GetComponent<Obstacle>().pickupType;
				player.GetComponent<Animation>().PlayQueued("Heal");
				master.score += 100;
				switch (type)
				{
					//we're a chevron
					case 0:	
						//set boosted on gamespace and speed up all the moving objects in the scene
						master.boosted = true;
						master.boostTimer = 0.0f;
						boostParticle.startColor = Color.green;
						boostParticle.Emit(200);
						boostParticle.Play();
						//since the player isn't actually moving, we emulate a boost by giving all the objects in the scene a boost instead,
						//not the most efficient way to do it, but somewhat of a workaround after deciding to keep the player static and the asteroids move
						foreach(GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
						{
							asteroid.GetComponent<Rigidbody>().AddForce(Vector3.right * asteroid.GetComponent<Obstacle>().forceApplied);
						}
						foreach(GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup"))
						{
							pickup.GetComponent<Rigidbody>().AddForce(Vector3.right * pickup.GetComponent<Obstacle>().forceApplied);
						}
					break;
					//we're a life
					case 1: 
						//grant a life
						boostParticle.startColor = Color.cyan;
						boostParticle.Emit(200);
						if(master.health < 8)
						{
							master.health ++;
							master.SendMessage("UpdateLives");
						}
					break;
					//we're a bomb
					case 2: 
						//destroy all asteroids in the scene
						Instantiate(Wave, transform.position, Quaternion.identity);
						foreach(GameObject asteroid in GameObject.FindGameObjectsWithTag("Asteroid"))
					    {
							if(asteroid.transform.position.y == transform.position.y)
							{
								Instantiate(explosion, asteroid.transform.position, Quaternion.identity);
								Destroy(asteroid);
							}
						}
					break;
					//we're a shield
					case 3: 
						//grant a shield to the player
						boostParticle.startColor = Color.yellow;
						boostParticle.Emit(200);
						master.shielded = true;
						master.shieldTimer = 0.0f;
					break;
				}
			}
			//hide the object and turn off its collider, destroy it shortly after (assuming Obstacle doesn't take care of that)
			//this is because sounds are on the objects, so we don't want to destroy it immediately
			hit.GetComponent<Renderer>().enabled = false;
			hit.GetComponent<Collider>().enabled = false;
			Destroy(hit.gameObject, 2f);
		}
	}
}
