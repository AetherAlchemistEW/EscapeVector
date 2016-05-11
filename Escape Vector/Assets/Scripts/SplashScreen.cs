using UnityEngine;
using System.Collections;

//this script handles the splash screen, designed to be reusable, easily modified in engine, and extensible.
public class SplashScreen : MonoBehaviour 
{
	//variable declaration
	private float splashTimer = 0.0f;
	public float splashLength;
	public int levelToLoad;
	public Texture[] splashMats;
	private int index = 0;
	
	void Update () 
	{
		//this allows the splash function to repeat for as many images as I have and then load the assigned scene, usually the main menu.
		if(index < splashMats.Length)
		{
			GetComponent<GUITexture>().texture = splashMats[index];
			splashTimer += Time.smoothDeltaTime;
			//fades the image in
			if(splashTimer < splashLength/2)
			{
				GetComponent<GUITexture>().color = new Vector4(0.5f,0.5f,0.5f,splashTimer/(splashLength/2));
			}
			//fades out
			else if(splashTimer < splashLength)
			{
				GetComponent<GUITexture>().color = new Vector4(0.5f,0.5f,0.5f,(splashLength-splashTimer)/splashLength);
			}
			//goes on to the next image
			else
			{
				index ++;
				splashTimer = 0;
			}
		}
		else
		{
			Application.LoadLevel(levelToLoad);
		}
	}
}
