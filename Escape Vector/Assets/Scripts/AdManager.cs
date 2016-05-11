using UnityEngine;
using System.Collections;
using System;
using Chartboost;

//script to handle in-app advertising, largely derived from the example supplied by Chartboost with some unnecessary features cut. Only works on Android.
public class AdManager : MonoBehaviour {
	
	// Use this for initialization
	void Start () 
	{
		#if UNITY_ANDROID
		//ShowAd();
		CBBinding.cacheInterstitial("Default");
		#endif
	}
	
	// Update is called once per frame
	#if UNITY_ANDROID
	public void Update() 
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
		{
			if (CBBinding.onBackPressed())
			{
				return;
			}
			else
			{
				Application.Quit();
			}
		}
	}
	#endif

	void OnEnable()
	{
		// Initialize the Chartboost plugin
		#if UNITY_ANDROID
		// Replace these with your own Android app ID and signature from the Chartboost web portal
		CBBinding.init ("53d5c1a8c26ee4365eda53b1", "c30040d6282239b09b38608efd37d5f7ae58a99c");
		#elif UNITY_IPHONE
		// Replace these with your own iOS app ID and signature from the Chartboost web portal
		CBBinding.init("CB_APP_ID_IOS", "CB_APP_SIG_IOS");
		#endif
	}

	#if UNITY_ANDROID
	
	// Manage Chartboost plugin lifecycle 
	void OnApplicationPause(bool paused) 
	{
		CBBinding.pause(paused);
	}
	
	void OnDisable() 
	{
		// Shut down the Chartboost plugin
		CBBinding.destroy();
	}
	#endif

	void ShowAd()
	{
		#if UNITY_ANDROID
		CBBinding.showInterstitial("Default");
		#endif
	}
}
