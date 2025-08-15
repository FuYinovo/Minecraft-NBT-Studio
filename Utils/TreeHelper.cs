using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace NBT_Studio.Utils;

public static class TreeHelper
{
    /// <summary>
    /// 查找页面的 ViewModel
    /// </summary>
    /// <param name="dependencyObject">起始控件</param>
    /// <param name="result">结果</param>
    /// <param name="maxTrying">最大尝试搜索父项次数</param>
    /// <typeparam name="T">ViewModel的类型</typeparam>
    /// <returns>是否成功</returns>
    public static bool GetViewModel<T>(DependencyObject dependencyObject, out T result, int maxTrying = 8) where T: class
    {
        var tried = 0;
        var child = dependencyObject;
        while (tried <  maxTrying)
        {
            var found = VisualTreeHelper.GetParent(child);
            if (found is Microsoft.UI.Xaml.Controls.Control { DataContext: T dataContext }) {
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