using System;
using UnityEngine;
using UnityEngine.UI;


public class InGameUpdateManager : MonoBehaviour
{
    [SerializeField] private RectTransform _gameUpdateDialog;
    [SerializeField] private RectTransform _updateDownloadingDialog;
    [SerializeField] private Slider _downloadingSlider;

    private IUpdateHandler _updateHandler;
    
    private bool _isUpdateAvailable;
    private bool _isUpdateRequested;
    private bool _isDownloadingScreenShown;

    
    private void OnEnable()
    {
        GameController.OnInGameUodateRequested += OnGameUpdateRequested;
    }

    private void OnDisable()
    {
        GameController.OnInGameUodateRequested -= OnGameUpdateRequested;
        _updateHandler.OnGameUodateInfoReceived -= OnGameUodateInfoReceived;
        _updateHandler.OnGameUodateDowloading -= OnGameUpdateDownloading;
        _updateHandler.OnGameUodateDowloaded -= OnGameUpdateDownloaded;
        _updateHandler.OnGameUodateFailed -= OnGameUpdateFailed;
    }
    
    private void Start()
    {
#if GP_BUILD
#elif AG_BUILD
#elif RS_BUILD
        _updateHandler = new RuStoreUpdateHandler();
#endif
        if (_updateHandler == null)
        {
            return;
        }
        
        _updateHandler.OnGameUodateInfoReceived += OnGameUodateInfoReceived;
        _updateHandler.OnGameUodateDowloading += OnGameUpdateDownloading;
        _updateHandler.OnGameUodateDowloaded += OnGameUpdateDownloaded;
        _updateHandler.OnGameUodateFailed += OnGameUpdateFailed;
        _updateHandler.Init();
    }
    

    
    private void OnGameUodateInfoReceived(bool isUpdateAvailable)
    {
        if (!isUpdateAvailable)
        {
            return;
        }

        _isUpdateAvailable = true;
        if (_isUpdateRequested)
        {
            ShowUpdateScreen();
        }
    }
    
    private void OnGameUpdateRequested()
    {
        if (_isUpdateRequested) return;
        
        _isUpdateRequested = true;
        if (_isUpdateAvailable)
        {
            ShowUpdateScreen();
        }
    }

    private void OnGameUpdateDownloading(float progress)
    {
        if (!_isDownloadingScreenShown)
        {
            ShowDownloadingScreen();
        }
        
        _downloadingSlider.value = Mathf.Clamp01(progress);
    }
    
    private void OnGameUpdateDownloaded()
    {
        //_updateHandler.InstallUpdate();
        InstallUpdate();
    }
    
    private void OnGameUpdateFailed()
    {
        Helper.Log("Update Downloading Failed");
        CloseDownloadingScreen();
    }
    
    
    private void ShowUpdateScreen()
    {
        if (_updateHandler == null) return;
        if (_gameUpdateDialog == null) return;
        
        GameController.Instance.SetIsGamePause(true);
        
        _gameUpdateDialog.gameObject.SetActive(true);
        SoundManager.Instance.PlaySound("ButtonClick");
    }

    public void CloseUpdateScreen()
    {
        GameController.Instance.SetIsGamePause(false);
        
        _gameUpdateDialog.gameObject.SetActive(false);
        SoundManager.Instance.PlaySound("ButtonClick");
    }
    
    public void UpdateGame()
    {
        _updateHandler.DownloadUpdate();
        CloseUpdateScreen();
        //ShowDownloadingScreen();
    }
    
    
    
    private void ShowDownloadingScreen()
    {
        if (_updateDownloadingDialog == null) return;
        
        GameController.Instance.SetIsGamePause(true);
        
        _isDownloadingScreenShown = true;
        _updateDownloadingDialog.gameObject.SetActive(true);
    }
    
    private void CloseDownloadingScreen()
    {
        if (_updateDownloadingDialog == null) return;
        
        GameController.Instance.SetIsGamePause(false);
        
        _isDownloadingScreenShown = false;
        _updateDownloadingDialog.gameObject.SetActive(false);
    }
    
    private void InstallUpdate()
    {
        CloseDownloadingScreen();
        _updateHandler.InstallUpdate();
    }
    
    
}

public interface IUpdateHandler
{
    public event Action<bool> OnGameUodateInfoReceived;
    public event Action<float> OnGameUodateDowloading;
    public event Action OnGameUodateDowloaded;
    public event Action OnGameUodateFailed;

    public void Init();
    public void DownloadUpdate();
    public void InstallUpdate();
    
}
