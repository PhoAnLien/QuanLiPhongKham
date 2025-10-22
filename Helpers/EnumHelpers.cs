using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DoAnLTW.Helpers
{
    public static class EnumHelpers
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            if (enumValue == null) return string.Empty;

            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            ?.GetName() ?? enumValue.ToString();
        }
    }
} 