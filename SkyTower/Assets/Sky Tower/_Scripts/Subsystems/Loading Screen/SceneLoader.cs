using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{ 
    public static event Action OnStartLoadingScene;
    public static event Action OnFininshedLoadingScene;

    [SerializeField] private Text progressPercentage;
    [SerializeField] private Image progressBar;
    
    
    private static SceneLoader _instance;
    private static bool _loadingFinished;

    private Animator animator;
    private AsyncOperation loadingOperation;


    public static void LoadScene (string sceneName)
    {
        LoadScene();

        _instance.loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        _instance.loadingOperation.allowSceneActivation = false;
    }

    public static void LoadScene(int sceneIndex)
    {
        LoadScene();

        _instance.loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        _instance.loadingOperation.allowSceneActivation = false;
    }

    private static void LoadScene()
    {
        _instance.animator.SetTrigger("loadingStart");
        OnStartLoadingScene?.Invoke();
    }
    
    
    void Start()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        animator = gameObject.GetComponent<Animator>();

        if (_loadingFinished)
        {
            OnFininshedLoadingScene?.Invoke();
            animator.SetTrigger("loadingFinished");
        }

    }

    
    void Update()
    {
        if (loadingOperation !=null)
        {
            progressPercentage.text = $"{Mathf.RoundToInt(loadingOperation.progress * 100)} %";
            progressBar.fillAmount = loadingOperation.progress;
        }
        
    }

    public void OnLoadingScreenLoad()
    {
        _loadingFinished = true;
        loadingOperation.allowSceneActivation = true;
        //FinishLoad();
    }

    /*private async void FinishLoad()
    {
        await Task.Delay(2000);
        loadingOperation.allowSceneActivation = true;
    }*/
    

}
