using System;
using System.Reflection;
using UnityEngine;

namespace Guinea.Core
{
    public static class Validator
    {
        public class Employee
        {
            // [StringLength]
            public string name;
            public int age;
            public string address;
        }

        public static bool IsValid(Type type, string fieldName, string value = null)
        {
            FieldInfo info = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#nullable enable
            StringLengthAttribute? attribute = info.GetCustomAttribute(typeof(StringLengthAttribute), false) as StringLengthAttribute;
#nullable disable
            if (attribute != null)
            {
                Commons.Logger.Log($"ErrorMessage: {String.Format(attribute.ErrorMessage, info.Name)}");
            }
            return true;
        }
    }

}