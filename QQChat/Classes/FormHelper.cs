using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QQChat.Classes
{
    public class FormHelper
    {
        private static Color[] rtextColor = {
                                                Color.FromArgb(0,0,128),                                                
                                                Color.FromArgb(64,0,0),
                                                Color.FromArgb(0,128,255),
                                                Color.FromArgb(128,128,0),
                                                Color.FromArgb(128,0,128),
                                                Color.FromArgb(255,128,128)
                                            };
        private static int rtextColorIndex = 0;
        public static Color PickColor()
        {
            lock(rtextColor)
            {
                return rtextColor[(rtextColorIndex++) % rtextColor.Length];
            }
        }
    }
}
