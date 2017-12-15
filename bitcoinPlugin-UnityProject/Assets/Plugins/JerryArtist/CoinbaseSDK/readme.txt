readme.txt

Overview
--------

The bitcoin sdk is used to create content purchases in Unity through coinbase's SDK.
Coinbase's sdk is documented here:
	https://coinbase.com/docs/api/overview
	https://coinbase.com/api/doc/1.0.html
	https://developers.coinbase.com/api/v2
Setup
-----

The to use this sdk, first setup your own coinbase account (through www.coinbase.com), 
then create an api key to use for your application.  Without this api key, the sample applications 
will not work.  Also do not give others access to your api key, as that may compromise your coinbase account.

Please make sure you have another email account to send btc test purchase email requests to.


Installation instructions
-------------------------

1. To run the sample programs, create a coinbase account, and create an api key.
2. open the "CoinbaseSimpleTestScene.unity" with unity (located in Plugins/JerryArtist/CoinbaseSDK)
3. select the maincamera
4. select the "CoinbaseSimpleSDKTest" script component
5. Paste the api key into the "api key" field section.
6. run the scene
7. fill in the email field
8. try to send purchase requests for the sample content to this email account


Purchasing Process
------------------

The following steps describe the purchasing process using the coinbase REST protocol.

1.0 a user clicks on purchase content
1.1	the sdk sends a transactions-request money request
		(example transaction request)
			 id: 52bdbaa65482dfd16500009c request: True status: pending
			 sender: jerryartist@gmail.com recipient: Jerome Kalkhof	
1.2		-coinbase sends back transaction id
1.3		-email is sent for btc transaction to the user's email from coinbase		
1.4		-user logs in, sends btc through coinbase website
			-coinbase sends verify email
			-user completes login
			-user sees pending requests
			-user clicks complete request (need funds in coinbase account to complete request)
				-user clicks on account settings - to see bitcoin address
				-user sends btc to his coinbase account
				(wait 1 hour for verification, but you can skip this and send btc right away)
				-user sends btc from coinbase acccount to merchant account				
2.0	the sdk application verifies the transaction for api_key	
2.1		the sdk will find transaction for userid, api_key, notes
2.2		verify status is complete
		(example transaction)
			 id: 52bdbeed6f5e009dee00000f request: False status: complete
			 sender: jerryartist@gmail.com recipient: Jerome Kalkhof		

