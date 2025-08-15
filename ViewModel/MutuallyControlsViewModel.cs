using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NBT_Studio.Interface;

namespace NBT_Studio.ViewModel;

[SuppressMessage("CommunityToolkit.Mvvm.SourceGenerators.ObservablePropertyGenerator",
    "MVVMTK0045:Using [ObservableProperty] on fields is not AOT compatible for WinRT")]
public class MutuallyControlsViewModel

{
    private readonly Dictionary<string, object> _groupToValue = new();
    private readonly Dictionary<string, List<IMutuallyControlsBehavior>> _registeredBehaviors = new();

    /// <summary>设置组值 </summary>
    public void SetSelectedValue(string groupName, object value)
    {
        _groupToValue[groupName] = value;
        UpdateMembers(groupName);
    }

    /// <summary>获取组值 </summary>
    public object GetSelectedValue(string groupName)
    {
        return _groupToValue[groupName];
    }

    /// <summary>注册</summary>
    public void Register(IMutuallyControlsBehavior controlsBehavior)
    {
        if (_registeredBehaviors.TryGetValue(controlsBehavior.GroupName, out var behaviors))
            behaviors.Add(controlsBehavior);
        else
            _registeredBehaviors[controlsBehavior.GroupName] = [controlsBehavior];
    }

    /// <summary>取消注册</summary>
    public void Unregister(IMutuallyControlsBehavior controlsBehavior)
    {
        if (_registeredBehaviors.TryGetValue(controlsBehavior.GroupName, out var behaviors))
            behaviors.Remove(controlsBehavior);
    }

    /// <summary>更新组成员的状态</summary>
    private void UpdateMembers(string groupName)
    {
        if (!_registeredBehaviors.TryGetValue(groupName, out var behaviors)) return;
        foreach (var behavior in behaviors) behavior.UpdateState();
    }
}