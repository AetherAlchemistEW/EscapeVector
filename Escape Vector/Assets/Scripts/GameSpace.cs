using UnityEngine;
using System.Collections;
using Chartboost;

//controls the game space and rules for the main game as well as GUI
public class GameSpace : MonoBehaviour 
{
	//variable declaration
	private GameObject[] spaces; //the gameplay spaces
	public GameObject occupied;	//which space is occupied
	public Transform player;
	public Material empty;
	public Material full;
	public ProceduralMaterial stars;

	//score and health and such
	public int health;
	public float score;
	public int speed;
	private bool gameEnded = false;

	//boosted
	public bool boosted = false;
	public bool shielded = false;
	public float boostTimer = 0.0f;
	public float shieldTimer = 0.0f;
	public GameObject shieldDisplay;
	public Texture[] livesDisplayTex;
	public Material livesDisplay;

	//scores array for handling highscore allocation
	public int[] ScoresArray;

	public GUISkin mySkin;

	private bool paused = false;

	public TextMesh scoreDisplay;

	public Texture optionsImg;

	public AudioSource[] soundSources;

	public Transform blendPoint;

	private float feedbackTimer;

	public Material playerMat;

	public AdManager ad;

	public ParticleSystem smoke;

	public ParticleSystem fire;
	// Use this for initialization
	void Start () 
	{
		if (stars) 
		{
			//random generation of the background
			stars.SetProceduralFloat("$randomseed", Random.Range(0,9000));
			stars.SetProceduralFloat("Nebula", Random.Range(0.0f,0.7f));
			stars.SetProceduralFloat("Bright_Stars", Random.Range(0.3f,1.0f));
			stars.SetProceduralFloat("Nebula_Color", Random.Range(0.0f,1.0f));
			stars.RebuildTexturesImmediately();
		}
		//checks what options are loaded and sets up the player
		CheckOptions();
		//load in high scores for later check
		for(int i = 0; i <= 4; i++)
		{
			ScoresArray[i] = PlayerPrefs.GetInt("HighScore"+i.ToString());
		}

		spaces = GameObject.FindGameObjectsWithTag("Space");	//occupy the spawn array
		CheckOccupied(); //set the occupied space
		UpdateLives();
		scoreDisplay.color = Color.yellow;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//increases the score as long as the player is alive
		if(player != null)
		{
			score += Time.smoothDeltaTime * health;
		}
		//if boosted, double the score gain and tell obstacle creator we're boosting, 
		//when it ends tell the creator we're not boosted.
		if(boosted)
		{
			boostTimer += Time.smoothDeltaTime;
			score += Time.smoothDeltaTime * health;
			//flicker
			if(boostTimer >= 3)
			{
				boosted = false;
			}
			scoreDisplay.color = Color.green;

		}
		//if we're shielded, adjust visuals
		if(shielded)
		{
			shieldDisplay.GetComponent<Renderer>().enabled = true;
			shieldDisplay.GetComponent<Renderer>().material.mainTextureOffset += new Vector2(0.01f,0.0f);
			shieldTimer+=Time.smoothDeltaTime;
			if(shieldTimer >= 5)
			{
				shielded = false;
				shieldDisplay.GetComponent<Renderer>().enabled = false;
			}
		}
		scoreDisplay.text = Mathf.Round(score).ToString();

		//this resets our visual queue through the score timer when we pick something up.
		if(!boosted && scoreDisplay.color != Color.yellow)
		{
			feedbackTimer += Time.smoothDeltaTime;
			if(feedbackTimer > 1)
			{
				scoreDisplay.color = Color.yellow;
				feedbackTimer = 0;
			}
		}
	}

	void CheckOccupied() //reset all the collisions and such, will happen every time the player moves or when the game restarts
	{
		foreach(GameObject space in spaces) //empty the spaces
		{
			space.GetComponent<Renderer>().material = empty;
			space.GetComponent<SpaceCube>().occupied = false;
			occupied = null;
			if(blendPoint.position == space.transform.position && !gameEnded)
			{
				space.GetComponent<Renderer>().material = full;
				occupied = space;
				occupied.GetComponent<SpaceCube>().occupied = true;
				//tell the space it is occupied for collision purposes
			}
		}
	}

	void OnGUI()
	{
		#if UNITY_ANDROID
		GUI.enabled = !CBBinding.isImpressionVisible();	//ad display related, only on Android
		#endif
		//loads in and sets up the GUI skin
		GUI.skin = mySkin;
		mySkin.label.fontSize = Screen.width/40;
		mySkin.box.fontSize = Screen.width/40;
		mySkin.button.fontSize = Screen.width/40;
		//GUI.Label(new Rect(Screen.width/4,10,Screen.width/3,50),"HP: " + health.ToString()); //hp display
		//GUI.Label(new Rect(Screen.width/3,10,Screen.width/4,50),"SCORE: " + Mathf.Round(score).ToString());

		//present GUI data
		if(gameEnded)
		{
			GUI.Label(new Rect(Screen.width/3,Screen.height/4,Screen.width/3,Screen.height/4),"GAME OVER!");
			ad.SendMessage("ShowAd");
			//press a key or tap restarts
			if(Input.GetKeyDown(KeyCode.Space) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}

		//pause button
		if(GUI.Button(new Rect(Screen.width/13*12, 0,Screen.width/13,Screen.height/10), optionsImg) || Input.GetKeyDown(KeyCode.Escape))
		{
			Time.timeScale = 0; //freeze time
			paused = true;
			player.GetComponent<Ship>().enabled = false;
			//audio.Play();
			GetComponent<FMOD_oneShot>().SendMessage("Play");
		}
		if(paused)
		{
			GUI.Box(new Rect(Screen.width/16 * 3, Screen.height/10 * 2, Screen.width/16 * 10, Screen.height/10 * 6), "Pause");
			if(GUI.Button(new Rect(Screen.width/16 * 4, Screen.height/10 * 3, Screen.width/16 * 8, Screen.height/ 10 * 2), "Quit"))
			{
				Time.timeScale = 1; //unfreeze time
				setScore();
				//audio.Play();
				GetComponent<FMOD_oneShot>().SendMessage("Play");
				Application.LoadLevel(1);
			}
			if(GUI.Button(new Rect(Screen.width/16 * 4, Screen.height/10 * 6, Screen.width/16 * 8, Screen.height/ 10 * 2), "Resume"))
			{
				Time.timeScale = 1; //unfreeze time
				paused = false;
				player.GetComponent<Ship>().enabled = true;
				//audio.Play();
				GetComponent<FMOD_oneShot>().SendMessage("Play");
			}
		}
	}

	void GameOver()	//when we game over, hide the ship and output our score
	{
		foreach(GameObject modelDisplay in GameObject.FindGameObjectsWithTag("ShipModels"))
		{
			modelDisplay.GetComponent<Renderer>().enabled = false;
		}
		player.GetComponent<Ship>().enabled = false;
		gameEnded = true;
		CheckOccupied();
		setScore();
	}

	void setScore()	//handles saving our score
	{
		if(score > ScoresArray[0])
		{
			ScoresArray[0] = Mathf.RoundToInt(score);
			PlayerPrefs.SetInt ("HighScore0", ScoresArray[0]);
		}
	}

	void CheckOptions()	//checks the options we set in the menu, some of these have been removed for various reasons but have been kept in code in case they are changed or reintroduced.
	{
		if(PlayerPrefs.GetInt("Wobble") == 1)//WorldWobble is true
		{
			Camera.main.transform.parent = player;
		}
		else
		{
			Camera.main.transform.parent = GameObject.Find("PlayerDolly").transform;
		}
		if(PlayerPrefs.GetInt("Starfield") == 1)//Starfield
		{
			GameObject.Find("Starfield").GetComponent<ParticleSystem>().enableEmission = true;
		}
		else
		{
			GameObject.Find("Starfield").GetComponent<ParticleSystem>().enableEmission = false;
		}
		if(PlayerPrefs.GetInt("Music") == 1)//Music
		{
			//Enable audio source
			Camera.main.GetComponent<AudioSource>().enabled = true;
		}
		else
		{
			//Disable audio source
			Camera.main.GetComponent<AudioSource>().enabled = false;
		}
		if(PlayerPrefs.GetInt("SFX") == 1)//SFX
		{
			//Enable audio source/s
			foreach(AudioSource sounds in soundSources)
			{
				sounds.enabled = true;
			}
		}
		else
		{
			//Disable audio source/s
			foreach(AudioSource sounds in soundSources)
			{
				sounds.enabled = false;
			}
		}
	}

	void UpdateLives()	//calculate how much health we have, update our visuals to reflect our health, specifically outline colour and particle effects
	{
		//livesDisplay.mainTexture = livesDisplayTex[health];
		Vector4 outlineColor;
		switch(health)
		{
		case 0:
			outlineColor = new Vector4(0.0f,0.0f,0.0f,1.0f);
			smoke.emissionRate = 0;
			fire.emissionRate = 0;
			break;
		case 1:
			outlineColor = new Vector4(1.0f,0.0f,0.0f,1.0f);
			//lots of fire emitting
			fire.emissionRate = 30;
			break;
		case 2:
			outlineColor = new Vector4(1.0f,1.0f,0.0f,1.0f);
			//start fire emitting
			fire.emissionRate = 10;
			break;
		case 3:
			outlineColor = new Vector4(0.0f,1.0f,0.0f,1.0f);
			//start smoke emitting
			smoke.emissionRate = 10;
			//stop fire emitting
			fire.emissionRate = 0;
			break;
		case 4:
			outlineColor = new Vector4(0.0f,1.0f,0.5f,1.0f);
			smoke.emissionRate = 0;
			break;
		case 5:
			outlineColor = new Vector4(0.0f,0.5f,1.0f,1.0f);
			break;
		case 6:
			outlineColor = new Vector4(0,1.0f,1.0f,1.0f);
			break;
		case 7:
			outlineColor = new Vector4(0.5f,1.0f,1.0f,1.0f);
			break;
		case 8:
			outlineColor = new Vector4(1.0f,1.0f,1.0f,1.0f);
			break;
		default:
			outlineColor = new Vector4(0.0f,0.0f,0.0f,1.0f);
			break;
		}
		playerMat.SetColor("_outlineColor", outlineColor);
	}
}
