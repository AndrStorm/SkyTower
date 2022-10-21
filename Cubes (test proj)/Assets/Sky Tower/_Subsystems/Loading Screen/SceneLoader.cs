using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{ 
    public Text progressPercentage;
    public Image progressBar;


    private static SceneLoader Instance;
    private static bool loadingFinished;

    private Animator animator;
    private AsyncOperation loadingOperation;


    public static void LoadScene (string sceneName)
    {
        Instance.animator.SetTrigger("loadingStart");

        Instance.loadingOperation = SceneManager.LoadSceneAsync(sceneName);
        Instance.loadingOperation.allowSceneActivation = false;
    }

    public static void LoadScene(int sceneIndex)
    {
        Instance.animator.SetTrigger("loadingStart");

        Instance.loadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
        Instance.loadingOperation.allowSceneActivation = false;
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
    }
    

}
