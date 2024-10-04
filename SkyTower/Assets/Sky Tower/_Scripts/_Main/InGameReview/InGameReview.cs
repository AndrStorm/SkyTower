using UnityEngine;


public class InGameReview : MonoBehaviour
{

    [SerializeField] private RectTransform GameReviewDialog;

    private IReviewHandler _reviewHandler;

    private void OnEnable()
    {
        GameController.OnInGameReviewRequested += ShowScreen;
    }

    private void OnDisable()
    {
        GameController.OnInGameReviewRequested -= ShowScreen;
    }

    private void Start()
    {
        //_reviewHandler = new AGReviewHandler();
#if GP_BUILD
        _reviewHandler = new GPReviewHandler();
#elif AG_BUILD
        _reviewHandler = new AGReviewHandler();
#elif RS_BUILD
        _reviewHandler = new RSReviewHandler();
#endif
    }

    private void ShowScreen()
    {
        if (_reviewHandler == null) return;
        GameReviewDialog.gameObject.SetActive(true);
        SoundManager.Instance.PlaySound("ButtonClick");
    }

    public void CloseScreen()
    {
        GameReviewDialog.gameObject.SetActive(false);
        SoundManager.Instance.PlaySound("ButtonClick");
    }

    public void MakeReview()
    {
        var reviewCourutine = _reviewHandler?.MakeReview();
        if (reviewCourutine != null) StartCoroutine(reviewCourutine);
        CloseScreen();
    }
    
    public void OpenStorePage()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        Application.OpenURL(@"market://details?id=" + Application.identifier);
    }

}
