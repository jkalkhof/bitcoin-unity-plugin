using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockchainWebChecker : MonoBehaviour
{


    static int debugLevel = 1;
    public BlockchainDataManager dataMgr = null;

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

    #region Singleton
    private static BlockchainWebChecker _instance = null;

    public static BlockchainWebChecker instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BlockchainWebChecker>();
                if (_instance == null)
                {
                    Debug.LogError("<color=red>BlockchainWebChecker Not Found!</color>");
                }
            }
            return _instance;
        }
    }

    void OnApplicationQuit()
    {
        _instance = null;
        DestroyImmediate(gameObject);
    }
    #endregion

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {

        dataMgr = new BlockchainDataManager();

    }

    static public IEnumerator GetTransactionInfo_Coroutine(string txHash, Callback callback)
    {
        ResponseData response = new ResponseData();

        if (debugLevel > 0) Debug.Log("BlockchainWebChecker: GetTransactionInfo: start");
        bool foundURL;

        string checkThisURL = serverApiUrl + "rawtx/" + txHash;
        BitcoinWebAsync webAsync = new BitcoinWebAsync();

        if (debugLevel > 0) Debug.Log("BlockchainWebChecker: URL: " + checkThisURL);

        //yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
        yield return instance.StartCoroutine(webAsync.GetURL(checkThisURL));


        //Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
        if (webAsync.isResponseCompleted)
        {
            if (debugLevel > 0) Debug.Log("Coroutine : response: " + webAsync.resultStr);

            while (instance.dataMgr == null) yield return null;


            response.message = instance.dataMgr.parseGetTransactionInfoResponse(webAsync.resultStr);
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

    public void GetExchangeRates(Callback callback)
    {
        StartCoroutine(GetExchangeRates_Coroutine(callback));
    }



    static public IEnumerator GetExchangeRates_Coroutine(Callback callback)
    {
        ResponseData response = new ResponseData();

        if (debugLevel > 0) Debug.Log("BlockchainWebChecker: GetExchangeRates: start");

        string checkThisURL = serverApiUrl + "ticker";
        BitcoinWebAsync webAsync = new BitcoinWebAsync();

        if (debugLevel > 0) Debug.Log("BlockchainWebChecker: URL: " + checkThisURL);

        //yield return Instance.StartCoroutine( webAsync.CheckForMissingURL(checkThisURL) );
        yield return instance.StartCoroutine(webAsync.GetURL(checkThisURL));


        //Debug.Log("Does "+ checkThisURL  +" exist? "+ webAsync.isURLmissing);
        if (webAsync.isResponseCompleted)
        {
            if (debugLevel > 0) Debug.Log("Coroutine : response: " + webAsync.resultStr);

            while (instance.dataMgr == null) yield return null;


            //response.message = instance.dataMgr.parseGetTransactionInfoResponse(webAsync.resultStr);
            response.message = instance.dataMgr.parseGetExchangeRates(webAsync.resultStr);
            response.success = true;

            messageLog = ""; // reset the message log			
            messageLog += response.message;
            messageLog += "\n";

        }
        else
        {
            Debug.Log("Coroutine : response not completed: ");
        }

        if (debugLevel > 0) Debug.Log("Coroutine GetExchangeRates: finish");

        if (callback != null)
        {
            callback(response);
        }

    }

}
