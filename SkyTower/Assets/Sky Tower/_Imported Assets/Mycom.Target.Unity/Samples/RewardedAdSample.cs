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
    internal static class RewardedSlots
    {
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

        public static UInt32 GetNewStyleSlotId()
        {
#if UNITY_IOS
            return 577499;
#elif UNITY_ANDROID
            return 577498;
#else
            return 0;
#endif
        }
    }

    public sealed class RewardedAdSample : MonoBehaviour
    {
        private readonly Object _syncRoot = new Object();
        private volatile RewardedAd _rewardedAd;

        private void Awake()
        {
            MyTargetManager.DebugMode = true;
            MyTargetManager.Config = new MyTargetConfig.Builder().WithTestDevices("TEST_DEVICE_ID").Build();

            var buttons = FindObjectsOfType<Button>().ToArray();

            var videoStyleButton = buttons.FirstOrDefault(button => button.name == "VideoStyle");
            if (videoStyleButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => Show(RewardedSlots.GetVideoStyleSlotId()));
                videoStyleButton.onClick = onClickEvent;
            }

            var newStyleButton = buttons.FirstOrDefault(button => button.name == "NewStyle");
            if (newStyleButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => Show(RewardedSlots.GetNewStyleSlotId()));
                newStyleButton.onClick = onClickEvent;
            }

            var standardAdButton = buttons.FirstOrDefault(button => button.name == "StandardAd");
            if (standardAdButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => SceneManager.LoadScene("StandardAdSample"));
                standardAdButton.onClick = onClickEvent;
            }

            var interstitialAdButton = buttons.FirstOrDefault(button => button.name == "InterstitialAd");
            if (interstitialAdButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => SceneManager.LoadScene("InterstitialAdSample"));
                interstitialAdButton.onClick = onClickEvent;
            }
        }

        private void OnDestroy()
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
        }

        private void OnLoadCompleted(Object sender, EventArgs eventArgs)
        {
            var isAutoClose = FindObjectsOfType<Toggle>().Where(toggle => toggle.name == "Autoclose")
                                                         .Select(toggle => toggle.isOn)
                                                         .FirstOrDefault();

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

        private void Show(UInt32 slotId)
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

                _rewardedAd.AdClicked += (s, e) => Debug.Log("RewardedAdSample: OnAdClicked");
                _rewardedAd.AdDismissed += (s, e) =>
                {
                    Debug.Log("RewardedAdSample: OnAdDismissed");
                    OnDestroy();
                };
                _rewardedAd.AdDisplayed += (s, e) => Debug.Log("RewardedAdSample: OnAdDisplayed");
                _rewardedAd.AdLoadFailed += (s, e) =>
                {
                    Debug.Log("RewardedAdSample: OnAdLoadFailed, error " + e.Message);
                    OnDestroy();
                };
                _rewardedAd.AdRewarded += (s, e) => Debug.Log("RewardedAdSample: OnAdRewarded");
                _rewardedAd.AdLoadCompleted += OnLoadCompleted;

                _rewardedAd.Load();
            }
        }
    }
}