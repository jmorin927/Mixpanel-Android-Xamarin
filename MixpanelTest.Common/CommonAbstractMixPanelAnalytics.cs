/*--------------------------------------------------------------------------------*/
// Using
/*--------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

/*--------------------------------------------------------------------------------*/
// Namespace: MixpanelTest.Common
/*--------------------------------------------------------------------------------*/

namespace MixpanelTest.Common
{

	/*--------------------------------------------------------------------------------*/
	// Namespace Types
	/*--------------------------------------------------------------------------------*/

	public enum MixPanelType
	{
		People = 0,
		Event,
		Identify,
	};

	/*--------------------------------------------------------------------------------*/

	public enum MixPanelAction
	{
		None = 0,

		Set,
		SetOnce,
		SetSuperOnce,
		Increment,
		Decrement,
		Append,
	};

	/*--------------------------------------------------------------------------------*/
	// Abstract Class: SClientCommonAbstractMixPanelAnalytics
	/*--------------------------------------------------------------------------------*/

	public abstract class CommonAbstractMixPanelAnalytics : ICommonAnalytics
    {

		/*--------------------------------------------------------------------------------*/
		// Constants
		/*--------------------------------------------------------------------------------*/

		public static readonly string MixPanel_API_Key = "this_needs_to_be_set_by_you_to_make_things_work";

		/*--------------------------------------------------------------------------------*/

		public static readonly string MixPanel_Identify = "MixpanelTest";
		public static readonly string MixPanel_DistinctId = "MixPanel" + MixPanel_Identify;

		/*--------------------------------------------------------------------------------*/
		// Properties
		/*--------------------------------------------------------------------------------*/

		protected object thisLock { get; set; } = new object();
		protected object mixPanel { get; set; } = null;

		protected bool initialized { get; set; }
		protected bool cacheEverything { get; set; }
		protected bool processingCache  { get; set; }

		protected Task mixPanelDelayedTask { get; set; } = null;
		protected List<CommonMixPanelCacheData> cachedEvents { get; private set; } = new List<CommonMixPanelCacheData> ();

		/*--------------------------------------------------------------------------------*/
		// Abstract Methods
		/*--------------------------------------------------------------------------------*/

		public abstract bool Initialize (
			string a_mixpanelAppToken
		);

		/*--------------------------------------------------------------------------------*/

		public abstract void TrackEvent (
			string a_event,
			Dictionary<string, object> a_properties = null
		);

		/*--------------------------------------------------------------------------------*/

		public abstract void TrackPeopleEvent (
			string a_event,
			MixPanelAction a_action = MixPanelAction.None,
			Dictionary<string, object> a_properties = null
		);

		/*--------------------------------------------------------------------------------*/

		public abstract void ProcessMixPanelCache ();

		/*--------------------------------------------------------------------------------*/

		public abstract Dictionary<string, object> InjectCommonProperties (
			Dictionary<string, object> a_properties
		);

		/*--------------------------------------------------------------------------------*/
		// Virtual Methods for ICommonAnalytics
		/*--------------------------------------------------------------------------------*/

		public virtual void Send (
			string a_key, 
			string a_type,
			string a_action,
			Dictionary<string, object> a_value
		)
		{
			throw new NotImplementedException ();
		}

		/*--------------------------------------------------------------------------------*/

		public virtual void TrackException (
			string key, 
			string message
		)
		{
			throw new NotImplementedException ();
		}

		/*--------------------------------------------------------------------------------*/
		// Public Methods
		/*--------------------------------------------------------------------------------*/

		public void Identify ()
		{
			try
			{
				lock (this.thisLock)
				{
					// Figure out index to insert at
					int index = 0;
					int identifyIndex = 0;
					int removeIndex = -1;
					if (this.cachedEvents.Count > 0)
					{
						foreach (CommonMixPanelCacheData cacheData in this.cachedEvents)
						{
							if (cacheData.type == MixPanelType.Identify)
							{
								removeIndex = index;
							}

							index++;
						}
					}

					if (removeIndex != -1)
					{
						this.cachedEvents.RemoveAt(removeIndex);
					}

					// Always cache identify
					this.cachedEvents.Insert (
						identifyIndex,
						new CommonMixPanelCacheData (
							MixPanelType.Identify,
							null
						)
					);

					this.cacheEverything = true;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(">> Exception: {0}", ex.Message);
			}
		}

		/*--------------------------------------------------------------------------------*/

		public void ForceCaching ()
		{
			this.cacheEverything = true;
		}

		/*--------------------------------------------------------------------------------*/

	}

	/*--------------------------------------------------------------------------------*/

}

/*--------------------------------------------------------------------------------*/
