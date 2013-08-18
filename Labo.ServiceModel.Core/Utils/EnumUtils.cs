using System;
using System.Collections.Generic;
using System.Globalization;
using Labo.ServiceModel.Core.Utils.Exceptions;

namespace Labo.ServiceModel.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Gets the names and values.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <typeparam name="TUnderlyingType">The type of the underlying type.</typeparam>
        /// <returns></returns>
        public static IDictionary<string, TUnderlyingType> GetNamesAndValues<TEnum, TUnderlyingType>()
            where TEnum : struct
            where TUnderlyingType : struct
        {
            return GetNamesAndValues<TUnderlyingType>(typeof(TEnum));
        }

        /// <summary>
        /// Gets the names and values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDictionary<string, ulong> GetNamesAndValues<T>() where T : struct
        {
            return GetNamesAndValues<ulong>(typeof(T));
        }

        /// <summary>
        /// Gets the names and values.
        /// </summary>
        /// <typeparam name="TUnderlyingType">The type of the underlying type.</typeparam>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns></returns>
        public static IDictionary<string, TUnderlyingType> GetNamesAndValues<TUnderlyingType>(Type enumType)
        {
            //Contract.Requires<ArgumentNullException>(enumType != null, "enumType");
            //Contract.Requires<InvalidOperationException>(enumType.IsEnum, "Underlying Type Must Be Enum");

            Type conversionType = typeof(TUnderlyingType);
            try
            {
                Array values = Enum.GetValues(enumType);
                string[] names = Enum.GetNames(enumType);
                IDictionary<string, TUnderlyingType> dictionary = new Dictionary<string, TUnderlyingType>();
                for (int i = 0; i < values.Length; i++)
                {
                    dictionary.Add(names[i], (TUnderlyingType)Convert.ChangeType(values.GetValue(i), conversionType, CultureInfo.InvariantCulture));
                }
                return dictionary;
            }
            catch (Exception exception)
            {
                EnumUtilsException coreEx = new EnumUtilsException("An Error Occurred While Getting Names and Values of Enum Type", exception);
                coreEx.Data.Add("TYPE", enumType.ToString());
                coreEx.Data.Add("UNDERLYINGTYPE", conversionType.ToString());
                throw coreEx;
            }
        }

        /// <summary>
        /// Parses the specified enum member name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumMemberName">Name of the enum member.</param>
        /// <returns></returns>
        public static T Parse<T>(string enumMemberName) where T : struct
        {
            return Parse<T>(enumMemberName, false);
        }

        /// <summary>
        /// Parses the specified enum member name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumMemberName">Name of the enum member.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static T Parse<T>(string enumMemberName, bool ignoreCase) where T : struct
        {
            return (T)Parse(typeof(T), enumMemberName, ignoreCase);
        }

        /// <summary>
        /// Parses the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="enumMemberName">Name of the enum member.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns></returns>
        public static object Parse(Type type, string enumMemberName, bool ignoreCase)
        {
            //Contract.Requires<ArgumentNullException>(type != null, "type");
            //Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(enumMemberName), "enumMemberName");
            //Contract.Requires<ArgumentException>(type.IsEnum, "type must be enum type");

            try
            {
                return Enum.Parse(type, enumMemberName, ignoreCase);
            }
            catch (Exception exception)
            {
                EnumUtilsException coreEx = new EnumUtilsException("An Error Occurred While Parsing Enum Value", exception);
                coreEx.Data.Add("VALUE", enumMemberName);
                coreEx.Data.Add("TYPE", type.ToString());
                coreEx.Data.Add("IGNORECASE", ignoreCase.ToString(CultureInfo.InvariantCulture));
                throw coreEx;
            }
        }

        /// <summary>
        /// Tries to parse the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumMemberName">Name of the enum member.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParse<T>(string enumMemberName, out T value) where T : struct
        {
            return TryParse(enumMemberName, false, out value);
        }

        /// <summary>
        /// Tries to parse the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumMemberName">Name of the enum member.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool TryParse<T>(string enumMemberName, bool ignoreCase, out T value) where T : struct
        {
            Type enumType = typeof(T);
            try
            {
                string[] names = Enum.GetNames(enumType);
                for (int i = 0; i < names.Length; i++)
                {
                    string str = names[i];
                    bool @equals = ignoreCase ? str.Equals(enumMemberName, StringComparison.InvariantCultureIgnoreCase) : str.Equals(enumMemberName);
                    if (@equals)
                    {
                        value = Parse<T>(enumMemberName, ignoreCase);
                        return true;
                    }
                }
                value = default(T);
                return false;
            }
            catch (Exception exception)
            {
                EnumUtilsException coreEx = new EnumUtilsException("An Error Occurred While Parsing Enum Value", exception);
                coreEx.Data.Add("VALUE", enumMemberName);
                coreEx.Data.Add("TYPE", enumType.ToString());
                throw coreEx;
            }
        }
    }
}