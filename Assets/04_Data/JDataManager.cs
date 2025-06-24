using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class JDataPath
{
    // 프로젝트 폴더 경로 Application.dataPath = Assets폴더임
    public static string dataPath = $"{Application.dataPath}/04_Data/";

    // Appdata/Locallow/회사이름/프로젝트명 
    public static string persistentPath = $"{Application.persistentDataPath}/";
}

public class JDataManager : MonoBehaviour
{
    #region SINGLETON
    public static JDataManager Instance { get; private set; }

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
    public Dictionary<string, TextAsset> JsonData = new Dictionary<string, TextAsset>();

    [Header("Json Dictionary")]
    public Dictionary<string, ItemData>    ItemData    { get; private set; } = new Dictionary<string, ItemData>();

    #endregion





    #region MONOBEHAVIOUR
    private void Awake()
    {
        if(SingletonInitialize() == false)
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
        var jsonDataOperationHandle = Addressables.LoadAssetsAsync<TextAsset>("GameData" /*어드레서블 라벨 이름*/, json => JsonData[json.name] = json);
        jsonDataOperationHandle.WaitForCompletion(); // 완료 안기다리면 종종 터짐
    }

    private void InitializeDictionary()
    {
        // 개별 json에서 로드
        ItemData    = LoadJsonToDictionary<string, ItemData>   ("ItemData",    m => m.StringKey);
    }

    private Dictionary<TKey, TValue> LoadJsonToDictionary<TKey, TValue>(string fileName, Func<TValue, TKey> keySelector)
    {
        List<TValue> list = JsonConvert.DeserializeObject<List<TValue>>(JsonData[fileName].text);

        return list.ToDictionary(keySelector);
    }
    #endregion
}
