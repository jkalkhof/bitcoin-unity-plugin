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

    private int debugLevel = 1;

    // 100,000,000 satoshi = 1 btc
    private float satoshiToBtc = 100000000.0f;

    private string messageLog = "";

	// Use this for initialization
	void Start () {
        

    }

    void Awake()
    {
        //InputField_SourceAmount.GetComponentInChildren<Text>().text = "1";
        InputField_SourceAmount.GetComponentInChildren<InputField>().text = "1";

        Debug.Log("BitcoinSDKGui: Awake: sourceAmount: " + InputField_SourceAmount.GetComponentInChildren<InputField>().text);
    }

    // Update is called once per frame
    void Update () {
		
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

        Debug.Log("BitcoinSDKTest: GetExchangeRatesCallback: " +
            sourceCurrencyAmount + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi");
        messageLog += sourceCurrencyAmount.ToString() + " USD => " + targetBtc + " btc => (" + targetSatoshi + ") " + targetSatoshiInt + " satoshi";


        string currencyAmountStr = targetSatoshiInt.ToString();
        InputField_TargetAmount.GetComponentInChildren<UnityEngine.UI.InputField>().text = currencyAmountStr;

        LoggingText.GetComponent<Text>().text = messageLog;
    }


}
