using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Labo.ServiceModel.Core.Utils.Reflection.Exceptions;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public delegate object GetHandler(object source);
    public delegate void SetHandler(object source, object value);
    public delegate object InstantiateObjectHandler();

    /// <summary>
    /// http://www.codeproject.com/KB/cs/Dynamic_Code_Generation.aspx
    /// </summary>
    internal static class DynamicMethodCompiler
    {
        internal static InstantiateObjectHandler CreateInstantiateStructHandler(Type type)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), null, type, true);

            ILGenerator generator = dynamicMethod.GetILGenerator();
            LocalBuilder tmpLocal = generator.DeclareLocal(type);
            generator.Emit(OpCodes.Ldloca, tmpLocal);
            generator.Emit(OpCodes.Initobj, type);
            generator.Emit(OpCodes.Ldloc, tmpLocal);
            generator.Emit(OpCodes.Box, type);
            generator.Emit(OpCodes.Ret);

            return (InstantiateObjectHandler)dynamicMethod.CreateDelegate(typeof(InstantiateObjectHandler));
        }

        internal static InstantiateObjectHandler CreateInstantiateObjectHandler(Type type, ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
            {
                throw new DynamicMethodCompilerException(string.Format(CultureInfo.CurrentCulture, "The type {0} must declare an empty constructor (the constructor may be private, internal, protected, protected internal, or public).", type));
            }

            DynamicMethod dynamicMethod = new DynamicMethod(".ctor",
                MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard,
                typeof(object), null, type, true);

            ILGenerator generator = dynamicMethod.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            return (InstantiateObjectHandler)dynamicMethod.CreateDelegate(typeof(InstantiateObjectHandler));
        }

        // CreateGetDelegate
        internal static GetHandler CreateGetHandler(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Call, getMethodInfo);
            BoxIfNeeded(getMethodInfo.ReturnType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }

        // CreateGetDelegate
        internal static GetHandler CreateGetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicGet = CreateGetDynamicMethod(type);
            ILGenerator getGenerator = dynamicGet.GetILGenerator();

            getGenerator.Emit(OpCodes.Ldarg_0);
            getGenerator.Emit(OpCodes.Ldfld, fieldInfo);
            BoxIfNeeded(fieldInfo.FieldType, getGenerator);
            getGenerator.Emit(OpCodes.Ret);

            return (GetHandler)dynamicGet.CreateDelegate(typeof(GetHandler));
        }

        // CreateSetDelegate
        internal static SetHandler CreateSetHandler(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(setMethodInfo.GetParameters()[0].ParameterType, setGenerator);
            setGenerator.Emit(OpCodes.Call, setMethodInfo);
            setGenerator.Emit(OpCodes.Ret);

            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }

        // CreateSetDelegate
        internal static SetHandler CreateSetHandler(Type type, FieldInfo fieldInfo)
        {
            DynamicMethod dynamicSet = CreateSetDynamicMethod(type);
            ILGenerator setGenerator = dynamicSet.GetILGenerator();

            setGenerator.Emit(OpCodes.Ldarg_0);
            setGenerator.Emit(OpCodes.Ldarg_1);
            UnboxIfNeeded(fieldInfo.FieldType, setGenerator);
            setGenerator.Emit(OpCodes.Stfld, fieldInfo);
            setGenerator.Emit(OpCodes.Ret);

            return (SetHandler)dynamicSet.CreateDelegate(typeof(SetHandler));
        }

        // CreateGetDynamicMethod
        private static DynamicMethod CreateGetDynamicMethod(Type type)
        {
            return DynamicMethodHelper.CreateDynamicMethod("DynamicGet", type, typeof(object), new [] { typeof(object) });
        }

        // CreateSetDynamicMethod
        private static DynamicMethod CreateSetDynamicMethod(Type type)
        {
            return DynamicMethodHelper.CreateDynamicMethod("DynamicSet", type, typeof(void), new [] { typeof(object), typeof(object) });
        }

        // BoxIfNeeded
        private static void BoxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Box, type);
            }
        }

        // UnboxIfNeeded
        private static void UnboxIfNeeded(Type type, ILGenerator generator)
        {
            if (type.IsValueType)
            {
                generator.Emit(OpCodes.Unbox_Any, type);
            }
        }
    }
}
