using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsManager : Singleton<UnityAdsManager>, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private string type = "video";

    [SerializeField] private string _androidGameId = "4618933";
    [SerializeField] private string _iOSGameId = "4618932";
    [SerializeField] private bool _testMode = true;
    
    [SerializeField] private string _interstitial_Android = "Interstitial_Android";
    [SerializeField] private string _banner_Android = "Banner_Android";
    [SerializeField] private string _rewarded_Android = "Rewarded_Android";
    [SerializeField] private string _interstitial_iOS = "Interstitial_iOS";
    [SerializeField] private string _bannerAndroid_iOS = "Banner_iOS";
    [SerializeField] private string _rewardedAndroid_iOS = "Rewarded_iOS";

    
    private string _gameId;
    


    private void Start()
    {
        InitializeAds();
    }
    
    private void InitializeAds()
    {
        _gameId = Application.platform == RuntimePlatform.IPhonePlayer 
            ? _iOSGameId : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode);
    }


    public static void ShowAds(string placementId)
    {
        if (Advertisement.isInitialized)
        {
            Advertisement.Show(placementId, Instance);
            Debug.Log("show ad " + placementId);
        }
        else
        {
            Debug.Log("ads are not ready");
        }
    }
    

    

    

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    
    
    
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"OnUnityAdsAdLoaded placementId {placementId}");
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
        Debug.Log($"OnUnityAdsShowStart placementId {placementId}");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log($"OnUnityAdsShowClick placementId {placementId}");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Debug.Log($"OnUnityAdsShowComplete placementId {placementId}");
        switch (showCompletionState)
        {
            case UnityAdsShowCompletionState.SKIPPED:
                Debug.Log($"SKIPPED");
                break;
            
            case UnityAdsShowCompletionState.COMPLETED:
                Debug.Log($"COMPLETED");
                break;
            
            case UnityAdsShowCompletionState.UNKNOWN:
                Debug.Log($"UNKNOWN");
                break;
            
            default:
                Debug.Log($"UNKNOWN");
                break;

        }
    }
}
