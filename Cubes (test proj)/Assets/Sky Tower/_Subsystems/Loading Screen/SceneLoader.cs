using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{ 
    public static event Action OnStartLoadingScene;
    public static event Action OnFininshedLoadingScene;

    public Text progressPercentage;
    public Image progressBar;


    private static SceneLoader Instance;
    private static bool loadingFinished;

    private Animator animator;
    private AsyncOperation loadingOperation;


    public static void LoadScene (string sceneName)
    {
        LoadScene();

        Instance.loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        Instance.loadingOperation.allowSceneActivation = false;
    }

    public static void LoadScene(int sceneIndex)
    {
        LoadScene();

        Instance.loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        Instance.loadingOperation.allowSceneActivation = false;
    }

    private static void LoadScene()
    {
        Instance.animator.SetTrigger("loadingStart");
        OnStartLoadingScene?.Invoke();
    }
    
    
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        animator = gameObject.GetComponent<Animator>();

        if (loadingFinished)
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
        loadingFinished = true;
        loadingOperation.allowSceneActivation = true;
        //FinishLoad();
    }

    /*private async void FinishLoad()
    {
        await Task.Delay(2000);
        loadingOperation.allowSceneActivation = true;
    }*/
    

}
