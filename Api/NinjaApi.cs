using Microsoft.Extensions.Configuration;
using net.hempux.Utilities;
using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace net.hempux.kabuto
{

public class NinjaApi
{
	private readonly String ACCESS_KEY_ID;
	private readonly String SECRET_ACCESS_KEY;
    private readonly String API_HOST;
    private readonly String CUSTOMERS_PATH = "/v1/customers";
    private readonly String DEVICES_PATH = "/v1/devices";
	


		public NinjaApi(){

			ACCESS_KEY_ID = AppSettings.Current.Ninja["AccessKey"];
			SECRET_ACCESS_KEY = AppSettings.Current.Ninja["SecretKey"];
			API_HOST = AppSettings.Current.Ninja["ApiHost"];

		}

    public string getCustomers() 
    {
        string customersResponse = doRequest("GET", CUSTOMERS_PATH);
        return customersResponse; 
    }

    public string getDevices() {
        string devicesResponse = doRequest("GET", DEVICES_PATH);
        return devicesResponse;
    }

		public string getDevice(int device)
		{
			string devicesResponse = doRequest("GET", (DEVICES_PATH + "/" + device));

			return devicesResponse;
		}


		private string doRequest(string httpMethod, string path, string contentType = null) {
		string contentMD5 = null;
		DateTime requestDate = DateTime.UtcNow;
		string stringToSign = getStringToSign(httpMethod, contentMD5, contentType, requestDate, path);
		string signature = getSignature(SECRET_ACCESS_KEY, stringToSign);
        string url = API_HOST + path;
        string responseText = null;

		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = httpMethod;
        request.ContentType = contentType;
		request.Headers.Add("Authorization", "NJ " + ACCESS_KEY_ID + ":" + signature);
		request.Headers.Add("x-nj-date", RFC1123_DATE_TIME_FORMATTER(requestDate));
		request.ProtocolVersion = HttpVersion.Version11;
		HttpWebResponse response = (HttpWebResponse)request.GetResponse();

		using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
		{
			responseText = reader.ReadToEnd();
		}

        return responseText;
    }

	private string RFC1123_DATE_TIME_FORMATTER(DateTime requestDate)
	{
		var usCulture = new CultureInfo("en-US");
		var str = requestDate.ToString("ddd, dd MMM yyyy HH:mm:ss \'GMT\'", usCulture);
		return str;
	}

	private String getStringToSign(String httpMethod, String contentMD5, String contentType, DateTime requestDate, String canonicalPath)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append((httpMethod + "\n"));
		stringBuilder.Append((contentMD5 != null ? contentMD5 + "\n" : "\n"));
		stringBuilder.Append((contentType != null ? contentType + "\n" : "\n"));
        stringBuilder.Append(RFC1123_DATE_TIME_FORMATTER(requestDate) + "\n");
		stringBuilder.Append(canonicalPath);

        return stringBuilder.ToString();
	}
	private String getSignature(String secretAccessKey, String stringToSign)
	{
		var enc = Encoding.UTF8;

		var stringToSignBytes = System.Text.Encoding.UTF8.GetBytes(stringToSign);
        string encodedString = System.Convert.ToBase64String(stringToSignBytes).Replace("\n", "").Replace("\r", "");

		HMACSHA1 hmac = new HMACSHA1(enc.GetBytes(secretAccessKey));
		hmac.Initialize();

        byte[] hmacBytes = hmac.ComputeHash(enc.GetBytes(encodedString));

		var signature = System.Convert.ToBase64String(hmacBytes).Replace("\n", "");

        return signature;
	}
}

}