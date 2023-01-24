#if UNITY_EDITOR

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameAnalyticsSDK;
using LootLocker;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class BuildManager : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{

    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.AndrStormGames.SkyTower");
    }

    public void OnPreprocessBuild(BuildReport report)
    {

#if AG_BUILD

    #if RU_VERSION
        Debug.Log("App Gallery ru");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.AndrStormGames.SkyTower.huawei.ru");
    #elif EN_VERSION
        Debug.Log("App Gallery en");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.AndrStormGames.SkyTower.huawei");
    #else
        Debug.Log("App Gallery default");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.AndrStormGames.SkyTower.huawei");
    #endif
        
        
#elif RS_BUILD

        Debug.Log("Ru Store ");
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.AndrStormGames.SkyTower");
        
#endif
    }
    
}


[InitializeOnLoad]
public class DefininitionsManager : Editor
{
    
    private static readonly string [] DefineKeywords = new string[] {
        "UNITY_POST_PROCESSING_STACK_V2",
        "TEST_BUILD",
        "AG_BUILD",
        //"RS_BUILD",
        "RU_VERSION",
        //"EN_VERSION",
    };
    
    
    static DefininitionsManager ()
    {
        SetUpDefinitions();
        LocaleDefininitionsManager.SetUp();
        LootLockerDefenitionsManager.SetUp();
    }

    private static void SetUpDefinitions()
    {
        List<string> allDefines = new List<string>();
        allDefines.AddRange(DefineKeywords.Except(allDefines));

        string definitions = string.Join(";", allDefines.ToArray());
        Debug.Log(definitions);
        
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, definitions);
    }
    
    //CustomEditor
    /*
     private static string[] oldDefines;
    private static void AddDefineSymbols ()
    {
        List<string> allDefines = new List<string>();
        allDefines.AddRange(DefineKeywords.Except(allDefines));
        PlayerSettings.SetScriptingDefineSymbolsForGroup(
            EditorUserBuildSettings.selectedBuildTargetGroup,
            string.Join(";", allDefines.ToArray()));
        
        Debug.Log("defines " + PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android));
    }

    private void OnEnable()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            Debug.Log("Defines Check");
            oldDefines = DefineKeywords; 
            if (check.changed)
            {
                AddDefineSymbols();
                Debug.Log("Defines Changed");
            }
            else
            {
                Debug.Log("Defines Checked");
            }
        }
    }
    */
    
    
    
    
    
    private static class LocaleDefininitionsManager 
    {
    
        public static void SetUp()
        {
            int i = -1;
            var specificSelectorsList = new List<int>();
            foreach (var selector in LocalizationSettings.StartupLocaleSelectors)
            {
                i++;
                if (selector.GetType() == typeof(SpecificLocaleSelector))
                {
                    specificSelectorsList.Add(i);
                }
            }

#if RU_VERSION
            LocaleIdentifier identifier = new LocaleIdentifier(new CultureInfo("ru"));
#else
        LocaleIdentifier identifier = new LocaleIdentifier(new CultureInfo("en"));      
#endif
        
            SpecificLocaleSelector newSelector = new SpecificLocaleSelector
            {
                LocaleId = identifier
            };
        
            LocalizationSettings.StartupLocaleSelectors.Add(newSelector);

            foreach (var selectorID in specificSelectorsList)
            {
                LocalizationSettings.StartupLocaleSelectors.RemoveAt(selectorID);
            }
        }

    }
    
    private static class LootLockerDefenitionsManager
    {
        private const string DEVELOP_MOD_API_KEY = "dev_cc3723a2ab584e9bb391faaf3ef1445d";//dev_cc3723a2ab584e9bb391faaf3ef1445d
        private const string LIVE_MOD_API_KEY = "prod_3d88b362e4ea45a68c3251216574fcad"; //prod_3d88b362e4ea45a68c3251216574fcad


        public static void SetUp()
        {
            var config = LootLockerConfig.Get();
            config.game_version = PlayerSettings.bundleVersion;
        
#if TEST_BUILD
        config.apiKey = DEVELOP_MOD_API_KEY;
        config.developmentMode = true;
#else
            config.apiKey = LIVE_MOD_API_KEY;
            config.developmentMode = false;
#endif

        }

    }
    
}
#endif