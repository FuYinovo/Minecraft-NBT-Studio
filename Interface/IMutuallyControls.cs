using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NBT_Studio.Interface;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public interface IMutuallyControls

{
    Dictionary<string, object> GroupToValue { get; }
    Dictionary<string, List<IMutuallyControlsBehavior>> RegisteredBehaviors { get; }

    /// <summary>设置组值 </summary>
    void SetSelectedValue(string groupName, object value)
    {
        GroupToValue[groupName] = value;
        UpdateMembers(groupName);
    }

    /// <summary>获取组值 </summary>
    object GetSelectedValue(string groupName)
    {
        return GroupToValue[groupName];
    }

    /// <summary>注册</summary>
    void Register(IMutuallyControlsBehavior controlsBehavior)
    {
        if (RegisteredBehaviors.TryGetValue(controlsBehavior.GroupName, out var behaviors))
            behaviors.Add(controlsBehavior);
        else
            RegisteredBehaviors[controlsBehavior.GroupName] = [controlsBehavior];
    }

    /// <summary>取消注册</summary>
    void Unregister(IMutuallyControlsBehavior controlsBehavior)
    {
        if (RegisteredBehaviors.TryGetValue(controlsBehavior.GroupName, out var behaviors))
            behaviors.Remove(controlsBehavior);
    }

    /// <summary>更新组成员的状态</summary>
    private void UpdateMembers(string groupName)
    {
        if (!RegisteredBehaviors.TryGetValue(groupName, out var behaviors)) return;
        foreach (var behavior in behaviors) behavior.UpdateState();
    }
}