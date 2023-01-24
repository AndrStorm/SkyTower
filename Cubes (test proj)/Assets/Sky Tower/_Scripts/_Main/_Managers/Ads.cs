using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour, IUnityAdsInitializationListener
{
    private string type = "video";

    [SerializeField] string _androidGameId = "4618933";
    [SerializeField] string _iOSGameId = "4618932";
    [SerializeField] bool _testMode = true;
    private string _gameId;


    private void Start()
    {
        InitializeAds();
        StartCoroutine(ShowAds());
    }

    IEnumerator ShowAds()
    {
        while (true)
        {
            if (Advertisement.IsReady(type))
            {
                Debug.Log("Ready ads");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void InitializeAds()
    {
        _gameId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOSGameId
            : _androidGameId;
        Advertisement.Initialize(_gameId, _testMode);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}
