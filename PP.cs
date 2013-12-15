/*
 * Created by SharpDevelop.
 * User: alfcas
 * Date: 02/11/2013
 * Time: 16:01
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

using System.IO;
using System.Net;
using System.Text;

using System.Runtime.InteropServices;



namespace PaypalAPI
{


	#region PayRequest
	public enum PaymentType {
		Simple,						//unique receiver
		Parallel,					//many receivers (no primary field)
		Chained						//many receivers (first primary others secondary)
	}
	public struct Receiver {
		public string email;
		public string amount;
		public bool primary;
	}
	
	public class PayRequest
	{
		private string m_ActionType;
		private PaymentType m_PaymentType;
		private string m_CancelUrl;
		private string m_ReturnUrl;
		private string m_NotificationUrl;
		private string m_Currency;
		private string m_Language;
		private string m_Memo;
		private string m_SenderEmail;
		private List<Receiver> m_ReceiverList;

		
		public PayRequest()
		{
			m_ActionType="PAY";
			m_PaymentType = PaymentType.Simple;
			m_CancelUrl="";
			m_ReturnUrl="";
			m_NotificationUrl="";
			m_Currency="";
			m_Language="";
			m_Memo="";
			m_SenderEmail="";
			m_ReceiverList = new List<Receiver>();
		}
		public void Clear()
		{
			m_PaymentType = PaymentType.Simple;
			m_CancelUrl="";
			m_ReturnUrl="";
			m_NotificationUrl="";
			m_Currency="";
			m_Language="";
			m_Memo="";
			m_SenderEmail="";
			m_ReceiverList.Clear();
		}

		public PaymentType PaymentType {
			get { return m_PaymentType; }
			set { m_PaymentType = value; }
		}
		public string CancelUrl {
			get { return m_CancelUrl; }
			set { m_CancelUrl = value; }
		}
		public string ReturnUrl {
			get { return m_ReturnUrl; }
			set { m_ReturnUrl = value; }
		}
		public string NotificationUrl {
			get { return m_NotificationUrl; }
			set { m_NotificationUrl = value; }
		}
		public string Memo {
			get { return m_Memo; }
			set { m_Memo = value; }
		}
		public string Currency {
			get { return m_Currency; }
			set { m_Currency = value; }
		}
		public string Language {
			get { return m_Language; }
			set { m_Language = value; }
		}
		public string SenderEmail {
			get { return m_SenderEmail; }
			set { m_SenderEmail = value; }
		}
		public List<Receiver> ReceiverList {
			get { return m_ReceiverList; }
		}
		
		public void AddReceiver( string email, string amount )
		{
			Receiver item;
			item.email = email;
			item.amount = amount;
			item.primary=false;
			m_ReceiverList.Add(item);
		}
		public void AddReceiver( string email, string amount, bool primary )
		{
			Receiver item;
			item.email = email;
			item.amount = amount;
			item.primary=primary;
			m_ReceiverList.Add(item);
		}
		
		public string PostData()
		{
			int iReceiver;
			
			string postData = "actionType=" + m_ActionType;
			//
			if (m_CancelUrl != "") {
				postData += "&cancelUrl=" + m_CancelUrl;
			}
			if (m_ReturnUrl != "") {
				postData += "&returnUrl=" + m_ReturnUrl;
			}
			if (m_NotificationUrl != "") {
				postData += "&ipnNotificationUrl=" + m_NotificationUrl;
			}
			// cliente
			if (m_SenderEmail != "") {
				postData += "&senderEmail=" + m_SenderEmail;
			}
			// fornitori
			switch (m_PaymentType) {
				case PaymentType.Simple:
					postData += "&receiverList.receiver(0).email=" + ReceiverList[0].email;
					postData += "&receiverList.receiver(0).amount=" + ReceiverList[0].amount;
					break;
					
				case PaymentType.Parallel:
					iReceiver=0;
					foreach (Receiver item in ReceiverList) {
						postData += "&receiverList.receiver(" + iReceiver.ToString("D1") + ").email=" + item.email;
						postData += "&receiverList.receiver(" + iReceiver.ToString("D1") + ").amount=" + item.amount;
						iReceiver++;
					}
					break;
					
				case PaymentType.Chained:
					iReceiver=0;
					foreach (Receiver item in ReceiverList) {
						postData += "&receiverList.receiver(" + iReceiver.ToString("D1") + ").email=" + item.email;
						postData += "&receiverList.receiver(" + iReceiver.ToString("D1") + ").amount=" + item.amount;
						if (ReceiverList.Count>1) {
							if (iReceiver == 0) {
								postData += "&receiverList.receiver(" + iReceiver.ToString("D1") + ").primary=true";
							} else {
								postData += "&receiverList.receiver(" + iReceiver.ToString("D1") + ").primary=false";
							}
						}
						iReceiver++;
					}
					break;
			}
			//
			if ( m_Currency != "" ) {
				postData += "&currencyCode=" + m_Currency;
			} else {
				postData += "&currencyCode=EUR";
			}
			//
			if ( m_Memo != "" ) {
				postData += "&memo=" + m_Memo;
			}
			//
			if ( m_Language != "" ) {
				postData += "&requestEnvelope.errorLanguage=" + m_Language;
			} else {
				postData += "&requestEnvelope.errorLanguage=en_US";
			}
			//
			return postData;
		}
	}
	#endregion


	#region PaymentDetails Request
	public class DetailsRequest
	{
		private string m_Currency;
		private string m_Language;
		private string m_PayKey;

		
		public DetailsRequest()
		{
			m_Currency="";
			m_Language="";
			m_PayKey="";
		}
		public void Clear()
		{
			m_Currency="";
			m_Language="";
			m_PayKey="";
		}

		public string Currency {
			get { return m_Currency; }
			set { m_Currency = value; }
		}
		public string Language {
			get { return m_Language; }
			set { m_Language = value; }
		}
		public string PaymentKey {
			get { return m_PayKey; }
			set { m_PayKey = value; }
		}
		
		public string PostData()
		{
			string postData = "requestEnvelope.detailLevel=ReturnAll";
			if ( m_Language != "" ) {
				postData += "&requestEnvelope.errorLanguage=" + m_Language;
			} else {
				postData += "&requestEnvelope.errorLanguage=en_US";
			}
			if ( m_Currency != "" ) {
				postData += "&currencyCode=" + m_Currency;
			} else {
				postData += "&currencyCode=EUR";
			}
			//
			postData += "&payKey=" + m_PayKey;
			//
			return postData;
		}
	}
	#endregion


	#region Refund Request
	public class RefundRequest
	{
		private string m_Currency;
		private string m_Language;
		private string m_PayKey;
		private string m_TransactionId;

		
		public RefundRequest()
		{
			m_Currency="";
			m_Language="";
			m_PayKey="";
			m_TransactionId="";
		}
		public void Clear()
		{
			m_Currency="";
			m_Language="";
			m_PayKey="";
			m_TransactionId="";
		}

		public string Currency {
			get { return m_Currency; }
			set { m_Currency = value; }
		}
		public string Language {
			get { return m_Language; }
			set { m_Language = value; }
		}
		public string PaymentKey {
			get { return m_PayKey; }
			set { m_PayKey = value; }
		}
		public string TransactionId {
			get { return m_TransactionId; }
			set { m_TransactionId = value; }
		}
		
		public string PostData()
		{
			string postData = "requestEnvelope.detailLevel=ReturnAll";
			//
			if ( m_Currency != "" ) {
				postData += "&currencyCode=" + m_Currency;
			} else {
				postData += "&currencyCode=EUR";
			}
			if ( m_Language != "" ) {
				postData += "&requestEnvelope.errorLanguage=" + m_Language;
			} else {
				postData += "&requestEnvelope.errorLanguage=en_US";
			}
			//
			postData += "&payKey=" + m_PayKey;
			//
			postData += "&transactionId=" + m_TransactionId;
			//
			return postData;
		}
	}
	#endregion


	#region ConvertCurrency Request
	public class ConvertCurrencyRequest
	{
		private string m_Amount;
		private string m_FromCurrency;
		private string m_ToCurrency;
		private string m_Language;

		
		public ConvertCurrencyRequest()
		{
			m_Amount="0";
			m_FromCurrency="";
			m_ToCurrency="";
			m_Language="";
		}
		public void Clear()
		{
			m_Amount="0";
			m_FromCurrency="";
			m_ToCurrency="";
			m_Language="";
		}

		public string Amount {
			get { return m_Amount; }
			set { m_Amount = value; }
		}
		public string FromCurrency {
			get { return m_FromCurrency; }
			set { m_FromCurrency = value; }
		}
		public string ToCurrency {
			get { return m_ToCurrency; }
			set { m_ToCurrency = value; }
		}
		public string Language {
			get { return m_Language; }
			set { m_Language = value; }
		}
		
		public string PostData()
		{
			string postData = "requestEnvelope.detailLevel=ReturnAll";
			//
			if ( m_Language != "" ) {
				postData += "&requestEnvelope.errorLanguage=" + m_Language;
			} else {
				postData += "&requestEnvelope.errorLanguage=en_US";
			}
			//
			if ( m_FromCurrency != "" ) {
				postData += "&baseAmountList.currency(0).code=" + m_FromCurrency;
			} else {
				postData += "&baseAmountList.currency(0).code=EUR";
			}
			//
			postData += "&baseAmountList.currency(0).amount=" + m_Amount;
			//
			if ( m_ToCurrency != "" ) {
				postData += "&convertToCurrencyList.currencyCode=" + m_ToCurrency;
			} else {
				postData += "&convertToCurrencyList.currencyCode=EUR";
			}
			//
			return postData;
		}
	}
	#endregion



	#region NVP
	public class NvpData {
		private Dictionary<string,string> m_nvpDict;
		private string[] m_nvpArray;
		private string m_nvpResponse;
		
		private string[] nvpPair;
		
		public NvpData()
		{
			m_nvpDict = new Dictionary<string, string>();
			m_nvpResponse="";
		}

		public void Clear() {
			m_nvpDict.Clear();
			m_nvpResponse="";
		}

		public string Response {
			get { return m_nvpResponse; }
			set {
				m_nvpResponse = value;
				// split nvp elements
				m_nvpArray = m_nvpResponse.Split('&');
				m_nvpDict.Clear();
				for (int i=0; i<m_nvpArray.Length; i++) {
					// split nvp pair key,value
					nvpPair=m_nvpArray[i].Split('=');
					m_nvpDict.Add(nvpPair[0],nvpPair[1]);
				}
			}
		}
		public string ReadableResponse {
			get { return m_nvpResponse.Replace('&','\n'); }
		}
		public string Find(string key)
		{
			string data;
			
			if ( m_nvpDict.ContainsKey(key) == true ) {
				m_nvpDict.TryGetValue(key, out data);
			} else {
				data = "";
			}
			return data;
		}
		public string Item(int item) {
			try {
				return m_nvpArray[item];
			} catch (Exception ex) {
				return "";
			}
		}
	}
	#endregion


	/// <summary>
	/// Classe PaypalAPI AdaptivePayments
	/// </summary>
	
	[ComVisible(true)]
	public class AdaptivePayments
	{
		private bool m_SandBox;
		private string m_UserID;
		private string m_Password;
		private string m_ApplicationID;
		private string m_SecuritySignature;
		private string m_Currency;
		private string m_Language;
		private string m_PayKey;
		private string m_LastErrorMessage;
		private PayRequest m_PayRequest;
		private NvpData m_nvpData = new NvpData();



		private const string DATAFORMAT="NV";


		#region API URI
		private enum URI {
			GO_PAY,
			API_PAY,
			API_PAYMENTDETAILS,
			API_REFUND,
			API_CONVERT
		}
		private string[] SANDBOX_URI =
		{
			"https://www.sandbox.paypal.com/webscr?cmd=_ap-payment&paykey=",
			"https://svcs.sandbox.paypal.com/AdaptivePayments/Pay",
			"https://svcs.sandbox.paypal.com/AdaptivePayments/PaymentDetails",
			"https://svcs.sandbox.paypal.com/AdaptivePayments/Refund",
			"https://svcs.sandbox.paypal.com/AdaptivePayments/ConvertCurrency"
		};
		private string[] LIVE_URI =
		{
			"https://www.paypal.com/webscr?cmd=_ap-payment&paykey=",
			"https://svcs.paypal.com/AdaptivePayments/Pay",
			"https://svcs.paypal.com/AdaptivePayments/PaymentDetails",
			"https://svcs.paypal.com/AdaptivePayments/Refund",
			"https://svcs.paypal.com/AdaptivePayments/ConvertCurrency"
		};
		private string[] m_Paypal_Uri;
		#endregion		


		#region Constructor
		public AdaptivePayments()
		{
			m_SandBox=false;
			m_Paypal_Uri = LIVE_URI;
			//
			m_UserID="";
			m_Password="";
			m_ApplicationID="";
			m_SecuritySignature="";
			m_Currency="EUR";
			m_Language="en_US";
			m_PayKey="";
			//
			NvpData nvpData = new NvpData();
			m_PayRequest = new PayRequest();
		}
		#endregion


		#region Public Properties
		[ComVisible(true)]
		public bool UseSandbox {
			get { return m_SandBox; }
			set {
				m_SandBox = value;
				if (value)
					m_Paypal_Uri=SANDBOX_URI;
				else
					m_Paypal_Uri=LIVE_URI;
			}
		}
		[ComVisible(true)]
		public string APIUserID {
			get { return m_UserID; }
			set { m_UserID = value; }
		}
		[ComVisible(true)]
		public string APIPassword {
			get { return m_Password; }
			set { m_Password = value; }
		}
		[ComVisible(true)]
		public string APISignature {
			get { return m_SecuritySignature; }
			set { m_SecuritySignature = value; }
		}
		[ComVisible(true)]
		public string AplicationID {
			get { return m_ApplicationID; }
			set { m_ApplicationID = value; }
		}
		
		[ComVisible(true)]
		public string PayUrl {
			get { return m_Paypal_Uri[(int)URI.GO_PAY] + m_PayKey; }
		}
		[ComVisible(true)]
		public string PayKey {
			get { return m_PayKey; }
		}
		[ComVisible(true)]
		public string LastError {
			get { return m_LastErrorMessage; }
		}
		[ComVisible(true)]
		public string LastResponse {
			get { return m_nvpData.ReadableResponse; }
		}

		#endregion


		#region Private methods
		private void LogException( string description )
		{
			m_LastErrorMessage = description;
		}


		private WebHeaderCollection APICustomHeaders()
		{
			// Initialize Adaptive Payments HTTP headers.
			// Use 3-Token Authentication and NVP payload.
			
			WebHeaderCollection valHeader = new WebHeaderCollection();
			
			try {
				if ( m_UserID=="" || m_Password=="" || m_SecuritySignature=="") {
					throw new System.ArgumentException("Credentials cannot be null", "PaypalAPI");
				}
				
				valHeader.Add("X-PAYPAL-SECURITY-USERID:" + m_UserID);
				valHeader.Add("X-PAYPAL-SECURITY-PASSWORD:" + m_Password);
				valHeader.Add("X-PAYPAL-SECURITY-SIGNATURE:" + m_SecuritySignature);
				valHeader.Add("X-PAYPAL-REQUEST-DATA-FORMAT:" + DATAFORMAT);
				valHeader.Add("X-PAYPAL-RESPONSE-DATA-FORMAT:" + DATAFORMAT);
				if (m_ApplicationID != "") {
					valHeader.Add("X-PAYPAL-APPLICATION-ID:" + m_ApplicationID);
				}
			} catch (Exception ex) {
				LogException("Exception:\n" + ex.ToString());
				return null;
			}
			return valHeader;
		}

		public string SendRequest(string apiUri, byte[] postData)
		{
			string response;
			string status;
			
			try {

				// Initialize URI Endpoint
				Uri address = new Uri(apiUri);

				// Create HttpWebRequest
				HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(address);

				// POST is the recommended PayPal API HTTP request method
				httpRequest.Method = "POST";

				// Set HTTP headers
				httpRequest.Headers = APICustomHeaders();

				// HTTP Content-Type header
				httpRequest.ContentType = "application/x-www-form-urlencoded";
				
				// HTTP Content-Length header
				httpRequest.ContentLength = postData.Length;

				// Get the request stream.
				Stream dataStream = httpRequest.GetRequestStream();
				// Write the data to the request stream.
				dataStream.Write(postData, 0, postData.Length);

				// Handle response.
				HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
				StreamReader reader = new StreamReader(httpResponse.GetResponseStream());

				// response data
				response = reader.ReadToEnd();

				// Closing objects
				dataStream.Close();
				reader.Close();
				httpResponse.Close();

				m_nvpData.Response=response;
				status = "OK";
				
			} catch (Exception ex) {
				LogException("Exception: " + ex.ToString());
				status = "KO";
			}
			return status;
		}
		#endregion


		#region Pay API
		[ComVisible(true)]
		public void SetPaymentType(string type)
		{
			switch (type.ToLower()) {
				case "simple":
					m_PayRequest.PaymentType=PaymentType.Simple;
					break;
				case "parallel":
					m_PayRequest.PaymentType=PaymentType.Parallel;
					break;
				case "chained":
					m_PayRequest.PaymentType=PaymentType.Chained;
					break;
			}
			m_PayRequest.PaymentType=PaymentType.Simple;
		}
		[ComVisible(true)]
		public void SetSenderEmail(string email)
		{
			m_PayRequest.SenderEmail=email;
		}
		[ComVisible(true)]
		public void SetCurrency(string currency)
		{
			m_Currency=currency;
		}
		[ComVisible(true)]
		public void SetLanguage(string language)
		{
			m_Language=language;
		}
		[ComVisible(true)]
		public void SetCancelUrl(string url)
		{
			m_PayRequest.CancelUrl=url;
		}
		[ComVisible(true)]
		public void SetReturnUrl(string url)
		{
			m_PayRequest.ReturnUrl=url;
		}
		[ComVisible(true)]
		public void SetNotificationUrl(string url)
		{
			m_PayRequest.NotificationUrl=url;
		}
		[ComVisible(true)]
		public void SetMemoNote(string memo)
		{
			m_PayRequest.Memo=memo;
		}
		[ComVisible(true)]
		public void AddReceiver( string email, string amount )
		{
			bool primaryReceiver;
			
			switch ( m_PayRequest.PaymentType ) {
				case PaymentType.Simple:
					if (m_PayRequest.ReceiverList.Count == 0) {
						m_PayRequest.AddReceiver(email, amount);
					}
					break;
					
				case PaymentType.Parallel:
					m_PayRequest.AddReceiver(email, amount);
					break;
					
				case PaymentType.Chained:
					if (m_PayRequest.ReceiverList.Count == 0) {
						primaryReceiver=true;
					} else {
						primaryReceiver=false;
					}
					m_PayRequest.AddReceiver(email, amount, primaryReceiver);
					break;
			}
		}

		[ComVisible(true)]
		public string Pay()
		{
			string status;
			string[] keySplit;
			string[] errorSplit;
			
			m_PayRequest.Currency = m_Currency;
			m_PayRequest.Language = m_Language;

			try {
				if (m_PayRequest.ReceiverList.Count == 0) {
					throw new System.ArgumentException("Receiver list is empty", "PaypalAPI");
				}
				
				status = SendRequest( m_Paypal_Uri[(int)URI.API_PAY],
									Encoding.UTF8.GetBytes(m_PayRequest.PostData()) );
				
				// split paykey, row 4
				keySplit = m_nvpData.Item(4).Split('=');
				//
				if (keySplit[0]=="payKey") {
					// payment created
					m_PayKey = keySplit[1];
				} else {
					// payment error
					errorSplit = m_nvpData.Item(9).Split('=');
					throw new System.ArgumentException("Payment refused:\n"+errorSplit[1], "PaypalAPI");
				}
			} catch (Exception ex) {
				LogException("Exception:\n" + ex.ToString());
				m_PayKey = "";
			}
			m_PayRequest.Clear();
			return m_PayKey;
		}
		#endregion


		#region PaymentDetails API
		[ComVisible(true)]
		public string PaymentDetails(string payKey)
		{
			DetailsRequest detailsRequest = new DetailsRequest();
			detailsRequest.Currency = m_Currency;
			detailsRequest.Language = m_Language;
			detailsRequest.PaymentKey = payKey;

			return SendRequest( m_Paypal_Uri[(int)URI.API_PAYMENTDETAILS],
								Encoding.UTF8.GetBytes(detailsRequest.PostData()) );
		}
		#endregion


		#region Refund API
		[ComVisible(true)]
		public string Refund(string payKey, string transactionId)
		{
			RefundRequest refundRequest = new RefundRequest();
			refundRequest.Currency = m_Currency;
			refundRequest.Language = m_Language;
			refundRequest.PaymentKey = payKey;
			refundRequest.TransactionId = transactionId;

			return SendRequest( m_Paypal_Uri[(int)URI.API_REFUND],
								Encoding.UTF8.GetBytes(refundRequest.PostData()) );
		}
		#endregion


		#region ConvertCurrency API
		[ComVisible(true)]
		public string ConvertCurrency(string amount, string fromCurrency, string toCurrency)
		{
			ConvertCurrencyRequest convertRequest = new ConvertCurrencyRequest();
			convertRequest.Language = m_Language;
			convertRequest.FromCurrency = fromCurrency;
			convertRequest.Amount = amount;
			convertRequest.ToCurrency = toCurrency;

			return SendRequest( m_Paypal_Uri[(int)URI.API_CONVERT],
								Encoding.UTF8.GetBytes(convertRequest.PostData()) );
		}
		#endregion



	}
}