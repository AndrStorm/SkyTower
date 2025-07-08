using UnityEngine;



public class AdsManager : Singleton<AdsManager>
{
    
    [SerializeField]private int _restartsToShowAds = 5;
    public int RestartsToShowAds => _restartsToShowAds;
    
    
    private IAdsGiver _currentAdsGiver;
    
    
    private void Start()
    {
        InitAds();
    }

    private void InitAds()
    {
        //сделать Ру адс
        InitYandexMediationAds();
        //InitVKAds();
        //InitUnityAds();
        
        
        if (_currentAdsGiver == null)
        {
            _currentAdsGiver = new DefaultAdsGiver();
            _currentAdsGiver.InitAds();
        }
        else
        {
            _currentAdsGiver.InitAds();
        }
    }

    private void InitUnityAds()
    {
        _currentAdsGiver = gameObject.AddComponent<UnityAdsManager>();
    }
    
    private void InitVKAds()
    {
        _currentAdsGiver = gameObject.AddComponent<VKAdsManager>();
    }
    
    private void InitYandexMediationAds()
    {
        _currentAdsGiver = gameObject.AddComponent<YandexMediationAdsManager>();
    }


    public void ShowFullScreenAd()
    {
        _currentAdsGiver.ShowFullScreenAd();
    }

    public void ShowBannerAd()
    {
        _currentAdsGiver.ShowBannerAd();
    }

    public void HideBannerAd()
    {
        _currentAdsGiver.HideBannerAd();
    }

    private class DefaultAdsGiver : IAdsGiver
    {
        public void InitAds()
        {
#if UNITY_EDITOR
            Helper.Log("Init Default Ads");
#endif
        }

        public void ShowFullScreenAd()
        {
#if UNITY_EDITOR
            Helper.Log("Show FS Ad");
#endif
        }

        public void ShowBannerAd()
        {
#if UNITY_EDITOR
            Helper.Log("Show Banner Ad");
#endif
        }

        public void HideBannerAd()
        {
#if UNITY_EDITOR
            Helper.Log("Hide Banner Ad");
#endif
        }
    }
}


