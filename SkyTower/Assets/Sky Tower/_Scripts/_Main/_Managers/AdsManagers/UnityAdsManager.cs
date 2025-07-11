using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : MonoBehaviour, IUnityAdsInitializationListener,
    IUnityAdsLoadListener, IUnityAdsShowListener, IAdsGiver
{
    
    [SerializeField] private string _androidGameId = "4618933";
    [SerializeField] private string _iOSGameId = "4618932";
    [SerializeField] private bool _testMode = false;
    
    [SerializeField] private BannerPosition _bannerPosition = BannerPosition.TOP_CENTER;

    [SerializeField] private string _interstitial_Android = "Interstitial_Android";
    [SerializeField] private string _interstitial_iOS = "Interstitial_iOS";
    [SerializeField] private string _banner_Android = "Banner_Android";
    [SerializeField] private string _banner_iOS = "Banner_iOS";
    [SerializeField] private string _rewarded_Android = "Rewarded_Android";
    [SerializeField] private string _rewarded_iOS = "Rewarded_iOS";
    
    /*[SerializeField] private int _restartsToShowAds = 3;
    public int RestartsToShowAds => _restartsToShowAds;*/

    private UnityAdsManager _instance;
    private string _gameId;
    

    
    private void Start()
    {
        //InitAds();
    }
    
    public void InitAds()
    {
        Helper.Log("Init Unity Ads");
        _instance = this;
        _gameId = IsiOS() ? _iOSGameId : _androidGameId;
        
        SetUpPersonolizedAds();
        
#if UNITY_EDITOR
        //error with gameId
        //_testMode = true;
#endif
        
        Advertisement.Initialize(_gameId, _testMode, _instance);
        Advertisement.Banner.SetPosition(_bannerPosition);
    }

    private void SetUpPersonolizedAds()
    {
#if GP_BUILD
        // If the user opts out of personalized ads:
        MetaData userMetaData = new MetaData("user");
        userMetaData.Set("nonbehavioral", "true");
        Advertisement.SetMetaData(userMetaData);
#else
        MetaData userMetaData = new MetaData("user");
        userMetaData.Set("nonbehavioral", "false");
        Advertisement.SetMetaData(userMetaData);
#endif
        
    }

    private bool IsiOS()
    {
        return Application.platform == RuntimePlatform.IPhonePlayer;
    }

    
    
    public void ShowFullScreenAd()
    {
        StartCoroutine(ShowFullScreenAdsCoroutine());
    }
    
    private IEnumerator ShowFullScreenAdsCoroutine()
    {
        if (!Advertisement.isInitialized)
        {
            yield return Helper.GetUnscaledWait(0.5f);
            //Debug.Log("banner is not ready");
        }
        Advertisement.Load(GetInterstitialId(), _instance);
    }
    
    
    public void ShowBannerAd()
    {
        StartCoroutine(ShowBannerCoroutine());
    }
    
    private IEnumerator ShowBannerCoroutine()
    {
        if (!Advertisement.isInitialized)
        {
            yield return Helper.GetUnscaledWait(0.5f);
            //Debug.Log("banner is not ready");
        }
        
        Advertisement.Banner.SetPosition(_bannerPosition);
        Advertisement.Banner.Load(GetBannerId());
        
        if (!Advertisement.Banner.isLoaded)
        {
            yield return Helper.GetUnscaledWait(0.5f);
            //Debug.Log("banner is not loaded"); 
        }
        
        Advertisement.Banner.Show(GetBannerId());
        //Debug.Log("GetBannerId() " + GetBannerId());
    }
    
    private string GetBannerId()
    {
        return IsiOS() ? _banner_iOS : _banner_Android;;
    }
    
    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
    }
    
    
    public string GetInterstitialId()
    {
        return IsiOS() ? _interstitial_iOS : _interstitial_Android;;
    }
    
    public string GetRewardedId()
    {
        return IsiOS() ? _rewarded_iOS : _rewarded_Android;;
    }
    

    
    
    
    private void OnAdsLoaded(string placementId) 
    {
        Advertisement.Show(placementId, _instance);
        //Debug.Log("OnAdsLoaded " + placementId);
    }

    
    public void OnInitializationComplete()
    {
        //Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }


    public void OnUnityAdsAdLoaded(string placementId)
    {
        OnAdsLoaded(placementId);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"OnUnityAdsFailedToLoad placementId {placementId} error {error}");
    }
    

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"OnUnityAdsShowFailure placementId {placementId} error {error}");
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //Debug.Log($"OnUnityAdsShowStart placementId {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //Debug.Log($"OnUnityAdsShowClick placementId {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        //Debug.Log($"OnUnityAdsShowComplete placementId {placementId}");
        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.SKIPPED:
                //Debug.Log($"SKIPPED");
                break;
            
            case UnityAdsShowCompletionState.COMPLETED:
                //Debug.Log($"COMPLETED");
                break;
            
            case UnityAdsShowCompletionState.UNKNOWN:
                //Debug.Log($"UNKNOWN");
                break;
            
            default:
                //Debug.Log($"UNKNOWN");
                break;

        }
    }
}
