/*--------------------------------------------------------------------------------*/
// Using
/*--------------------------------------------------------------------------------*/

using System;
using System.Diagnostics;
using System.Collections.Generic;

using Android.Content;

using Org.Json;

using Com.Mixpanel.Android.Mpmetrics;

/*--------------------------------------------------------------------------------*/
// Namespace: MixpanelTest.Common.Droid
/*--------------------------------------------------------------------------------*/

namespace MixpanelTest.Common.Droid
{
	/*--------------------------------------------------------------------------------*/
	// Class: CommonDroidMixPanelAnalytics
	/*--------------------------------------------------------------------------------*/

	public class CommonDroidMixPanelAnalytics : CommonAbstractMixPanelAnalytics
    {

		/*--------------------------------------------------------------------------------*/
		// Properties
		/*--------------------------------------------------------------------------------*/

		public MixpanelAPI MixPanel 
		{
			get
			{
				lock (this.thisLock)
				{
					return this.mixPanel as MixpanelAPI;
				}
			}
			private set
			{
				lock (this.thisLock)
				{
					this.mixPanel = value as MixpanelAPI;
				}
			}
		}

		/*--------------------------------------------------------------------------------*/

		private Context _context;

		/*--------------------------------------------------------------------------------*/
		// Constructors
		/*--------------------------------------------------------------------------------*/

		public CommonDroidMixPanelAnalytics (
			Context a_context
		)
		{
			this._context = a_context;
		}

		/*--------------------------------------------------------------------------------*/
		// CommonAbstractMixPanelAnalytics Abstract Overrides
		/*--------------------------------------------------------------------------------*/

		public override bool Initialize (
			string a_mixpanelAppToken
		)
		{
			if (!this.initialized)
			{
				try
				{
					this.mixPanel = MixpanelAPI.GetInstance(this._context, a_mixpanelAppToken);

					if (this.MixPanel != null)
					{
						this.initialized = true;

						return true;
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(">> Exception: " + ex.Message);
				}
			}

			return false;
		}

		/*--------------------------------------------------------------------------------*/

		public override void TrackEvent (
			string a_event,
			Dictionary<string, object> a_properties = null
		)
		{
			try
			{
				if ((this.MixPanel != null) &&
				    (!this.cacheEverything))
				{
					var mutableProperties = InjectCommonProperties (a_properties);

					// Track event
					this.MixPanel.Track (a_event, new JSONObject(mutableProperties));
				}
				else
				{
					// Cache event
					this.cachedEvents.Add (
						new CommonMixPanelCacheData (
							MixPanelType.Event,
							a_event,
							a_properties
						)
					);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}
		}

		/*--------------------------------------------------------------------------------*/

		public override void TrackPeopleEvent (
			string a_event,
			MixPanelAction a_action = MixPanelAction.None,
			Dictionary<string, object> a_properties = null
		)
		{
			try
			{
				if ((this.MixPanel != null) &&
				    (!this.cacheEverything))
				{
					var mutableProperties = InjectCommonProperties (a_properties);

					// Track event
					if (a_action == MixPanelAction.Append)
					{
						this.MixPanel.People.Append ("People", new JSONObject(mutableProperties));
					}
					else if (a_action == MixPanelAction.Increment)
					{
						this.MixPanel.People.Increment (a_event, 1);
					}
					else if (a_action == MixPanelAction.Decrement)
					{
						this.MixPanel.People.Increment (a_event, -1);
					}
					else if (a_action == MixPanelAction.Set)
					{
						this.MixPanel.People.Set ("People", new JSONObject(mutableProperties));
					}
					else if (a_action == MixPanelAction.SetOnce)
					{
						this.MixPanel.People.SetOnce ("People", new JSONObject(mutableProperties));
					}
					else if (a_action == MixPanelAction.SetSuperOnce)
					{
						this.MixPanel.RegisterSuperProperties (new JSONObject(a_properties));
						this.MixPanel.People.SetOnce ("People", new JSONObject(mutableProperties));
					}
				}
				else
				{
					// Cache event
					this.cachedEvents.Add (
						new CommonMixPanelCacheData (
							MixPanelType.People,
							a_event,
							a_properties,
							a_action
						)
					);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}
		}

		/*--------------------------------------------------------------------------------*/

		public override void ProcessMixPanelCache ()
		{
			try
			{
				if (this.MixPanel != null)
				{
					lock (this.thisLock)
					{
						if ((!this.processingCache) &&
						    (this.cachedEvents.Count > 0))
						{
							this.processingCache = true;

							foreach (CommonMixPanelCacheData cacheData in this.cachedEvents)
							{
								Dictionary<string, object> mutableProperties = InjectCommonProperties (cacheData.properties);

								if (cacheData.type == MixPanelType.Event)
								{
									this.MixPanel.Track (cacheData.key, new JSONObject(mutableProperties));
								}
								else if (cacheData.type == MixPanelType.Identify)
								{
									this.MixPanel.Identify (CommonAbstractMixPanelAnalytics.MixPanel_Identify);
								}
								else if (cacheData.type == MixPanelType.People)
								{
									if (cacheData.action == MixPanelAction.Append)
									{
										this.MixPanel.People.Append ("People", new JSONObject(mutableProperties));
									}
									else if (cacheData.action == MixPanelAction.Increment)
									{
										this.MixPanel.People.Increment (cacheData.key, 1);
									}
									else if (cacheData.action == MixPanelAction.Decrement)
									{
										this.MixPanel.People.Increment (cacheData.key, -1);
									}
									else if (cacheData.action == MixPanelAction.Set)
									{
										this.MixPanel.People.Set ("People", new JSONObject(mutableProperties));
									}
									else if (cacheData.action == MixPanelAction.SetOnce)
									{
										this.MixPanel.People.SetOnce ("People", new JSONObject(mutableProperties));
									}
									else if (cacheData.action == MixPanelAction.SetSuperOnce)
									{
										this.MixPanel.RegisterSuperProperties (new JSONObject(cacheData.properties));
										this.MixPanel.People.SetOnce ("People", new JSONObject(mutableProperties));
									}
								}
							} 

							// Clear cached events
							this.cachedEvents.Clear ();

							// Clear flags
							this.processingCache = false;
							this.cacheEverything = false;
						}

						// Force flush
						this.MixPanel.Flush ();
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}
		}

		/*--------------------------------------------------------------------------------*/

		public override Dictionary<string, object> InjectCommonProperties (
			Dictionary<string, object> a_properties
		)
		{
			try
			{
				if (a_properties == null)
				{
					a_properties = new Dictionary<string, object>();
				}

				a_properties["$time"] = this.GetCurrentUTCTimeSeconds();
				a_properties["$distinct_id"] = CommonAbstractMixPanelAnalytics.MixPanel_DistinctId;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}

			return a_properties;		
		}

		/*--------------------------------------------------------------------------------*/
		// ICommonAnalytics Overrides
		/*--------------------------------------------------------------------------------*/

		public override void Send (
			string a_key, 
			string a_type = null,
			string a_action = null,
			Dictionary<string, object> a_value = null
		)
		{
			try
			{
				// People Event
				if (String.Compare(a_type, MixPanelType.People.ToString(), false) == 0)
				{
					MixPanelAction action = MixPanelAction.None;
					if (!String.IsNullOrEmpty(a_action))
					{
						action = (MixPanelAction)Enum.Parse(typeof(MixPanelAction), a_action); 
					}

					// Track it
					this.TrackPeopleEvent (
						a_key,
						action,
						a_value
					);
				}
				// Event
				else if (String.Compare(a_type, MixPanelType.Event.ToString(), false) == 0)
				{
					// Track it
					this.TrackEvent (
						a_key,
						a_value
					);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}
		}

		/*--------------------------------------------------------------------------------*/
		// Private Methods
		/*--------------------------------------------------------------------------------*/

		private Int64 GetCurrentUTCTimeSeconds()
		{
			return GetUTCTimeSeconds(DateTime.Now);
		}

		/*--------------------------------------------------------------------------------*/

		private Int64 GetUTCTimeSeconds(
			DateTime a_dateTime
		)
		{
			Int64 unixTimeStamp = 0;

			try
			{
				DateTime zuluTime = a_dateTime.ToUniversalTime();
				DateTime unixEpoch = new DateTime(1970, 1, 1);
				unixTimeStamp = (Int64)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}

			return unixTimeStamp;
		}

		/*--------------------------------------------------------------------------------*/

	}

	/*--------------------------------------------------------------------------------*/

}

/*--------------------------------------------------------------------------------*/
