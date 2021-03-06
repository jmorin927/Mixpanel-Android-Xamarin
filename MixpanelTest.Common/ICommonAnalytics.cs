﻿/*--------------------------------------------------------------------------------*/
// Using
/*--------------------------------------------------------------------------------*/

using System.Collections.Generic;

/*--------------------------------------------------------------------------------*/
// Namespace: MixpanelTest.Common
/*--------------------------------------------------------------------------------*/

namespace MixpanelTest.Common
{

	/*--------------------------------------------------------------------------------*/
	// Interface: ICommonAnalytics
	/*--------------------------------------------------------------------------------*/

	public interface ICommonAnalytics
	{

		/*--------------------------------------------------------------------------------*/
		// Methods
		/*--------------------------------------------------------------------------------*/

		void Send (
			string a_key, 
			string a_type,
			string a_action = null,
			Dictionary<string,object> a_value = null
		);

		/*--------------------------------------------------------------------------------*/

		void TrackException (
			string a_key, 
			string a_message
		);

		/*--------------------------------------------------------------------------------*/

	}

	/*--------------------------------------------------------------------------------*/

}

/*--------------------------------------------------------------------------------*/
