using System;

namespace Labo.ServiceModel.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        /// Determines whether [is number type] (long, int, short or byte).
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <returns>
        ///   <c>true</c> if [is number type] [the specified @object]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumberType(object @object)
        {
            return IsNumberType(GetType(@object));
        }

        /// <summary>
        /// Determines whether [is numeric type] (long, int, short, byte, float, double or decimal).
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <returns>
        ///   <c>true</c> if [is numeric type] [the specified @object]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumericType(object @object)
        {
            return IsNumericType(GetType(@object));
        }

        /// <summary>
        /// Determines whether [is number type] (long, int, short or byte).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="includeNullables"> </param>
        /// <returns>
        ///   <c>true</c> if [is number type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumberType(Type type, bool includeNullables = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (includeNullables && IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }

            return type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(byte);
        }

        /// <summary>
        /// Determines whether [is numeric type] (long, int, short, byte, float, double or decimal).
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="includeNullables"> </param>
        /// <returns>
        ///   <c>true</c> if [is numeric type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumericType(Type type, bool includeNullables = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (includeNullables && IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }

            return type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(byte) ||
                   type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        /// <summary>
        /// Determines whether [is floating point number type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="includeNullables"> </param>
        /// <returns>
        ///   <c>true</c> if [is floating point number type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFloatingPointNumberType(Type type, bool includeNullables = false)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (includeNullables && IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }

            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <returns></returns>
        public static Type GetType(object @object)
        {
            if (@object == null)
            {
                return typeof(object);
            }
            return @object.GetType();
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object">The object.</param>
        /// <returns></returns>
        public static Type GetType<T>(T @object)
        {
            return typeof(T);
        }

        /// <summary>
        /// Gets the default value of the object.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <returns></returns>
        public static object GetDefaultValue(object @object)
        {
            Type type = GetType(@object);
            return GetDefaultValueOfType(type);
        }

        /// <summary>
        /// Gets the default value of the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static object GetDefaultValueOfType(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        /// <summary>
        /// Determines whether the specified type is nullable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type is nullable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullable(Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }
    }
}
