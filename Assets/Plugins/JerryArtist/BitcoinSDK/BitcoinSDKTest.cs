using System.Collections;
using System.Collections.Generic;

using UnityEngine;
//using BlockchainWebChecker;

public class BitcoinSDKTest : MonoBehaviour
{

    private GUIStyle labelStyle = new GUIStyle();
    public GUISkin mySkin;

    private string transactionNotes = "Sample transaction from bitcoin sdk";
    private string currencyTypeStr = "BTC - satoshis";
    //private string currencyAmountStr = "12903";
    private string currencyAmountStr = "300000";

    private string sourceCurrencyTypeStr = "USD";
    private string sourceCurrencyAmountStr = "1";

    // today 1 btc = 7750 usd
    // 1/7550 = 0.00012903
    // 12903 * 0.00000001 btc/satoshi = 0.00012903
    // 0.00012903 * $7750/btc = $0.999 

    // 100,000,000 satoshi = 1 btc
    private float satoshiToBtc = 100000000.0f;

    //private string sendToWalletAddress = "1GsBUQCNLdphxhuX6aZ7QAJjpnMq8MF6p8"; // jerry's donation wallet address 4
    private string sendToWalletAddress = "1C53cU1oqmqwco38ZawdVQqVemaqa7aWQi"; // jerry's mobile wallet address 2

    private string transactionHashStr = "10990f1892354ac9f11d634332041f2616e31ceb87fbc9077ac615cdf22c0d51"; // test transaction
    //private string transactionHashStr = "";

    public string messageLog = "";
    private Vector2 scrollPosition;

    private int debugLevel = 1;

    // Use this for initialization
    void Start()
    {
        Debug.Log("BitcoinSDKTest: Start");

        if (mySkin != null)
        {
            labelStyle = mySkin.label;
        }
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.normal.textColor = Color.white;

    }

    void Awake()
    {
        Debug.Log("BitcoinSDKTest: Awake");
    }

    // Update is called once per frame
    void Update()
    {

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

        GUI.Label(new Rect(centerx - (buttonWidth / 2), buttonVerticalOffset, buttonWidth, buttonHeight),
            "Bitcoin SDK Game Client", labelStyle);



        //GUILayout.BeginArea(new Rect(
        //    (centerx * 0.5f) - (buttonWidth / 2),
        //    buttonVerticalOffset + (buttonSpacer * 1),
        //    buttonWidth, buttonHeight * 12));

        GUILayout.BeginArea(new Rect(
            Screen.width * ((1f - .7f) / 2f),
            buttonVerticalOffset + (buttonSpacer * 1),
            buttonWidth * 2, buttonHeight * 12));

        if (GUILayout.Button("Get Exchange Rates", GUILayout.Height(buttonHeight)))
        {
            BlockchainWebChecker.instance.GetExchangeRates(GetExchangeRatesCallback);
        }


        //GUILayout.EndArea();

        //GUILayout.BeginArea(new Rect(
        //    (Screen.width * 0.75f) - (buttonWidth / 2), 
        //    buttonVerticalOffset + (buttonSpacer * 1),
        //    buttonWidth, buttonHeight * 12));

        GUILayout.BeginHorizontal();
        GUILayout.Label("source currency:");
        sourceCurrencyTypeStr = GUILayout.TextField(sourceCurrencyTypeStr);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("source amount:");
        sourceCurrencyAmountStr = GUILayout.TextField(sourceCurrencyAmountStr);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("currency type:");
        currencyTypeStr = GUILayout.TextField(currencyTypeStr);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("currency amount:");
        currencyAmountStr = GUILayout.TextField(currencyAmountStr);
        GUILayout.EndHorizontal();



        GUILayout.Label("transaction notes:");
        transactionNotes = GUILayout.TextArea(transactionNotes, GUILayout.Height(buttonHeight));

        GUILayout.Label("send btc to wallet address:");
        sendToWalletAddress = GUILayout.TextArea(sendToWalletAddress, GUILayout.Height(buttonHeight));


        if (GUILayout.Button("Post Transactions Request Money", GUILayout.Height(buttonHeight)))
        {

            long amount = long.Parse(currencyAmountStr);
            //BitcoinIntegration.instance.StartRequest(sendToWalletAddress, amount, transactionNotes);
            BitcoinIntegration.instance.StartRequest(sendToWalletAddress, amount, transactionNotes, SendRequestCallback);

            //long amount = long.Parse(currencyAmountStr);
            //BitcoinIntegration.instance.StartRequest2(sendToWalletAddress, amount, transactionNotes);
        }


        GUILayout.Label("transaction hash (receipt):");
        transactionHashStr = GUILayout.TextArea(transactionHashStr, GUILayout.Height(buttonHeight));


        if (GUILayout.Button("Verify Transactions (complete)", GUILayout.Height(buttonHeight)))
        {
           
            //StartCoroutine(BlockchainWebChecker.GetTransactionInfo_Coroutine(BitcoinIntegration.instance.getCurrentTxHash(), GetTXHashCallback));
            StartCoroutine(BlockchainWebChecker.GetTransactionInfo_Coroutine(transactionHashStr, GetTXHashCallback));

        }

        GUILayout.EndArea();

        Rect scrollOuterRect = new Rect(Screen.width * ((1f - .7f) / 2f),
                            Screen.height * (1.0f - .3f),
                            Screen.width * .7f,
                            Screen.height * .3f);

        Rect scrollInnerRect = new Rect(0f, 0f,
                            Screen.width * .7f,
                            Screen.height);

        scrollPosition = GUI.BeginScrollView(
                            scrollOuterRect,
                            //Rect (10,300,100,100),
                            scrollPosition,
                            //Rect (0, 0, 220, 200)
                            scrollInnerRect
                            );

        // use a callback, rather than constantly appending from another class...
        //messageLog = BitcoinIntegration.instance.messageLog;
        messageLog = GUI.TextArea(new Rect(0f, 0f, Screen.width, Screen.height),
                            messageLog);

        GUI.EndScrollView();

    }

    // request btc transfer
    public void SendRequestCallback(BitcoinIntegration.ResponseData data)
    {
        Debug.Log("BitcoinSDKTest: SendRequestCallback");
        messageLog += "SendRequestCallback\n";
        messageLog += data.message;

        transactionHashStr = BitcoinIntegration.instance.getCurrentTxHash();
    }

    // verify
    public void GetTXHashCallback(BlockchainWebChecker.ResponseData data)
    {
        Debug.Log("BitcoinSDKTest: GetTXHashCallback");
        messageLog += "GetTXHashCallback\n";

        if (debugLevel > 1)
        {
            messageLog += data.message;
        }

        long amount = long.Parse(currencyAmountStr);

        bool verifyTest = BlockchainWebChecker.instance.dataMgr.verifyTransaction(sendToWalletAddress, amount);
        if (verifyTest)
        {
            messageLog += "verified " + amount + "\n";
        }
        else
        {
            messageLog += "verify failed! \n";
        }
    }

    public void GetExchangeRatesCallback(BlockchainWebChecker.ResponseData data)
    {
        Debug.Log("BitcoinSDKTest: GetExchangeRatesCallback");
        messageLog += "GetExchangeRatesCallback\n";

        if (debugLevel > 1)
        {
            messageLog += data.message;
        }

        // convert currency to BTC
        float sourceCurrencyAmount = float.Parse(sourceCurrencyAmountStr);
        float targetBtc = BlockchainWebChecker.instance.dataMgr.convertCurrencyToBtc(sourceCurrencyAmount, "USD");

        float targetSatoshi = targetBtc * satoshiToBtc;
        int targetSatoshiInt = Mathf.RoundToInt(targetSatoshi);

        Debug.Log("BitcoinSDKTest: GetExchangeRatesCallback: " +
            sourceCurrencyAmount + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi");
        messageLog += sourceCurrencyAmount.ToString() + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi";


        currencyAmountStr = targetSatoshiInt.ToString();
    }

}