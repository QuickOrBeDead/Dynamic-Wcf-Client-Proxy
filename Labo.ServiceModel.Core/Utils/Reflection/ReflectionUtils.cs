using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using System.Globalization;
using Labo.ServiceModel.Core.Utils.Conversion;
using Labo.ServiceModel.Core.Utils.Reflection.Exceptions;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member info.</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(MemberInfo memberInfo)
            where T : class
        {
            return GetCustomAttribute<T>(memberInfo, false);
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterInfo">The parameter info.</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(ParameterInfo parameterInfo)
            where T : class
        {
            return GetCustomAttribute<T>(parameterInfo, false);
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameterInfo">The parameter info.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(ParameterInfo parameterInfo, bool inherit)
            where T : class 
        {
            object[] attributes = parameterInfo.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length == 0)
            {
                return null;
            }
            return (T)attributes[0];
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
        public static T GetCustomAttribute<T>(MemberInfo memberInfo, bool inherit)
            where T : class
        {
            object[] attributes = memberInfo.GetCustomAttributes(typeof(T), inherit);
            if (attributes.Length == 0)
            {
                return null;
            }
            return (T)attributes[0];
        }

        /// <summary>
        /// Determines whether [has custom attribute] [the specified member info].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>
        ///   <c>true</c> if [has custom attribute] [the specified member info]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasCustomAttribute<T>(MemberInfo memberInfo, bool inherit)
           where T : class
        {
            return GetCustomAttribute<T>(memberInfo, inherit) != null;
        }

        /// <summary>
        /// Gets the custom attributes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
        public static List<T> GetCustomAttributes<T>(MemberInfo memberInfo, bool inherit)
            where T : class
        {
           object[] attributes = memberInfo.GetCustomAttributes(typeof(T), inherit);
            List<T> result = new List<T>(attributes.Length);
            for (int i = 0; i < attributes.Length; i++)
            {
                result.Add((T)attributes[i]);
            }
            return result;
        }

        /// <summary>
        /// Gets the name of the method by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodByName(Type type, string methodName, IDictionary<string, Type> parameters)
        {
            return GetMethodByName(type, BindingFlags.Instance | BindingFlags.Public, methodName, parameters);
        }

        /// <summary>
        /// Gets the name of the method by.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static MethodInfo GetMethodByName(Type type, BindingFlags bindingFlags, string methodName, IDictionary<string, Type> parameters)
        {
            MethodInfo[] methodInfos = type.GetMethods(bindingFlags);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo methodInfo = methodInfos[i];
                if(methodInfo.Name == methodName)
                {
                    ParameterInfo[] parameterInfos = methodInfo.GetParameters();

                    if (parameterInfos.Length != parameters.Count)
                    {
                        continue;
                    }

                    bool parameterFound = true;
                    foreach (KeyValuePair<string, Type> parameter in parameters)
                    {
                        parameterFound = false;

                        for (int j = 0; j < parameterInfos.Length; j++)
                        {
                            ParameterInfo parameterInfo = parameterInfos[j];
                            if(parameterInfo.Name == parameter.Key && parameterInfo.ParameterType == parameter.Value)
                            {
                                parameterFound = true;
                                break;
                            }
                        }

                        if(!parameterFound)
                        {
                            break;
                        }
                    }

                    if (!parameterFound)
                    {
                        continue;
                    }
                    return methodInfo;
                }
            }
            return null;
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(object @object, string methodName, IDictionary<string, Parameter> parameters)
        {
            return InvokeMethod(@object, methodName, BindingFlags.Instance | BindingFlags.Public, parameters);
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(object @object, string methodName, BindingFlags bindingFlags, IDictionary<string, Parameter> parameters)
        {
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }
            Type objectType = @object.GetType();
            MethodInfo methodInfo = GetMethodByName(objectType, methodName, parameters.ToDictionary(x => x.Key, y => y.Value.Type));
            if (methodInfo == null)
            {
                throw new ReflectionUtilsException(String.Format("'{0}' method of '{1}' type couldn't be found", methodName, objectType.AssemblyQualifiedName));
            }
            return InvokeMethod(methodInfo, @object, BindingFlags.Instance | BindingFlags.Public, parameters);
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="object">The @object.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(MethodInfo methodInfo, object @object, IDictionary<string, Parameter> parameters)
        {
            return InvokeMethod(methodInfo, @object, BindingFlags.Instance | BindingFlags.Public, parameters);
        }

        /// <summary>
        /// Invokes the method.
        /// </summary>
        /// <param name="methodInfo">The method info.</param>
        /// <param name="object">The @object.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object InvokeMethod(MethodInfo methodInfo, object @object, BindingFlags bindingFlags, IDictionary<string, Parameter> parameters)
        {
            if (methodInfo == null)
            {
                throw new ArgumentNullException("methodInfo");
            }
            if (@object == null)
            {
                throw new ArgumentNullException("object");
            }
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            if(parameterInfos.Length != parameters.Count)
            {
                throw new ReflectionUtilsException(String.Format("'{0}' method parameters count doesn't match the specified parameters count", methodInfo));
            }

            object[] arguments = new object[parameters.Count];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                Parameter parameter;
                
                if(!parameters.TryGetValue(parameterInfo.Name, out parameter))
                {
                    throw new ReflectionUtilsException(String.Format("'{0}' parameter doesn't exist in '{1}' method", parameterInfo.Name, methodInfo));                    
                }

                arguments[parameterInfo.Position] = ConversionUtils.ChangeType(parameter.Value, parameter.Type);
            }

            return methodInfo.Invoke(@object, bindingFlags, null, arguments, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the method definition.
        /// </summary>
        /// <param name="object">The @object.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <returns></returns>
        public static Method GetMethodDefinition(object @object, string methodName)
        {
            MethodInfo methodInfo = @object.GetType().GetMethod(methodName);
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            Method method = new Method { Name = methodName };
            Dictionary<Type, Class> classDefinitions = new Dictionary<Type, Class>();

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                Type parameterType = parameterInfo.ParameterType;
                Reflection.Parameter parameter = new Reflection.Parameter { Name = parameterInfo.Name, Type = parameterType, Position = parameterInfo.Position };       

                if (!parameterType.IsValueType && parameterType != typeof(string))
                {
                    Class classDefinition = GetClassDefinition(parameterType, classDefinitions);
                    parameter.Definition = classDefinition;
                    classDefinitions[parameterType] = classDefinition;
                }

                method.Parameters.Add(parameter);
            }

            Type returnType = methodInfo.ReturnType;
            Class returnClass = GetClassDefinition(returnType, classDefinitions);
            method.ReturnValue = new Instance { Type = returnType, Definition = returnClass };
            return method;
        }

        /// <summary>
        /// Gets the class definition.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        public static Class GetClassDefinition(Type classType)
        {
            return GetClassDefinition(classType, new Dictionary<Type, Class>());
        }

        /// <summary>
        /// Gets the class definition.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="classDefinitions">The class definitions.</param>
        /// <returns></returns>
        private static Class GetClassDefinition(Type classType, IDictionary<Type, Class> classDefinitions)
        {
            Class @class;
            if(classDefinitions.TryGetValue(classType, out @class))
            {
                return @class;
            }

            @class = new Class { Type = classType };
            if(classType.IsValueType || classType == typeof(string))
            {
                return @class;
            }
            PropertyInfo[] propertyInfos = classType.GetProperties();
            for (int j = 0; j < propertyInfos.Length; j++)
            {
                PropertyInfo propertyInfo = propertyInfos[j];
                Type propertyType = propertyInfo.PropertyType;
                Property property = new Property { Name = propertyInfo.Name, Type = propertyType };
                if (!propertyType.IsValueType && propertyType != typeof(string))
                {
                    if(classType == propertyType)
                    {
                        property.Definition = @class;
                    }
                    else
                    {
                        property.Definition = GetClassDefinition(propertyType, classDefinitions);                        
                    }
                }
                
                @class.Properties.Add(property);
            }
            classDefinitions[classType] = @class;
            @class.Definition = @class;
            return @class;
        }

        public struct Parameter : IEquatable<Parameter>
        {
            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            /// <value>
            /// The type.
            /// </value>
            public Type Type { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>
            /// The value.
            /// </value>
            public object Value { get; set; }

            /// <summary>
            /// Implements the operator ==.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>
            /// The result of the operator.
            /// </returns>
            public static bool operator ==(Parameter left, Parameter right)
            {
                return left.Type == right.Type && left.Value == right.Value;
            }

            /// <summary>
            /// Implements the operator !=.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            /// <returns>
            /// The result of the operator.
            /// </returns>
            public static bool operator !=(Parameter left, Parameter right)
            {
                return !(left == right);
            }

            /// <summary>
            /// Indicates whether the current object is equal to another object of the same type.
            /// </summary>
            /// <returns>
            /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
            /// </returns>
            /// <param name="other">An object to compare with this object.</param>
            public bool Equals(Parameter other)
            {
                return Equals(other.Type, Type) && Equals(other.Value, Value);
            }

            /// <summary>
            /// Indicates whether this instance and a specified object are equal.
            /// </summary>
            /// <returns>
            /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
            /// </returns>
            /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                return obj is Parameter && Equals((Parameter)obj);
            }

            /// <summary>
            /// Returns the hash code for this instance.
            /// </summary>
            /// <returns>
            /// A 32-bit signed integer that is the hash code for this instance.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                }
            }
        }
    }
}
