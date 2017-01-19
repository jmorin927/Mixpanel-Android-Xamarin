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
	// Abstract Class: SClientCommonAbstractMixPanelAnalytics
	/*--------------------------------------------------------------------------------*/

	public abstract class CommonAbstractMixPanelAnalytics : ICommonAnalytics
    {

		/*--------------------------------------------------------------------------------*/
		// Namespace Constants
		/*--------------------------------------------------------------------------------*/

		public static readonly string MixPanel_API_Key = "this_needs_to_be_set_by_you";

		public static readonly string MixPanel_Identify = "MixpanelTest";
		public static readonly string MixPanel_DistinctId = "MixPanel" + MixPanel_Identify;

		/*--------------------------------------------------------------------------------*/
		// Types
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
		// Class: MixPanelCacheData
		/*--------------------------------------------------------------------------------*/

		public class MixPanelCacheData : Dictionary<string, object>
		{

			/*--------------------------------------------------------------------------------*/
			// Properties
			/*--------------------------------------------------------------------------------*/

			public MixPanelType type { get; set; }
			public MixPanelAction action { get; set; }
			public string key { get; set; }
			public Dictionary<string, object> properties { get; set; }

			/*--------------------------------------------------------------------------------*/
			// Constructors
			/*--------------------------------------------------------------------------------*/

			public MixPanelCacheData (
				MixPanelType a_type,
				string a_key,
				Dictionary<string, object> a_properties = null,
				MixPanelAction a_action = MixPanelAction.None
			)
			{
				this.type = a_type;
				this.key = a_key;
				this.properties = a_properties;
				this.action = a_action;
			}

			/*--------------------------------------------------------------------------------*/

		}

		/*--------------------------------------------------------------------------------*/
		// Properties
		/*--------------------------------------------------------------------------------*/

		protected object thisLock { get; set; } = new object();
		protected object mixPanel { get; set; } = null;

		protected bool initialized { get; set; }
		protected bool cacheEverything { get; set; }
		protected bool processingCache  { get; set; }

		protected Task mixPanelDelayedTask { get; set; } = null;
		protected List<MixPanelCacheData> cachedEvents { get; private set; } = new List<MixPanelCacheData> ();

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
		// Virtual Methods
		/*--------------------------------------------------------------------------------*/

		public virtual void Send (
			string a_key, 
			string a_value
		)
		{
		}

		/*--------------------------------------------------------------------------------*/

		public virtual void Send (
			string a_key, 
			Dictionary<string, object> a_value
		)
		{
		}

		/*--------------------------------------------------------------------------------*/

		public virtual void Send (
			string a_key, 
			string a_type,
			Dictionary<string, object> a_value
		)
		{
		}

		/*--------------------------------------------------------------------------------*/

		public virtual void Send (
			string a_key, 
			string a_type,
			string a_action,
			Dictionary<string, object> a_value
		)
		{
		}

		/*--------------------------------------------------------------------------------*/

		public virtual void TrackException (
			string key, 
			string message
		)
		{
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
						foreach (MixPanelCacheData cacheData in this.cachedEvents)
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
						new MixPanelCacheData (
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
