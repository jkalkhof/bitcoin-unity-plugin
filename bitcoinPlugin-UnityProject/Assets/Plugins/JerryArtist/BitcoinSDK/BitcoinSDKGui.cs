using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BitcoinSDKGui : MonoBehaviour {

    public GameObject Button_ExchRates;
    public GameObject InputField_SourceCurrency;
    public GameObject InputField_SourceAmount;
    public GameObject InputField_TargetCurrency;
    public GameObject InputField_TargetAmount;
    public GameObject InputField_TransactionNotes;
    public GameObject InputField_WalletAddress;
    public GameObject Button_RequestMoney;
    public GameObject InputField_TxHash;
    public GameObject Button_VerifyTransaction;
    public GameObject LoggingText;

    private string transactionNotes = "Sample transaction from bitcoin sdk";
    private string currencyTypeStr = "BTC - satoshis";
    //private string currencyAmountStr = "12903";
    private string currencyAmountStr = "300000";

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

    void Awake()
    {
        InputField_SourceCurrency.GetComponentInChildren<InputField>().text = sourceCurrencyTypeStr;
        //InputField_SourceAmount.GetComponentInChildren<Text>().text = "1";
        InputField_SourceAmount.GetComponentInChildren<InputField>().text = sourceCurrencyAmountStr;
        InputField_TargetCurrency.GetComponentInChildren<InputField>().text = currencyTypeStr;
        InputField_TargetAmount.GetComponentInChildren<InputField>().text = currencyAmountStr;
        InputField_TransactionNotes.GetComponentInChildren<InputField>().text = transactionNotes;
        InputField_WalletAddress.GetComponentInChildren<InputField>().text = sendToWalletAddress;
        InputField_TxHash.GetComponentInChildren<InputField>().text = transactionHashStr;

        Debug.Log("BitcoinSDKGui: Awake: sourceAmount: " + InputField_SourceAmount.GetComponentInChildren<InputField>().text);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void requestMoney()
    {

        currencyAmountStr = InputField_TargetAmount.GetComponentInChildren<UnityEngine.UI.InputField>().text;
        sendToWalletAddress = InputField_WalletAddress.GetComponentInChildren<InputField>().text;
        transactionNotes = InputField_TransactionNotes.GetComponentInChildren<InputField>().text;

        long amount = long.Parse(currencyAmountStr);
        //BitcoinIntegration.instance.StartRequest(sendToWalletAddress, amount, transactionNotes);
        BitcoinIntegration.instance.StartRequest(sendToWalletAddress, amount, transactionNotes, SendRequestCallback);

        //long amount = long.Parse(currencyAmountStr);
        //BitcoinIntegration.instance.StartRequest2(sendToWalletAddress, amount, transactionNotes);

    }

    // request btc transfer
    public void SendRequestCallback(BitcoinIntegration.ResponseData data)
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: SendRequestCallback");
        messageLog += "SendRequestCallback\n";
        messageLog += data.message;

        transactionHashStr = BitcoinIntegration.instance.getCurrentTxHash();
        InputField_TxHash.GetComponentInChildren<InputField>().text = transactionHashStr;

        LoggingText.GetComponent<Text>().text = messageLog;
    }

    public void getExchangeRates()
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: getExchangeRates");

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
        string sourceCurrencyAmountStr = InputField_SourceAmount.GetComponentInChildren<UnityEngine.UI.InputField>().text;

        float sourceCurrencyAmount = float.Parse(sourceCurrencyAmountStr);
        float targetBtc = BlockchainWebChecker.instance.dataMgr.convertCurrencyToBtc(sourceCurrencyAmount, "USD");

        float targetSatoshi = targetBtc * satoshiToBtc;
        int targetSatoshiInt = Mathf.RoundToInt(targetSatoshi);

        Debug.Log("BitcoinSDKGui: GetExchangeRatesCallback: " +
            sourceCurrencyAmount + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi");
        messageLog += sourceCurrencyAmount.ToString() + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi";


        string currencyAmountStr = targetSatoshiInt.ToString();
        InputField_TargetAmount.GetComponentInChildren<UnityEngine.UI.InputField>().text = currencyAmountStr;

        LoggingText.GetComponent<Text>().text = messageLog;
    }

    public void verifyTransactions()
    {
        transactionHashStr = InputField_TxHash.GetComponentInChildren<InputField>().text;
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

        currencyAmountStr = InputField_TargetAmount.GetComponentInChildren<InputField>().text;
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
