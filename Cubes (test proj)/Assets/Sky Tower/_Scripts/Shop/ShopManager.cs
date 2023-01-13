using System.Collections.Generic;
using UnityEngine;

public class ShopManager : Singleton<ShopManager>
{
    
    
    [SerializeField]private Material cubeLock;
    
    [SerializeField]private CubeScriptable[] cubeScriptableObjects;
    [SerializeField]private List<Transform> scoreLabel;
    [SerializeField]private List<Transform> shopCubes;
    [SerializeField]private Material[] cubesMatUnlock;
    
    [SerializeField]private Vector3 cubeOffset;


    private int[] needScoreToUnlock;
    

    public const string PressToSelect = "Press\n to select";
    public const string AlreadySelected = " ";

    private const string BEST_SCORE = "bestScore";
    


    protected override void Awake()
    {
        base.Awake();
        
        needScoreToUnlock = new int[cubeScriptableObjects.Length];
        
        int j = 0;
        foreach (var cube in cubeScriptableObjects)
        {
            needScoreToUnlock[j++] = cube.scoreToAchieve;
        }

#if UNITY_EDITOR
        
        if (needScoreToUnlock.Length != scoreLabel.Count)
            Debug.Log("needScore.Length != scoreText.Count");
#endif
        
    }
    
    private void Start()
    {

        for (int i = 0; i < needScoreToUnlock.Length; i++)
        {
            shopCubes[i].position = Helper.CanvasToWorld(scoreLabel[i].GetComponent<RectTransform>()) + cubeOffset;

            UnlockCubeByScore(i+1);
            LockCubeBySelection(i+1);

        }      
    }

    

    public int GetLabelRefCubeID(string labelName)
    {
        int cubeID = -1;

        if (labelName == "Score1")
            cubeID = 1;
        else if (labelName == "Score2")
            cubeID = 2;
        else if (labelName == "Score3")
            cubeID = 3;
        else if (labelName == "Score4")
            cubeID = 4;
        else if (labelName == "Score5")
            cubeID = 5;
        else if (labelName == "Score6")
            cubeID = 6;
        else if (labelName == "Score7")
            cubeID = 7;
        else if (labelName == "Score8")
            cubeID = 8;
        else if (labelName == "Score9")
            cubeID = 9;
        else if (labelName == "Score10")
            cubeID = 10;
        else if (labelName == "Score11")
            cubeID = 11;
        else if (labelName == "Score12")
            cubeID = 12;

        return cubeID;
    }
    
    public int GetNeedScoreToUnlock(int cubeIndex)
    {
        return needScoreToUnlock[cubeIndex - 1];
    }
    
    public List<Transform> GetShopCubesList()
    {
        return shopCubes;
    }

    public void UnlockCubeByScore(int cubeIndex)
    {
        if (PlayerPrefs.GetInt(BEST_SCORE) >= needScoreToUnlock[cubeIndex-1])
        {
            UnlockCube(cubeIndex);
            return;
        }
        
        LockByScore(cubeIndex);
    }
    
    public void LockCubeBySelection(int cubeIndex)
    {
        if (PlayerPrefs.GetInt($"Cube{cubeIndex}") == 1) return;

        if (PlayerPrefs.GetInt(BEST_SCORE) >= needScoreToUnlock[cubeIndex-1])
        {
            LockBySelection(cubeIndex);
        }
            
    }
    
    

    private void UnlockCube(int cubeIndex)
    {
        shopCubes[cubeIndex - 1].gameObject.GetComponent<MeshRenderer>().material = cubesMatUnlock[cubeIndex - 1];
        
        scoreLabel[cubeIndex - 1].GetChild(0).gameObject.SetActive(false);
        scoreLabel[cubeIndex - 1].GetChild(1).gameObject.SetActive(false);
    }
    
    private void LockByScore(int cubeIndex)
    {
        PlayerPrefs.SetInt($"Cube{cubeIndex}", 0);
        shopCubes[cubeIndex-1].gameObject.GetComponent<MeshRenderer>().material = cubeLock;
        
        scoreLabel[cubeIndex - 1].GetChild(0).gameObject.SetActive(true);
        scoreLabel[cubeIndex - 1].GetChild(1).gameObject.SetActive(false);
    }
    
    private void LockBySelection(int cubeIndex)
    {
        shopCubes[cubeIndex-1].gameObject.GetComponent<MeshRenderer>().material = cubeLock;
        
        scoreLabel[cubeIndex - 1].GetChild(0).gameObject.SetActive(false);
        scoreLabel[cubeIndex - 1].GetChild(1).gameObject.SetActive(true);
    }



    

    
}
