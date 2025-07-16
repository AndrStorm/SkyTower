#if GP_BUILD
using System.Collections;
using Google.Play.Review;
using UnityEngine;


public class GPReviewHandler : IReviewHandler
{
    //https://play.google.com/store/apps/details?id=com.AndrStormGames.SkyTower

    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private bool _isInit;

    private void Init()
    {
        _isInit = true;
        _reviewManager = new ReviewManager();
    }
    
    
    public IEnumerator MakeReview()
    {
        if(!_isInit) Init();
        
        return LaunchReview();
    }
    
    private IEnumerator LaunchReview()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.Log($"GP RequestReviewInfo error {requestFlowOperation.Error.ToString()}");
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();
        
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.Log($"GP launchFlowOperation error {requestFlowOperation.Error.ToString()}");
            yield break;
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
    }
}
#endif