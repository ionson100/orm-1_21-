using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM_1_21_.Utils
{
    internal class Check
    {
        public static T NotNull<T>(T value, string parameterName,Action action=null) where T : class
        {
            if (value == null)
            {
                if (action != null)
                {
                    action.Invoke();
                }
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        public static string NotEmpty(string value, string parameterName, Action action = null)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (action != null)
                {
                    action.Invoke();
                }
                throw new ArgumentException($"{parameterName}  ArgumentIsNull");
            }

            return value;
        }
    }
}
