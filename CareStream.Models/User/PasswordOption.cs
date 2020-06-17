﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CareStream.Models.User
{
    public class PasswordOption
    {
        public int Id { get; set; }

        public PasswordOptionType Type { get; set; }
    }

    public enum PasswordOptionType
    {
        [Description("Auto-generate password")]
        AutoGeneratePassword,
        [Description("Let me create the password")]
        ManualCreatedPassword
    }

    public static class EnumExtensionMethods
    {
        public static string GetDescription(this Enum GenericEnum)
        {
            Type genericEnumType = GenericEnum.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember(GenericEnum.ToString());
            if ((memberInfo != null && memberInfo.Length > 0))
            {
                var _Attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Count() > 0))
                {
                    return ((DescriptionAttribute)_Attribs.ElementAt(0)).Description;
                }
            }
            return GenericEnum.ToString();
        }
    }
}
