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
    internal static class StandardAdSlots
    {
        public static UInt32 GetNative300x250Slot()
        {
#if UNITY_IOS
            return 93231;
#elif UNITY_ANDROID
            return 93231;
#else
            return 0;
#endif
        }

        public static UInt32 GetNative320x50Slot()
        {
#if UNITY_IOS
            return 794557;
#elif UNITY_ANDROID
            return 794557;
#else
            return 0;
#endif
        }

        public static UInt32 GetNative728x90Slot()
        {
#if UNITY_IOS
            return 794557;
#elif UNITY_ANDROID
            return 794557;
#else
            return 0;
#endif
        }

        public static UInt32 GetWeb300x250Slot()
        {
#if UNITY_IOS
            return 93231;
#elif UNITY_ANDROID
            return 93231;
#else
            return 0;
#endif
        }

        public static UInt32 GetWeb320x50Slot()
        {
#if UNITY_IOS
            return 794557;
#elif UNITY_ANDROID
            return 794557;
#else
            return 0;
#endif
        }

        public static UInt32 GetWeb728x90Slot()
        {
#if UNITY_IOS
            return 794557;
#elif UNITY_ANDROID
            return 794557;
#else
            return 0;
#endif
        }
    }

    public sealed class StandardAdSample : MonoBehaviour
    {
        private readonly Object _syncRoot = new Object();
        private volatile MyTargetView _myTargetView;

        private void Awake()
        {
            MyTargetManager.DebugMode = true;
            MyTargetManager.Config = new MyTargetConfig.Builder().WithTestDevices("TEST_DEVICE_ID").Build();

            var buttons = FindObjectsOfType<Button>();

            var web320x50Button = buttons.FirstOrDefault(button => button.name == "Web320x50");
            if (web320x50Button)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => OnButtonClicked
                    (StandardAdSlots.GetWeb320x50Slot(), MyTargetView.AdSize.Size320x50));
                web320x50Button.onClick = onClickEvent;
            }

            var web300x250Button = buttons.FirstOrDefault(button => button.name == "Web300x250");
            if (web300x250Button)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => OnButtonClicked(StandardAdSlots.GetWeb300x250Slot(), MyTargetView.AdSize.Size300x250));
                web300x250Button.onClick = onClickEvent;
            }

            var web728x90Button = buttons.FirstOrDefault(button => button.name == "Web728x90");
            if (web728x90Button)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => OnButtonClicked(StandardAdSlots.GetWeb728x90Slot(), MyTargetView.AdSize.Size728x90));
                web728x90Button.onClick = onClickEvent;
            }

            var native320x50Button = buttons.FirstOrDefault(button => button.name == "Native320x50");
            if (native320x50Button)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => OnButtonClicked(StandardAdSlots.GetNative320x50Slot(), MyTargetView.AdSize.Size320x50));
                native320x50Button.onClick = onClickEvent;
            }

            var native300x250Button = buttons.FirstOrDefault(button => button.name == "Native300x250");
            if (native300x250Button)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => OnButtonClicked(StandardAdSlots.GetNative300x250Slot(), MyTargetView.AdSize.Size300x250));
                native300x250Button.onClick = onClickEvent;
            }

            var native728x90Button = buttons.FirstOrDefault(button => button.name == "Native728x90");
            if (native728x90Button)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => OnButtonClicked(StandardAdSlots.GetNative728x90Slot(), MyTargetView.AdSize.Size728x90));
                native728x90Button.onClick = onClickEvent;
            }

            var interstitialAdButton = buttons.FirstOrDefault(button => button.name == "InterstitialAd");
            if (interstitialAdButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => SceneManager.LoadScene("InterstitialAdSample"));
                interstitialAdButton.onClick = onClickEvent;
            }

            var rewardedAdButton = buttons.FirstOrDefault(button => button.name == "RewardedAd");
            if (rewardedAdButton)
            {
                var onClickEvent = new Button.ButtonClickedEvent();
                onClickEvent.AddListener(() => SceneManager.LoadScene("RewardedAdSample"));
                rewardedAdButton.onClick = onClickEvent;
            }
        }

        private void OnButtonClicked(UInt32 slotId, MyTargetView.AdSize adSize)
        {
            lock (_syncRoot)
            {
                _myTargetView?.Stop();
                _myTargetView?.Dispose();

                _myTargetView = new MyTargetView(slotId, adSize)
                {
                    CustomParams =
                    {
                        Age = 23,
                        Gender = GenderEnum.Male,
                        Lang = "ru-RU"
                    }
                };

                _myTargetView.AdClicked += (s, e) => 
                    Debug.Log("StandardAdSample: OnAdClicked");
                
                _myTargetView.AdLoadFailed += (s, e) => 
                    Debug.Log("StandardAdSample: OnAdLoadFailed");
                
                _myTargetView.AdLoadCompleted += OnAdLoadCompleted;

                _myTargetView.Load();
            }
        }

        private void OnDestroy()
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

        private void OnAdLoadCompleted(Object sender, EventArgs eventArgs)
        {
            Debug.Log("OnAdLoadCompleted");

            var isAutoClose = FindObjectsOfType<Toggle>()
                .Where(toggle => toggle.name == "Autoclose")
                .Select(toggle => toggle.isOn)
                .FirstOrDefault();

            ThreadPool.QueueUserWorkItem(state => StartImpl(isAutoClose));
        }

        private async void StartImpl(Boolean isAutoClose)
        {
            const Int32 timeout = 100000;

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
    }
}