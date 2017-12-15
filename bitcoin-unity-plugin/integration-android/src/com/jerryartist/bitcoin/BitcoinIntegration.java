/**
 * Copyright 2012-2014 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.jerryartist.bitcoin;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.net.Uri;
import android.text.Spannable;
import android.text.SpannableStringBuilder;
import android.text.style.TypefaceSpan;
import android.widget.Toast;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.google.protobuf.ByteString;
import com.unity3d.player.UnityPlayer;

import org.bitcoin.protocols.payments.Protos;
import org.bitcoinj.core.Address;
import org.bitcoinj.core.AddressFormatException;
import org.bitcoinj.core.NetworkParameters;
import org.bitcoinj.script.ScriptBuilder;

//import com.unity3d.player;

import java.util.ArrayList;

/**
 * @author Andreas Schildbach
 * modified by jkalkhof for unity compatibility
 */
public final class BitcoinIntegration {
    private static final String INTENT_EXTRA_PAYMENTREQUEST = "paymentrequest";
    private static final String INTENT_EXTRA_PAYMENT = "payment";
    private static final String INTENT_EXTRA_TRANSACTION_HASH = "transaction_hash";

    private static final String MIMETYPE_PAYMENTREQUEST = "application/bitcoin-paymentrequest"; // BIP 71

    // from SampleActivity.java
    private static final String[] DONATION_ADDRESSES_MAINNET = { "18CK5k1gajRKKSC7yVSTXT9LUzbheh1XY4",
            "1PZmMahjbfsTy6DsaRyfStzoWTPppWwDnZ" };
    private static final long AMOUNT0 = 500000;
    private static final long AMOUNT1 = 500000;
    private static final String MEMO = "Sample donation";
    private static final int REQUEST_CODE = 0;

    // Needed to get the battery level.
    private Context context;
    private Activity mActivity;

    private static final Logger log = LoggerFactory.getLogger(BitcoinIntegration.class);

    // so we can initialize this library as a class using the unity current activity as a context
    // example in .cs :
    /*
        if (Application.platform == RuntimePlatform.Android)
        {
            using (var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (var androidPlugin = new AndroidJavaObject("de.schildbach.wallet.integration.android.BitcoinIntegration", currentActivity))
                    {
     */
    public BitcoinIntegration()
    {
        log.info("BitcoinIntegration: initialize");

    }

    public void setContext(android.content.Context context)
    {
        log.info("BitcoinIntegration: initialize - Context");

        this.context = context;
    }

    public BitcoinIntegration(com.unity3d.player.UnityPlayerActivity unityPlayerActivity)
    {
        log.info("BitcoinIntegration: initialize - UnityPlayerActivity");

        this.context = unityPlayerActivity.getBaseContext();
        this.mActivity = (Activity) unityPlayerActivity;
    }

    public void testMethod(String data) {
        log.info("The data was {}", data);
    }

    public void handleRequestUnity(String address0, String address1, java.lang.Long amount0, java.lang.Long amount1, String memoStr) {

        ArrayList<String> arrayList = new ArrayList<String>();
        arrayList.add(address0);
        arrayList.add(address1);
        String [] addresses = arrayList.toArray(new String[arrayList.size()]);

        log.info("handleRequestUnity: address0: {}", addresses[0]);
        log.info("handleRequestUnity: address1: {}", addresses[0]);
        log.info("handleRequestUnity: amount0: {}", amount0);
        log.info("handleRequestUnity: amount1: {}", amount1);
        log.info("handleRequestUnity: memoStr: {}", memoStr);

        handleRequest(this.mActivity, addresses, amount0, amount1, memoStr);
    }

    public static void handleRequestUnityStatic(com.unity3d.player.UnityPlayerActivity parentActivity, String address0, String address1, long amount0, long amount1, String memoStr) {
        ArrayList<String> arrayList = new ArrayList<String>();
        arrayList.add(address0);
        arrayList.add(address1);
        String [] addresses = arrayList.toArray(new String[arrayList.size()]);

        log.info("handleRequestUnityStatic: address0: {}", addresses[0]);
        log.info("handleRequestUnityStatic: address1: {}", addresses[1]);
        log.info("handleRequestUnityStatic: amount0: {}", amount0);
        log.info("handleRequestUnityStatic: amount1: {}", amount1);
        log.info("handleRequestUnityStatic: memoStr: {}", memoStr);

        handleRequest((Activity) parentActivity, addresses, amount0, amount1, memoStr);
    }

    static public void handleRequest(Activity parentActivity, String[] addresses, long amount0, long amount1, String memoStr) {

        try {
            final NetworkParameters params = Address.getParametersFromAddress(addresses[0]);

            /*
               https://github.com/bitcoin/bips/blob/master/bip-0070.mediawiki
               https://github.com/bitcoin/bips/blob/master/bip-0070/paymentrequest.proto
                https://en.bitcoin.it/wiki/Script

                message Output {
                        optional uint64 amount = 1 [default = 0]; // amount is integer-number-of-satoshis
                        required bytes script = 2; // usually one of the standard Script forms
                }
             */
            final Protos.Output.Builder output1 = Protos.Output.newBuilder();
            output1.setAmount(amount0);
            output1.setScript(ByteString
                    .copyFrom(ScriptBuilder.createOutputScript(new Address(params, addresses[0])).getProgram()));

            log.info("handleRequest: amount0: {}", amount0);
            log.info("handleRequest: addresses[0]: {}", addresses[0]);


            final Protos.Output.Builder output2 = Protos.Output.newBuilder();
            if ((addresses[1] != null) && (addresses[1].length() != 0)) {
                output2.setAmount(amount1);
                output2.setScript(ByteString
                        .copyFrom(ScriptBuilder.createOutputScript(new Address(params, addresses[1])).getProgram()));

                log.info("handleRequest: amount1: {}", amount1);
                log.info("handleRequest: addresses[1]: {}", addresses[1]);
            }

            /*
                from BIP70
                https://github.com/bitcoin/bips/blob/master/bip-0070.mediawiki

                mimetype: application/bitcoin-paymentrequest

                message PaymentDetails {
                    optional string network = 1 [default = "main"];
                    repeated Output outputs = 2;
                    required uint64 time = 3;
                    optional uint64 expires = 4;
                    optional string memo = 5;
                    optional string payment_url = 6;
                    optional bytes merchant_data = 7;
                }
             */

            final Protos.PaymentDetails.Builder paymentDetails = Protos.PaymentDetails.newBuilder();
            paymentDetails.setNetwork(params.getPaymentProtocolId());
            paymentDetails.addOutputs(output1);
            if ((addresses[1] != null) && (addresses[1].length() != 0)) {
                paymentDetails.addOutputs(output2);
            }
            paymentDetails.setMemo(memoStr);
            paymentDetails.setTime(System.currentTimeMillis());

            log.info("handleRequest:MEMO: {}", memoStr);

            final Protos.PaymentRequest.Builder paymentRequest = Protos.PaymentRequest.newBuilder();
            paymentRequest.setSerializedPaymentDetails(paymentDetails.build().toByteString());

            BitcoinIntegration.requestForResult(parentActivity, REQUEST_CODE,
                    paymentRequest.build().toByteArray());

        } catch (final AddressFormatException x) {
            throw new RuntimeException(x);
        }
    }

//    @Override
//    protected void onActivityResult(final int requestCode, final int resultCode, final Intent data) {
//
//        log.info("onActivityResult: requestCode: {}", Integer.toString(requestCode));
//
//        if (requestCode == REQUEST_CODE) {
//            if (resultCode == Activity.RESULT_OK) {
//                final String txHash = BitcoinIntegration.transactionHashFromResult(data);
//                if (txHash != null) {
//                    final SpannableStringBuilder messageBuilder = new SpannableStringBuilder("Transaction hash:\n");
//                    messageBuilder.append(txHash);
//                    messageBuilder.setSpan(new TypefaceSpan("monospace"), messageBuilder.length() - txHash.length(),
//                            messageBuilder.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);
//
//                    if (BitcoinIntegration.paymentFromResult(data) != null)
//                        messageBuilder.append("\n(also a BIP70 payment message was received)");
//
////                    donateMessage.setText(messageBuilder);
////                    donateMessage.setVisibility(View.VISIBLE);
//                }
//
//                Toast.makeText(this, "Thank you!", Toast.LENGTH_LONG).show();
//            } else if (resultCode == Activity.RESULT_CANCELED) {
//                Toast.makeText(this, "Cancelled.", Toast.LENGTH_LONG).show();
//            } else {
//                Toast.makeText(this, "Unknown result.", Toast.LENGTH_LONG).show();
//            }
//        }
//    }

    /**
     * Request any amount of Bitcoins (probably a donation) from user, without feedback from the app.
     * 
     * @param context
     *            Android context
     * @param address
     *            Bitcoin address
     */
    public static void request(final Context context, final String address) {
        final Intent intent = makeBitcoinUriIntent(address, null);

        start(context, intent);
    }

    /**
     * Request specific amount of Bitcoins from user, without feedback from the app.
     * 
     * @param context
     *            Android context
     * @param address
     *            Bitcoin address
     * @param amount
     *            Bitcoin amount in satoshis
     */
    public static void request(final Context context, final String address, final long amount) {
        final Intent intent = makeBitcoinUriIntent(address, amount);

        start(context, intent);
    }

    /**
     * Request payment from user, without feedback from the app.
     * 
     * @param context
     *            Android context
     * @param paymentRequest
     *            BIP70 formatted payment request
     */
    public static void request(final Context context, final byte[] paymentRequest) {
        final Intent intent = makePaymentRequestIntent(paymentRequest);

        start(context, intent);
    }

    /**
     * Request any amount of Bitcoins (probably a donation) from user, with feedback from the app. Result
     * intent can be received by overriding {@link android.app.Activity#onActivityResult()}. Result indicates
     * either {@link Activity#RESULT_OK} or {@link Activity#RESULT_CANCELED}. In the success case, use
     * {@link #transactionHashFromResult(Intent)} to read the transaction hash from the intent.
     * 
     * Warning: A success indication is no guarantee! To be on the safe side, you must drive your own Bitcoin
     * infrastructure and validate the transaction.
     * 
     * @param activity
     *            Calling Android activity
     * @param requestCode
     *            Code identifying the call when {@link android.app.Activity#onActivityResult()} is called
     *            back
     * @param address
     *            Bitcoin address
     */
    public static void requestForResult(final Activity activity, final int requestCode, final String address) {
        final Intent intent = makeBitcoinUriIntent(address, null);

        startForResult(activity, requestCode, intent);
    }

    /**
     * Request specific amount of Bitcoins from user, with feedback from the app. Result intent can be
     * received by overriding {@link android.app.Activity#onActivityResult()}. Result indicates either
     * {@link Activity#RESULT_OK} or {@link Activity#RESULT_CANCELED}. In the success case, use
     * {@link #transactionHashFromResult(Intent)} to read the transaction hash from the intent.
     * 
     * Warning: A success indication is no guarantee! To be on the safe side, you must drive your own Bitcoin
     * infrastructure and validate the transaction.
     * 
     * @param activity
     *            Calling Android activity
     * @param requestCode
     *            Code identifying the call when {@link android.app.Activity#onActivityResult()} is called
     *            back
     * @param address
     *            Bitcoin address
     */
    public static void requestForResult(final Activity activity, final int requestCode, final String address,
            final long amount) {
        final Intent intent = makeBitcoinUriIntent(address, amount);

        startForResult(activity, requestCode, intent);
    }

    /**
     * Request payment from user, with feedback from the app. Result intent can be received by overriding
     * {@link android.app.Activity#onActivityResult()}. Result indicates either {@link Activity#RESULT_OK} or
     * {@link Activity#RESULT_CANCELED}. In the success case, use {@link #transactionHashFromResult(Intent)}
     * to read the transaction hash from the intent.
     * 
     * Warning: A success indication is no guarantee! To be on the safe side, you must drive your own Bitcoin
     * infrastructure and validate the transaction.
     * 
     * @param activity
     *            Calling Android activity
     * @param requestCode
     *            Code identifying the call when {@link android.app.Activity#onActivityResult()} is called
     *            back
     * @param paymentRequest
     *            BIP70 formatted payment request
     */
    public static void requestForResult(final Activity activity, final int requestCode, final byte[] paymentRequest) {
        final Intent intent = makePaymentRequestIntent(paymentRequest);

        startForResult(activity, requestCode, intent);
    }

    /**
     * Get payment request from intent. Meant for usage by applications accepting payment requests.
     * 
     * @param intent
     *            intent
     * @return payment request or null
     */
    public static byte[] paymentRequestFromIntent(final Intent intent) {
        final byte[] paymentRequest = intent.getByteArrayExtra(INTENT_EXTRA_PAYMENTREQUEST);

        return paymentRequest;
    }

    /**
     * Put BIP70 payment message into result intent. Meant for usage by Bitcoin wallet applications.
     * 
     * @param result
     *            result intent
     * @param payment
     *            payment message
     */
    public static void paymentToResult(final Intent result, final byte[] payment) {
        result.putExtra(INTENT_EXTRA_PAYMENT, payment);
    }

    /**
     * Get BIP70 payment message from result intent. Meant for usage by applications initiating a Bitcoin
     * payment.
     * 
     * You can use the transactions contained in the payment to validate the payment. For this, you need your
     * own Bitcoin infrastructure though. There is no guarantee that the payment will ever confirm.
     * 
     * @param result
     *            result intent
     * @return payment message
     */
    public static byte[] paymentFromResult(final Intent result) {
        final byte[] payment = result.getByteArrayExtra(INTENT_EXTRA_PAYMENT);

        return payment;
    }

    /**
     * Put transaction hash into result intent. Meant for usage by Bitcoin wallet applications.
     * 
     * @param result
     *            result intent
     * @param txHash
     *            transaction hash
     */
    public static void transactionHashToResult(final Intent result, final String txHash) {
        result.putExtra(INTENT_EXTRA_TRANSACTION_HASH, txHash);
    }

    /**
     * Get transaction hash from result intent. Meant for usage by applications initiating a Bitcoin payment.
     * 
     * You can use this hash to request the transaction from the Bitcoin network, in order to validate. For
     * this, you need your own Bitcoin infrastructure though. There is no guarantee that the transaction has
     * ever been broadcasted to the Bitcoin network.
     * 
     * @param result
     *            result intent
     * @return transaction hash
     */
    public static String transactionHashFromResult(final Intent result) {
        final String txHash = result.getStringExtra(INTENT_EXTRA_TRANSACTION_HASH);

        return txHash;
    }

    private static final int SATOSHIS_PER_COIN = 100000000;

    private static Intent makeBitcoinUriIntent(final String address, final Long amount) {
        log.info("makeBitcoinUriIntent:");

        final StringBuilder uri = new StringBuilder("bitcoin:");
        if (address != null)
            uri.append(address);
        if (amount != null)
            uri.append("?amount=")
                    .append(String.format("%d.%08d", amount / SATOSHIS_PER_COIN, amount % SATOSHIS_PER_COIN));

        final Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(uri.toString()));

        return intent;
    }

    private static Intent makePaymentRequestIntent(final byte[] paymentRequest) {
        log.info("makePaymentRequestIntent:");

        final Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setType(MIMETYPE_PAYMENTREQUEST);
        intent.putExtra(INTENT_EXTRA_PAYMENTREQUEST, paymentRequest);

        return intent;
    }

    private static void start(final Context context, final Intent intent) {
        log.info("start:");

        final PackageManager pm = context.getPackageManager();
        if (pm.resolveActivity(intent, 0) != null)
            context.startActivity(intent);
        else
            redirectToDownload(context);
    }

    private static void startForResult(final Activity activity, final int requestCode, final Intent intent) {
        log.info("startForResult:");

        if (requestCode == REQUEST_CODE) {
            if (requestCode == Activity.RESULT_OK) {
                final String txHash = BitcoinIntegration.transactionHashFromResult(intent);
                if (txHash != null) {
                    final SpannableStringBuilder messageBuilder = new SpannableStringBuilder("Transaction hash:\n");
                    messageBuilder.append(txHash);
                    messageBuilder.setSpan(new TypefaceSpan("monospace"), messageBuilder.length() - txHash.length(),
                            messageBuilder.length(), Spannable.SPAN_EXCLUSIVE_EXCLUSIVE);

                    if (BitcoinIntegration.paymentFromResult(intent) != null)
                        messageBuilder.append("\n(also a BIP70 payment message was received)");

                    log.info("startForResult: {}", messageBuilder);

                    UnityPlayer.UnitySendMessage("BitcoinSvcManager", "RequestResultTransactionHash", txHash);
                }

                UnityPlayer.UnitySendMessage("BitcoinSvcManager", "RequestResult", "Result ok");

                log.info("startForResult: Thank you!");
            } else if (requestCode == Activity.RESULT_CANCELED) {
                log.info("startForResult: Cancelled.!");

                UnityPlayer.UnitySendMessage("BitcoinSvcManager", "RequestResult", "Cancelled");
            } else {
                log.info("startForResult: Unknown result.");

                UnityPlayer.UnitySendMessage("BitcoinSvcManager", "RequestResult", "Unknown result");
            }
        }

        final PackageManager pm = activity.getPackageManager();
        if (pm.resolveActivity(intent, 0) != null)
            activity.startActivityForResult(intent, requestCode);
        else
            redirectToDownload(activity);
    }

    private static void redirectToDownload(final Context context) {

        log.info("redirectToDownload:");

        Toast.makeText(context, "No Bitcoin application found.\nPlease install Bitcoin Wallet.", Toast.LENGTH_LONG)
                .show();

        final Intent marketIntent = new Intent(Intent.ACTION_VIEW,
                Uri.parse("market://details?id=de.schildbach.wallet"));
        final Intent binaryIntent = new Intent(Intent.ACTION_VIEW,
                Uri.parse("https://github.com/bitcoin-wallet/bitcoin-wallet/releases"));

        final PackageManager pm = context.getPackageManager();
        if (pm.resolveActivity(marketIntent, 0) != null)
            context.startActivity(marketIntent);
        else if (pm.resolveActivity(binaryIntent, 0) != null)
            context.startActivity(binaryIntent);
        // else out of luck
    }
}
