using RuStore.AppUpdate;
//using RuStore.AppUpdateExample.UI;
using System;
using RuStore;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RuStoreUpdateHandler : IUpdateHandler, IInstallStateUpdateListener 
{
    /*[SerializeField]
    private Image _updateLoadingBar;
    [SerializeField]
    private MessageBox _messageBox;*/
    
    public event Action<bool, bool> OnGameUodateInfoReceived;
    public event Action<float> OnGameUodateDowloading;
    public event Action OnGameUodateDowloaded;
    public event Action OnGameUodateFailed;


    private UpdateType _currentUpdateTupe;
    
    public void Init() 
    {
        RuStoreAppUpdateManager.Instance.Init();
        GetAppUpdateInfo();
    }
    
    private void GetAppUpdateInfo() 
    {
        RuStoreAppUpdateManager.Instance.GetAppUpdateInfo(
            onFailure: OnAppUpdateError,
            onSuccess: OnAppUodateInfoReceived);
    }
    
    private void OnAppUodateInfoReceived(AppUpdateInfo info)
    {
        bool isUpdateAvailable = false;
        bool isUpdateDownloaded = 
            info.installStatus == 
            AppUpdateInfo.InstallStatus.DOWNLOADED;
       
        var message = "Обновление недоступно";
        switch (info.updateAvailability) {
            case AppUpdateInfo.UpdateAvailability.UPDATE_AVAILABLE:
                message = $"Доступно обновление v{info.availableVersionCode}";
                isUpdateAvailable = true;
                break;
            case AppUpdateInfo.UpdateAvailability.DEVELOPER_TRIGGERED_UPDATE_IN_PROGRESS:
                message = "Обновление в процессе";
                break;
            default:
                message = "Обновление недоступно";
                break;
        }
        
        ShowMessage("Обновление", message);
        var isImmediateUpdateAllowed = RuStoreAppUpdateManager.Instance.IsImmediateUpdateAllowed();
        Debug.LogFormat("isImmediateUpdateAllowed: {0}", isImmediateUpdateAllowed);
        
        OnGameUodateInfoReceived?.Invoke(isUpdateAvailable, isUpdateDownloaded);
    }
    
    private void OnAppUpdateError(RuStoreError error) 
    {
        ShowMessage("Error", $"{error.name} : {error.description}");
    }



    public void DownloadUpdate()
    {
        StartFlexibleUpdate();
    }
    
    private void StartImmediateUpdate()
    {
        _currentUpdateTupe = UpdateType.IMMEDIATE;
        RuStoreAppUpdateManager.Instance.RegisterListener(this);
        RuStoreAppUpdateManager.Instance.StartUpdateFlow(UpdateType.IMMEDIATE, OnAppUpdateError,
            (result) => {
                Debug.LogFormat("Update flow result -> {0}", result);
                if (result == UpdateFlowResult.RESULT_CANCELED ||
                    result == UpdateFlowResult.RESULT_ACTIVITY_NOT_FOUND) {
                    RuStoreAppUpdateManager.Instance.UnregisterListener(this);
                }
            });
    }
    
    private void StartFlexibleUpdate() 
    {
        _currentUpdateTupe = UpdateType.FLEXIBLE;
        RuStoreAppUpdateManager.Instance.RegisterListener(this);
        RuStoreAppUpdateManager.Instance.StartUpdateFlow(UpdateType.FLEXIBLE, OnAppUpdateError,
            (result) => {
                Debug.LogFormat("Update flow result -> {0}", result);
                if (result == UpdateFlowResult.RESULT_CANCELED) {
                    RuStoreAppUpdateManager.Instance.UnregisterListener(this);
                }
            });
    }
    
    private void StartSilentUpdate() 
    {
        _currentUpdateTupe = UpdateType.SILENT;
        RuStoreAppUpdateManager.Instance.RegisterListener(this);
        RuStoreAppUpdateManager.Instance.StartUpdateFlow(UpdateType.SILENT, OnAppUpdateError,
            (result) => {
                Debug.LogFormat("Update flow result -> {0}", result);
            });
    }
    
    
    
    void IInstallStateUpdateListener.OnStateUpdated(InstallState state) 
    {
        if (state.installStatus == InstallState.InstallStatus.DOWNLOADED) 
        {
            ShowUpdateProgress(progress: 1f);
            OnGameUodateDowloaded?.Invoke();
        } 
        else if (state.installStatus == InstallState.InstallStatus.FAILED) 
        {
            RuStoreAppUpdateManager.Instance.UnregisterListener(this);
            ShowUpdateProgress(progress: 0f);
            OnGameUodateFailed?.Invoke();
        } 
        else if (state.installStatus == InstallState.InstallStatus.DOWNLOADING)
        {
            float progress = (float)state.bytesDownloaded / (float)state.totalBytesToDownload;
            ShowUpdateProgress(progress: progress);
            OnGameUodateDowloading?.Invoke(progress);
        }
    }



    public void InstallUpdate()
    {
        Helper.Log($"Finish update type {_currentUpdateTupe}");
        if (_currentUpdateTupe == UpdateType.SILENT)
        {
            FinishSilentUpdate();
        }
        else
        {
            FinishFlexibleUpdate();
        }
    }
    
    private void FinishSilentUpdate() 
    {
        RuStoreAppUpdateManager.Instance.CompleteUpdate(UpdateType.SILENT, OnAppUpdateError);
    }
    
    private void FinishFlexibleUpdate() 
    {
        RuStoreAppUpdateManager.Instance.CompleteUpdate(UpdateType.FLEXIBLE, OnAppUpdateError);
    }
    
    
    private void ShowUpdateProgress(float progress) 
    {
        Helper.Log($"Update progress {progress}");
        /*var scale = _updateLoadingBar.transform.localScale;
        scale.x = progress;
        _updateLoadingBar.transform.localScale = scale;*/
    }
    
    
    
    
    
    
    
    private void ShowMessage(string title, string message, Action onClose = null) 
    {
        Helper.Log($"Title: {title}, Messege {message}");
        /*_messageBox.Show(
            title: title,
            message: message,
            onClose: onClose);*/
    }

}
