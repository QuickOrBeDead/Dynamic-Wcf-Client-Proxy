using System;
using System.ComponentModel;
using System.Globalization;
using System.Security;
using System.Threading;
using Labo.ServiceModel.Core.Utils.Conversion.Exceptions;

namespace Labo.ServiceModel.Core.Utils.Conversion
{
    public sealed class ConversionUtils
    {
        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value)
        {
            return (T)ChangeType(value, typeof(T), Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, T defaultValue)
        {
            return ChangeType(value, defaultValue, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, T defaultValue, CultureInfo culture)
        {
            object changed = ChangeType(value, typeof(T), culture);
            if (changed == null)
            {
                return defaultValue;
            }
            return (T)changed;
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static T ChangeType<T>(object value, CultureInfo culture)
        {
            return (T)ChangeType(value, typeof(T), culture);
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type)
        {
            return ChangeType(value, type, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Changes the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static object ChangeType(object value, Type type, CultureInfo culture)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            try
            {
                if (value == null)
                {
                    return null;
                }
                if (type == typeof(string))
                {
                    return Convert.ToString(value, culture);
                }
                Type valueType = value.GetType();
                if ((type.IsClass || type.IsInterface) && type.IsAssignableFrom(valueType))
                {
                    return value;
                }
                if (type.IsEnum)
                {
                    return EnumUtils.Parse(type, value.ToString(), true);
                }
                if ((type == valueType) || (type == typeof(object)))
                {
                    return value;
                }
                if ((type == typeof(bool)) || (type == typeof(bool?)))
                {
                    if (valueType == typeof(string))
                    {
                        if (value.Equals("1") || value.Equals("on") || value.Equals("yes"))
                        {
                            return true;
                        }
                        if (value.Equals("0") || value.Equals("no"))
                        {
                            return false;
                        }
                        if (value.Equals(string.Empty))
                        {
                            return (type == typeof(bool?)) ? new bool?() : false;
                        }
                    }
                    else if (valueType == typeof(int))
                    {
                        return value.Equals(1);
                    }
                }

                TypeConverter typeConverter = TypeDescriptor.GetConverter(type);
                if (typeConverter != null && typeConverter.CanConvertFrom(valueType))
                {
                    return typeConverter.ConvertFrom(null, culture, value);
                }

                TypeConverter valueConverter = TypeDescriptor.GetConverter(valueType);
                if (valueConverter != null && valueConverter.CanConvertTo(type))
                {
                    return valueConverter.ConvertTo(value, type);
                }

                if (TypeUtils.IsNullable(type))
                {
                    type = Nullable.GetUnderlyingType(type);
                    if (valueConverter != null && valueConverter.CanConvertTo(type))
                    {
                        return valueConverter.ConvertTo(value, type);
                    }
                }

                return TryConvertByIConvertible(value, type, culture);
            }
            catch (Exception exception)
            {
                throw GetConversionException(value, type, culture, exception);
            }
        }

        private static object TryConvertByIConvertible(object value, Type type, CultureInfo culture)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible != null)
            {
                if (type == typeof(Boolean))
                {
                    return convertible.ToBoolean(culture);
                }
                if (type == typeof(Byte))
                {
                    return convertible.ToByte(culture);
                }
                if (type == typeof(Char))
                {
                    return convertible.ToChar(culture);
                }
                if (type == typeof(DateTime))
                {
                    return convertible.ToDateTime(culture);
                }
                if (type == typeof(Decimal))
                {
                    return convertible.ToDecimal(culture);
                }
                if (type == typeof(Double))
                {
                    return convertible.ToDouble(culture);
                }
                if (type == typeof(Int16))
                {
                    return convertible.ToInt16(culture);
                }
                if (type == typeof(Int32))
                {
                    return convertible.ToInt32(culture);
                }
                if (type == typeof(Int64))
                {
                    return convertible.ToInt64(culture);
                }
                if (type == typeof(SByte))
                {
                    return convertible.ToSByte(culture);
                }
                if (type == typeof(Single))
                {
                    return convertible.ToSingle(culture);
                }
                if (type == typeof(UInt16))
                {
                    return convertible.ToUInt16(culture);
                }
                if (type == typeof(UInt32))
                {
                    return convertible.ToUInt32(culture);
                }
                if (type == typeof(UInt64))
                {
                    return convertible.ToUInt64(culture);
                }
            }

            throw GetConversionException(value, type, culture);
        }

        private static ConversionException GetConversionException(object value, Type type, CultureInfo culture, Exception exception = null)
        {
            const string errorMessage = "An Error Occurred While Changing Value";
            ConversionException conversionEx = exception == null ? new ConversionException(errorMessage) : new ConversionException(errorMessage, exception);
            conversionEx.Data.Add("VALUE", Convert.ToString(value, CultureInfo.CurrentCulture));
            conversionEx.Data.Add("TYPE", type.ToString());
            conversionEx.Data.Add("CULTURE", culture.Name);
            return conversionEx;
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T Parse<T>(string value)
        {
            return (T)Parse(typeof(T), value, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static T Parse<T>(string value, CultureInfo culture)
        {
            return (T)Parse(typeof(T), value, culture);
        }

        /// <summary>
        /// Parses the value with the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object Parse(Type type, string value)
        {
            return Parse(type, value, Thread.CurrentThread.CurrentCulture);
        }

        /// <summary>
        /// Parses the value with the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        public static object Parse(Type type, string value, CultureInfo culture)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            try
            {
                return TypeDescriptor.GetConverter(type).ConvertFromString(null, culture, value);
            }
            catch (Exception exception)
            {
                ConversionException conversionEx = new ConversionException("An Error Occurred While Casting DB Value", exception);
                conversionEx.Data.Add("VALUE", value);
                conversionEx.Data.Add("TYPE", type.ToString());
                conversionEx.Data.Add("CULTURE", culture.Name);
                throw conversionEx;
            }
        }

        /// <summary>
        /// Registers the type converter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TC">The type of the C.</typeparam>
        // CA2135 violation - the LinkDemand should be removed, and the method marked [SecurityCritical] instead
        // [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
        [SecurityCritical]
        public static void RegisterTypeConverter<T, TC>() where TC : TypeConverter
        {
            TypeDescriptor.AddAttributes(typeof(T), new Attribute[] { new TypeConverterAttribute(typeof(TC)) });
        }
    }
}
