using System;
using System.Linq;
using Microsoft.UI.Xaml.Markup;
using NBT_Studio.Attribute;
using NBT_Studio.Utils;

namespace NBT_Studio.Xaml.Markup;

public partial class AdvancedEnumValuesExtension : MarkupExtension
{
    public Type? Type { get; set; }

    protected override object ProvideValue()
    {
        if (Type is null) throw new NullReferenceException("没有传递枚举类型!");
        if (!Type.IsEnum) throw new InvalidCastException("传递类型不是枚举!");

        var values = System.Enum.GetValues(Type).Cast<System.Enum>() // 所有枚举
            .Where(x => !ReflectionHelper.IsEnumAttributeExists<HideAttribute>(x)) // 没有 Hide 特性的枚举
            .OrderBy(GetEnumOrder)
            .ToArray(); // 按 Order 特性排序

        // 将 Enum[] 转换为 {this.Type}[]（与传入类型一致的数组）
        // 避免与接收控件的 SelectedItem 类型不一致，导致初始选择空白
        var typedValues = Array.CreateInstance(Type, values.Length);
        Array.Copy(values, typedValues, values.Length);
        return typedValues;

        static int GetEnumOrder(System.Enum targetEnum)
        {
            return ReflectionHelper.GetEnumAttribute<OrderAttribute>(targetEnum)?.Order ?? int.MaxValue;
        }
    }
}