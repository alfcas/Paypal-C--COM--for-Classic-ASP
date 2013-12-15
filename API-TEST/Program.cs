/*
 * Created by SharpDevelop.
 * User: alfcas
 * Date: 02/11/2013
 * Time: 17:44
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using System.Runtime.InteropServices;
using PaypalAPI;


namespace PaypalAPI
{
	class Program
	{
		
		static AdaptivePayments c_pay = new AdaptivePayments();
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Paypal test API");

			c_pay.APIUserID="s1_1267233049_biz_api1.hotmail.com";
			c_pay.APIPassword="1267233061";
			c_pay.APISignature="AupFS7g3FEMezxo6DzUYh75GAgL.AfIb3IoACaxz8CHNnVmEFvu0BPU1";
			//c_pay.AplicationID="APP-80W284485P519543T";
			//
			c_pay.UseSandbox=true;
			//
			c_pay.SetCancelUrl("http://prenotacom.arescrs.com/ares/test/paypal/listener/cancel.asp");
			c_pay.SetReturnUrl("http://prenotacom.arescrs.com/ares/test/paypal/listener/success.asp");
			c_pay.SetNotificationUrl("http://prenotacom.arescrs.com/ares/test/paypal/listener/ipn.asp");
			//
			c_pay.SetPaymentType("chained");
			c_pay.SetSenderEmail("byr02_1267640704_per@hotmail.com");
			c_pay.AddReceiver("s2_1267478028_biz@hotmail.com","150.00");
			c_pay.AddReceiver("slr03_1267641251_biz@hotmail.com","100.00");
			c_pay.AddReceiver("slr04_1267641479_biz@hotmail.com","50.00");
			//
			c_pay.SetCurrency("USD");
			c_pay.SetLanguage("en_US");
			//
			if ( c_pay.Pay() != "" ) {
				Console.WriteLine("Key:"+c_pay.PayKey );
				Console.WriteLine("Url:"+c_pay.PayUrl );
				Console.WriteLine( c_pay.LastResponse );
			} else {
				Console.WriteLine(c_pay.LastError);
			}


			c_pay.SetCurrency("USD");
			c_pay.SetLanguage("en_US");
			Console.WriteLine( c_pay.PaymentDetails("AP-3LJ13212FT819671F") );
			Console.WriteLine( c_pay.LastResponse );

			c_pay.SetCurrency("USD");
			c_pay.SetLanguage("en_US");
			Console.WriteLine( c_pay.Refund("AP-3LJ13212FT819671F","0123456789") );
			Console.WriteLine( c_pay.LastResponse );

			Console.WriteLine( c_pay.ConvertCurrency("100","EUR","USD") );
			Console.WriteLine( c_pay.LastResponse );

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}