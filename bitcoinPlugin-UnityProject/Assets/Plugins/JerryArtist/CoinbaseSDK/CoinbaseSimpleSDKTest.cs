using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinbaseSimpleSDKTest : MonoBehaviour {
	
	private bool querying = false;
	private string label = "";
	private string status = "playing";
	private string[] services = new System.String[0];
	private GUIStyle labelStyle = new GUIStyle();
	public GUISkin mySkin;
	
	public string messageLog = "";
	private Vector2 scrollPosition;	
	
	public Texture lockOpen;
	public Texture lockClosed;
	
	private string emailStr = "";
	
	private bool debugMode = false;
	
	public enum TransactionState {
		None,
		Pending,
		Complete
	};
	

	// this class is used to display the transaction status of each content purchase type
	public class ContentPurchaseData {
		public bool contentLock = true;	
		public string description = "Content 1";
		
		public string currencyTypeStr = "USD";
		public string currencyAmountStr = "1";
		
		public string transactionNotes = "";
		public TransactionState transactionStateContent = TransactionState.None;		
		
	}
	
	List<ContentPurchaseData> contentPurchaseList = new List<ContentPurchaseData>();
	
	
	public string apiKey = "YOUR_API_KEY_FROM_COINBASE";
	
	public void VerifyTransactionsCallback(CoinbaseWebChecker.ResponseData data)
	{
		CoinbaseWebChecker.messageLog = ""; // reset the message log
		CoinbaseWebChecker.messageLog += "finished callback\n";
		
		for (int i = 0; i < contentPurchaseList.Count; i++) {
			
			ContentPurchaseData contentPurchaseData = contentPurchaseList[i];
			contentPurchaseData.transactionStateContent = TransactionState.None;
			
			// check for pending transactions
			// ** this needs to run after get transactions is complete!
			CoinbaseDataManager.ResponseData response = CoinbaseWebChecker.dataMgr.GetTransactionsRequests(emailStr, contentPurchaseData.transactionNotes);					
			if (response.success) {
				contentPurchaseData.transactionStateContent = TransactionState.Pending;				
			} 
			CoinbaseWebChecker.messageLog += "Verify Requests: for content["+i+"]:" + response.success;
			CoinbaseWebChecker.messageLog += "\n";
			CoinbaseWebChecker.messageLog += response.message;			
			
			
			CoinbaseDataManager.ResponseData response2 = CoinbaseWebChecker.dataMgr.VerifyTransactionsComplete(emailStr, contentPurchaseData.transactionNotes);
			if (response2.success) {
				contentPurchaseData.transactionStateContent = TransactionState.Complete;	
				contentPurchaseData.contentLock = false;
			}
			//CoinbaseWebChecker.messageLog = ""; // reset the message log
			CoinbaseWebChecker.messageLog += "Verify Transactions: for content["+i+"]:" + response2.success;
			CoinbaseWebChecker.messageLog += "\n";
			CoinbaseWebChecker.messageLog += response2.message;		
		
		}
	}	
	
	
	
	// Use this for initialization
	void Start () {
		if (mySkin != null) {
			labelStyle = mySkin.label;
		}
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.normal.textColor = Color.white;	
		
		// setup default purchases
		ContentPurchaseData content1 = new ContentPurchaseData();
		content1.description = "Content1";
		content1.transactionNotes = "Sample transaction from unity coinbase sdk";		
		contentPurchaseList.Add(content1);
		
		ContentPurchaseData content2 = new ContentPurchaseData();
		content2.description = "Content2";
		content2.transactionNotes = "jerryartist.com : app1 : content2";
		contentPurchaseList.Add(content2);		
		
		ContentPurchaseData content3 = new ContentPurchaseData();
		content3.description = "Content3";
		content3.transactionNotes = "jerryartist.com : app1 : content3";
		contentPurchaseList.Add(content3);			
		
		
		// get set of existing transactions before startup
		CoinbaseWebChecker.Callback myCallback = VerifyTransactionsCallback;
		this.StartCoroutine(CoinbaseWebChecker.GetTransactions_Coroutine(apiKey, myCallback));
		
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
			"Coinbase Simple SDK Test Client", labelStyle);
	
		
		
		GUILayout.BeginArea (new Rect(
			columnOffset, // left collumn
			//(centerx * 0.5f) - (buttonWidth/2), // left collumn
			//centerx - (buttonWidth/2), // centered button horizontally
			buttonVerticalOffset + (buttonSpacer * 1), 
			(Screen.width/2) - columnOffset, 
			buttonHeight * 12));
		
		
		// email label, email texfield
		GUILayout.BeginHorizontal ();		
		GUILayout.Label ("email:");
		emailStr = GUILayout.TextField (emailStr);		
		GUILayout.EndHorizontal();
		
		ContentPurchaseData content1 = contentPurchaseList[0];
		
		if (debugMode) {
			if (GUILayout.Button ("Get Transactions", GUILayout.Height (buttonHeight))) {
				//CoinbaseWebChecker.GetTransactions(apiKey, DidAThing);				
				//StartCoroutine(CoinbaseWebChecker.GetTransactions(apiKey, DidAThing));
				this.StartCoroutine(CoinbaseWebChecker.GetTransactions_Coroutine(apiKey, null));
			}
			
			
			if (GUILayout.Button ("Get Transactions (Requests)", GUILayout.Height (buttonHeight))) {
				CoinbaseWebChecker.messageLog = ""; // reset the message log
				//CoinbaseWebChecker.messageLog += CoinbaseWebChecker.dataMgr.GetTransactionsRequests();			
				
				CoinbaseDataManager.ResponseData response = CoinbaseWebChecker.dataMgr.GetTransactionsRequests(emailStr, content1.transactionNotes);			
				CoinbaseWebChecker.messageLog += "Verify Requests: " + response.success;
				CoinbaseWebChecker.messageLog += "\n";
				CoinbaseWebChecker.messageLog += response.message;			
			}
			
			if (GUILayout.Button ("Delete Transactions (Cancel Requests)", GUILayout.Height (buttonHeight))) {
				StartCoroutine(CoinbaseWebChecker.DeleteTransactionsCancelRequests(apiKey, emailStr, content1.transactionNotes));		
			}		
		}
		
		
		for (int i = 0; i < contentPurchaseList.Count; i++) {			
			ContentPurchaseData contentPurchaseData = contentPurchaseList[i];
			DisplayPurchaseContentGUI(contentPurchaseData);
		}
		

		
		
		GUILayout.EndArea ();

		
		
		GUILayout.BeginArea (new Rect(
			(Screen.width * 0.75f) - (buttonWidth/2), // right collumn
			//centerx - (buttonWidth/2), // centered button horizontally
			buttonVerticalOffset + (buttonSpacer * 1), 
			buttonWidth, buttonHeight * 12));		
		
		

						
		if (debugMode) {
			
			GUILayout.BeginHorizontal ();		
			GUILayout.Label ("currency type:");
			content1.currencyTypeStr = GUILayout.TextField (content1.currencyTypeStr);		
			GUILayout.EndHorizontal();
	
			GUILayout.BeginHorizontal ();		
			GUILayout.Label ("currency amount:");
			content1.currencyAmountStr = GUILayout.TextField (content1.currencyAmountStr);		
			GUILayout.EndHorizontal();		
			
			GUILayout.Label ("transaction notes:");
			content1.transactionNotes = GUILayout.TextArea(content1.transactionNotes, GUILayout.Height (buttonHeight));
				
			
			if (GUILayout.Button ("Post Transactions Request Money", GUILayout.Height (buttonHeight))) {
				
				string transactionRequestJson = CoinbaseWebChecker.dataMgr.CreateJsonTransactionRequest(
					emailStr, content1.currencyTypeStr, content1.currencyAmountStr, content1.transactionNotes);
				
				CoinbaseWebChecker.messageLog += transactionRequestJson;
				CoinbaseWebChecker.messageLog += "\n";
				
				StartCoroutine(CoinbaseWebChecker.PostTransactionsRequestMoney(apiKey,transactionRequestJson));			
			}		
			
			if (GUILayout.Button ("Verify Transactions (complete)", GUILayout.Height (buttonHeight))) {
				
				CoinbaseDataManager.ResponseData response = CoinbaseWebChecker.dataMgr.VerifyTransactionsComplete(emailStr, content1.transactionNotes);
				
				CoinbaseWebChecker.messageLog = ""; // reset the message log
				CoinbaseWebChecker.messageLog += "Verify Transactions: " + response.success;
				CoinbaseWebChecker.messageLog += "\n";
				CoinbaseWebChecker.messageLog += response.message;
			}		
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
	
	private void DisplayPurchaseContentGUI(ContentPurchaseData content) {
		
		GUILayout.BeginHorizontal ();	
		if (content.contentLock) {
			if (GUILayout.Button(lockClosed, GUILayout.Height (100f), GUILayout.Width (100f))) {
				CoinbaseWebChecker.messageLog += "lock is closed\n";	
			}
			
			if (content.transactionStateContent == TransactionState.None) {
				
				GUILayout.BeginVertical();
				if (GUILayout.Button ("purchase " + content.description + " for $1 USD", GUILayout.Height(50))) {
					string transactionRequestJson = CoinbaseWebChecker.dataMgr.CreateJsonTransactionRequest(
						emailStr, content.currencyTypeStr, content.currencyAmountStr, content.transactionNotes);
					
					CoinbaseWebChecker.messageLog += transactionRequestJson;
					CoinbaseWebChecker.messageLog += "\n";
					
					StartCoroutine(CoinbaseWebChecker.PostTransactionsRequestMoney(apiKey,transactionRequestJson));							
				}
				if (GUILayout.Button ("verify pending purchase", GUILayout.Height(50))) {
					messageLog += "verify pending purchase:\n";
					CoinbaseWebChecker.Callback myCallback = VerifyTransactionsCallback;
					this.StartCoroutine(CoinbaseWebChecker.GetTransactions_Coroutine(apiKey, myCallback));						
				}
				GUILayout.EndVertical();
				
			} else if (content.transactionStateContent == TransactionState.Pending) {
				
				GUILayout.BeginVertical();
				if (GUILayout.Button ("verify pending purchase", GUILayout.Height(50))) {
					messageLog += "verify pending purchase:\n";
					CoinbaseWebChecker.Callback myCallback = VerifyTransactionsCallback;
					this.StartCoroutine(CoinbaseWebChecker.GetTransactions_Coroutine(apiKey, myCallback));					
				}
				if (GUILayout.Button ("cancel pending purchase", GUILayout.Height(50))) {
					messageLog += "cancel pending purchase:\n";
					this.StartCoroutine (CoinbaseWebChecker.DeleteTransactionsCancelRequests(apiKey, emailStr, content.transactionNotes));
					content.transactionStateContent = TransactionState.None;
				}
				GUILayout.EndVertical();
				
			} else if (content.transactionStateContent == TransactionState.Complete) {
				
				if (GUILayout.Button ("Purchase complete", GUILayout.Height(100))) {
					// this shouldn't be visible, because we change the contentLock
				}
			}
			
			
		} else {
			if (GUILayout.Button(lockOpen, GUILayout.Height (100f), GUILayout.Width (100f))) {
				CoinbaseWebChecker.messageLog += "lock is open\n";	
			}
			
			if (GUILayout.Button ("purchase verified",GUILayout.Height(100))) {
				CoinbaseWebChecker.messageLog += "lock is open\n";					
			}
		}
		GUILayout.EndHorizontal();		
	}
	
}
