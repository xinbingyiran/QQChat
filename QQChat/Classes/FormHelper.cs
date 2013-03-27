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
        [DllImport("user32.dll")] //闪烁窗体
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        //闪烁窗体参数
        private const UInt32 FLASHW_STOP = 0;//停止闪动.系统将窗体恢复到初始状态.
        private const UInt32 FLASHW_CAPTION = 1;//闪动窗体的标题.
        private const UInt32 FLASHW_TRAY = 2;//闪动任务栏按钮
        private const UInt32 FLASHW_ALL = 3;//闪动窗体标题和任务栏按钮
        private const UInt32 FLASHW_TIMER = 4;//连续不停的闪动,直到此参数被设置为:FLASHW_STOP
        private const UInt32 FLASHW_TIMERNOFG = 12;//连续不停的闪动,直到窗体用户被激活.通常用法将参数设置为: FLASHW_ALL | FLASHW_TIMERNOFG

        /// <summary>
        /// 窗口闪烁
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        public static void FlashWindow(IntPtr handle)
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(System.Runtime.InteropServices.Marshal.SizeOf(fInfo));
            fInfo.hwnd = handle;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;//这里是闪动窗标题和任务栏按钮,直到用户激活窗体
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);
        }
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
