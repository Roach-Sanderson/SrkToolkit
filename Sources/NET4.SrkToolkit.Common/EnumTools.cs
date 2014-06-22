﻿
#if ASS_DOMAIN
namespace SrkToolkit.Domain.Internals
#else
namespace SrkToolkit
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Resources;
    using System.Globalization;

    /// <summary>
    /// Helps you work with enums.
    /// </summary>
#if ASS_DOMAIN
    internal static class EnumTools
#else
    public static class EnumTools
#endif
    {
        /// <summary>
        /// Gets the description of a enum value from a resource file.
        /// </summary>
        /// <remarks>
        /// The key for the description is [EnumTypeName]_[EnumValueAsString].
        /// </remarks>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="resourceManager">The resource manager generated by the resource file.</param>
        /// <returns>A localized description of the value or the value itself as string.</returns>
        public static string GetDescription<TEnum>(TEnum value, ResourceManager resourceManager)
            where TEnum : struct
        {
            return GetDescription(value, resourceManager, null);
        }

        public static string GetDescription<TEnum>(TEnum value, ResourceManager resourceManager, CultureInfo culture)
            where TEnum : struct
        {
            if (resourceManager == null)
                throw new ArgumentNullException("resourceManager");

            
            Type type = value.GetType();
            string typeName = null;
            string result;
            do
            {
                typeName = typeName == null ? type.Name : (type.Name + "_" + typeName);
                string key = typeName + "_" + value.ToString();
                result = resourceManager.GetString(key, culture);
            }
            while (result == null && (type = type.DeclaringType) != null);
            return result ?? value.ToString();
        }
    }
}
