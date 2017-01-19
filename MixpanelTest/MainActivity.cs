/*------------------------------------------------------------------------------------*/
// Using
/*------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

using Android.App;
using Android.Widget;
using Android.OS;

using MixpanelTest.Common;
using MixpanelTest.Common.Droid;

/*------------------------------------------------------------------------------------*/
// Namespace: MixpanelTest
/*------------------------------------------------------------------------------------*/

namespace MixpanelTest
{

	/*------------------------------------------------------------------------------------*/
	// Class: MainActivity
	/*------------------------------------------------------------------------------------*/

	[Activity(Label = "MixpanelTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{

		/*------------------------------------------------------------------------------------*/
		// Properties
		/*------------------------------------------------------------------------------------*/

		private int _count = 1;
		private CommonDroidMixPanelAnalytics _mixpanelAnalytics;

		/*------------------------------------------------------------------------------------*/
		// Activity Implementation
		/*------------------------------------------------------------------------------------*/

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);
			button.Click += (sender, e) => 
			{ 
				button.Text = string.Format("{0} clicks!", this._count++); 

				this._mixpanelAnalytics.Send(
					"Button Pressed", 
					MixPanelType.Event.ToString(),
					MixPanelAction.Increment.ToString()
				);
			};

			// Initialize Mixpanel
			this.initMixpanelAnalytics ();
		}

		/*------------------------------------------------------------------------------------*/
		// Private Methods
		/*------------------------------------------------------------------------------------*/

		private void initMixpanelAnalytics ()
		{
			try
			{
				// Instantiate mixpanel analytics
				this._mixpanelAnalytics = new CommonDroidMixPanelAnalytics(this);
				this._mixpanelAnalytics.Initialize(CommonAbstractMixPanelAnalytics.MixPanel_API_Key);

				// Force caching
				this._mixpanelAnalytics.ForceCaching ();

				// Set mixpanel user properties
				Dictionary<string, object> userProperties = new Dictionary<string, object> {
					{ "$email", "email@someplace.com" },
					{ "$phone", "14081234567" },
					{ "$name", "Jonathan Morin" }
				};

				// Set people user properties
				this._mixpanelAnalytics.TrackPeopleEvent (
					null,
					MixPanelAction.Set,
					userProperties
				);

				// Set mixpanel identify
				this._mixpanelAnalytics.Identify ();

				// Now process the cache
				this._mixpanelAnalytics.ProcessMixPanelCache ();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(">> Exception: {0}", ex.Message);
			}
		}

		/*------------------------------------------------------------------------------------*/

	}

	/*------------------------------------------------------------------------------------*/

}

/*------------------------------------------------------------------------------------*/
