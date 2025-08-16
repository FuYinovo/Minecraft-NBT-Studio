using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace NBT_Studio.Utils;

public static class TreeHelper
{
    /// <summary>
    /// 从一个起始控件向上查找父控件，若其 DataContext 实现了 [T] 接口或继承了 [T] 类，则返回其 DataContext
    /// </summary>
    /// <param name="dependencyObject">起始控件</param>
    /// <param name="result">结果</param>
    /// <param name="maxTrying">最大尝试搜索父项次数</param>
    /// <typeparam name="T">DataContext 的实现类型</typeparam>
    /// <returns>是否成功</returns>
    public static bool GetSpecificDataContext<T>(DependencyObject dependencyObject, out T result, int maxTrying = 8)
        where T : class
    {
        var tried = 0;
        var child = dependencyObject;
        while (tried < maxTrying)
        {
            var found = VisualTreeHelper.GetParent(child);
            if (found is Microsoft.UI.Xaml.Controls.Control { DataContext: T dataContext })
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
}