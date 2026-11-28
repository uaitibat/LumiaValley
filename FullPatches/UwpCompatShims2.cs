// Segundo lote de shims de perfil .NET 5 -> UWP (reflexao/IO/XML)
// para o experimento fullport. Nao sao exercitados em runtime no Lumia.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Xml;

namespace System.Reflection
{
    public static class UwpReflectionExtensions
    {
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsSubclassOf(this Type type, Type other)
        {
            return other.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        // UWP ja fornece GetField/GetSetMethod/GetGetMethod via
        // TypeExtensions/PropertyInfoExtensions (nao redefinir: ambiguous CS0121).
        public static Assembly GetAssembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }

        public static object InvokeMember(this Type type, string name, BindingFlags invokeAttr, object binder,
            object target, object[] args)
        {
            MethodInfo method = type.GetRuntimeMethod(name, new Type[0]);
            return method == null ? null : method.Invoke(target, args);
        }

        public static Delegate CreateDelegate3(this MethodInfo method, Type type, object target)
        {
            return method.CreateDelegate(type, target);
        }
    }
}

namespace System
{
    public static class UwpDateTimeExtensions
    {
        public static string ToLongTimeString(this DateTime value)
        {
            return value.ToString("HH:mm:ss");
        }
    }
}

namespace System.Runtime.InteropServices
{
    public sealed class OSPlatform
    {
        public static readonly OSPlatform Linux = new OSPlatform();
        public static readonly OSPlatform OSX = new OSPlatform();
        public static readonly OSPlatform Windows = new OSPlatform();
    }

    public static class RuntimeInformation
    {
        public static bool IsOSPlatform(OSPlatform platform) { return false; }
    }
}

