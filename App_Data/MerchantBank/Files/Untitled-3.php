<?php
add_filter("wcra_wcra_test_callback" , "wcra_wcra_test_callback_handler");
function wcra_wcra_test_callback_handler($param){
//$param = All GET/POST values will be received from endpoint
//do your stuff here
//get form inputs
$custmerEmail = $param['custmerEmail'];
$transactionAmount = $param['transactionAmount'];
$paymentCurrency = $param['paymentCurrency'];
$Status = $param['Status'];
$Description = $param['Description'];
//send an http request to proxy using curl
// set post fields
$post = [
    'custmerEmail' => $custmerEmail,
    'transactionAmount' => $transactionAmount,
    'paymentCurrency'   => $paymentCurrency,
    'Status' => $Status,
    'Description' => $Description
];

$ch = curl_init('http://192.168.100.172:5000/WebService/OnlinePayments');
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, $post);

// execute!
$response = curl_exec($ch);

// close the connection, release resources used
curl_close($ch);

// do anything you want with your response
$json = json_decode($result, true);
wp_redirect($json['url']);
exit;
}
?>