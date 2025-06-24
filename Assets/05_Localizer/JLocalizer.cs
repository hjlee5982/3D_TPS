using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class JLocalizeDataPath
{
    public static string dataPath = $"{Application.dataPath}/05_Localizer/";
}

public class JLocalizer : MonoBehaviour
{
    #region SINGLETON
    public static JLocalizer Instance { get; private set; }

    private bool SingletonInitialize(bool dontDestroy = true)
    {
        if (Instance == null)
        {
            Instance = this;

            if (dontDestroy == true)
            {
                DontDestroyOnLoad(gameObject);
            }
            return true;
        }
        else
        {
            Destroy(gameObject);
            return false;
        }
    }
    #endregion





    #region VARIABLES
    [Header("Addressable")]
    private Dictionary<string, TextAsset> _jsonData = new Dictionary<string, TextAsset>();

    [Header("Json Dictionary")]
    private Dictionary<string, Dictionary<string, string>> _localizer = new Dictionary<string, Dictionary<string, string>>();

    [Header("현재 언어")]
    private string _currentLanguage = "KR";
    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        if (SingletonInitialize() == false)
        {
            return;
        }

        InitializeAddressable();
        InitializeDictionary();
    }
    #endregion





    #region FUNCTIONS
    private void InitializeAddressable()
    {
        var jsonDataOperationHandle = Addressables.LoadAssetsAsync<TextAsset>("LocalizeData", json => _jsonData[json.name] = json);
        jsonDataOperationHandle.WaitForCompletion();
    }

    private void InitializeDictionary()
    {
        Dictionary<string, LocalizeData> localizer = LoadJsonToDictionary<string, LocalizeData>("LocalizeData", m => m.ID);

        _localizer = localizer.ToDictionary(kvp => kvp.Key, kvp =>
        {
            var dict = new Dictionary<string, string>();
            var fields = typeof(LocalizeData).GetFields();

            foreach(FieldInfo field in fields)
            {
                if(field.Name == "ID")
                {
                    continue;
                }

                String value = field.GetValue(kvp.Value)?.ToString() ?? "";
                dict[field.Name] = value;
            }

            return dict;
        });
    }

    private Dictionary<TKey, TValue> LoadJsonToDictionary<TKey, TValue>(string fileName, Func<TValue, TKey> keySelector)
    {
        List<TValue> list = JsonConvert.DeserializeObject<List<TValue>>(_jsonData[fileName].text);

        return list.ToDictionary(keySelector);
    }

    public void SetCurrentLanguage(string launguage)
    {
        _currentLanguage = launguage;
    }

    public string GetText(string ID)
    {
        return _localizer[ID][_currentLanguage];
    }
    #endregion
}
