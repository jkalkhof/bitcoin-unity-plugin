using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
//using System.Web.Script.Serialization.JavaScriptSerializer;


public class CoinbaseDataManager {
	
	private static int debugLevel = 0;
		
	public string accountBalanceAmount = null;
	public string accountBalanceCurrency = null;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	
	public string parseGetAccountBalanceResponse(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {
			string errorStr = "parseGetAccountBalanceResponse: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return errorStr;
		}			
		
		string messageLog = "";
		
		/*
		 {"amount":"0.00000000","currency":"BTC"}
		*/
		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());
		
		if (dict.ContainsKey("amount")) {
			this.accountBalanceAmount = (string) dict["amount"];
			if (debugLevel > 0) Debug.Log("dict['amount']: " + accountBalanceAmount);
			messageLog += "dict['amount']: " + accountBalanceAmount;
			messageLog += "\n";
		}		
		
		if (dict.ContainsKey("currency")) {
			this.accountBalanceCurrency = (string) dict["currency"];
			if (debugLevel > 0) Debug.Log("dict['currency']: " + accountBalanceCurrency);
			messageLog += "dict['currency']: " + accountBalanceCurrency;
		}			
		
		return messageLog;
	}
	
	public string parseGetCurrenciesResponse(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {
			string errorStr = "parseGetCurrenciesResponse: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return errorStr;
		}			
		
		string messageLog = "";		
		
		/*
		 [["Afghan Afghani (AFN)","AFN"],["Albanian Lek (ALL)","ALL"],["Algerian Dinar (DZD)","DZD"], ... ,["Zimbabwean Dollar (ZWL)","ZWL"]]
		 */
		//var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		var dict = Json.Deserialize(jsonString) as List<object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());		
		
		List<object> currenciesList = (List<object>) dict;
		for (int i = 0; i < currenciesList.Count; i++) {
			List<object> currencyObj = (List<object>) currenciesList[i];
			
			
			string currencyName = (string) currencyObj[0];
			string currencyCode = (string) currencyObj[0];
			
			messageLog += "currencyObj["+i+"]: ";
			messageLog += currencyName + " ";
			messageLog += currencyCode;
			messageLog += "\n";
		}		
		
		
		return messageLog;
	}
	
	public string parseGetCurrenciesExchangeRatesResponse(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {
			string errorStr = "parseGetCurrenciesExchangeRatesResponse: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return errorStr;
		}		
		
		string messageLog = "";			
		/*
		 {"btc_to_pgk":"28.152994","btc_to_gyd":"2743.906541","btc_to_mmk":"11611.550858", ... ,"brl_to_btc":"0.037652"}
		 */

		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());
		
		
		
		if (dict.ContainsKey("btc_to_usd")) {
			string exchangeRateBTCtoUSD = (string) dict["btc_to_usd"];
			if (debugLevel > 0) Debug.Log("dict['btc_to_usd']: " + exchangeRateBTCtoUSD);
			messageLog += "dict['btc_to_usd']: " + exchangeRateBTCtoUSD;
			messageLog += "\n";
		}		
		
		if (dict.ContainsKey("usd_to_btc")) {
			this.accountBalanceCurrency = (string) dict["usd_to_btc"];
			if (debugLevel > 0) Debug.Log("dict['usd_to_btc']: " + accountBalanceCurrency);
			messageLog += "dict['usd_to_btc']: " + accountBalanceCurrency;
		}			
		
		return messageLog;		
	}
	
	public string parseGetPricesSpotRateResponse(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {
			string errorStr = "parseGetPricesSpotRateResponse: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return errorStr;
		}			
		
		string messageLog = "";			
		/*
		  {"amount":"749.10","currency":"USD"}
		*/
		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());
		
		
		
		if (dict.ContainsKey("amount")) {
			string amount = (string) dict["amount"];
			if (debugLevel > 0) Debug.Log("dict['amount']: " + amount);
			messageLog += "dict['amount']: " + amount;
			messageLog += "\n";
		}		
		
		if (dict.ContainsKey("currency")) {
			string currency = (string) dict["currency"];
			if (debugLevel > 0) Debug.Log("dict['currency']: " + currency);
			messageLog += "dict['currency']: " + currency;
			messageLog += "\n";
		}			
		
		return messageLog;			
		
	}
	
	public class Person {
		public string id;
		public string name;
		public string email;
		
		public void parseFromDict(Dictionary<string,object> personDict) {
			
			this.id = (string) personDict["id"];
			this.name = (string) personDict["name"];
			this.email = (string) personDict["email"];
			
			if (debugLevel > 0) Debug.Log("\tPerson[]:" +
				" id: " + this.id +
				" name: " + this.name +
				" email: " + this.email 				
				);				
		}
		
	}
	
	public class Amount {
		public float amount = 0.0f;
		public string currency = "BTC";
		
		
		public void parseFromDict(Dictionary<string,object> amountDict) {

			this.amount = float.Parse((string) amountDict["amount"]);
			this.currency = (string) amountDict["currency"];
			
			if (debugLevel > 0) Debug.Log("\tAmount[]:" +
				" amount: " + this.amount +
				" currency: " + this.currency 				
				);				
			
		}
	}
	
	public class Transaction {
	    public string id;
	    public string created_at;
		public string notes;
		
		public Amount amount;
		
		public bool request;
		public string status;
		
		public Person sender = null;
		public Person recipient = null;
		public string recipient_addres;
		
		public void parseFromDict(Dictionary<string,object> transactionDict) {
			
			this.id = (string) transactionDict["id"];
			this.created_at = (string) transactionDict["created_at"];
			
			if (transactionDict.ContainsKey ("notes")) {
				this.notes = (string) transactionDict["notes"];
			}
			
			if (transactionDict.ContainsKey ("request")) {
				this.request = (bool) transactionDict["request"];
			}
			this.status = (string) transactionDict["status"];
			
			if (debugLevel > 0) Debug.Log("transaction[]:" +
				" id: " + this.id +
				" created_at: " + this.created_at +
				" request: " + this.request +
				" status: " + this.status
				);						
			
			Dictionary<string,object> amountDictObj = (Dictionary<string,object>) transactionDict["amount"];			
			this.amount = new CoinbaseDataManager.Amount();
			this.amount.parseFromDict(amountDictObj);
			
			if (transactionDict.ContainsKey ("recipient")) {
				Dictionary<string,object> personDictObj = (Dictionary<string,object>) transactionDict["sender"];			
				this.sender = new CoinbaseDataManager.Person();
				this.sender.parseFromDict(personDictObj);
			}
			
			if (transactionDict.ContainsKey ("recipient_address")) {
				this.recipient_addres = (string) transactionDict["recipient_address"];
			}
			
			if (transactionDict.ContainsKey ("recipient")) {
				Dictionary<string,object> personDictObj = (Dictionary<string,object>) transactionDict["recipient"];			
				this.recipient = new CoinbaseDataManager.Person();
				this.recipient.parseFromDict(personDictObj);				
					
			}
		}		
		
		public string printToString() {
			string tempLog = "";			
	
			tempLog += "\t";
			tempLog += " id: " + this.id;
			//tempLog += " created_at: " + this.created_at;
			tempLog += "\n";
			
			tempLog += "\t";
			tempLog += " notes: " + this.notes;
			tempLog += "\n";
			
			tempLog += "\t";
			tempLog += " request: " + this.request;
			tempLog += " status: " + this.status;
			tempLog += "\n";
			
			tempLog += "\t";
			if (this.sender != null) {
				tempLog += " sender: " + this.sender.name;
			}
			if (this.recipient != null) {
				tempLog += " recipient: " + this.recipient.name;
			}
			tempLog += "\n";
			
			return tempLog;
		}		
	}	
	
	//public List<Transaction> transactionList = new List<Transaction>();
	public Dictionary<string,Transaction> transactionStore = new Dictionary<string, Transaction>();	
	
	public List<Transaction> transactionRequestList = new List<Transaction>();
	
	public string parseGetTransactionsResponse(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {
			string errorStr = "parseGetTransactionsResponse: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return errorStr;
		}	
		
		/*
		{
		  "current_user": {
		    "id": "5011f33df8182b142400000e",
		    "email": "user2@example.com",
		    "name": "User Two"
		  },
		  "balance": {
		    "amount": "50.00000000",
		    "currency": "BTC"
		  },
		  "total_count": 2,
		  "num_pages": 1,
		  "current_page": 1,
		  "transactions": [
		    {
		      "transaction": {
		        "id": "5018f833f8182b129c00002f",
		        "created_at": "2012-08-01T02:34:43-07:00",
		        "amount": {
		          "amount": "-1.10000000",
		          "currency": "BTC"
		        },
		        "request": true,
		        "status": "pending",
		        "sender": {
		          "id": "5011f33df8182b142400000e",
		          "name": "User Two",
		          "email": "user2@example.com"
		        },
		        "recipient": {
		          "id": "5011f33df8182b142400000a",
		          "name": "User One",
		          "email": "user1@example.com"
		        }
		      }
		    },
		    {
		      "transaction": {
		        "id": "5018f833f8182b129c00002e",
		        "created_at": "2012-08-01T02:36:43-07:00",
		        "hsh": "9d6a7d1112c3db9de5315b421a5153d71413f5f752aff75bf504b77df4e646a3",
		        "amount": {
		          "amount": "-1.00000000",
		          "currency": "BTC"
		        },
		        "request": false,
		        "status": "complete",
		        "sender": {
		          "id": "5011f33df8182b142400000e",
		          "name": "User Two",
		          "email": "user2@example.com"
		        },
		        "recipient_address": "37muSN5ZrukVTvyVh3mT5Zc5ew9L9CBare"
		      }
		    }
		 ]
		}
  		 
		 */
		
		string messageLog = "";			
		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());
		
		if (dict.ContainsKey("transactions")) {
			
			messageLog += "transactions:\n";
			
			List<object> transactionsList = (List<object>) dict["transactions"];
			for (int i = 0; i < transactionsList.Count; i++) {
				messageLog += "transaction["+i+"]:\n";
				Dictionary<string,object> transactionDict = (Dictionary<string,object>) transactionsList[i];				
				
				Dictionary<string,object> transactionDictObj = (Dictionary<string,object>) transactionDict["transaction"];
				Transaction transaction = new Transaction();
				transaction.parseFromDict(transactionDictObj);
				
				
				
				// check to see if transaction id is in transactionlist?
				//transactionList.Add (transaction);
				if (transactionStore.ContainsKey (transaction.id)) {
					messageLog += "transaction exists in local storage: id: " + transaction.id + "\n";	
				} else {
					transactionStore.Add (transaction.id, transaction);
					
					messageLog += "new transaction \n";
					messageLog += transaction.printToString();	
				}
				
			}
		}
		
		return messageLog;
	}
		
	// analyze the store of transactions, and display the transactions which are requests
	//public string GetTransactionsRequests() {
	public ResponseData GetTransactionsRequests(string emailStr, string transactionNotes) {
		ResponseData response = new ResponseData();	
		string messageLog = "";
		
		transactionRequestList = new List<Transaction>();
		
		bool foundTransactionRequest = false;
		
		// iterate through transactionsStore dictionary
		foreach(KeyValuePair<string, Transaction> item in transactionStore)
		{

		  
			Transaction transaction = item.Value;
			
			if ((transaction.request == true) &&
				(transaction.sender != null) &&
				(transaction.sender.email == emailStr) &&
				(transaction.notes == transactionNotes)			
				)
			{
		  		messageLog += "transaction: id: " + item.Key;
				messageLog += "\n";
				
				messageLog += transaction.printToString();	
				messageLog += "\n";
				
				transactionRequestList.Add (transaction);
				
				foundTransactionRequest = true;
			}
		}		
		
		if (!foundTransactionRequest) {
			messageLog += "no transaction requests found";
			messageLog += "\n";
		}
		
		response.message = messageLog;
		response.success = foundTransactionRequest;
		
		return response;
	}
	
	public class ResponseData {
		public string message = "";
		public bool success = false;
		
	}
	
	
	public ResponseData VerifyTransactionsComplete(string emailStr, string transactionNotes) {
		ResponseData response = new ResponseData();			
			
		string messageLog = "";
		
		transactionRequestList = new List<Transaction>();
		
		bool foundTransaction = false;
		
		// iterate through transactionsStore dictionary
		foreach(KeyValuePair<string, Transaction> item in transactionStore)
		{

		  
			Transaction transaction = item.Value;
			
			if ((transaction.request == false) && 
				(transaction.status == "complete") &&
				(transaction.sender != null) &&
				(transaction.sender.email == emailStr) &&
				(transaction.notes != null) &&
				(transaction.notes == transactionNotes)
				) {
				
		  		messageLog += "transaction: id: " + item.Key;
				messageLog += "\n";
				
				messageLog += transaction.printToString();	
				messageLog += "\n";
				
				//transactionRequestList.Add (transaction);
				
				foundTransaction = true;
			}
		}		
		
		if (!foundTransaction) {
			messageLog += "couldn't verify transaction for email: " + emailStr;
			messageLog += "\n";
		}
		
		// save responses in data structure class
		response.message = messageLog;
		response.success = foundTransaction;
		
		return response;		
		
		
	}
	
	
	public string CreateJsonTransactionRequest(string emailStr, string currencyType, string amountStr, string transactionNotes) {		
		/*
		{
		  "transaction": {
			"from": "jerryartist@gmail.com",
			"amount_currency_iso": "USD",
			"amount_string": "1",
			"notes": "Sample transaction for you"
		  }
		}
	 	*/
		
		Dictionary<string,object> transactionObj = new Dictionary<string, object>();
		transactionObj.Add("from",emailStr);
		transactionObj.Add ("amount_currency_iso", currencyType);
		//transactionObj.Add ("amount_currency_iso","USD");
		transactionObj.Add ("amount_string",amountStr);
		//transactionObj.Add ("amount_string","1");
		transactionObj.Add ("notes",transactionNotes);
		//transactionObj.Add ("notes","Sample transaction from unity coinbase sdk");
		
		
		Dictionary<string,object> doc = new Dictionary<string, object>();
		doc.Add("transaction", transactionObj);
			
			
		
		string json = Json.Serialize (doc);
		
		if (debugLevel > 0) Debug.Log ("CoinbaseDataManager: CreateJsonTransactionRequest: " + json);
		return json;
	}
	
	
	public string CreateJsonPostUsers(string emailStr, string passwordStr) {
		/*
		{
		  "user": {
		    "email": "newuser@example.com",
		    "password": "test123!"
		  }
		}
		*/
		
		Dictionary<string,object> userObj = new Dictionary<string, object>();
		userObj.Add("email",emailStr);
		userObj.Add ("password",passwordStr);

		
		Dictionary<string,object> doc = new Dictionary<string, object>();
		doc.Add("user", userObj);		
		
		string json = Json.Serialize (doc);
		
		if (debugLevel > 0) Debug.Log ("CoinbaseDataManager: CreateJsonPostUsers: " + json);
		return json;		
		
	}
	
	
		
	public string parsePostTransactionsRequestMoneyResponse(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {	
			string errorStr = "parsePostTransactionsRequestMoneyResponse: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return errorStr;
		}		
		
		/*
		{
			"success": true, 
			"transaction": {
				"amount": {
					"amount": "0.00141300", 
					"currency": "BTC"
				}, 
				"created_at": "2013-12-26T00:42:40-08:00", 
				"hsh": null, 
				"id": "52bbec005bb6b8365e000009", 
				"notes": "Sample transaction for you", 
				"recipient": {
					"email": "lostfreeman@yahoo.com", 
					"id": "527e8c4405e43da63b0001ba", 
					"name": "Jerome Kalkhof"
				}, 
				"request": true, 
				"sender": {
					"email": "jerryartist@gmail.com", 
					"id": "52bbb6290403ccb089000039", 
					"name": "jerryartist@gmail.com"
				}, 
				"status": "pending"
			}
		}		
		*/
		
		string messageLog = "";			
		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());
		
		if (dict.ContainsKey("success")) {		
			bool success = (bool) dict["success"];
			
			if (success) {
				Dictionary<string,object> transactionDictObj = (Dictionary<string,object>) dict["transaction"];
				Transaction transaction = new Transaction();
				transaction.parseFromDict(transactionDictObj);
			
				// check to see if transaction id is in transactionlist?
				if (transactionStore.ContainsKey (transaction.id)) {
					messageLog += "transaction exists in local storage: id: " + transaction.id + "\n";	
				} else {
					transactionStore.Add (transaction.id, transaction);
					
					messageLog += "new transaction \n";
					messageLog += transaction.printToString();	
				}				
			} else {
				Debug.Log ("transaction request money failure!");	
			}
				
		}
		
		return messageLog;
	}
	
	public bool parseDeleteTransactionsCancelRequest(string jsonString) {
		if ((jsonString == null) || (jsonString.Length == 0)) {	
			string errorStr = "parseDeleteTransactionsCancelRequest: invalid jsonString!\n";
			Debug.Log (errorStr);	
			return false;
		}		
		
		/*
			{
			  "success": true,
			}
		 */
		var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
		if (debugLevel > 0) Debug.Log("deserialized: " + dict.GetType());
		
		if (dict.ContainsKey("success")) {		
			bool success = (bool) dict["success"];
			
			if (debugLevel > 0) Debug.Log("parseDeleteTransactionsCancelRequest: success: " + success);
			return success;
		}
		
		return false;
	}
}
