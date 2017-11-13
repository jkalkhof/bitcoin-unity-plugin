using UnityEngine;
using System.Collections;

public class CoinbaseWebChecker : MonoBehaviour {

	static public CoinbaseWebChecker Instance;
	static int debugLevel = 0;
	static public CoinbaseDataManager dataMgr = null;
	
	static public string messageLog = "";
	
	static string serverApiUrl = "https://coinbase.com/api/v1/";
	
	public class ResponseData {
		public string message = "";
		public bool success = false;
		
	}	
	
	void Awake () {
		Instance = GetComponent<CoinbaseWebChecker>();
		dataMgr = new CoinbaseDataManager();	
		
	}
	
	public delegate void Callback(ResponseData response);
	
	//CoinbaseWebChecker.Callback myCallback = DidAThing;
	
//	static private IEnumerator CheckURL () {
//		Debug.Log ("Coroutine CheckURL: start");
//		bool foundURL;
//		string checkThisURL = "http://stuffv1/app";
//		WebAsync webAsync = new WebAsync();
// 
//		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
//		yield return Instance.StartCoroutine( webAsync.GetURL(checkThisURL) );
//		
//		
//		//Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
//		if (webAsync.isResponseCompleted) {
//			Debug.Log ("Coroutine CheckURL: response: " + webAsync.resultStr);
//		} else {
//			Debug.Log ("Coroutine CheckURL: response not completed: ");
//		}
//		
//		Debug.Log ("Coroutine CheckURL: finish");
//	}
	
	static public IEnumerator GetAccountBalance (string apiKey) {
		if (debugLevel > 0) Debug.Log ("Coroutine GetAccountBalance: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "account/balance?api_key=" + apiKey;
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();
 
		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.GetURL(checkThisURL) );
		
		
		//Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine : response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			messageLog = ""; // reset the message log
			messageLog += dataMgr.parseGetAccountBalanceResponse(webAsync.resultStr);
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine : response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetAccountBalance: finish");
	}	
		
	static public IEnumerator GetCurrencies() {
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetCurrencies: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "currencies";
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();		

		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.GetURL(checkThisURL) );
		
		
		//Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine : response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			messageLog = ""; // reset the message log
			messageLog += dataMgr.parseGetCurrenciesResponse(webAsync.resultStr);
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine : response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetCurrencies: finish");		
		
	}
		
	static public IEnumerator GetCurrenciesExchangeRates() {
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetCurrenciesExchangeRates: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "currencies/exchange_rates";
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();		

		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.GetURL(checkThisURL) );
		
		
		//Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine : response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			messageLog = ""; // reset the message log
			messageLog += dataMgr.parseGetCurrenciesExchangeRatesResponse(webAsync.resultStr);
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine : response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetCurrenciesExchangeRates: finish");			
	}
	
	static public IEnumerator GetPricesSpotRate() {
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetPricesSpotRate: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "prices/spot_rate?currency=USD";
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();		

		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.GetURL(checkThisURL) );
		
		
		//Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine : response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			messageLog = ""; // reset the message log
			messageLog += dataMgr.parseGetPricesSpotRateResponse(webAsync.resultStr);
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine : response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetPricesSpotRate: finish");			
	}
	
	
	// Assets/Plugins/JerryArtist/CoinbaseSDK/CoinbaseSimpleSDKTest.cs(70,50): error CS0236: A field initializer cannot reference the nonstatic field, method, or property 
	//`CoinbaseSimpleSDKTest.DidAThing(CoinbaseDataManager.ResponseData)'
	// DoAsynchronousStuff.Callback myCallback = DidAThing;
 	//CoinbaseWebChecker.Callback myCallback = DidAThing;
 
//	public void DidAThing(CoinbaseDataManager.ResponseData data)
//	{
//		CoinbaseWebChecker.messageLog += "finished callback\n";
//	}		
	

	
	// static public IEnumerator GetTransactions(string apiKey) {
	//IEnumerator GetTransactions_Coroutine(string apiKey, Callback callback) {
	static public IEnumerator GetTransactions_Coroutine(string apiKey, Callback callback) {
		ResponseData response = new ResponseData();
			
		if (debugLevel > 0) Debug.Log ("Coroutine GetTransactions: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "transactions?api_key=" + apiKey;
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();		

		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.GetURL(checkThisURL) );
		
		
		//Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine : response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			
			
			response.message = dataMgr.parseGetTransactionsResponse(webAsync.resultStr);
			response.success = true;
			
			messageLog = ""; // reset the message log			
			messageLog += response.message;
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine : response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine GetTransactions: finish");	
		
		if (callback != null) {
			callback(response);	
		}
	}
		
	//  An object reference is required to access non-static member 
	// `UnityEngine.MonoBehaviour.StartCoroutine(System.Collections.IEnumerator)'

//	public static void GetTransactions(string apiKey, Callback callback) {
//		StartCoroutine(GetTransactions_Coroutine(apiKey, callback));
//	}		
	
	static public IEnumerator PostTransactionsRequestMoney(string apiKey, string jsonStr) {
		
		if (debugLevel > 0) Debug.Log ("Coroutine PostTransactionsRequestMoney: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "transactions/request_money?api_key="+apiKey;
		
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();
 
		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.PostURL(checkThisURL,jsonStr));
		
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine Post: response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			messageLog = ""; // reset the message log
			messageLog += dataMgr.parsePostTransactionsRequestMoneyResponse(webAsync.resultStr);			
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine Post: response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine PostTransactionsRequestMoney: finish");		
		
		
	}
	
	static public IEnumerator PostUsers(string jsonStr) {
		if (debugLevel > 0) Debug.Log ("Coroutine PostUsers: start");
		bool foundURL;
		
		string checkThisURL = serverApiUrl + "users";
		
		CoinbaseWebAsync webAsync = new CoinbaseWebAsync();
 
		//yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
		yield return Instance.StartCoroutine( webAsync.PostURL(checkThisURL,jsonStr));
		
		if (webAsync.isResponseCompleted) {
			if (debugLevel > 0) Debug.Log ("Coroutine Post: response: " + webAsync.resultStr);
			
			while (dataMgr == null) yield return null;
			//messageLog = ""; // reset the message log
			//messageLog += dataMgr.parsePostTransactionsRequestMoneyResponse(webAsync.resultStr);			
			messageLog += webAsync.resultStr;			
			messageLog += "\n";
			
		} else {
			Debug.Log ("Coroutine Post: response not completed: ");
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine PostUsers: finish");		
				
		
		
	}
	
	
	// from sdk: Authenticated resource which lets a user cancel a money request. Money requests can be canceled by the sender or the recipient
	static public IEnumerator DeleteTransactionsCancelRequests(string apiKey, string emailStr, string transactionNotes) {		
		if (debugLevel > 0) Debug.Log ("Coroutine DeleteTransactionsCancelRequests: start");
		bool foundURL;
		
		dataMgr.GetTransactionsRequests(emailStr, transactionNotes);
		if (dataMgr.transactionRequestList.Count == 0) {
			string errorMessage = "Coroutine DeleteTransactionsCancelRequests: no transaction requests";
			Debug.Log (errorMessage);
			messageLog += errorMessage;
			yield return null;
		}
		
		//for (int transactionIndex = 0; transactionIndex < dataMgr.transactionRequestList.Count; transactionIndex++) {
		int transactionIndex = 0;		
		while (transactionIndex < dataMgr.transactionRequestList.Count) {
			CoinbaseDataManager.Transaction transaction = dataMgr.transactionRequestList[transactionIndex];
			messageLog += "delete transaction["+transactionIndex+"] id: " + transaction.id;
			messageLog += "\n";
			
			
			string checkThisURL = serverApiUrl + "transactions/"+transaction.id+"/cancel_request?api_key=" + apiKey;
			CoinbaseWebAsync webAsync = new CoinbaseWebAsync();		
	
			
			yield return Instance.StartCoroutine( webAsync.DeleteURL(checkThisURL) );
			
			if (webAsync.isResponseCompleted) {
				if (debugLevel > 0) Debug.Log ("Coroutine : response: " + webAsync.resultStr);
				
				while (dataMgr == null) yield return null;
				//messageLog += dataMgr.parseGetTransactionsResponse(webAsync.resultStr);
				bool success = dataMgr.parseDeleteTransactionsCancelRequest(webAsync.resultStr);
				if (success) {
					// remove from local storage
					dataMgr.transactionRequestList.RemoveAt(transactionIndex);
					
					if (dataMgr.transactionStore.ContainsKey(transaction.id)) {
						dataMgr.transactionStore.Remove (transaction.id);	
					}
					
				} else {
					transactionIndex++;	
				}
			} else {
				Debug.Log ("Coroutine : response not completed: ");
				transactionIndex++;
			}
		}
		
		if (debugLevel > 0) Debug.Log ("Coroutine DeleteTransactionsCancelRequests: finish");			
		
	}
	
	// Use this for initialization
	void Start () {
		
		if (debugLevel > 0) Debug.Log ("WebChecker: doing other stuff....");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
