using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace Inhuman
{
    public class IOS
    {
        //============================================================================================================================================//
        [DllImport("__Internal")]
        private static extern void _Popup(string title, string message, string buttonText);

        [DllImport("__Internal")]
        private static extern void _PopupYesNo(string title, string message);

        [DllImport("__Internal")]
        private static extern void _ComposeEmail(string to, string subject, string body);

        [DllImport("__Internal")]
        private static extern bool _IsMusicPlaying();

        //============================================================================================================================================//
        public static void Popup(string title, string message, string buttonText)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                _Popup(title, message, buttonText);
        }

        //============================================================================================================================================//
        public static void PopupYesNo(string title, string message)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                _PopupYesNo(title, message);
        }

        //============================================================================================================================================//
        public static void ComposeEmail(string to, string subject, string body)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                _ComposeEmail(to, subject, body);
        }

        //============================================================================================================================================//
        public static bool IsMusicPlaying()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return _IsMusicPlaying();
            }
            else
            {
                return false;
            }
        }
    }
}