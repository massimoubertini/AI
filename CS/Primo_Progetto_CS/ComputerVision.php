<?php
require_once 'HTTP/Request2.php';
$request = new Http_Request2('https://api.projectoxford.ai/vision/v1.0/analyze');
$url = $request->getUrl();
$headers = array(
    // richiesta intestazion
    'Content-Type' => 'application/json',
    'Ocp-Apim-Subscription-Key' => '{subscription key}',
);
$request->setHeader($headers);
$parameters = array(
     // richiesta parametr
    'visualFeatures' => 'Categories',
    'details' => '{string}',
);
$url->setQueryVariables($parameters);
$request->setMethod(HTTP_Request2::METHOD_POST);
// richiesta body
$request->setBody("{body}");
try {
    $response = $request->send();
    echo $response->getBody();
}
catch (HttpException $ex)
{ echo $ex; }
?>