using System;

namespace Guinea.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class StringLengthAttribute : Attribute
    {
        public string ErrorMessage { get; private set; }
        public StringLengthAttribute()
        {
            ErrorMessage = "You can't leave field {0} empty";
        }
        public StringLengthAttribute(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}