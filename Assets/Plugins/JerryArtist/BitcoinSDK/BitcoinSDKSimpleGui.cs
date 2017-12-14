using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BitcoinSDKSimpleGui : MonoBehaviour {

    //public GameObject Button_Purchase1;
    //public GameObject Button_Verify1;
    //public GameObject InputField_Transaction1;
    //public GameObject RawImage_Lock1;

    public GameObject LoggingText;

    public Texture lockTexture;
    public Texture unlockTexture;
    

    [System.Serializable]
    public class BitcoinPurchaseObjectGroup
    {
        public GameObject Button_Purchase1;
        public GameObject Button_Verify1;
        public GameObject InputField_Transaction1;
        public GameObject RawImage_Lock1;
    }


    public BitcoinPurchaseObjectGroup[] objectGroups;


    public class BitcoinAsset
    {
        public string transactionNotes = "Sample transaction from bitcoin sdk";
        public string currencyTypeStr = "satoshis";
        public string currencyAmountStr = "0"; // in satoshis
        public string sourceCurrencyTypeStr = "USD";
        public string sourceCurrencyAmountStr = "1";
        public string sendToWalletAddress = "";
        public string transactionHashStr = ""; // test transaction                                                                                                                
    }

    //Array<BitcoinAsset> availableAssets = new Array<BitcoinAsset>()
    BitcoinAsset[] availableAssets =
    {
        new BitcoinAsset{
            transactionNotes = "Asset 1 purchase",
            currencyTypeStr = "satoshis",
            currencyAmountStr = "300000", // in satoshis - overridden by sourceCurrencyAmount when tryping to purchase
            sourceCurrencyTypeStr = "USD",
            sourceCurrencyAmountStr = "1",
            sendToWalletAddress = "1C53cU1oqmqwco38ZawdVQqVemaqa7aWQi", // jerry's mobile wallet address 2
            transactionHashStr = "10990f1892354ac9f11d634332041f2616e31ceb87fbc9077ac615cdf22c0d51" // test transaction validation
        },
        new BitcoinAsset{
            transactionNotes = "Asset 2 purchase",
            currencyTypeStr = "satoshis",
            currencyAmountStr = "0", // in satoshis - overridden by sourceCurrencyAmount when tryping to purchase
            sourceCurrencyTypeStr = "USD",
            sourceCurrencyAmountStr = "1",
            sendToWalletAddress = "1GsBUQCNLdphxhuX6aZ7QAJjpnMq8MF6p8", // jerry's donation wallet address 4
            transactionHashStr = "" // test transaction validation
        },
    };

    int currentBtcAsset = 0;


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
    public void purchase(int index)
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: purchase " + index);

        currentBtcAsset = index;
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
        float sourceCurrencyAmount = float.Parse(availableAssets[currentBtcAsset].sourceCurrencyAmountStr);
        float targetBtc = BlockchainWebChecker.instance.dataMgr.convertCurrencyToBtc(sourceCurrencyAmount, availableAssets[currentBtcAsset].sourceCurrencyTypeStr);

        float targetSatoshi = targetBtc * satoshiToBtc;
        int targetSatoshiInt = Mathf.RoundToInt(targetSatoshi);

        Debug.Log("BitcoinSDKGui: GetExchangeRatesCallback: " +
            sourceCurrencyAmount + " " + availableAssets[currentBtcAsset].sourceCurrencyTypeStr + " => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi");
        messageLog += sourceCurrencyAmount.ToString() + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi";


        availableAssets[currentBtcAsset].currencyAmountStr = targetSatoshiInt.ToString();      

        LoggingText.GetComponent<Text>().text = messageLog;

        requestMoney();
    }

    // request btc transfer
    private void requestMoney()
    {
        long amount = long.Parse(availableAssets[currentBtcAsset].currencyAmountStr);
        BitcoinIntegration.instance.StartRequest(availableAssets[currentBtcAsset].sendToWalletAddress, amount, availableAssets[currentBtcAsset].transactionNotes, SendRequestCallback);
    }

    // request btc transfer - callback
    public void SendRequestCallback(BitcoinIntegration.ResponseData data)
    {
        if (debugLevel > 0) Debug.Log("BitcoinSDKGui: SendRequestCallback");
        messageLog += "SendRequestCallback\n";
        messageLog += data.message;

        availableAssets[currentBtcAsset].transactionHashStr = BitcoinIntegration.instance.getCurrentTxHash();
        objectGroups[currentBtcAsset].InputField_Transaction1.GetComponentInChildren<InputField>().text = availableAssets[currentBtcAsset].transactionHashStr;

        LoggingText.GetComponent<Text>().text = messageLog;

        if ((availableAssets[currentBtcAsset].transactionHashStr != null) && (availableAssets[currentBtcAsset].transactionHashStr.Length > 0))
        {
            verifyTransactions(currentBtcAsset);
        }
    }

    public void verifyTransactions(int index)
    {
        currentBtcAsset = index;
        StartCoroutine(BlockchainWebChecker.GetTransactionInfo_Coroutine(availableAssets[currentBtcAsset].transactionHashStr, GetTXHashCallback));

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

        long amount = long.Parse(availableAssets[currentBtcAsset].currencyAmountStr);

        bool verifyTest = BlockchainWebChecker.instance.dataMgr.verifyTransaction(availableAssets[currentBtcAsset].sendToWalletAddress, amount);
        if (verifyTest)
        {
            messageLog += "verified " + amount + "\n";
            objectGroups[currentBtcAsset].RawImage_Lock1.GetComponentInChildren<RawImage>().texture = unlockTexture;
        }
        else
        {
            messageLog += "verify failed! \n";
            objectGroups[currentBtcAsset].RawImage_Lock1.GetComponentInChildren<RawImage>().texture = lockTexture;
        }

        LoggingText.GetComponent<Text>().text = messageLog;
    }
}
