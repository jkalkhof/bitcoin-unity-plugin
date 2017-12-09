using System;
using System.Net;
using System.Threading;
using System.Collections;
using UnityEngine;

using UnityEngine.Networking;

/// <summary>
///  The RequestState class passes data across async calls.
/// </summary>
public class RequestState
{
	public WebRequest webRequest;
	public WebResponse webResponse;
	public string errorMessage;
 
	public RequestState ()
	{
		webRequest = null;
		webResponse = null;
		errorMessage = null;
	}
}
 
/// <summary>
/// Simplify getting web requests asynchronously
/// </summary>
public class CoinbaseWebAsync {
	private int debugLevel = 0;
	const int TIMEOUT = 10; // seconds
 
	public bool isResponseCompleted = false;
	public RequestState requestState;
 
	public bool isURLcheckingCompleted = false;
	public bool isURLmissing = false;
 
	public string resultStr = "";
	public string userSessionToken = null;
	

	
	/// <summary>
	/// Updates the isURLmissing parameter.
	/// If the URL returns 404 or the connection is broken, it's missing. Else, we suppose it's fine.
	/// This should or can be used along with web async instance's isURLcheckingCompleted parameter
	/// inside a IEnumerator method capable of yield return for it, although it's mostly for clarity.
	/// Here's an usage example:
	/// 
	/// WebAsync webAsync = new WebAsync(); StartCoroutine( webAsync.CheckForMissingURL(url) );
	/// while (! webAsync.isURLcheckingCompleted) yield return null;
	/// bool result = webAsync.isURLmissing;
	/// 
	/// </summary>
	/// <param name='url'>
	/// A fully formated URL.
	/// </param>
	public IEnumerator CheckForMissingURL (string url) {
		isURLcheckingCompleted = false;
		isURLmissing = false;
 
		if (debugLevel > 0) Debug.Log ("WebAsync: CheckForMissingURL: " + url);
		
		Uri httpSite = new Uri(url);
		WebRequest webRequest = WebRequest.Create(httpSite);
 
		// We need no more than HTTP's head
		//webRequest.Method = "HEAD";
		//webRequest.Method = "GET";
 
		// Get the request's reponse
		requestState = null;
 
		// Manually iterate IEnumerator, because Unity can't do it (and this does not inherit StartCoroutine from MonoBehaviour)
		IEnumerator e = GetResponse(webRequest);
		while (e.MoveNext()) yield return e.Current;
		while (! isResponseCompleted) yield return null; // this while is just to be sure and clear
 
		// Deal up with the results
		if (requestState.errorMessage != null) {
			if ( requestState.errorMessage.Contains("404") || requestState.errorMessage.Contains("NameResolutionFailure") ) {
				isURLmissing = true;
			} else {
				Debug.LogError("[WebAsync] Error trying to verify if URL '"+ url +"' exists: "+ requestState.errorMessage);
			}
		}
 
		isURLcheckingCompleted = true;
	}
	
	public IEnumerator GetURL (string url) {
		isURLcheckingCompleted = false;
		isURLmissing = false;
 
		if (debugLevel > 0) Debug.Log ("WebAsync: GetURL: " + url);
		
		Uri httpSite = new Uri(url);
		WebRequest webRequest = WebRequest.Create(httpSite);
 
		// We need no more than HTTP's head
		//webRequest.Method = "HEAD";
		//webRequest.Method = "GET";
 
		if (debugLevel > 0) Debug.Log("WebAsync: GetURL:  HttpHeaders are " + webRequest.Headers);			
		
		
		// Get the request's reponse
		requestState = null;
 
		// Manually iterate IEnumerator, because Unity can't do it (and this does not inherit StartCoroutine from MonoBehaviour)
		IEnumerator e = GetResponse(webRequest);
		while (e.MoveNext()) yield return e.Current;
		while (! isResponseCompleted) yield return null; // this while is just to be sure and clear
 
		// Deal up with the results
		if (requestState.errorMessage != null) {
			if ( requestState.errorMessage.Contains("404") || requestState.errorMessage.Contains("NameResolutionFailure") ) {
				isURLmissing = true;
			} else {
				Debug.LogError("[WebAsync] Error trying to verify if URL '"+ url +"' exists: "+ requestState.errorMessage);
			}
		}
 
		isURLcheckingCompleted = true;
	}	
 
	public IEnumerator DeleteURL (string url) {
		isURLcheckingCompleted = false;
		isURLmissing = false;
 
		if (debugLevel > 0) Debug.Log ("WebAsync: DeleteURL: " + url);
		
		Uri httpSite = new Uri(url);
		WebRequest webRequest = WebRequest.Create(httpSite);
 		
		// We need no more than HTTP's head
		//webRequest.Method = "HEAD";
		//webRequest.Method = "GET";
		webRequest.Method = "DELETE";
 			
		
		if (debugLevel > 0) Debug.Log("WebAsync: DeleteURL:  HttpHeaders are " + webRequest.Headers);			
		
		
		// Get the request's reponse
		requestState = null;
 
		// Manually iterate IEnumerator, because Unity can't do it (and this does not inherit StartCoroutine from MonoBehaviour)
		IEnumerator e = GetResponse(webRequest);
		while (e.MoveNext()) yield return e.Current;
		while (! isResponseCompleted) yield return null; // this while is just to be sure and clear
 
		// Deal up with the results
		if (requestState.errorMessage != null) {
			if ( requestState.errorMessage.Contains("404") || requestState.errorMessage.Contains("NameResolutionFailure") ) {
				isURLmissing = true;
			} else {
				Debug.LogError("[WebAsync] Error trying to verify if URL '"+ url +"' exists: "+ requestState.errorMessage);
			}
		}
 
		isURLcheckingCompleted = true;
	}			
	
	public IEnumerator PostURL (string url, string Parameters) {
		isURLcheckingCompleted = false;
		isURLmissing = false;
 		bool validPost = false;
		
		if (debugLevel > 0) Debug.Log ("WebAsync: PostURL: " + url + " params: " + Parameters);
		
		Uri httpSite = new Uri(url);
		WebRequest webRequest = WebRequest.Create(httpSite);
 
		//Add these, as we're doing a POST
	    //webRequest.ContentType = "application/x-www-form-urlencoded";
		webRequest.ContentType = "application/json";
		webRequest.Method = "POST";
 
		//webRequest.Headers["stuff"] = appSecret;

		if (debugLevel > 0) Debug.Log("WebAsync: PostURL: HttpHeaders are " + webRequest.Headers);			
		
		try {
			//We need to count how many bytes we're sending. Post'ed Faked Forms should be name=value&
			byte [] bytes = System.Text.Encoding.ASCII.GetBytes(Parameters);
			webRequest.ContentLength = bytes.Length;
			System.IO.Stream os = webRequest.GetRequestStream ();
			os.Write (bytes, 0, bytes.Length); //Push it out there
			os.Close ();

			// Get the request's reponse
			requestState = null;
	 
			validPost = true;
		} catch (Exception	ex) {
			Debug.Log ("WebAsync: PostURL: Exception: " + ex.Message);
		}				
			
		// solution 1
//		System.Net.WebResponse resp = webRequest.GetResponse();
//	   	if (resp == null) {
//			Debug.Log ("WebAsync: PostURL: empty response");
//			yield return null;
//		} else {
//			Debug.Log ("WebAsync: PostURL: got response");
//	   		System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
//	   		this.resultStr = sr.ReadToEnd().Trim();		
//	   		//yield return null;
//		}
		
		// solution 3
//		// Send the request:
//		using (Stream post = req.GetRequestStream())  
//		{  
//			post.Write(formData, 0, formData.Length);  
//		}
//		
//		// Pick up the response:
//		string result = null;
//		using (HttpWebResponse resp = req.GetResponse()
//		                            as HttpWebResponse)  
//		{  
//			StreamReader reader = 
//		  	  new StreamReader(resp.GetResponseStream());
//			result = reader.ReadToEnd();
//		}		
//		
			
			
		if (validPost) {
			/// solution 2
			// Manually iterate IEnumerator, because Unity can't do it (and this does not inherit StartCoroutine from MonoBehaviour)
			IEnumerator e = GetResponse(webRequest);
			while (e.MoveNext()) yield return e.Current;
			while (! isResponseCompleted) yield return null; // this while is just to be sure and clear
	 
			// Deal up with the results
			if (requestState.errorMessage != null) {
				if ( requestState.errorMessage.Contains("404") || requestState.errorMessage.Contains("NameResolutionFailure") ) {
					isURLmissing = true;
				} else {
					Debug.LogError("[WebAsync] Error: "+ requestState.errorMessage);
				}
			}
 
		}		
			
		isURLcheckingCompleted = true;
		
	}			
	
	/// <summary>
	/// Equivalent of webRequest.GetResponse, but using our own RequestState.
	/// This can or should be used along with web async instance's isResponseCompleted parameter
	/// inside a IEnumerator method capable of yield return for it, although it's mostly for clarity.
	/// Here's an usage example:
	/// 
	/// WebAsync webAsync = new WebAsync(); StartCoroutine( webAsync.GetReseponse(webRequest) );
	/// while (! webAsync.isResponseCompleted) yield return null;
	/// RequestState result = webAsync.requestState;
	/// 
	/// </summary>
	/// <param name='webRequest'>
	/// A System.Net.WebRequest instanced var.
	/// </param>
	public IEnumerator GetResponse (WebRequest webRequest) {
		if (debugLevel > 0) Debug.Log ("WebAsync: GetResponse: " + webRequest.RequestUri.OriginalString);
		isResponseCompleted = false;
		requestState = new RequestState();
 
		
		// Put the request into the state object so it can be passed around
		requestState.webRequest = webRequest;
 
		// Do the actual async call here
		IAsyncResult asyncResult = (IAsyncResult) webRequest.BeginGetResponse(
			new AsyncCallback(RespCallback), requestState);
 
		// WebRequest timeout won't work in async calls, so we need this instead
		ThreadPool.RegisterWaitForSingleObject(
			asyncResult.AsyncWaitHandle,
			new WaitOrTimerCallback(ScanTimeoutCallback),
			requestState,
			(TIMEOUT *1000), // obviously because this is in miliseconds
			true
			);
 
		// Wait until the the call is completed
		while (!asyncResult.IsCompleted) { yield return null; }
 
		if (debugLevel > 0) Debug.Log ("WebAsync: GetResponse: asyncResult complete");		
		
			
		// Help debugging possibly unpredictable results
		if (requestState != null) {
			if (requestState.errorMessage != null) {
				// this is not an ERROR because there are at least 2 error messages that are expected: 404 and NameResolutionFailure - as can be seen on CheckForMissingURL
				//Debug.Log("[WebAsync] Error message while getting response from request '"+ webRequest.RequestUri.ToString() +"': "+ requestState.errorMessage);
				if (debugLevel > 0) Debug.Log("[WebAsync] Error message while getting response from request '"+ webRequest.RequestUri.ToString() +"': "+ requestState.errorMessage);
				
				Debug.Log ("[WebAsync] Did you setup your api key from coinbase?");
				Debug.Log("[WebAsync] Error message while getting response from request : "+ requestState.errorMessage);				
				
			}
			
			// requestState.webResponse is set by RespCallback
			if (requestState.webResponse == null) {
				Debug.Log ("[WebAsync] webResponse is null! Are you connected to the internet?");
			} else if (requestState.webResponse.GetResponseStream() == null) {
				Debug.Log ("[WebAsync] webResponse GetResponseStream is null!");
			} else {
				System.IO.StreamReader sr = new System.IO.StreamReader(requestState.webResponse.GetResponseStream());
		   		this.resultStr = sr.ReadToEnd().Trim();		
	 			//Debug.Log ("GetResponse: webResponse: " + this.resultStr);				
			}

            // Show results as text
            // unitywebrequest versus system.net.webrequest
            //Debug.Log("[WebAsync] " + webRequest.downloadHandler.text);
        }
		
		

		
		isResponseCompleted = true;
	}
 
	private void RespCallback (IAsyncResult asyncResult)
	{
		if (debugLevel > 0) Debug.Log ("WebAsync: RespCallback: asyncResult complete");
		
		WebRequest webRequest = requestState.webRequest;
 
		try {
			requestState.webResponse = webRequest.EndGetResponse(asyncResult);

            // never gets here - when using https!!!
            //Debug.Log("WebAsync: RespCallback: length: " + requestState.webResponse.ContentLength);
        } catch (WebException webException) {
			HttpWebResponse response = (HttpWebResponse) webException.Response;
        	Debug.LogError("WebAsync: RespCallback: statusCode: " + response.StatusCode +
				" statusDescription: " + 
				response.StatusDescription);
			
			requestState.webResponse = response;
			
			requestState.errorMessage = "WebAsync: RespCallback: "+ webException.Message;
		}
	}
 
	private void ScanTimeoutCallback (object state, bool timedOut)
	{
		
		
		if (timedOut)  {
			if (debugLevel > 0) Debug.Log ("WebAsync: ScanTimeoutCallback: timed out");
			RequestState requestState = (RequestState)state;
			if (requestState != null) 
				requestState.webRequest.Abort();
		} else {
			if (debugLevel > 0) Debug.Log ("WebAsync: ScanTimeoutCallback: unregister");
			
			RegisteredWaitHandle registeredWaitHandle = (RegisteredWaitHandle)state;
			if (registeredWaitHandle != null)
				registeredWaitHandle.Unregister(null);
		}
	}
}