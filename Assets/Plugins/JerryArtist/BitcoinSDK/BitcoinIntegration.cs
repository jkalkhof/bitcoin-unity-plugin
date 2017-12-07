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
                    //using (var androidPlugin = new AndroidJavaObject("de.schildbach.wallet.integration.android.BitcoinIntegration", currentActivity))
					//using (var androidPlugin = new AndroidJavaObject("com.jerryartist.unity_android_btc.BitcoinIntegration", currentActivity))
					
					
					// v2 - use class, then call?
					// var androidPluginClass = new AndroidJavaClass("de.schildbach.wallet.integration.android.BitcoinIntegration");
					// using (var androidPlugin = androidPluginClass.Call<AndroidJavaObject>("de.schildbach.wallet.integration.android.BitcoinIntegration", currentActivity))
					// error: java.lang.NoSuchMethodError: no non-static method with name='de.schildbach.wallet.integration.android.BitcoinIntegration' signature='(Lcom.unity3d.player.UnityPlayerActivity;)Ljava/lang/Object;' in class Ljava.lang.Object;
                    
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


                        //						jobjectArray arr = env->NewObjectArray( 5, env->FindClass( "java/lang/String" ), env->NewStringUTF( "" ) );
                        //    					const char *message[5]= { "first", "second", "third", "fourth", "fifth" }

                        //androidPlugin.Call("handleRequestUnity", currentActivity, address0, address1, memoStr);

                        //androidPlugin.Call("handleRequestUnity", currentActivity, address0, address1, amount0, amount1, memoStr);

                        // name='handleRequestUnity'
                        // signature='(
                        //	Lcom.unity3d.player.UnityPlayerActivity;
                        //	Ljava.lang.String;
                        //	Ljava.lang.String;
                        //	Ljava.lang.Long;
                        //	Ljava.lang.Long;
                        //	Ljava.lang.String;)V'

                        /*
				         // to get the activity
				         //var androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
						var androidJC = new AndroidJavaClass("android.app.Activity");
				        var jo = androidJC.GetStatic<AndroidJavaObject>("currentActivity");

						// E/Unity: AndroidJavaException: java.lang.NoSuchFieldError: no "Ljava/lang/Object;" field "currentActivity" in class "Landroid/app/Activity;" or its superclasses
						//	java.lang.NoSuchFieldError: no "Ljava/lang/Object;" field "currentActivity" in class "Landroid/app/Activity;" or its superclasses
						*/

                        /*
						// try to cast to android.app.Activity
						//var serviceObj = new AndroidJavaObject("android.app.Activity", currentActivity);
						//var jo = Cast(currentActivity, "android.App.Activity");
						string destClass = "android.app.Activity";

						Debug.Log("BitcoinIntegration: ClassForName: " + destClass);

	    				using (var clazz = new AndroidJavaClass("java.lang.Class"))
	    				{
							var destClassAJC = clazz.CallStatic<AndroidJavaObject>("forName", destClass);

							var jo = destClassAJC.Call<AndroidJavaObject>("cast", currentActivity);
	
							object[] args  = new object[6];
							args[0] = jo;
							args[1] = address0;
							args[2] = address1;
							args[3] = amount0;
							args[4] = amount1;
							args[5] = memoStr;
	
							androidPlugin.Call("handleRequestUnity", args);
					    }
						*/

                        /*
						// try to cast to "de.schildbach.wallet.integration.android.BitcoinIntegration"
						string destClass = "de.schildbach.wallet.integration.android.BitcoinIntegration";
						Debug.Log("BitcoinIntegration: ClassForName: " + destClass);
	    				using (var clazz = new AndroidJavaClass("java.lang.Class"))
	    				{
							var destClassAJC = clazz.CallStatic<AndroidJavaObject>("forName", destClass);

							//var jo = destClassAJC.Call<AndroidJavaObject>("cast", androidPlugin);
							var jo = destClassAJC.Call<AndroidJavaObject>("cast", androidPlugin);
	
							object[] args  = new object[6];
							args[0] = address0;
							args[1] = address1;
							args[2] = amount0;
							args[3] = amount1;
							args[4] = memoStr;
	
							jo.Call("handleRequestUnity", args);
					    }
						*/

                        // run a non-static method
                        //currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>                                                                   {
                        //              		//currentActivity.Call("testMethod", "testString");
                        //	object[] args  = new object[5];						
                        //	args[0] = address0;
                        //	args[1] = address1;
                        //	args[2] = amount0;
                        //	args[3] = amount1;
                        //	args[4] = memoStr;

                        //	androidPlugin.Call("handleRequestUnity", args);
                        //     				 }));
                        // error:
                        // JNI ERROR (app bug): accessed deleted global reference 0x1006a2


                        // generic call
                        object[] args = new object[5];
                        args[0] = address0;
                        args[1] = address1;
                        args[2] = amount0;
                        args[3] = amount1;
                        args[4] = memoStr;

                        androidPlugin.Call("handleRequestUnity", args);
                        // v1 error: 
                        // E/Unity: AndroidJavaException: java.lang.NoSuchMethodError: no non-static method with name='handleRequestUnity' signature='(Ljava.lang.String;Ljava.lang.String;Ljava.lang.Long;Ljava.lang.Long;Ljava.lang.String;)V' in class Ljava.lang.Object;
                        // no non-static method with name='handleRequestUnity' signature='(Ljava.lang.String;Ljava.lang.String;Ljava.lang.Long;Ljava.lang.Long;Ljava.lang.String;)V'
                        //androidPlugin.Call("handleRequest");

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
