using System.Collections;
using UnityEngine;


public class RSReviewHandler : IReviewHandler
{
    //https://apps.rustore.ru/app/com.AndrStormGames.SkyTower
    //private const string _gameStoreLink = "https://apps.rustore.ru/app/com.AndrStormGames.SkyTower";
    
    public IEnumerator MakeReview()
    {
        Application.OpenURL(@"market://details?id=" + Application.identifier);
        return null;
    }
}
