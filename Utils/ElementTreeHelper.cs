using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace NBT_Studio.Utils;

public static class ElementTreeHelper
{
    /// <summary>
    ///     从一个起始控件向上查找父控件，若其 DataContext 实现了 [T] 接口或继承了 [T] 类，则返回其 DataContext
    /// </summary>
    /// <param name="dependencyObject">起始控件</param>
    /// <param name="result">结果</param>
    /// <param name="maxTrying">最大尝试搜索父项次数</param>
    /// <typeparam name="T">DataContext 的实现类型</typeparam>
    /// <returns>是否成功</returns>
    public static bool GetSpecificDataContext<T>(DependencyObject dependencyObject, out T result, int maxTrying = 8)
        where T : class
    {
        //  自身即目标 DataContext
        if (dependencyObject is Control { DataContext: T context })
        {
            result = context;
            return true;
        }

        // 向上查找
        var tried = 0;
        var child = dependencyObject;
        while (tried < maxTrying)
        {
            var found = VisualTreeHelper.GetParent(child);
            if (found is Control { DataContext: T dataContext })
            {
                result = dataContext;
                return true;
            }

            child = found;
            tried++;
        }

        result = null!;
        return false;
    }

    public static bool GetChild<T>(DependencyObject parent, out T result) where T : class
    {
        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T target)
            {
                result = target;
                return true;
            }

            if (VisualTreeHelper.GetChildrenCount(child) > 0) GetChild(child, out result);
        }

        result = null!;
        return false;
    }

    public static List<T> GetAllChildren<T>(DependencyObject parent) where T : class
    {
        var got = new List<T>();
        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T target) got.Add(target);

            if (VisualTreeHelper.GetChildrenCount(child) > 0) got.AddRange(GetAllChildren<T>(child));
        }

        return got;
    }
}