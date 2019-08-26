import http.client, urllib.request, urllib.parse, urllib.error, base64
headers = {
    # richiesta intestazioni
    'Content-Type': 'application/json',
    'Ocp-Apim-Subscription-Key': '{subscription key}',
}
params = urllib.parse.urlencode({
    # richiesta parametri
    'visualFeatures': 'Categories',
    'details': '{string}',
})
try:
    conn = http.client.HTTPSConnection('api.projectoxford.ai')
    conn.request("POST", "/vision/v1.0/analyze?%s" % params, "{body}", headers)
    response = conn.getresponse()
    data = response.read()
    print(data)
    conn.close()
except Exception as e:
    print("[Errno {0}] {1}".format(e.errno, e.strerror))