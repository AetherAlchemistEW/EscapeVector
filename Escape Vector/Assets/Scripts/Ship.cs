using UnityEngine;
using System.Collections;

//Controls the ship, this is a complex system handling all the player controls and ship movement, has a dolly system to create nice smooth movement
public class Ship : MonoBehaviour 
{
	//variable declaration
	public Transform playerDolly;
	public GameSpace master;
	private Vector2 startPoint; //touch control stuff
	private Vector2 endPoint;
	public ParticleSystem explosion;
	public ParticleSystem healthParticle;

	//customisation stuff
	public GameObject[] ships;
	public Material[] ShipColor;

	public Transform blendPoint;

	public int touchDeadZone;

	private int shipIndex;

	private bool inverted;

	// Use this for initialization
	void Start () 
	{
		//loads in all our preferences so we can set up our ship according to our customisation in the menu
		shipIndex = PlayerPrefs.GetInt("ShipIndex");
		SetShip(shipIndex);
		SetColorA(PlayerPrefs.GetInt("MatColorA"));
		SetColorB(PlayerPrefs.GetInt("MatColorB"));
		if(PlayerPrefs.GetInt("Inverted") == 1)//Inverted
		{
			inverted = true;

		}
		else
		{
			inverted = false;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		//float verticalAxis = Input.GetAxis("Vertical") * (inverted ? -1 : 1); //turnery opperator to invert the vertical axis, don't often use it, starting to see the usefulness
		explosion.GetComponent<ParticleSystem>().enableEmission = false;
		healthParticle.GetComponent<ParticleSystem>().enableEmission = false;
		//check if PC build
		//#if UNITY_STANDALONE
		//keyboard commands
		if(Input.GetKeyDown(KeyCode.LeftArrow) && blendPoint.position.z > -2)
		{
			Move ("Left");
		}
		if(Input.GetKeyDown(KeyCode.RightArrow) && blendPoint.position.z < 2)
		{
			Move ("Right");
		}
		if(blendPoint.position.y < 2)
		{
			if(inverted && Input.GetKeyDown(KeyCode.DownArrow))
			{
				Move ("Up");
			}
			else if(!inverted && Input.GetKeyDown(KeyCode.UpArrow))
			{
				Move ("Up");
			}
		}
		if(blendPoint.position.y > -2)
		{
			if(inverted && Input.GetKeyDown(KeyCode.UpArrow))
			{
				Move ("Down");
			}
			else if(!inverted && Input.GetKeyDown(KeyCode.DownArrow))
			{
				Move ("Down");
			}
		}

		/*//gamepad and keyboard commands
		if(Input.GetAxis("Horizontal") < 0 && blendPoint.position.z > -2)
		{
			Move ("Left");
		}
		if(Input.GetAxis("Horizontal") > 0 && blendPoint.position.z < 2)
		{
			Move ("Right");
		}
		if(verticalAxis > 0 && blendPoint.position.y < 2)
		{
			Move ("Up");
		}
		if(verticalAxis < 0 && blendPoint.position.y > -2)
		{
			Move ("Down");
		}*/
		//#endif
		//check if iOS or Android build
		//touch controls
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
			{
				startPoint = Input.GetTouch(0).position;	//start point of the swipe
				endPoint = Input.GetTouch(0).position;
			}
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				endPoint = Input.GetTouch(0).position;	//recording for the end positon, use for visuals
			}

			//swipe movement
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				if(Vector2.Distance(startPoint, endPoint) > touchDeadZone) //stop taps from moving you
				{
					//do the calcs on the swipe
					if(Mathf.Abs(startPoint.x - endPoint.x) > Mathf.Abs(startPoint.y - endPoint.y)) //difference comparison to check swipe dominant axis.
					{
						//moving horizontally
						if(startPoint.x > endPoint.x && blendPoint.position.z > -2)
						{
							Move ("Left");
						}
						else if(startPoint.x < endPoint.x && blendPoint.position.z < 2)
						{
							Move ("Right");
						}
					}
					else
					{
						//moving vertically, having to try a really blunt implementation because of a bug I couldn't figure out.
						if(!inverted && blendPoint.position.y > -2 && startPoint.y > endPoint.y)
						{
							Move ("Down");
							/*if(inverted && startPoint.y < endPoint.y)		Weird bug... no idea why this isn't working...
							{
								Move ("Down");
							}
							else if(!inverted && startPoint.y > endPoint.y)
							{
								Move ("Down");
							}*/
						}
						else if(!inverted && blendPoint.position.y < 2 && startPoint.y < endPoint.y)
						{
							Move ("Up");
							/*if(inverted && startPoint.y > endPoint.y)		
							{
								Move ("Up");
							}
							else if(!inverted && startPoint.y < endPoint.y)
							{
								Move ("Up");
							}*/
						}
						else if(inverted && blendPoint.position.y > -2 && startPoint.y < endPoint.y)
						{
							Move ("Down");
						}
						else if(inverted && blendPoint.position.y < 2 && startPoint.y > endPoint.y)
						{
							Move ("Up");
						}
					}
				}
			}
		}
		Vector3 position = Vector3.Lerp(playerDolly.position, blendPoint.position, 7 * Time.smoothDeltaTime);
		playerDolly.position = position;
	}

	void Move(string direction)	//controls our movement, this way it doesn't matter what controls are being used
	{
		if(GetComponent<Animation>().isPlaying)
		{
			GetComponent<Animation>().Stop();
		}
		switch(direction)
		{
			case "Up":
				blendPoint.Translate(Vector3.up * 2);
				GetComponent<Animation>().Play("MoveUp");
				//play up sound
			break;

			case "Down":
				blendPoint.Translate(Vector3.down * 2);
				GetComponent<Animation>().Play("MoveDown");
				//play down sound
			break;

			case "Left":
				blendPoint.Translate(Vector3.back * 2);
				GetComponent<Animation>().Play("MoveLeft");
				//play left sound
			break;

			case "Right":
				blendPoint.Translate(Vector3.forward * 2);
				GetComponent<Animation>().Play("MoveRight");
				//play right sound
			break;

			default:
			break;
		}
		master.SendMessage("CheckOccupied");
	}

	void SetShip(int index)	//sets up what ship we've decided to use
	{
		//ship A
		foreach (GameObject currentShip in ships)
		{
			currentShip.GetComponent<Renderer>().enabled = false;
		}
		ships[index].GetComponent<Renderer>().enabled = true;
		PlayerPrefs.SetInt("ShipIndex", index);
		GameObject.Find("GameSpace").GetComponent<GameSpace>().playerMat = ships[index].GetComponent<Renderer>().material;
		//charge fee if not previous selection
	}

	//sets up the colours on our ship
	void SetColorA(int index)
	{
		switch(index)
		{
		case 0:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.black);//new Vector4(0.1f,0.1f,0.1f,1));
			break;
		case 1:
			ShipColor[shipIndex].SetColor("_detailColor1",  Color.white);//new Vector4(0.9f,0.9f,0.9f,1));
			break;
		case 2:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.red);//new Vector4(0.5f,0,0,1));
			break;
		case 3:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.yellow);//new Vector4(0.5f,0.5f,0,1));
			break;
		case 4:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.green);//new Vector4(0,0.5f,0,1));
			break;
		case 5:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.cyan);//new Vector4(0,0.5f,0.5f,1));
			break;
		case 6:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.blue);//new Vector4(0,0,0.5f,1));
			break;
		case 7:
			ShipColor[shipIndex].SetColor("_detailColor1", Color.magenta);//new Vector4(0.5f,0f,0.5f,1));
			break;
		}
		PlayerPrefs.SetInt("MatColorA", index);
	}
	
	void SetColorB(int index)
	{
		switch(index)
		{
		case 0:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.black);
			break;
		case 1:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.white);
			break;
		case 2:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.red);
			break;
		case 3:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.yellow);
			break;
		case 4:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.green);
			break;
		case 5:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.cyan);
			break;
		case 6:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.blue);
			break;
		case 7:
			ShipColor[shipIndex].SetColor("_detailColor2", Color.magenta);
			break;
		}
		PlayerPrefs.SetInt("MatColorB", index);
	}
}
