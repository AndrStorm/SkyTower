using System;
using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.Serialization;


public class InGameUpdateManager : MonoBehaviour
{
    
    //public static event Action<bool> OnAppUodateInfoReceived;

    
    [SerializeField] private RectTransform GameUpdateDialog;

    private IUpdateHandler _updateHandler;
    
    private void OnEnable()
    {
        GameController.OnInGameUodateRequested += RequestGameUpdateInfo;
    }

    private void OnDisable()
    {
        GameController.OnInGameUodateRequested -= RequestGameUpdateInfo;
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
            _updateHandler = new DefaultUpdateHandler();
        }
        
        Helper.Log("Init Update Handler");
        _updateHandler.OnGameUodateInfoReceived += OnGameUodateInfoReceived;
        _updateHandler.Init();
    }

    private void OnDestroy()
    {
        _updateHandler.Destroy();
        _updateHandler.OnGameUodateInfoReceived -= OnGameUodateInfoReceived;
    }

    
    private void OnGameUodateInfoReceived(bool isUpdateAvailable)
    {
        if (!isUpdateAvailable)
        {
            Helper.Log("Update is not available");
            return;
        }
        
        Helper.Log("Update is available");
        ShowScreen();
    }

    private void RequestGameUpdateInfo()
    {
        
    }
    
    private void ShowScreen()
    {
        if (_updateHandler == null) return;
        GameUpdateDialog.gameObject.SetActive(true);
        SoundManager.Instance.PlaySound("ButtonClick");
    }

    public void CloseScreen()
    {
        GameUpdateDialog.gameObject.SetActive(false);
        SoundManager.Instance.PlaySound("ButtonClick");
    }
    
    public void UpdateGame()
    {
        //PlayerPrefs.SetInt(GameController.MAKE_REVIEW_PRESSED, 1);
        
        //var reviewCourutine = _reviewHandler?.MakeReview();
        //if (reviewCourutine != null) StartCoroutine(reviewCourutine);
        CloseScreen();
    }
    
    private class DefaultUpdateHandler : IUpdateHandler
    {
        public event Action<bool> OnGameUodateInfoReceived;
        public void Init() { }
        public void Destroy() { }
    }
}

public interface IUpdateHandler
{
    public event Action<bool> OnGameUodateInfoReceived;
    
    public void Init();
    public void Destroy();
    //public IEnumerator MakeReview();
}
