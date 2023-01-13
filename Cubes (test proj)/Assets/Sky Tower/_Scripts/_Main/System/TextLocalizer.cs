using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;


public class TextLocalizer : Singleton<TextLocalizer>
{

    public static readonly string BASE_TABLE = "SkyTowerText";
    
    [SerializeField] private List<LocalePresets> presetsList;


    private StringTable baseTable;
    private bool isTableLoaded;
    
    
    
    
    
    private void OnEnable()
    {
        StartCoroutine(LoadTable());
        LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChange;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChange;
    }

    
    
    
    private IEnumerator LoadTable()
    {
        isTableLoaded = false;
        
        var loadingBaseTableOperation = LocalizationSettings.StringDatabase.GetTableAsync(BASE_TABLE);
        yield return loadingBaseTableOperation;

        if (loadingBaseTableOperation.Status == AsyncOperationStatus.Succeeded)
        {
            baseTable = loadingBaseTableOperation.Result;
        }
        else
        {
            Debug.Log("Loading Table is Failed");
        }

        isTableLoaded = true;
    }

    private IEnumerator SetText(TMP_Text tmpText, string tableName, string entryName, params object[] objs)
    {
        yield return null;
        while (!isTableLoaded)
        {
            yield return Helper.GetWait(0.1f);
        }
        
        tmpText.text = GetLocalizedString(tableName, entryName, objs);
    }
    
    
    
    
    
    public void SetLocalizedText(TMP_Text tmpText, string tableName, string entryName, params object[] objs)
    {
        foreach (var preset in presetsList)
        {
            if (string.CompareOrdinal(preset.localeName,0 ,LocalizationSettings.SelectedLocale.name,0,preset.localeName.Length) == 0)
            {
                tmpText.fontSharedMaterial = preset.fontMaterial;
            }
        }
        StartCoroutine(SetText(tmpText, tableName, entryName, objs));
    }


    
    

    private void OnSelectedLocaleChange(Locale obj)
    {
        StartCoroutine(LoadTable());
    }
    
    private string GetLocalizedString(string tableName, string entryName, params object[] objs)
    {
        var table = GetTableByName(tableName);
        var entry = table.GetEntry(entryName);
        return entry.GetLocalizedString(objs);
    }

    private StringTable GetTableByName(string tableName)
    {
        return baseTable;
    }


    [Serializable]
    public class LocalePresets
    {
        public string localeName;
        public Material fontMaterial;
    }
}
