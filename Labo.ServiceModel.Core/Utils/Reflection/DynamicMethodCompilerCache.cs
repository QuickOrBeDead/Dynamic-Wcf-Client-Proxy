using System;
using System.Collections.Generic;
using System.Reflection;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public static class DynamicMethodCompilerCache
    {
        private static readonly Dictionary<ConstructorInfo, InstantiateObjectHandler> s_CreateInstanceCache = new Dictionary<ConstructorInfo, InstantiateObjectHandler>();
        private static readonly Dictionary<Type, Dictionary<PropertyInfo, PropertyAccessItem>> s_PropertyAccessItemCache = new Dictionary<Type, Dictionary<PropertyInfo, PropertyAccessItem>>();

        public static object CreateInstance(Type type)
        {
            ConstructorInfo constructorInfo = type.GetConstructor(Constants.CONSTRUCTOR_INFO_BINDING_FLAGS, null, Type.EmptyTypes, null);

            InstantiateObjectHandler createHandler;
            if (!s_CreateInstanceCache.TryGetValue(constructorInfo, out createHandler))
            {
                lock (s_CreateInstanceCache)
                {
                    createHandler = DynamicMethodCompiler.CreateInstantiateObjectHandler(type, constructorInfo);
                    s_CreateInstanceCache.Add(constructorInfo, createHandler);
                }
            }

            return createHandler.Invoke();
        }

        public static T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        public static void SetPropertyValue(object @object, string propertyName, object value)
        {
            Type objectType = @object.GetType();
            PropertyInfo propertyInfo = objectType.GetProperty(propertyName, Constants.PROPERTY_INFO_BINDING_FLAGS);

            SetPropertyValue(@object, objectType, propertyInfo, value);
        }

        public static void SetPropertyValue(object @object, Type objectType, PropertyInfo propertyInfo, object value)
        {
            PropertyAccessItem propertyAccessItem = GetPropertyAccessItem(objectType, propertyInfo);
            propertyAccessItem.Setter.Invoke(@object, value);
        }

        public static object GetPropertyValue(object @object, string propertyName)
        {
            Type objectType = @object.GetType();
            PropertyInfo propertyInfo = objectType.GetProperty(propertyName, Constants.PROPERTY_INFO_BINDING_FLAGS);

            return GetPropertyValue(@object, propertyInfo, objectType);
        }

        public static object GetPropertyValue(object @object, PropertyInfo propertyInfo)
        {
            Type objectType = @object.GetType();
            return GetPropertyValue(@object, propertyInfo, objectType);
        }

        private static object GetPropertyValue(object @object, PropertyInfo propertyInfo, Type objectType)
        {
            PropertyAccessItem propertyAccessItem = GetPropertyAccessItem(objectType, propertyInfo);
            return propertyAccessItem.Getter.Invoke(@object);
        }

        private static PropertyAccessItem GetPropertyAccessItem(Type objectType, PropertyInfo propertyInfo)
        {
            PropertyAccessItem propertyAccessItem;
            Dictionary<PropertyInfo, PropertyAccessItem> propertyAccessors;
            if (!s_PropertyAccessItemCache.TryGetValue(objectType, out propertyAccessors))
            {
                lock (s_PropertyAccessItemCache)
                {
                    propertyAccessItem = CreatePropertyAccessItem(objectType, propertyInfo);

                    propertyAccessors = new Dictionary<PropertyInfo, PropertyAccessItem>();
                    propertyAccessors.Add(propertyInfo, propertyAccessItem);

                    s_PropertyAccessItemCache.Add(objectType, propertyAccessors);
                }
            }
            else
            {
                if (!propertyAccessors.TryGetValue(propertyInfo, out propertyAccessItem))
                {
                    lock (propertyAccessors)
                    {
                        propertyAccessItem = CreatePropertyAccessItem(objectType, propertyInfo);

                        propertyAccessors.Add(propertyInfo, propertyAccessItem);
                    }
                }
            }
            return propertyAccessItem;
        }

        private static PropertyAccessItem CreatePropertyAccessItem(Type objectType, PropertyInfo propertyInfo)
        {
            PropertyAccessItem propertyAccessItem = new PropertyAccessItem();
            if (propertyInfo.CanRead)
            {
                propertyAccessItem.Getter = DynamicMethodCompiler.CreateGetHandler(objectType, propertyInfo);
            }
            if (propertyInfo.CanWrite)
            {
                propertyAccessItem.Setter = DynamicMethodCompiler.CreateSetHandler(objectType, propertyInfo);
            }
            return propertyAccessItem;
        }
    }
}
