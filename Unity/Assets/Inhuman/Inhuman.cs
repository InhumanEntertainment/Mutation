using UnityEngine;
using System.Collections;

namespace Inhuman
{
	static public class Utilities
	{
		static public void MessageBox(string title, string message)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                IOS.Popup(title, message, "Ok");
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                // Create Android Plugin //
            }
            
        }		
	}
}