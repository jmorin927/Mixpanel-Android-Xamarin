/*--------------------------------------------------------------------------------*/
// Using
/*--------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;

/*--------------------------------------------------------------------------------*/
// Namespace: MixpanelTest.Common
/*--------------------------------------------------------------------------------*/

namespace MixpanelTest.Common
{

	/*--------------------------------------------------------------------------------*/
	// Class: CommonMixPanelCacheData
	/*--------------------------------------------------------------------------------*/

	public class CommonMixPanelCacheData : Dictionary<string, object>
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

		public CommonMixPanelCacheData (
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

}

/*--------------------------------------------------------------------------------*/
