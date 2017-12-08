using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class BitcoinIntegration  {
public class BitcoinIntegration : MonoBehaviour {

    #region Singleton
    private static BitcoinIntegration _instance = null;
    //private Delegate callLoginCallback;
	
    public static BitcoinIntegration instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BitcoinIntegration>();
                if (_instance == null)
                {
                    Debug.LogError("<color=red>BitcoinIntegration Not Found!</color>");
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

	private static int debugLevel = 1;

	// Use this for initialization
	void Start () {
		Debug.Log("BitcoinIntegration: Start");
	}
	
	void Awake () {
		Debug.Log("BitcoinIntegration: Awake");
	}

	// Update is called once per frame
	void Update () {
		
	}
    
	public void StartRequest(string sendToWalletAddress, long amount, string transactionNotes) {
		
		if (debugLevel > 0) Debug.Log ("BitcoinIntegration: StartRequest: address: " + sendToWalletAddress + " amount: " + amount + " notes: " + transactionNotes);

		this.StartCoroutine(BitcoinIntegration.instance.StartRequest_Coroutine(sendToWalletAddress, amount, transactionNotes));

	}

	public void StartRequest2(string sendToWalletAddress, long amount, string transactionNotes) {
		
		if (debugLevel > 0) Debug.Log ("BitcoinIntegration: StartRequestStatic: address: " + sendToWalletAddress + " amount: " + amount + " notes: " + transactionNotes);

		this.StartCoroutine(BitcoinIntegration.instance.StartRequest_Coroutine_Static(sendToWalletAddress, amount, transactionNotes));

	}


	// https://medium.com/@tarasleskiv/type-casting-androidjavaobject-in-unity-from-c-54a4bda3607e
//	public static AndroidJavaObject ClassForName(string className)
//	{
//		Debug.Log("BitcoinIntegration: ClassForName: " + className);
//
//	    using (var clazz = new AndroidJavaClass("java.lang.Class"))
//	    {
//	        return clazz.CallStatic("forName", className);
//	    }
//	}

	// Cast extension method
//	public static AndroidJavaObject Cast(this AndroidJavaObject source, string destClass)
//	{
//		Debug.Log("BitcoinIntegration: Cast to: " + destClass);
//
//	    using (var destClassAJC = ClassForName(destClass))
//	    {
//	        return destClassAJC.Call<AndroidJavaObject>("cast", source);
//	    }
//	}

    //public void StartRequest()
	// amount is integer-number-of-satoshis
	// 1 Satoshi	= 0.00000001 ฿
	// today $1 USD = 0.000129032258065 btc
	// 7750 USD / 1 btc = 1/ ? btc
	// 1/7750 = 0.00012903
	public IEnumerator StartRequest_Coroutine(string sendToWalletAddress, long amount, string transactionNotes) 
    {
		if (debugLevel > 0) Debug.Log ("BitcoinIntegration: StartRequest_Coroutine: " + Application.platform.ToString());

       	if (Application.platform == RuntimePlatform.Android)
        {
            using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
					using (var androidPlugin = new AndroidJavaObject("de.schildbach.wallet.integration.android.BitcoinIntegration", currentActivity))                          
                    {
						AndroidJavaObject address0 = new AndroidJavaObject("java.lang.String", sendToWalletAddress);
                        AndroidJavaObject address1 = new AndroidJavaObject("java.lang.String", sendToWalletAddress);

                        // org.bitcoinj.core.AddressFormatException: Input too short
                        //AndroidJavaObject address1 = new AndroidJavaObject("java.lang.String", "");

                        // this is value in satoshis
                        long tempLong = 0;
                        AndroidJavaObject amount0 = new AndroidJavaObject("java.lang.Long", amount);
                        AndroidJavaObject amount1 = new AndroidJavaObject("java.lang.Long", tempLong);

                        // AndroidJavaException: java.lang.ClassNotFoundException: long
                        //AndroidJavaObject amount0 = new AndroidJavaObject("long", amount);
                        //AndroidJavaObject amount1 = new AndroidJavaObject("long", tempLong);

                        AndroidJavaObject memoStr = new AndroidJavaObject("java.lang.String", transactionNotes);

                        // generic call
                        object[] args = new object[5];
                        args[0] = address0;
                        args[1] = address1;
                        args[2] = amount0;
                        args[3] = amount1;
                        args[4] = memoStr;

                        androidPlugin.Call("handleRequestUnity", args);


                    }
                }
            }
        }
		
		yield break;
    }

	public IEnumerator StartRequest_Coroutine_Static(string sendToWalletAddress, long amount, string transactionNotes) 
    {
		if (debugLevel > 0) Debug.Log ("BitcoinIntegration: StartRequest_Coroutine: " + Application.platform.ToString());

       	if (Application.platform == RuntimePlatform.Android)
        {
            using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
					// v2 - use class, then call?
					var androidPluginClass = new AndroidJavaClass("de.schildbach.wallet.integration.android.BitcoinIntegration");
					// using (var androidPlugin = androidPluginClass.Call<AndroidJavaObject>("de.schildbach.wallet.integration.android.BitcoinIntegration", currentActivity))


					androidPluginClass.Call("testMethod", new object[] { "testString" } );

					AndroidJavaObject address0 = new AndroidJavaObject("java.lang.String", sendToWalletAddress); 
					AndroidJavaObject address1 = new AndroidJavaObject("java.lang.String", "");

					// this is value in satoshis
					long tempLong = 0;
					AndroidJavaObject amount0 = new AndroidJavaObject("java.lang.Long", amount);
					AndroidJavaObject amount1 = new AndroidJavaObject("java.lang.Long", tempLong);

					AndroidJavaObject memoStr = new AndroidJavaObject("java.lang.String", transactionNotes);

                    // no non-static with signature!
                    // signature='(Lcom.unity3d.player.UnityPlayerActivity;Ljava.lang.String;Ljava.lang.String;Ljava.lang.Long;Ljava.lang.Long;Ljava.lang.String;)V'
                    // public static void handleRequestUnityStatic(com.unity3d.player.UnityPlayerActivity parentActivity, String address0, String address1, long amount0, long amount1, String memoStr) {
                    //object[] args = new object[6];
                    //args[0] = currentActivity;
                    //args[1] = address0;
                    //args[2] = address1;
                    //args[3] = amount0;
                    //args[4] = amount1;
                    //args[5] = memoStr;

                    //androidPluginClass.Call("handleRequestUnityStatic", args);


                    // attempt 2
                    object[] args = new object[6];
                    args[0] = currentActivity;
                    args[1] = address0;
                    args[2] = address1;
                    args[3] = amount0;
                    args[4] = amount1;
                    args[5] = memoStr;

                    androidPluginClass.CallStatic("handleRequestUnityStatic", args);

                }
			}
		}

		yield break;
	}
}
