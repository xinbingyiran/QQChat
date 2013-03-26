using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageDeal
{
    public interface IMessageDeal
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        string IName { get; }
        /// <summary>
        /// 获取相应的菜单项，格式为[显示内容-传递内容]
        /// </summary>
        Dictionary<string,string> Menus { get; }
        /// <summary>
        /// 处理消息，并返回处理结果
        /// </summary>
        /// <param name="message">要处理的消息</param>
        /// <param name="isDealed">是否完成处理，如完成处理</param>
        /// <returns></returns>
        string DealMessage(string message, out bool isDealed);
        /// <summary>
        /// 菜单处理程序
        /// </summary>
        /// <param name="menuName">菜单名称[传递内容]</param>
        /// <returns></returns>
        bool OnMenu(string menuName);
    }
}
