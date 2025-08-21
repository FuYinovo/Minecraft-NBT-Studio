using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using CommunityToolkit.Mvvm.Messaging;
using NBT_Studio.Attribute;
using NBT_Studio.Enum;
using NBT_Studio.Interface;
using NBT_Studio.Message;
using NBT_Studio.Model;
using NBT_Studio.Utils;

namespace NBT_Studio.Service;

public sealed class SettingsManager
{
    private static readonly SettingsManager Instance = new();

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private readonly string _filePath;
    private readonly Dictionary<string, ISetting> _settings = new();

    private SettingsManager()
    {
        _filePath = AssetHelper.GetFilePathFromUri(AssetHelper.GetConfigUri());
        InitSettingsEnums();
        LoadSettings();
        NotifySettings();
    }

    /// <summary>
    /// 获取 <see cref="SettingsManager"/> 的唯一实例
    /// </summary>
    public static SettingsManager GetInstance() => Instance;


    /// <summary>
    /// 获取词条值
    /// </summary>
    /// <example>
    /// <code>
    /// bool x = GetValue&lt;bool>(BooleanSettings.QuickCreate)
    /// Sort y = GetValue&lt;Sort>(EnumSettings.Sort)
    /// </code>
    /// </example>
    /// <param name="settingEnum">词条</param>
    /// <typeparam name="T">预期类型</typeparam>
    /// <exception cref="NullReferenceException">词条不存在</exception>
    /// <exception cref="InvalidOperationException">预期类型错误</exception>
    public T GetValue<T>(System.Enum settingEnum) where T : IConvertible
    {
        if (_settings.TryGetValue(settingEnum.ToString(), out var value))
            return value.Attribute.ValueType == typeof(T)
                ? (T)value.Value
                : throw new InvalidOperationException(
                    $"尝试获取[{settingEnum}]的值并转为[{typeof(T)}]，但是词条类型为[{value.Attribute.ValueType}]!");
        throw new NullReferenceException($"尝试获取[{settingEnum}]，但是不存在该词条!");
    }

    /// <summary>
    /// 设置词条值
    /// </summary>
    /// <example>
    /// <code>
    /// SetValue(BooleanSettings.QuickCreate, ture);
    /// SetValue(EnumSettings.Theme, Theme.Dark);
    /// </code>
    /// </example>
    /// <param name="settingEnum">词条</param>
    /// <param name="newValue">新的值</param>
    /// <exception cref="NullReferenceException">词条不存在</exception>
    /// <exception cref="InvalidOperationException">类型错误</exception>
    public void SetValue<TEnum, TValue>(TEnum settingEnum, TValue newValue)
        where TValue : IConvertible
        where TEnum : System.Enum
    {
        if (!_settings.TryGetValue(settingEnum.ToString(), out var value))
            throw new NullReferenceException($"尝试设置[{settingEnum}]，但是不存在该词条!");

        if (typeof(TValue) != value.Attribute.ValueType)
            throw new InvalidOperationException(
                $"尝试将[{settingEnum}]设置为[{newValue}]，但其类型[{typeof(TValue)}]与词条类型[{value.Attribute.ValueType}]不匹配!");

        value.Value = newValue;
        WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(newValue.GetType(), newValue));
        ApplySettings();
    }

    /// <summary>
    /// 从枚举初始化词条
    /// </summary>
    /// <remarks>
    /// 详情见 <see cref="InitSettingsEnums"/>
    /// </remarks>
    private void InitSettingsFromEnums<TEnums, TValue>()
        where TEnums : System.Enum
        where TValue : IConvertible
    {
        foreach (var field in typeof(TEnums).GetFields())
        {
            var attr = field.GetCustomAttribute<SettingAttribute>();
            if (attr is null) continue;

            _settings[field.Name] = new Setting<TValue>(attr);
        }
    }

    /// <summary>
    /// 从配置文件加载词条值
    /// </summary>
    private void LoadSettings()
    {
        try
        {
            var jsonText = File.ReadAllText(_filePath);
            var jsonSettings = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonText, _jsonOptions);
            if (jsonSettings is null)
            {
                ResetSettings();
                return;
            }

            foreach (var (key, value) in jsonSettings)
            {
                var jsonElement = (JsonElement)value;
                _settings[key].Value = jsonElement.ValueKind switch
                {
                    JsonValueKind.String => ParseJsonString(key, jsonElement.GetString()!),
                    JsonValueKind.Number => jsonElement.GetInt32(),
                    JsonValueKind.False => false,
                    JsonValueKind.True => true,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
        catch (Exception)
        {
            ResetSettings();
        }

        return;

        object ParseJsonString(string key, string str)
        {
            // 「&;」开头字符串解析为枚举；其他正常解析为字符串
            return str.StartsWith("&;")
                ? System.Enum.Parse(_settings[key].Attribute.ValueType, str[2..], true)
                : str;
        }
    }

    /// <summary>
    /// 将词条值保存到配置文件
    /// </summary>
    private void ApplySettings()
    {
        var jsonText = _settings
            .ToDictionary(
                x => x.Key, // 枚举字符串以「&;」开头
                x => x.Value.Attribute.ValueType.IsEnum ? $"&;{x.Value.Value}" : x.Value.Value);
        var jsonSettings = JsonSerializer.Serialize(jsonText, _jsonOptions);
        File.WriteAllText(_filePath, jsonSettings);
    }

    /// <summary>
    /// 重置所有词条
    /// </summary>
    private void ResetSettings()
    {
        foreach (var setting in _settings.Values) setting.Reset();
    }

    /// <summary>
    /// 通知更新
    /// </summary>
    private void NotifySettings()
    {
        foreach (var setting in _settings.Values)
            WeakReferenceMessenger.Default.Send(new SettingsChangedMessage(setting.Attribute.ValueType,
                setting.Value));
    }

    private void InitSettingsEnums()
    {
        InitSettingsFromEnums<BooleanSettings, bool>();
        InitSettingsFromEnums<EnumSettings, System.Enum>();
    }
}