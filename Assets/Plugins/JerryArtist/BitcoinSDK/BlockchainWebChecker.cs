using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockchainWebChecker : MonoBehaviour {

    static public BlockchainWebChecker Instance;
    static int debugLevel = 1;
    static public BlockchainDataManager dataMgr = null;

    static public string messageLog = "";

    // TODO https version is failing in webrequest
    //static string serverApiUrl = "https://blockchain.info/";
    static string serverApiUrl = "http://blockchain.info/";

    public delegate void Callback(ResponseData response);

    public class ResponseData
    {
        public string message = "";
        public bool success = false;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {
        Instance = GetComponent<BlockchainWebChecker>();
        dataMgr = new BlockchainDataManager();

    }

    static public IEnumerator GetTransactionInfo_Coroutine(string txHash, Callback callback)
    {
        ResponseData response = new ResponseData();

        if (debugLevel > 0) Debug.Log("BlockchainWebChecker: GetTransactionInfo: start");
        bool foundURL;

        string checkThisURL = serverApiUrl + "rawtx/" + txHash;
        CoinbaseWebAsync webAsync = new CoinbaseWebAsync();

        if (debugLevel > 0) Debug.Log("BlockchainWebChecker: URL: " + checkThisURL);

        //yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
        yield return Instance.StartCoroutine(webAsync.GetURL(checkThisURL));


        //Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
        if (webAsync.isResponseCompleted)
        {
            if (debugLevel > 0) Debug.Log("Coroutine : response: " + webAsync.resultStr);

            while (dataMgr == null) yield return null;


            response.message = dataMgr.parseGetTransactionInfoResponse(webAsync.resultStr);
            response.success = true;

            messageLog = ""; // reset the message log			
            messageLog += response.message;
            messageLog += "\n";

        }
        else
        {
            Debug.Log("Coroutine : response not completed: ");
        }

        if (debugLevel > 0) Debug.Log("Coroutine GetTransactions: finish");

        if (callback != null)
        {
            callback(response);
        }
    }
}
