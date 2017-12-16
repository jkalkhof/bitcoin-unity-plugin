# bitcoin-unity-plugin
An android plugin for unity3d which is relying on the bitcoinj library.
Uses integration-android.jar to handle btc requests to bitcoinj and the user's wallet app.

Migrated plugin from code from bitcoin-wallet project - see https://github.com/bitcoin-wallet/bitcoin-wallet
Simply calls bitcoinj to assemble the BIP70 message to the wallet app requesting payment.
Checks with bitcoin.info website to verify transactions.
Copy and paste transaction info to gui from wallet if not immediately confirmed.
Checks with bitcoin.info to get exchange rates for asset purchases in app.

Use at your own risk!!

This project contains several sub-projects:
* bitcoin-unity-plugin - Android code to handle requests from Unity and format them for bitcoinj and create android intent request.
* bitcoinPlugin-UnityProject - Unity code which has a sample app requesting $1 USD payment to unlock an asset in the app.

Run the app in the Unity IDE to see how the code should work.
See stubbed responses from the android plugin will give valid transaction # to verify
against the transfer of btc to the sample wallet address.

See sample video of plugin working here:
https://youtu.be/KFW2Qx_-WHU
