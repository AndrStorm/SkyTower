using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mycom.Target.Unity.Ads;
using Mycom.Target.Unity.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

namespace Mycom.Target.Unity.Samples
{
    internal static class InterstitialSlots
    {
        public static UInt32 GetImageSlotId()
        {
#if UNITY_IOS
            return 6498;
#elif UNITY_ANDROID
            return 6481;
#else
            return 0;
#endif
        }

        public static UInt32 GetPromoSlotId()
        {
#if UNITY_IOS
            return 6899;
#elif UNITY_ANDROID
            return 6896;
#else
            return 0;
#endif
        }

        public static UInt32 GetPromoVideoSlotId()
        {
#if UNITY_IOS
            return 22091;
#elif UNITY_ANDROID
            return 10138;
#else
            return 0;
#endif
        }

        public static UInt32 GetVideoStyleSlotId()
        {
#if UNITY_IOS
            return 38838;
#elif UNITY_ANDROID
            return 38837;
#else
            return 0;
#endif
        }
    }

    public sealed class InterstitialAdSample : MonoBehaviour
    {
        private readonly Object _syncRoot = new Object();
        private volatile InterstitialAd _interstitialAd;

        private void Awake()
        {
            MyTargetManager.DebugMode = true;
            MyTargetManager.Config = new MyTargetConfig.Builder().WithTestDevices("TEST_DEVICE_ID").Build();

            var buttons = FindObjectsOfType<Button>().ToArray();

            var imageButton = buttons.FirstOrDefault(button => button.name == "Image");
            if (imageButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => Show(InterstitialSlots.GetImageSlotId()));
                imageButton.onClick = onClickEvent;
            }

            var promoButton = buttons.FirstOrDefault(button => button.name == "Promo");
            if (promoButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => Show(InterstitialSlots.GetPromoSlotId()));
                promoButton.onClick = onClickEvent;
            }

            var promoVideoButton = buttons.FirstOrDefault(button => button.name == "PromoVideo");
            if (promoVideoButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => Show(InterstitialSlots.GetPromoVideoSlotId()));
                promoVideoButton.onClick = onClickEvent;
            }

            var videoStyleButton = buttons.FirstOrDefault(button => button.name == "VideoStyle");
            if (videoStyleButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => Show(InterstitialSlots.GetVideoStyleSlotId()));
                videoStyleButton.onClick = onClickEvent;
            }

            var standardAdButton = buttons.FirstOrDefault(button => button.name == "StandardAd");
            if (standardAdButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => SceneManager.LoadScene("StandardAdSample"));
                standardAdButton.onClick = onClickEvent;
            }

            var rewardedAdButton = buttons.FirstOrDefault(button => button.name == "RewardedAd");
            if (rewardedAdButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => SceneManager.LoadScene("RewardedAdSample"));
                rewardedAdButton.onClick = onClickEvent;
            }
        }

        private void OnDestroy()
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

        private void OnLoadCompleted(Object sender, EventArgs eventArgs)
        {
            Debug.Log("IntersititialAdSample:  OnAdLoadCompleted");

            var isAutoClose = FindObjectsOfType<Toggle>()
                .Where(toggle => toggle.name == "Autoclose")
                .Select(toggle => toggle.isOn)
                .FirstOrDefault();

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

        private void Show(UInt32 slotId)
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
                    CustomParams =
                    {
                        Age = 23,
                        Gender = GenderEnum.Male,
                        Lang = "ru-RU"
                    }
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
                
                _interstitialAd.AdLoadCompleted += OnLoadCompleted;

                _interstitialAd.Load();
            }
        }
    }
}