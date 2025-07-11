/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mycom.Target.Unity.Ads;
using Mycom.Target.Unity.Common;
using Object = System.Object;*/

public class VKAdsManager /*: MonoBehaviour, IAdsGiver*/
{
    /*private static class AdSlots
    {
        public static UInt32 GetBanner300x250()
        {
#if UNITY_IOS
            return 0;
#elif UNITY_ANDROID
            return 1862021;
#else
            return 0;
#endif
        }

        public static UInt32 GetBanner320x50()
        {
#if UNITY_IOS
            return 0;
#elif UNITY_ANDROID
            return 1862036;
#else
            return 0;
#endif
        }

        public static UInt32 GetBanner728x90()
        {
#if UNITY_IOS
            return 0;
#elif UNITY_ANDROID
            return 1862413;
#else
            return 0;
#endif
        }
        
        public static UInt32 GetFullScreenBlock()
        {
#if UNITY_IOS
            return 0;
#elif UNITY_ANDROID
            return 1862024;
#else
            return 0;
#endif
        }
        
        public static UInt32 GetRewardedVideoBlock()
        {
#if UNITY_IOS
            return 0;
#elif UNITY_ANDROID
            return 1862027;
#else
            return 0;
#endif
        }
        
    }
    
    private readonly Object _syncRoot = new Object();
    private volatile InterstitialAd _interstitialAd;
    private volatile MyTargetView _myTargetView;
    private volatile RewardedAd _rewardedAd;


    public void InitAds()
    {
        Helper.Log("VK ADS InitAds");
        //MyTargetManager.DebugMode = true;
        //null ref ex
        /*MyTargetManager.Config = new MyTargetConfig.Builder().
            WithTestDevices("TEST_DEVICE_ID").Build();#1#
    }
    
    
    public void ShowFullScreenAd()
    {
        
    }

    private int bannerCounter;
    public void ShowBannerAd()
    {
        Helper.Log($"bannerCounter {bannerCounter}");
        int counter = bannerCounter % 5;
        switch (counter)
        {
            case 0:
                OnBannerShow(AdSlots.GetBanner320x50(), MyTargetView.AdSize.Size320x50);
                break;
            case 1:
                OnBannerShow(AdSlots.GetBanner300x250(), MyTargetView.AdSize.Size300x250);
                break;
            case 3:
                ShowInterstitial(AdSlots.GetFullScreenBlock());
                break;
            case 4:
                ShowRewaredAd(AdSlots.GetRewardedVideoBlock());
                break;
            default:
                OnBannerShow(AdSlots.GetBanner728x90(), MyTargetView.AdSize.Size728x90);
                break;
        }
        bannerCounter++;
        
    }

    public void HideBannerAd()
    {
        
    }
    
    
    
    private void ShowRewaredAd(UInt32 slotId)
    {
        if (_rewardedAd != null)
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_rewardedAd != null)
            {
                return;
            }

            _rewardedAd = new RewardedAd(slotId)
            {
                CustomParams =
                {
                    Age = 23,
                    Gender = GenderEnum.Male,
                    Lang = "ru-RU"
                }
            };

            _rewardedAd.AdClicked += (s, e) => 
                Debug.Log("RewardedAdSample: OnAdClicked");
            _rewardedAd.AdDisplayed += (s, e) => 
                Debug.Log("RewardedAdSample: OnAdDisplayed");
            _rewardedAd.AdRewarded += (s, e) => 
                Debug.Log("RewardedAdSample: OnAdRewarded");
            _rewardedAd.AdDismissed += (s, e) =>
            {
                Debug.Log("RewardedAdSample: OnAdDismissed");
                OnDestroy();
            };
            _rewardedAd.AdLoadFailed += (s, e) =>
            {
                Debug.Log("RewardedAdSample: OnAdLoadFailed, error " + e.Message);
                OnDestroy();
            };
            
            _rewardedAd.AdLoadCompleted += OnRewardedAdLoadCompleted;

            _rewardedAd.Load();
        }
    }
    
    private void OnRewardedAdLoadCompleted(Object sender, EventArgs eventArgs)
    {
        var isAutoClose = false;

        ThreadPool.QueueUserWorkItem(async state =>
        {
            _rewardedAd?.Show();

            if (!isAutoClose)
            {
                return;
            }

            await Task.Delay(120000);

            _rewardedAd?.Dismiss();
        });
    }
    
    
    
    private void ShowInterstitial(UInt32 slotId)
    {
        if (_interstitialAd != null)
        {
            return;
        }

        lock (_syncRoot)
        {
            if (_interstitialAd != null)
            {
                return;
            }

            _interstitialAd = new InterstitialAd(slotId)
            {
                /*CustomParams =
                {
                    Age = 23,
                    Gender = GenderEnum.Male,
                    Lang = "ru-RU"
                }#1#
            };

            _interstitialAd.AdClicked += (s, e) => 
                Debug.Log("InterstitialAdSample: OnAdClicked");
                
            _interstitialAd.AdDismissed += (s, e) =>
            {
                Debug.Log("InterstitialAdSample: OnAdDismissed");
                OnDestroy();
            };
                
            _interstitialAd.AdDisplayed += (s, e) => 
                Debug.Log("InterstitialAdSample: OnAdDisplayed");
                
            _interstitialAd.AdLoadFailed += (s, e) =>
            {
                Debug.Log("InterstitialAdSample: OnAdLoadFailed, error " + e.Message);
                OnDestroy();
            };
                
            _interstitialAd.AdVideoCompleted += (s, e) => 
                Debug.Log("InterstitialAdSample: OnAdVideoCompleted");
                
            _interstitialAd.AdLoadCompleted += OnInterstitialLoadCompleted;

            _interstitialAd.Load();
        }
    }
    
    private void OnInterstitialLoadCompleted(Object sender, EventArgs eventArgs)
    {
        Debug.Log("IntersititialAdSample:  OnAdLoadCompleted");

        var isAutoClose = false;

        ThreadPool.QueueUserWorkItem(async state =>
        {
            _interstitialAd?.Show();

            if (!isAutoClose)
            {
                return;
            }

            await Task.Delay(100000);

            _interstitialAd?.Dismiss();
        });
    }
    
    

    private void OnBannerShow(UInt32 slotId, MyTargetView.AdSize adSize)
    {
        Helper.Log("VK ADS OnBannerShow");
        lock (_syncRoot)
        {
            _myTargetView?.Stop();
            _myTargetView?.Dispose();

            _myTargetView = new MyTargetView(slotId, adSize)
            {
                /*CustomParams =
                {
                    Age = 23,
                    Gender = GenderEnum.Male,
                    Lang = "ru-RU"
                }#1#
            };
            

            _myTargetView.AdShown += (s, e) => 
                Debug.Log("StandardAdSample: AdShown");
            _myTargetView.AdClicked += (s, e) => 
                Debug.Log("StandardAdSample: OnAdClicked");
            _myTargetView.AdLoadFailed += (s, e) => 
                Debug.Log("StandardAdSample: OnAdLoadFailed" + e.Message);

            _myTargetView.AdLoadCompleted += OnBannerAdLoadCompleted;
            

            _myTargetView.Load();
        }
    }
    
    private void OnBannerAdLoadCompleted(Object sender, EventArgs eventArgs)
    {
        Debug.Log("OnAdLoadCompleted");

        _myTargetView.X = 0;
        _myTargetView.Y = 0;
        
        var isAutoClose = false;

        if (bannerCounter == 2)
        {
            _myTargetView.Start();
        }
        else
        {
            ThreadPool.QueueUserWorkItem(state => StartImpl(isAutoClose));
        }

        
    }
    
    private async void StartImpl(bool isAutoClose)
    {
        const int timeout = 100000;

        _myTargetView?.Start();

        if (!isAutoClose)
        {
            return;
        }

        await Task.Delay(timeout);

        if (_myTargetView != null)
        {
            _myTargetView.X = 50;
            _myTargetView.Y = 50;
        }

        await Task.Delay(timeout);

        if (_myTargetView != null)
        {
            _myTargetView.Width += 50;
        }

        await Task.Delay(timeout);

        _myTargetView?.Dispose();
        _myTargetView = null;
    }
    
    
    private void OnDestroy()
    {
        OnDestroyBanner();
        OnDestroyInterstitial();
        OnDestroyRewarded();
    }
    
    private void OnDestroyBanner()
    {
        if (_myTargetView == null)
        {
            return;
        }

        lock (_syncRoot)
        {
            _myTargetView?.Dispose();
            _myTargetView = null;
        }
    }
    
    private void OnDestroyInterstitial()
    {
        if (_interstitialAd == null)
        {
            return;
        }

        lock (_syncRoot)
        {
            _interstitialAd?.Dispose();

            _interstitialAd = null;
        }
    }
    
    private void OnDestroyRewarded()
    {
        if (_rewardedAd == null)
        {
            return;
        }

        lock (_syncRoot)
        {
            _rewardedAd?.Dispose();
            _rewardedAd = null;
        }
    }*/
}
