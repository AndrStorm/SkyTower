using UnityEngine;
using System;
using System.Collections;
using YandexMobileAds;
using YandexMobileAds.Base;

public class YandexMediationAdsManager : MonoBehaviour, IAdsGiver
{
    private String _message = "";

    private Banner _banner;
    private InterstitialAdLoader _interstitialAdLoader;
    private Interstitial _interstitial;

    public void InitAds()
    {
        _interstitialAdLoader = new InterstitialAdLoader();
        _interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
        _interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        RequestInterstitialLoad();
    }

    public void ShowFullScreenAd()
    {
        RequestInterstitial();
    }

    public void ShowBannerAd()
    {
        RequestBanner();
    }

    public void HideBannerAd()
    {
        CloseBannner();
    }


    
    private void DisplayMessage(String message)
    {
        _message = message + (_message.Length == 0 ? "" : "\n--------\n" + _message);
        print(message);
    }


    
    #region Interstitial
    private void CloseInterstitialAdd()
    {
        if (_interstitial != null)
        {
            _interstitial.Destroy();
            _interstitial = null; 
        }
    }
    
    private void RequestInterstitial()
    {
        StartCoroutine(ShowInterstitialAdsCoroutine());
    }
    
    
    private IEnumerator ShowInterstitialAdsCoroutine()
    {
        if (_interstitial == null)
        {
            //Debug.Log("Interstitial is not ready");
            RequestInterstitialLoad();
            
            int i = 0;
            while (_interstitial == null && i < 10)
            {
                i++;
                yield return Helper.GetUnscaledWait(0.2f);
                //Debug.Log("Interstitial is not ready");
            }
        }
        
        if (_interstitial != null)
        {
            ShowInterstitial();
        }
        else
        {
            //Debug.Log("Interstitial Time is out");
        }
    }
    
    private void RequestInterstitialLoad()
    {
        CloseInterstitialAdd();
        
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);
        
        // Replace demo Unit ID 'demo-interstitial-yandex' with actual Ad Unit ID
        string adUnitId = "R-M-16170795-2";
        
        _interstitialAdLoader.LoadAd(CreateAdRequest(adUnitId));
        //DisplayMessage("Interstitial is requested");
    }
    
    private void ShowInterstitial()
    {
        if (_interstitial == null)
        {
            //DisplayMessage("Interstitial is not ready yet");
            return;
        }

        _interstitial.OnAdClicked += HandleInterstitialAdClicked;
        _interstitial.OnAdShown += HandleInterstitialAdShown;
        _interstitial.OnAdFailedToShow += HandleInterstitialAdFailedToShow;
        _interstitial.OnAdImpression += HandleInterstitialImpression;
        _interstitial.OnAdDismissed += HandleInterstitialAdDismissed;

        _interstitial.Show();
    }

    private AdRequestConfiguration CreateAdRequest(string adUnitId)
    {
        return new AdRequestConfiguration.Builder(adUnitId).Build();
    }
    

    #region Interstitial callback handlers

    private void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
    {
        //DisplayMessage("HandleAdLoaded event received");
        _interstitial = args.Interstitial;
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        //DisplayMessage($"HandleAdFailedToLoad event received with message: {args.Message}");
    }
    private void HandleInterstitialAdClicked(object sender, EventArgs args)
    {
        //DisplayMessage("HandleAdClicked event received");
    }

    private void HandleInterstitialAdShown(object sender, EventArgs args)
    {
        //DisplayMessage("HandleAdShown event received");
    }

    private void HandleInterstitialAdDismissed(object sender, EventArgs args)
    {
        //DisplayMessage("HandleAdDismissed event received");
        CloseInterstitialAdd();
        RequestInterstitialLoad();
    }

    private void HandleInterstitialImpression(object sender, ImpressionData impressionData)
    {
        //var data = impressionData == null ? "null" : impressionData.rawData;
        //DisplayMessage($"HandleImpression event received with data: {data}");
    }

    private void HandleInterstitialAdFailedToShow(object sender, AdFailureEventArgs args)
    {
        DisplayMessage($"HandleInterstitialAdFailedToShow event received with message: {args.Message}");
    }

    #endregion
    #endregion
    
    
    #region Banner
    private void CloseBannner()
    {
        if (_banner != null)
        {
            _banner.Destroy();
            _banner = null;
        }
    }
    
    private void RequestBanner()
    {
        //Sets COPPA restriction for user age under 13
        MobileAds.SetAgeRestrictedUser(true);

        // Replace demo Unit ID 'demo-banner-yandex' with actual Ad Unit ID
        string adUnitId = "R-M-16170795-1";

        CloseBannner();
        // Set sticky banner width
        BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
        // Or set inline banner maximum width and height
        // BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
        _banner = new Banner(adUnitId, bannerSize, AdPosition.TopCenter);

        _banner.OnAdLoaded += HandleBannerAdLoaded;
        _banner.OnAdFailedToLoad += HandleBannerAdFailedToLoad;
        _banner.OnReturnedToApplication += HandleBannerReturnedToApplication;
        _banner.OnLeftApplication += HandleBannerLeftApplication;
        _banner.OnAdClicked += HandleBannerAdClicked;
        _banner.OnImpression += HandleBannerImpression;

        _banner.LoadAd(CreateAdRequest());
        DisplayMessage("Banner is requested");
    }

    // Example how to get screen width for request
    private int GetScreenWidthDp()
    {
        int screenWidth = (int)Screen.safeArea.width;
        return ScreenUtils.ConvertPixelsToDp(screenWidth);
    }

    private AdRequest CreateAdRequest()
    {
        return new AdRequest.Builder().Build();
    }

    #region Banner callback handlers

    private void HandleBannerAdLoaded(object sender, EventArgs args)
    {
        //this.DisplayMessage("HandleAdLoaded event received");
        _banner.Show();
    }

    private void HandleBannerAdFailedToLoad(object sender, AdFailureEventArgs args)
    {
        this.DisplayMessage("HandleAdFailedToLoad event received with message: " + args.Message);
    }

    private void HandleBannerLeftApplication(object sender, EventArgs args)
    {
        //this.DisplayMessage("HandleLeftApplication event received");
    }

    private void HandleBannerReturnedToApplication(object sender, EventArgs args)
    {
        //this.DisplayMessage("HandleReturnedToApplication event received");
    }

    private void HandleBannerAdClicked(object sender, EventArgs args)
    {
        //this.DisplayMessage("HandleAdClicked event received");
        CloseBannner();
    }

    private void HandleBannerImpression(object sender, ImpressionData impressionData)
    {
        //var data = impressionData == null ? "null" : impressionData.rawData;
        //DisplayMessage("HandleImpression event received with data: " + data);
    }

    #endregion
    #endregion
}
