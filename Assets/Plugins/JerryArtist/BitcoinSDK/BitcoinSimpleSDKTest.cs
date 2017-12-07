using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitcoinSimpleSDKTest : MonoBehaviour {

	private string sendToWalletAddress = "1GsBUQCNLdphxhuX6aZ7QAJjpnMq8MF6p8"; // jerry's donation wallet address 4

	private GUIStyle labelStyle = new GUIStyle();

	private static int debugLevel = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnGUI()
	{
	 	float centerx = Screen.width / 2;
		float centery = Screen.height / 2;
	
	 	float buttonWidth = Screen.width * .35f;
	 	float buttonHeight = Screen.height / 20;
	 	float buttonSpacing = buttonHeight * .10f;
	 	float buttonSpacer = buttonHeight + buttonSpacing;
		float buttonVerticalOffset = Screen.height * .002f;		
		
		float columnOffset = Screen.width * 0.05f;
		
		GUI.Label(new Rect(centerx - (buttonWidth/2), buttonVerticalOffset, buttonWidth, buttonHeight), 
			"Bitcoin Simple SDK Test Client", labelStyle);
	
		GUILayout.BeginArea (new Rect(
			columnOffset, // left collumn
			//(centerx * 0.5f) - (buttonWidth/2), // left collumn
			//centerx - (buttonWidth/2), // centered button horizontally
			buttonVerticalOffset + (buttonSpacer * 1), 
			(Screen.width/2) - columnOffset, 
			buttonHeight * 5));

		if (GUILayout.Button ("Start Request", GUILayout.Height (buttonHeight))) {
			//this.StartCoroutine(BitcoinSimpleSDKTest.StartRequest_Coroutine());
			BitcoinIntegration.instance.StartRequest(sendToWalletAddress, 1, "simple btc test");
		}
		
		GUILayout.EndArea ();
		
		

	}



}
