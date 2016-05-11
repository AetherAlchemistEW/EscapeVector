using UnityEngine;
using System.Collections;

//used to destroy the shockwave after it has served its purpose, can be reused on other assets like explosions if needed.
public class Cleanup : MonoBehaviour 
{
	void FixedUpdate () 
	{
		Destroy(this.gameObject, 2f); //destroys the objet after two seconds
		if(this.CompareTag("Wave")) //if it's the shockwave we'll scale it out gradually to emulate a blast
		{
			this.transform.localScale += new Vector3(1.5f,0,1.5f);
		}
	}
}
