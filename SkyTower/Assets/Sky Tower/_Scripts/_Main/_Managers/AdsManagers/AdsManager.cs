using UnityEngine;


public class AdsManager : Singleton<AdsManager>
{
    
    [SerializeField]private int _restartsToShowFullScreenAds = 3;
    
    [SerializeField]private bool _isRandomRangeRestarts = true;
    [SerializeField]private int _maxRestartsToShowFSAds = 6;
    [SerializeField]private int _minRestartsToShowFSAds = 4;

    [SerializeField] [Range(0, 100)] private int _chanceToShowBanner = 50;

    
    private const string RESTARTS_NUMBER = "RestartsNumber";

    private int _currentRestartsToShowAds;
    
    public int RestartsToShowAds
    {
        get
        {
            if (_currentRestartsToShowAds != 0) 
                return _currentRestartsToShowAds;
            
            _currentRestartsToShowAds = GetRestartsNumber();
            
            return _currentRestartsToShowAds == 0 
                ? GenerateNewRestartsToShowNumber() 
                : _currentRestartsToShowAds;
        }
    }


    private IAdsGiver _currentAdsGiver;
    
    
    private void Start()
    {
        InitAds();
    }

    private void InitAds()
    {
        InitYandexMediationAds();
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
    
    
    private void InitYandexMediationAds()
    {
        _currentAdsGiver = gameObject.AddComponent<YandexMediationAdsManager>();
    }


    private int GetRestartsNumber()
    {
        return PlayerPrefs.GetInt(RESTARTS_NUMBER);
    }

    private void SaveRestartNumber(int number)
    {
        PlayerPrefs.SetInt(RESTARTS_NUMBER, number);
    }

    private int GenerateNewRestartsToShowNumber()
    {
        if (_isRandomRangeRestarts)
        {
            _currentRestartsToShowAds = Random.Range(_minRestartsToShowFSAds,
                _maxRestartsToShowFSAds);
        }
        else
        {
            _currentRestartsToShowAds = _restartsToShowFullScreenAds;
        }
        
        Helper.Log($"Generate restarts number - {_currentRestartsToShowAds}");
        SaveRestartNumber(_currentRestartsToShowAds);
        return _currentRestartsToShowAds;
    }
    
    
    public void ShowFullScreenAd()
    {
        Helper.Log($"Show FS");
        GenerateNewRestartsToShowNumber();
        _currentAdsGiver.ShowFullScreenAd();
    }

    public void ShowBannerAd()
    {
        if (Random.Range(1, 100) <= _chanceToShowBanner)
        {
            return;
        }
        
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


