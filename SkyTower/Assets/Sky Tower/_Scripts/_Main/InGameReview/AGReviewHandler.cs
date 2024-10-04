using System.Collections;
using UnityEngine;


public class AGReviewHandler : IReviewHandler
{
    //https://appgallery.huawei.com/app/C107614499
    //private const string _gameStoreLink = @"market://details?id=com.AndrStormGames.SkyTower.huawei.ru";

    public IEnumerator MakeReview()
    {
        Application.OpenURL(@"market://details?id=" + Application.identifier);
        return null;
    }
}
