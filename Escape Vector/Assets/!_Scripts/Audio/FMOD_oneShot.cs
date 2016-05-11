using UnityEngine;
using System.Collections;
using FMOD.Studio; //Declares and defines the FMOD API.

public class FMOD_oneShot : MonoBehaviour 

{

	//
	/* VARIABLE DECLERATION AND SETUP*/
	//

	public FMODAsset soundToPlay; //Declares FMOD Asset, this appears in the inspector of the object that this script is added to. Drop the FMOD Asset to be played here.
	public string AssetPath = ""; //Declares a public Asset Path variable, this appears in the inspector of the object that this script is added to and displays the current FMOD Asset's director path.
	public string AssetID = ""; //Declares a public Asset ID variable, this appears in the inspector of the object that this script is added to and displays the current FMOD Asset's GUID.
	public bool PlayOnStart;

	//
	// Use this for initialization
	//

	void Start () 
	
	{
		AssetPath = (soundToPlay.path); // Gets the path of the currently placed FMOD Asset and displays it within Asset Path string.
		AssetID = (soundToPlay.id); // Gets the GUID number of the currently placed FMOD Asset and displays it within Asset ID string.

	//Public gameObject example:
		if(PlayOnStart)
		{
			FMOD_StudioSystem.instance.PlayOneShot(soundToPlay, transform.position); //Plays a one shot sound refering to the "Sound To Play" variable, and uses the objects transform.position.
		}
	//Call from GUID example:

		//FMOD_StudioSystem.instance.PlayOneShot("put guid here", transform.position); //Plays a one shot sound refering to the "Sound To Play" variable, and uses the objects transform.position.

	//Call from Path example:

		//FMOD_StudioSystem.instance.PlayOneShot("put path here", transform.position); //Plays a one shot sound refering to the "Sound To Play" variable, and uses the objects transform.position.

		//Debug.Log ("Played one shot sound");



	}
	
	// Update is called once per frame
	void Update () 
	
	{
	

	}

	void Play()
	{
		FMOD_StudioSystem.instance.PlayOneShot(soundToPlay, transform.position);
	}
}
