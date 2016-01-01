using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace trainNewSun
{
    public static class CodeHelper
    {
        /// <summary>
        /// 一键分享微博
        /// </summary>
        /// <returns></returns>
        public static string ShareWeibo()
        {
            StringBuilder sbr = new StringBuilder();
            sbr.Append(@"<a class='bshareDiv' href='http://www.bshare.cn/share'>分享按钮</a><script type='text/javascript' charset='utf-8' src='http://static.bshare.cn/b/buttonLite.js#uuid=&amp;style=3&amp;fs=4&amp;textcolor=#fff&amp;bgcolor=#F60&amp;text=分享到&amp;pophcol=3'></script>");
            return sbr.ToString();
        }

 
    }
}
