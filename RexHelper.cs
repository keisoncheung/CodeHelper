using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PublicHelper 
{
    public static class RexHelper
    {
        public enum rexType
        {
            /// <summary>
            /// 手机号码 
            /// </summary>
            Phone,
            /// <summary>
            /// 电话号码
            /// </summary>
            Tel,
            /// <summary>
            /// 电子邮箱
            /// </summary>
            Email,
             
        }

        private static string PatternPhone { get { return @"(^(01|1)[3,4,5,8][0-9])\d{8}$"; } }

        /// <summary>
        /// 正则匹配
        /// </summary>
        public static Func<string, rexType, bool> IsMatch = (str, rex) => { return Regex.IsMatch(str, GetPattern(rex)); };

        /// <summary>
        /// 
        /// </summary>
        private static Func<rexType, string> GetPattern = g =>
        {
            string pattern = string.Empty;
            switch (g)
            {
                case rexType.Phone:
                    pattern = PatternPhone;
                    break;
                default:
                    pattern = PatternPhone;
                    break;
            }
            return pattern;
        };
    }
}
