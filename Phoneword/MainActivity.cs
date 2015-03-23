using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Core;
using Android.Views.InputMethods;
using System.Collections.Generic;

namespace Phoneword
{
	[Activity (Label = "Phoneword", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		List<string> _phoneNumbers = new List<string>();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			var callButton = FindViewById<Button> (Resource.Id.CallButton);
			var translateButton = FindViewById<Button> (Resource.Id.TranslateButton);
			var phoneNumberText = FindViewById<EditText> (Resource.Id.PhoneNumberText);
			var callHistoryButton = FindViewById<Button> (Resource.Id.CallHistoryButton);

			callButton.Enabled = false;

			string translatedNumber = "";
			translateButton.Click += delegate {
				var inputMethodManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
				inputMethodManager.HideSoftInputFromWindow(phoneNumberText.WindowToken, 0);

				translatedNumber = PhonewordTranslator.ToNumber (phoneNumberText.Text);

				if(string.IsNullOrEmpty(translatedNumber))
				{
					callButton.Text = "Call";
					callButton.Enabled = false;
				}
				else
				{
					callButton.Text = "Call " + translatedNumber;
					callButton.Enabled = true;
				}

			};

			callButton.Click += delegate {
				var callDialog = new AlertDialog.Builder(this);

				callDialog.SetMessage("Call" + translatedNumber + "?");

				callDialog.SetNeutralButton("Call", delegate {
					_phoneNumbers.Add(translatedNumber);
					callHistoryButton.Enabled = true;
					var callIntent = new Intent(Intent.ActionCall);
					callIntent.SetData(Android.Net.Uri.Parse("tel:" + translatedNumber));
					StartActivity(callIntent);
				});

				callDialog.SetNegativeButton("Cancel", delegate {
					// Nothing to do
				});

				callDialog.Show();
			};

			callHistoryButton.Click += delegate {
				var intent = new Intent(this, typeof(CallHistoryActivity));
				intent.PutStringArrayListExtra("phone_numbers", _phoneNumbers);
				StartActivity(intent);
			};
		}
	}
}