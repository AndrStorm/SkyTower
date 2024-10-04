using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;

public static class SaveSystem
{
    private const string BANWORDS_PATH = "banWords.storm";
    
    public static void SaveBanWords(BanWordsData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + BANWORDS_PATH, FileMode.Create);
        
        formatter.Serialize(stream,data);
        stream.Close();
    }
    
    
    public static BanWordsData LoadBanWords()
    {
        if (!File.Exists(Application.persistentDataPath + BANWORDS_PATH))
        {
            return null;
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(Application.persistentDataPath + BANWORDS_PATH, FileMode.Open);
        
        BanWordsData data = formatter.Deserialize(stream) as BanWordsData;
        stream.Close();
        return data;
    }
    
    
#if UNITY_EDITOR

    [MenuItem("Developer/Delete Banwords File")]
    private static void DeleteBanwordsFile()
    {
        File.Delete(Application.persistentDataPath + BANWORDS_PATH);
    }
    
#endif
    
}

[Serializable]
public class BanWordsData
{
    public List<string> banWords;

    public BanWordsData(List<string> words)
    {
        banWords = words;
    }
}



