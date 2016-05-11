using UnityEngine;
using System.Collections;

//handles the main menu in the Menu scene
public class Menu : MonoBehaviour 
{
	//variable declaration
	//what menu option is selected?
	private bool Options = false;
	private bool HighScores = false;
	public bool startGame = false;

	//screen dimensions
	private float width;
	private float height;

	//high scores
	public int[] ScoresArray;

	//options, some are legacy
	private bool worldWobble = true;
	private bool inverted = true;
	private bool music = true;
	private bool sounds = true;

	//GUISkin
	public GUISkin mySkin;

	//procedural backdrop
	public ProceduralMaterial starsBG;

	public bool customise; //for customisation screen.

	//variables for handling the ships and customisation
	public GameObject[] ships;
	public Material[] ShipColor;

	public Texture backArrow;
	public Texture[] swatches;
	public Texture[] shipPics;

	public GUIStyle startButton;

	//private int shipIndex = 0;

	public Transform theShip;

	private Vector2 touchPos;

	//audio.Play();
	private FMOD_oneShot fmodAudio;
	// Use this for initialization
	void Start () 
	{
		//audio.Play();
		fmodAudio = GetComponent<FMOD_oneShot>();
		if (starsBG) //randomise backdrop
		{
			starsBG.SetProceduralFloat("$randomseed", Random.Range(0,9000));
			starsBG.SetProceduralFloat("Nebula", Random.Range(0.0f,0.7f));
			starsBG.SetProceduralFloat("Bright_Stars", Random.Range(0.3f,1.0f));
			starsBG.SetProceduralFloat("Nebula_Color", Random.Range(0.0f,1.0f));
			starsBG.RebuildTexturesImmediately();
		}
		GetComponent<GUITexture>().pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);

		width = Screen.width;
		height = Screen.height;
		//check playerprefs keys exist for options and make them if they don't
		CheckOptions();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(startGame) //start the game, run at the end of our animation and fade
		{
			Application.LoadLevel(2);
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		//handle our options 
		if(music)
		{
			GameObject.Find("Starfield").GetComponent<AudioSource>().enabled = true;
		}
		else
		{
			GameObject.Find("Starfield").GetComponent<AudioSource>().enabled = false;
		}
		if(sounds)
		{
			GetComponent<AudioSource>().enabled = true;
		}
		else
		{
			GetComponent<AudioSource>().enabled = false;
		}
		//if we're not in the customise menu, play our idle animations, these are randomly selected from a list of subtle animations to stop it from getting too repetitive
		if(!customise)
		{
			//play idle animations
			if(!GetComponent<Animation>().isPlaying)
			{
				//system is designed to have a variable length 
				string animationName = "";
				switch(Random.Range(0,4))
				{
				case 0:
					animationName = "ShipIdleOne";
					break;
				case 1:
					animationName = "ShipIdleTwo";
					break;
				case 2:
					animationName = "ShipIdleThree";
					break;
				case 3:
					animationName = "ShipIdleTwo";
					break;
				case 4:
					break;
				default:
					break;
				}
				GetComponent<Animation>().PlayQueued(animationName);
			}
		}
	}

	void OnGUI()
	{
		GUI.skin = mySkin;
		//left side has buttons, right has options
		mySkin.button.fontSize = (Screen.width/50);
		mySkin.label.fontSize = Screen.width/50;
		mySkin.box.fontSize = Screen.width/50;
		startButton.fontSize = (Screen.width/40);
		/*if(GUI.Button(new Rect(0,0,100,100),"CBTest"))
		{
			Application.LoadLevel(2);
		}*/
		//if we haven't started yet and we aren't customising, show the main menu items
		if(!startGame)
		{
			if(!customise)
			{
				//show buttons
				//Options
				if(GUI.Button (new Rect(width/10,height/6,width/3,height/6), "Options"))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//disable all bools 
					if(!Options)
					{
						HighScores = false;
						Options = true;
					}
					else
					{
						HighScores = false;
						Options = false;
					}
					//enable options
				}
				if(Options)
				{
					//options stuff
					GUI.Box(new Rect(width/2, height/7, width/10*4, height/7*5), "Options");
					//WorldWobble
					worldWobble = GUI.Toggle(new Rect(width/2, height/7*2, width/10*4, height/7), worldWobble, "Camera Animation");
					//Starfield
					inverted = GUI.Toggle(new Rect(width/2, height/7*3, width/10*4, height/7), inverted, "Inverted");
					//Music
					//music = GUI.Toggle(new Rect(width/2, height/7*4, width/10*4, height/7), music, "Music");
					//SFX
					//sounds = GUI.Toggle(new Rect(width/2, height/7*5, width/10*4, height/7), sounds, "SFX");
				}
				if(GUI.Button (new Rect(width/10,height/6 * 2,width/3,height/6), "High Scores"))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					if(!HighScores)
					{
						HighScores = true;
						Options = false;
					}
					else
					{
						HighScores = false;
						Options = false;
					}
					//enable highScores
				}
				if(HighScores)
				{
					GUI.Box(new Rect(width/2, height/7, width/10*4, height/7*5), "High Scores");
					//import and sort top 5 high scores
					for(int i = 0; i <= 4; i++)
					{
						int tempVal = 0;
						for(int j = i; j < 4; j++)
						{
							if(ScoresArray[j] > ScoresArray[j+1])
							{
								tempVal = ScoresArray[j];
								ScoresArray[j] = ScoresArray[j+1];
								ScoresArray[j+1] = tempVal;
							}
						}
					}
					//set playerprefs again
					for(int i = 0; i <= 4; i++)
					{
						PlayerPrefs.SetInt("HighScore"+i.ToString(), ScoresArray[i]);
					}

					//display high scores
					GUI.Label(new Rect(width/5*3, height/10*3, width/10*2, height/10), ScoresArray[4].ToString());
					GUI.Label(new Rect(width/5*3, height/10*4, width/10*2, height/10), ScoresArray[3].ToString());
					GUI.Label(new Rect(width/5*3, height/10*5, width/10*2, height/10), ScoresArray[2].ToString());
					GUI.Label(new Rect(width/5*3, height/10*6, width/10*2, height/10), ScoresArray[1].ToString());
					GUI.Label(new Rect(width/5*3, height/10*7, width/10*2, height/10), ScoresArray[0].ToString());
				}
				if(GUI.Button (new Rect(width/10,height/6 * 3,width/3,height/6), "Customise Ship"))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					GetComponent<Animation>().Stop();
					customise = true;
					GetComponent<Animation>().PlayQueued("PanUp");
				}
				if(GUI.Button(new Rect(width/10,height/6 * 4,width/3,height/6), "Start Game", startButton))
				{
					GetComponent<Animation>().Stop();
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//StartGame
					HighScores = false;
					Options = false;
					//Fade out and load level
					SetOptions();
					GetComponent<Animation>().PlayQueued("StartGame");
				}
			}
			else
			{
				//customise screen
				GUI.Box (new Rect(0,Screen.height/3*2,Screen.width,Screen.height/3),"Ship");
				GUI.Box (new Rect(Screen.width/4*2,0,Screen.width/4,Screen.height/3*2),"Colour A");
				GUI.Box (new Rect(Screen.width/4*3,0,Screen.width/4,Screen.height/3*2),"Colour B");

				//manipulate ship controls, touch
				if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
				{
					touchPos = Input.GetTouch(0).position;
				}
				if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					Vector2 currentPos = Input.GetTouch(0).position;
					theShip.transform.Rotate(new Vector3((touchPos.x-currentPos.x)/200,0,(touchPos.y-currentPos.y)/200));
				}
				//manipulate ship, mouse
				if(Input.GetMouseButtonDown(1))
				{
					touchPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				}
				if(Input.GetMouseButton(1))
				{
					Vector2 currentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
					theShip.transform.Rotate(new Vector3((touchPos.x-currentPos.x)/200,0,(touchPos.y-currentPos.y)/200));
				}

				//ship selection
				if(GUI.Button(new Rect(0,(Screen.height/3*2)+(Screen.height/10), Screen.width/4, (Screen.height/3)-(Screen.height/10)), shipPics[0]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					SetShip(0);
				}
				if(GUI.Button(new Rect(Screen.width/4,(Screen.height/3*2)+(Screen.height/10), Screen.width/4, (Screen.height/3)-(Screen.height/10)), shipPics[1]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					SetShip(1);
				}
				if(GUI.Button(new Rect(Screen.width/4*2,(Screen.height/3*2)+(Screen.height/10), Screen.width/4, (Screen.height/3)-(Screen.height/10)), shipPics[2]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					SetShip(2);
				}
				if(GUI.Button(new Rect(Screen.width/4*3,(Screen.height/3*2)+(Screen.height/10), Screen.width/4, (Screen.height/3)-(Screen.height/10)), shipPics[3]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					SetShip(3);//ship D
				}

				//colour A
				if(GUI.Button(new Rect(Screen.width/8*4, Screen.height/8, Screen.width/8, Screen.height/8), swatches[0]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Black
					SetColorA(0);
				}
				if(GUI.Button(new Rect(Screen.width/8*4, Screen.height/8*2, Screen.width/8, Screen.height/8), swatches[1]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//White
					SetColorA(1);
				}
				if(GUI.Button(new Rect(Screen.width/8*4, Screen.height/8*3, Screen.width/8, Screen.height/8), swatches[2]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Red
					SetColorA(2);
				}
				if(GUI.Button(new Rect(Screen.width/8*4, Screen.height/8*4, Screen.width/8, Screen.height/8), swatches[3]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Orange
					SetColorA(3);
				}
				if(GUI.Button(new Rect(Screen.width/8*5, Screen.height/8, Screen.width/8, Screen.height/8), swatches[4]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Yellow
					SetColorA(4);
				}
				if(GUI.Button(new Rect(Screen.width/8*5, Screen.height/8*2, Screen.width/8, Screen.height/8), swatches[5]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Green
					SetColorA(5);
				}
				if(GUI.Button(new Rect(Screen.width/8*5, Screen.height/8*3, Screen.width/8, Screen.height/8), swatches[6]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Blue
					SetColorA(6);
				}
				if(GUI.Button(new Rect(Screen.width/8*5, Screen.height/8*4, Screen.width/8, Screen.height/8), swatches[7]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Purple
					SetColorA(7);
				}

				//colour B
				if(GUI.Button(new Rect(Screen.width/8*6, Screen.height/8, Screen.width/8, Screen.height/8), swatches[0]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Black
					SetColorB(0);
				}
				if(GUI.Button(new Rect(Screen.width/8*6, Screen.height/8*2, Screen.width/8, Screen.height/8), swatches[1]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//White
					SetColorB(1);
				}
				if(GUI.Button(new Rect(Screen.width/8*6, Screen.height/8*3, Screen.width/8, Screen.height/8), swatches[2]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Red
					SetColorB(2);
				}
				if(GUI.Button(new Rect(Screen.width/8*6, Screen.height/8*4, Screen.width/8, Screen.height/8), swatches[3]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Orange
					SetColorB(3);
				}
				if(GUI.Button(new Rect(Screen.width/8*7, Screen.height/8, Screen.width/8, Screen.height/8), swatches[4]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Yellow
					SetColorB(4);
				}
				if(GUI.Button(new Rect(Screen.width/8*7, Screen.height/8*2, Screen.width/8, Screen.height/8), swatches[5]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Green
					SetColorB(5);
				}
				if(GUI.Button(new Rect(Screen.width/8*7, Screen.height/8*3, Screen.width/8, Screen.height/8), swatches[6]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Blue
					SetColorB(6);
				}
				if(GUI.Button(new Rect(Screen.width/8*7, Screen.height/8*4, Screen.width/8, Screen.height/8), swatches[7]))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					//Purple
					SetColorB(7);
				}

				//exit customise
				if(GUI.Button(new Rect(0,0,Screen.width/8,Screen.height/10), backArrow))
				{
					//audio.Play();
					fmodAudio.SendMessage("Play");
					GetComponent<Animation>().PlayQueued("PanDown");
				}
			}
		}
	}

	void SetOptions()	//saves the options out to player prefs so we can load them in the main game
	{
		if(worldWobble)//WorldWobble
		{
			PlayerPrefs.SetInt("Wobble", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Wobble", 0);
		}
		
		if(inverted)//inverted
		{
			PlayerPrefs.SetInt("Inverted", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Inverted", 0);
		}
		
		if(music)//Music
		{
			PlayerPrefs.SetInt("Music", 1);
		}
		else
		{
			PlayerPrefs.SetInt("Music", 0);
		}
		
		if(sounds)//SFX
		{
			PlayerPrefs.SetInt("SFX", 1);
		}
		else
		{
			PlayerPrefs.SetInt("SFX", 0);
		}
	}

	void CheckOptions()	//checks the options we've set and sets up the menu accordingly 
	{
		if(!PlayerPrefs.HasKey("Wobble"))//WorldWobble
		{
			PlayerPrefs.SetInt("Wobble", 1);
		}
		if(!PlayerPrefs.HasKey("Inverted"))//Starfield
		{
			PlayerPrefs.SetInt("Inverted", 0);
		}
		if(!PlayerPrefs.HasKey("Music"))//Music
		{
			PlayerPrefs.SetInt("Music", 1);
		}
		if(!PlayerPrefs.HasKey("SFX"))//SFX
		{
			PlayerPrefs.SetInt("SFX", 1);
		}
		//high scores
		if(!PlayerPrefs.HasKey("HighScore0"))//HighScore 1
		{
			PlayerPrefs.SetInt("HighScore0", 0);
		}
		if(!PlayerPrefs.HasKey("HighScore1"))//HighScore 2
		{
			PlayerPrefs.SetInt("HighScore1", 0);
		}
		if(!PlayerPrefs.HasKey("HighScore2"))//HighScore 3
		{
			PlayerPrefs.SetInt("HighScore2", 0);
		}
		if(!PlayerPrefs.HasKey("HighScore3"))//HighScore 4
		{
			PlayerPrefs.SetInt("HighScore3", 0);
		}
		if(!PlayerPrefs.HasKey("HighScore4"))//HighScore 5
		{
			PlayerPrefs.SetInt("HighScore4", 0);
		}
		//populate array
		for(int i = 0; i <= 4; i++)
		{
			ScoresArray[i] = PlayerPrefs.GetInt("HighScore"+i.ToString());
		}
		//set options bools
		if(PlayerPrefs.GetInt("Wobble") == 1)//WorldWobble is true
		{
			worldWobble = true;
		}
		else
		{
			worldWobble = false;
		}
		if(PlayerPrefs.GetInt("Inverted") == 1)//Inverted
		{
			inverted = true;
		}
		else
		{
			inverted = false;
		}
		if(PlayerPrefs.GetInt("Music") == 1)//Music
		{
			music = true;
		}
		else
		{
			music = false;
		}
		if(PlayerPrefs.GetInt("SFX") == 1)//SFX
		{
			sounds = true;
		}
		else
		{
			sounds = false;
		}

		//ship and material colors
		if(!PlayerPrefs.HasKey("ShipIndex"))
		{
			PlayerPrefs.SetInt("ShipIndex", 0);
		}
		else
		{
			SetShip(PlayerPrefs.GetInt("ShipIndex"));
		}
		if(!PlayerPrefs.HasKey("MatColorA"))
		{
			PlayerPrefs.SetInt("MatColorA", 1);
		}
		else
		{
			SetColorA(PlayerPrefs.GetInt("MatColorA"));
		}
		if(!PlayerPrefs.HasKey("MatColorB"))
		{
			PlayerPrefs.SetInt("MatColorB", 6);
		}
		else
		{
			SetColorB(PlayerPrefs.GetInt("MatColorB"));
		}
		foreach (Material color in ShipColor)
		{
			color.SetColor("_outlineColor", Color.white);
		}
	}

	void SetShip(int index)	//sets what ship we're using
	{
		//ship A
		foreach (GameObject currentShip in ships)
		{
			currentShip.GetComponent<Renderer>().enabled = false;
		}
		ships[index].GetComponent<Renderer>().enabled = true;
		PlayerPrefs.SetInt("ShipIndex", index);
		//charge fee if not previous selection
		//shipIndex = index;
	}

	//these two methods set up the colours which on our ship
	void SetColorA(int index)
	{
		switch(index)
		{
		case 0:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.black);//new Vector4(0.1f,0.1f,0.1f,1));
			}
			break;
		case 1:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1",  Color.white);//new Vector4(0.9f,0.9f,0.9f,1));
			}
			break;
		case 2:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.red);//new Vector4(0.5f,0,0,1));
			}
			break;
		case 3:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.yellow);//new Vector4(0.5f,0.5f,0,1));
			}
			break;
		case 4:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.green);//new Vector4(0,0.5f,0,1));
			}
			break;
		case 5:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.cyan);//new Vector4(0,0.5f,0.5f,1));
			}
			break;
		case 6:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.blue);//new Vector4(0,0,0.5f,1));
			}
			break;
		case 7:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor1", Color.magenta);//new Vector4(0.5f,0f,0.5f,1));
			}
			break;
		}
		PlayerPrefs.SetInt("MatColorA", index);
	}

	void SetColorB(int index)
	{
		switch(index)
		{
		case 0:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.black);
			}
			break;
		case 1:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.white);
			}
			break;
		case 2:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.red);
			}
			break;
		case 3:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.yellow);
			}
			break;
		case 4:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.green);
			}
			break;
		case 5:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.cyan);
			}
			break;
		case 6:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.blue);
			}
			break;
		case 7:
			foreach (Material color in ShipColor)
			{
				color.SetColor("_detailColor2", Color.magenta);
			}
			break;
		}
		PlayerPrefs.SetInt("MatColorB", index);
	}
}
