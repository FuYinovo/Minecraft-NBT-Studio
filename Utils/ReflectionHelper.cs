using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace NBT_Studio.Utils;

public static class ReflectionHelper
{
    /// <summary>
    ///     获取一个枚举的描述信息（Description）
    /// </summary>
    /// <param name="targetEnum">枚举</param>
    /// <returns>描述信息</returns>
    public static string? GetEnumDescription(System.Enum targetEnum)
    {
        var field = GetEnumField(targetEnum);
        return field?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    ///     实例化一个类，通过其实现的接口获取某个属性
    /// </summary>
    /// <param name="objectType">类</param>
    /// <param name="propertyName">属性名称</param>
    /// <param name="interfaceName">接口名称</param>
    /// <returns>属性</returns>
    public static object? GetInterfaceProperty(Type objectType, string interfaceName, string propertyName)
    {
        var interfaceType = objectType.GetInterface(interfaceName);
        var property = interfaceType?.GetProperty(propertyName);
        return property?.GetValue(Activator.CreateInstance(objectType));
    }

    /// <summary>
    /// 判断一个枚举是否存在某个特性
    /// </summary>
    /// <param name="targetEnum">枚举</param>
    /// <typeparam name="T">特性类</typeparam>
    /// <returns>存在性</returns>
    public static bool IsEnumAttributeExists<T>(System.Enum targetEnum) where T : System.Attribute
    {
        var field = GetEnumField(targetEnum);
        return field != null && field.IsDefined(typeof(T));
    }

    public static T? GetEnumAttribute<T>(System.Enum targetEnum) where T : System.Attribute
    {
        var field = GetEnumField(targetEnum);
        return field?.GetCustomAttribute<T>();
    }

    private static FieldInfo? GetEnumField(System.Enum targetEnum)
    {
        return targetEnum.GetType().GetField(targetEnum.ToString());
    }
}