using UnityEngine;
using System.Collections;

public class CoinbaseSDKTest : MonoBehaviour {
	
	private bool querying = false;
	private string label = "";
	private string status = "playing";
	private string[] services = new System.String[0];
	private GUIStyle labelStyle = new GUIStyle();
	public GUISkin mySkin;
	
	public string messageLog = "";
	private Vector2 scrollPosition;	

	private string emailStr = "testuser@gmail.com";
	private string passwordStr = "";
	
	private string transactionNotes = "Sample transaction from unity coinbase sdk";
	private string currencyTypeStr = "USD";
	private string currencyAmountStr = "1";
	
	public string apiKey = "YOUR_API_KEY_FROM_COINBASE";
	
	// Use this for initialization
	void Start () {
		if (mySkin != null) {
			labelStyle = mySkin.label;
		}
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;	
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
	 	float centerx = Screen.width / 2;
		//float centery = Screen.height / 2;
	
	 	float buttonWidth = Screen.width / 3;
	 	float buttonHeight = Screen.height / 20;
	 	float buttonSpacing = buttonHeight * .10f;
	 	float buttonSpacer = buttonHeight + buttonSpacing;
		float buttonVerticalOffset = Screen.height * .002f;		
		
		GUI.Label(new Rect(centerx - (buttonWidth/2), buttonVerticalOffset, buttonWidth, buttonHeight), 
			"Coinbase SDK Game client", labelStyle);
	
		
		
		GUILayout.BeginArea (new Rect(
			(centerx * 0.5f) - (buttonWidth/2), // left collumn
			//centerx - (buttonWidth/2), // centered button horizontally
			buttonVerticalOffset + (buttonSpacer * 1), 
			buttonWidth, buttonHeight * 12));
		
	
		
		
		if (GUILayout.Button ("Get Account Balance", GUILayout.Height (buttonHeight))) {
			StartCoroutine(CoinbaseWebChecker.GetAccountBalance(apiKey));
		}		
		
		if (GUILayout.Button ("Get Currencies", GUILayout.Height (buttonHeight))) {
			StartCoroutine(CoinbaseWebChecker.GetCurrencies());
		}		
		
		if (GUILayout.Button ("Get Currencies Exchange Rates", GUILayout.Height (buttonHeight))) {
			StartCoroutine(CoinbaseWebChecker.GetCurrenciesExchangeRates());
		}
		
		if (GUILayout.Button ("Get Prices Spot Rate", GUILayout.Height (buttonHeight))) {
			StartCoroutine(CoinbaseWebChecker.GetPricesSpotRate());
		}
		
		if (GUILayout.Button ("Get Transactions", GUILayout.Height (buttonHeight))) {
			//StartCoroutine(CoinbaseWebChecker.GetTransactions(apiKey));
		}
		

		
		if (GUILayout.Button ("Get Transactions (Requests)", GUILayout.Height (buttonHeight))) {
			//CoinbaseWebChecker.messageLog += CoinbaseWebChecker.dataMgr.GetTransactionsRequests();			
		}
		
		if (GUILayout.Button ("Delete Transactions (Cancel Requests)", GUILayout.Height (buttonHeight))) {
			//StartCoroutine(CoinbaseWebChecker.DeleteTransactionsCancelRequests(apiKey));		
		}		
		
		GUILayout.EndArea ();

		
		
		GUILayout.BeginArea (new Rect(
			(Screen.width * 0.75f) - (buttonWidth/2), // right collumn
			//centerx - (buttonWidth/2), // centered button horizontally
			buttonVerticalOffset + (buttonSpacer * 1), 
			buttonWidth, buttonHeight * 12));		
		
		
		// email label, email texfield
		GUILayout.BeginHorizontal ();		
		GUILayout.Label ("email:");
		emailStr = GUILayout.TextField (emailStr);		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal ();		
		GUILayout.Label ("password:");
		passwordStr = GUILayout.PasswordField (passwordStr,"*"[0], 25);		
		GUILayout.EndHorizontal();	
		
		
		if (GUILayout.Button ("Post Users (Signup)", GUILayout.Height (buttonHeight))) {
			string postUsersJson = CoinbaseWebChecker.dataMgr.CreateJsonPostUsers(emailStr,passwordStr);
			CoinbaseWebChecker.messageLog += postUsersJson;
			CoinbaseWebChecker.messageLog += "\n";
			
			StartCoroutine(CoinbaseWebChecker.PostUsers(postUsersJson));
		}		
		
		GUILayout.BeginHorizontal ();		
		GUILayout.Label ("currency type:");
		currencyTypeStr = GUILayout.TextField (currencyTypeStr);		
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal ();		
		GUILayout.Label ("currency amount:");
		currencyAmountStr = GUILayout.TextField (currencyAmountStr);		
		GUILayout.EndHorizontal();		
		
		GUILayout.Label ("transaction notes:");
		transactionNotes = GUILayout.TextArea(transactionNotes, GUILayout.Height (buttonHeight));
		
		
		if (GUILayout.Button ("Post Transactions Request Money", GUILayout.Height (buttonHeight))) {
			
			string transactionRequestJson = CoinbaseWebChecker.dataMgr.CreateJsonTransactionRequest(
				emailStr, currencyTypeStr, currencyAmountStr, transactionNotes);
			
			CoinbaseWebChecker.messageLog += transactionRequestJson;
			CoinbaseWebChecker.messageLog += "\n";
			
			StartCoroutine(CoinbaseWebChecker.PostTransactionsRequestMoney(apiKey,transactionRequestJson));			
		}		
		
		if (GUILayout.Button ("Verify Transactions (complete)", GUILayout.Height (buttonHeight))) {
			
			CoinbaseDataManager.ResponseData response = CoinbaseWebChecker.dataMgr.VerifyTransactionsComplete(emailStr, transactionNotes);
			
			CoinbaseWebChecker.messageLog += "Verify Transactions: " + response.success;
			CoinbaseWebChecker.messageLog += "\n";
			CoinbaseWebChecker.messageLog += response.message;
		}		
		
		GUILayout.EndArea ();
		
		
		
		
		
		Rect scrollOuterRect = new Rect (Screen.width * (( 1f - .7f) / 2f ), 
							Screen.height * (1.0f - .3f), 
							Screen.width * .7f, 
							Screen.height * .3f);
	
		Rect scrollInnerRect = new Rect (0f,0f, 
							Screen.width * .7f, 
							Screen.height);
	
	    scrollPosition = GUI.BeginScrollView (
	    					scrollOuterRect, 
				    		//Rect (10,300,100,100),
				            scrollPosition, 
				            //Rect (0, 0, 220, 200)
				            scrollInnerRect				            
				            );
	                           
		messageLog = CoinbaseWebChecker.messageLog;
		messageLog = GUI.TextArea (new Rect (0f,0f,Screen.width, Screen.height),
							messageLog);
	
		GUI.EndScrollView();
		
	}
	
	
	
}
