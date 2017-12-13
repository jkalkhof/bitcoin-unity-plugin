using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitcoinSDKSimpleGui : MonoBehaviour {

    public GameObject Button_Purchase1;
    public GameObject Button_Verify1;
    public GameObject InputField_Transaction1;

    public GameObject LoggingText;

    private string transactionNotes = "Sample transaction from bitcoin sdk";
    private string currencyTypeStr = "BTC - satoshis";
    //private string currencyAmountStr = "12903";
    private string currencyAmountStr = "300000"; // in satoshis

    private string sourceCurrencyTypeStr = "USD";
    private string sourceCurrencyAmountStr = "1";

    //private string sendToWalletAddress = "1GsBUQCNLdphxhuX6aZ7QAJjpnMq8MF6p8"; // jerry's donation wallet address 4
    private string sendToWalletAddress = "1C53cU1oqmqwco38ZawdVQqVemaqa7aWQi"; // jerry's mobile wallet address 2

    private string transactionHashStr = "10990f1892354ac9f11d634332041f2616e31ceb87fbc9077ac615cdf22c0d51"; // test transaction
    //private string transactionHashStr = "";

    private int debugLevel = 1;

    // 100,000,000 satoshi = 1 btc
    private float satoshiToBtc = 100000000.0f;

    private string messageLog = "";

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // start purchase sequence
    public void purchase1()
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: purchase1");

        BlockchainWebChecker.instance.GetExchangeRates(GetExchangeRatesCallback);

    }

    public void GetExchangeRatesCallback(BlockchainWebChecker.ResponseData data)
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: GetExchangeRatesCallback");
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

        Debug.Log("BitcoinSDKGui: GetExchangeRatesCallback: " +
            sourceCurrencyAmount + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi");
        messageLog += sourceCurrencyAmount.ToString() + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi";


        currencyAmountStr = targetSatoshiInt.ToString();      

        LoggingText.GetComponent<Text>().text = messageLog;

        requestMoney();
    }

    // request btc transfer
    private void requestMoney()
    {
        long amount = long.Parse(currencyAmountStr);
        BitcoinIntegration.instance.StartRequest(sendToWalletAddress, amount, transactionNotes, SendRequestCallback);
    }

    // request btc transfer - callback
    public void SendRequestCallback(BitcoinIntegration.ResponseData data)
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: SendRequestCallback");
        messageLog += "SendRequestCallback\n";
        messageLog += data.message;

        transactionHashStr = BitcoinIntegration.instance.getCurrentTxHash();
        InputField_Transaction1.GetComponentInChildren<InputField>().text = transactionHashStr;

        LoggingText.GetComponent<Text>().text = messageLog;

        if ((transactionHashStr != null) && (transactionHashStr.Length > 0))
        {
            verifyTransactions();
        }
    }

    public void verifyTransactions()
    {
        
        StartCoroutine(BlockchainWebChecker.GetTransactionInfo_Coroutine(transactionHashStr, GetTXHashCallback));

    }

    // verify
    public void GetTXHashCallback(BlockchainWebChecker.ResponseData data)
    {
        Debug.Log("BitcoinSDKGui: GetTXHashCallback");
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

        LoggingText.GetComponent<Text>().text = messageLog;
    }
}
