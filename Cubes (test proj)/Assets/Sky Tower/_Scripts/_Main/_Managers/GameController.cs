using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEditor;
using UnityEngine.Localization.Components;
using Random = UnityEngine.Random;


public class GameController : Singleton<GameController>
{
    public static event Action<int> OnBestScoreIncrised;
    public static event Action OnInGameReviewRequested;

    public static bool isGamePause;

    public int lastScore;
    public int bestScore;
    
    
    #region Inspector

    
    
    [Header("Cinematic Game property")]
    [SerializeField]private float camMovSpeed = 3f;
    [Tooltip("How far will the camera move backward when the tower will become wider")]
    [SerializeField]private float cameraMoveBackMul =0.9f;
    [SerializeField]private bool isCameraMoveForward;
    [SerializeField]private int cameraMoveThreshold = 1;
    [SerializeField]private float slowMotionScale = 0.65f, slowMotionDuration=3.5f;
    [SerializeField]private float loseCamYMul =0.9f, loseCamZMul =1.2f, loseCamMovSpeedMul=5.35f;

    [Header("Spawn Game property")] 
    [SerializeField]private float spawnGamePauseDuration = 0.012f;
    [SerializeField]private float spawnPauseDuration = 0.18f;
    [SerializeField]private float spawnShakeAmount = 0.01f;
    [SerializeField]private float spawnShakeDur = 0.05f;

    
    [Header("Sound Game property")]
    [SerializeField]private float actionMusicVolumeMul = 0.5f;

    [SerializeField] private float minWindAmbientVol = 0.02f;
    [SerializeField] private float maxWindAmbientVol = 0.07f;
    [SerializeField] private float maxWindAmbientHeight = 100;
    [SerializeField]private int maxWindTowerHeight = 100;
    [Range(0f,1f)] [Tooltip("Add value to Tower Height mul and dot Product mul")]
    [SerializeField]private float minWindFactor=0.05f;
    [SerializeField]private float windVolumeMul=0.8f, windFadingSpeed=0.35f;
    [SerializeField]private float ambColorIntensity = 0.65f;

    [Header("Game property")]
    [SerializeField]private float bottomBlindZone = 50f;
    [SerializeField]private float topBlindZone = 50f;
    [SerializeField]private float restartButtonDelay = 3f;
    [SerializeField]private float towerVelocityThreshold = 0.23f;
    [SerializeField]private float finalPushVelocityMul = 3.5f;
    

    [SerializeField]private float timeLeftLow = 1f;
    [SerializeField]private float timeLeftMedium = 2f;
    [SerializeField]private Color[] phaseColors;
    [SerializeField]private Vector3 phaseLightIntensity;
    [SerializeField]private Vector3 phaseSpawnerFlicker;
    
    
    [Header("Game property - Background")]
    [SerializeField]private Gradient[] backGroundGradients1;
    [SerializeField]private Gradient[] backGroundGradients2;
    [SerializeField] private Material _backgroundMaterial;
    [SerializeField]private int lastColorScore = 375;
    
    private Color _bgColor1;
    private Color _bgColor2;
    private Color _targetBgColor1;
    private Color _targetBgColor2;
    

    [Header("References")]
    public GameObject allCubes;
    [SerializeField]private TextMeshProUGUI scoreText;
    [SerializeField]private Transform cubeSpawner;
    [SerializeField]private Transform cameraViewDirection;
    [SerializeField]private GameObject restartButton;
    [SerializeField]private CubeScriptable cubeToCreate;
    [SerializeField]private GameObject[] canvasMenu;

    
    
    #endregion


    
    #region Fields

    private bool isTestModOnlyUp;
    
    
    private AudioSource windSource;
    private Rigidbody allCubesRb;

    
    private readonly List<CubeInfo> cubeInfos = new List<CubeInfo>();
    private List<CubeScriptable> cubesToCreate;


    private DifficultySettings currentDifficulty;
    private CubePos lastCube = new CubePos(0, 1, 0);

    
    private Vector3 cameraTargetPos,cameraStartPos;
    private Vector3 lastSpawnerVector = Vector3.up;
    private float timerAutoPlace;
    private float maxZx;
    private bool isGameLost,isGameStart;
    private bool isSpawnPause;


    private List<Vector3> cubesPositions = new List<Vector3>
    {
        new Vector3(0, 1, 0)
    };
    
    
    private Coroutine moveCubeSpawner;
    private static Camera _playerCam;
    
    
    private const string LAST_SCORE = "lastScore";
    private const string BEST_SCORE = "bestScore";
    private const string RESTART_COUNTER = "RestartCounter";
    private const string IS_ADS_WAS_SHOWN = "IsAdsWasShown";

    
    
    #endregion

    
    
    #region MonoBehaviour
    
    private void OnEnable()
    {
        AchievmentsWindow.OnAchievmentsWindowOpen += SetIsGamePause;
        GameSettings.OnSettingsWindowOpen += SetIsGamePause;
        LeaderboardUI.OnLeaderboardOpen += SetIsGamePause;
    }
    
    private void OnDisable()
    {
        AchievmentsWindow.OnAchievmentsWindowOpen -= SetIsGamePause;
        GameSettings.OnSettingsWindowOpen -= SetIsGamePause;
        LeaderboardUI.OnLeaderboardOpen -= SetIsGamePause;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _playerCam = Helper.MainCamera;
        
        allCubesRb = allCubes.GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        HandleAdsOnStart();
        
        isGamePause = false;
        
        currentDifficulty = DifficultyManager.Instance.GetDifficulty(0f);
        
        windSource = SoundManager.Instance.GetSoundSource("Wind");

        cubesToCreate = new List<CubeScriptable>();
        foreach (var cube in CubesManager.Instance.GetCubesDictionary())
        {
            if (cube.Value.active)
            {
                cubesToCreate.Add(cube.Value);
            }
        }

        ResetAutoPlaceTimer();
        //-
        SetDefaultBgColor();
        //targetBgColor = _playerCam.backgroundColor; 
        //targetBgColor = backgroundImage.color;

        cameraStartPos = _playerCam.transform.localPosition;
        cameraTargetPos = cameraStartPos;
        
        lastScore = PlayerPrefs.GetInt(LAST_SCORE);
        bestScore = PlayerPrefs.GetInt(BEST_SCORE);
        
        CreateCube(new Vector3(0, 1, 0));
        moveCubeSpawner = StartCoroutine(MoveCubeSpawner());
    }
    
    private void Update()
    {
        if(isGamePause) return;
        
        if(isGameLost)
            TiltCamera();

        MoveCamera();
        ChangeBgColor();
        //-
        //ChangeAmbientLight(_playerCam.backgroundColor, ambColorIntensity); 
        ChangeAmbientLight(_bgColor1, ambColorIntensity);

        
        if (isGameStart && !isGameLost && allCubesRb.velocity.magnitude >= towerVelocityThreshold)
            LoseGame();
        
        CalculateWindSound(allCubes == null);
    }
    
    #endregion
    
    
    
    #region Coroutins
    
    private IEnumerator MoveCubeSpawner()
    {
        float spawnerTime = 0f;
        while (true)
        {
            if (isGamePause)
            {
                yield return Helper.GetWait(0.05f);
                continue;
            }
            
            if (isGameStart)
            {
                if (timerAutoPlace <= 0f)
                {
                    InitializeCubeCreation();
                    InitializeSpawnerMovement();
                    spawnerTime = 0f;
                }
                else
                {
                    timerAutoPlace -= 0.05f;

                    if (timerAutoPlace < timeLeftLow)
                    {
                        PhaseColorManager.Instance.ChangeLampColor(phaseColors[2], phaseLightIntensity.z,
                            phaseSpawnerFlicker.z);
                    }
                    else if (timerAutoPlace < timeLeftMedium)
                    {
                        PhaseColorManager.Instance.ChangeLampColor(phaseColors[1], phaseLightIntensity.y,
                            phaseSpawnerFlicker.y);
                    }
                    else
                    {
                        PhaseColorManager.Instance.ChangeLampColor(phaseColors[0], phaseLightIntensity.x,
                            phaseSpawnerFlicker.x);
                    }
                }
            }
            
            yield return Helper.GetWait(0.05f);
            spawnerTime += 0.05f;
            
            
            if (spawnerTime >= currentDifficulty.timeToPlaceCube)
            {
                spawnerTime = 0f;
                InitializeSpawnerMovement();
            }

        }
    }

    private IEnumerator PauseGameTime(float t)
    {
        Time.timeScale = 0f;
        //AudioListener.pause = true;
        isGamePause = true;
        
        yield return Helper.GetUnscaledWait(t);
        
        Time.timeScale = 1f;
        //AudioListener.pause = false;
        isGamePause = false;
    }
    
    private IEnumerator PauseGameTimeWhileIsGamePause(float delay)
    {
        yield return Helper.GetUnscaledWait(delay);
        
        Time.timeScale = 0f;
        //AudioListener.pause = true;
        isGamePause = true;

        while (isGamePause)
        {
            yield return Helper.GetUnscaledWait(0.1f);
        }
        
        Time.timeScale = 1f;
        //AudioListener.pause = false;
    }
    
    private IEnumerator PauseCubeSpawn(float t)
    {
        isSpawnPause = true;
        
        yield return Helper.GetUnscaledWait(t);

        isSpawnPause = false;
    }
    
    private IEnumerator SlowDownGame(float scale, float t)
    {
        Time.timeScale = scale;

        yield return Helper.GetUnscaledWait(t);
        
        Time.timeScale = 1f;
    }

    private IEnumerator ShowRestartButton(float delay)
    {
        yield return Helper.GetUnscaledWait(delay);
        restartButton.SetActive(true);
    }
    
    #endregion
    
    
    
    #region Methods
    
    public void HandleInput(Vector2 screenPosition)
    {
        bool isTowerDestroyed = allCubes == null;
        bool isGameContinue = !isGamePause && !isGameLost && !isTowerDestroyed && cubeSpawner != null;
        bool isCorectInput = !Helper.IsOverUI() && !isSpawnPause;
        bool isBlindZone = screenPosition.y < bottomBlindZone || screenPosition.y > Screen.height - topBlindZone;
        if (isGameContinue && isCorectInput && !isBlindZone)
        {
            if (!isGameStart)
                StartGame();

            InitializeCubeCreation();
            InitializeSpawnerMovement();
            
        }
    }

    public Vector3 GetLastCubePosition()
    {
        return lastCube.getVector();
    }

    public List<CubeInfo> GetCubeInfos()
    {
        return cubeInfos;
    }
    
    public void SlowDownTheGame()
    {
        StartCoroutine(SlowDownGame(slowMotionScale, slowMotionDuration));
    }
    
    public bool IsGameLost()
    {
        if (isGameLost)
            return true;
        return false;
    }

    public void LoseGame()
    {
        HandleAdsOnLose();
        SoundManager.Instance.ResetMusicVolume();
        
        isGameLost = true;

        Destroy(PhaseColorManager.Instance.gameObject);
        Destroy(cubeSpawner.gameObject);
        StopCoroutine(moveCubeSpawner);
        StartCoroutine(ShowRestartButton(restartButtonDelay));

        cameraTargetPos = new Vector3(cameraTargetPos.x,
            -cameraTargetPos.z * loseCamYMul, cameraTargetPos.z * loseCamZMul);
        camMovSpeed *= loseCamMovSpeedMul;
        
        allCubesRb.velocity *= finalPushVelocityMul;
        
        VfxManager.Instance.MoveWindVFX(Vector3.up);
        VfxManager.Instance.EnableWind(false);
    }


    private void HandleAdsOnStart()
    {
        int restartsToAds = UnityAdsManager.Instance.RestartsToShowAds;
        int currentRestarts = PlayerPrefs.GetInt(RESTART_COUNTER);
        if (currentRestarts == 0) return;

        HandleInGameReview(restartsToAds, currentRestarts);
        
        bool isAdsNeeded = PlayerPrefs.GetInt(RESTART_COUNTER) % restartsToAds == 0;
        if (isAdsNeeded && PlayerPrefs.GetInt(IS_ADS_WAS_SHOWN) == 0)
        {
            string placementId = UnityAdsManager.Instance.GetInterstitialId();
            UnityAdsManager.Instance.ShowFullScreenAds(placementId);
            PlayerPrefs.SetInt(IS_ADS_WAS_SHOWN, 1);
        }
    }

    private void HandleInGameReview(int restartsToAds, int currentRestarts)
    {
        if(currentRestarts == restartsToAds - 1) OnInGameReviewRequested?.Invoke();
    }

    private void HandleAdsOnLose()
    {
        UnityAdsManager.Instance.ShowBannerAds();
    }

    private void CalculateWindSound(bool isTowerDestroyed)
    {
        if (PlayerPrefs.GetInt("sound") != 1)
        {
            return;
        }
        
        if(isTowerDestroyed)
        {
            //FadeWindSound();
            windSource.volume = Mathf.MoveTowards(windSource.volume, 0f, windFadingSpeed * Time.deltaTime);
        }
        else if (isGameLost)
        {
            
            float dotProduct = Vector3.Dot(Vector3.down, allCubesRb.velocity.normalized);
            float windFactor = Mathf.Clamp01(dotProduct + minWindFactor);
            float towerHeightMul = Mathf.Clamp01(minWindFactor + (float) lastCube.y / maxWindTowerHeight);
            windSource.volume = windFactor * windVolumeMul * towerHeightMul;
            windSource.pitch = 0.75f + dotProduct;
        }
        else
        {
            windSource.pitch = 1f;
            windSource.volume = Mathf.Lerp(minWindAmbientVol, maxWindAmbientVol, (lastCube.y - 1) / maxWindAmbientHeight);
            
        }
        
    }

    private void StartGame()
    {
        isGameStart = true;

        var musicVolume = SoundManager.Instance.GetMusicVolume();
        SoundManager.Instance.SetMusicVolume(actionMusicVolumeMul * musicVolume);
        
        
        PlayerPrefs.SetInt(LAST_SCORE, 0);
        lastScore = 0;
        //scoreText.text = $"Score: 0";
        scoreStringEvent.RefreshString();
        
        foreach (GameObject obj in canvasMenu)
            Destroy(obj);
    }
    
    private void SetIsGamePause(bool isOpen)
    {
        isGamePause = isOpen;
    }
    

    private void ResetAutoPlaceTimer()
    {
        timerAutoPlace = currentDifficulty.timeToCubeAutoPlace;
    }

    private void ChangeAmbientLight(Color ambientColor, float intensity = 1f)
    {
        ambientColor = new Color(ambientColor.r * intensity, ambientColor.g * intensity,
            ambientColor.b * intensity, ambientColor.a);
        RenderSettings.ambientLight = ambientColor;
    }

    private void InitializeSpawnerMovement()
    {
        MoveSpawner();
        Vector3 cubeSpawnerPos = cubeSpawner.transform.position;
        PhaseColorManager.Instance.gameObject.transform.position =
            new Vector3(cubeSpawnerPos.x, cubeSpawnerPos.y + 2,
                cubeSpawnerPos.z);
    }

    private void InitializeCubeCreation()
    {
        StartCoroutine(PauseGameTime(spawnGamePauseDuration));
        StartCoroutine(PauseCubeSpawn(spawnPauseDuration));
        ResetAutoPlaceTimer();
        
        
        Vector3 vfxDir = cubeSpawner.position - lastCube.getVector();
        
        CreateCube(cubeSpawner.position);

        //method PlayOnSpawnVFX(vfxDir,vfxPos);
        Vector3 vfxPos = lastCube.getVector();
        vfxPos.x -= vfxDir.x/2;
        vfxPos.z -= vfxDir.z/2;
        if (vfxDir.y < 0.1f) vfxPos.y += 0.5f;
 
        Quaternion vfxRotation = Quaternion.LookRotation
            (vfxDir, Vector3.up) * Quaternion.Euler(90, 0, 0);
        VfxManager.Instance.PlaySpawnVfx(vfxPos, vfxRotation, cubeToCreate.cubeId);
        
        
        CameraShaker.Instance.ShakeCamera(spawnShakeAmount,spawnShakeDur);
        SoundManager.Instance.PlaySound("CubeSpawn", Random.Range(0.935f,1.075f));
        PhaseColorManager.Instance.ChangeLampColor(phaseColors[0],
            phaseLightIntensity.x, phaseSpawnerFlicker.x);

        
        ChangeScore();
        IncriseDifficulty();
        
        ChangeTargetBgColor();
        MoveCameraTargetPos();
        MoveCameraTiltDirection();
        
        VfxManager.Instance.MoveWindVFX(GetLastCubePosition());
    }

    private void IncriseDifficulty()
    {
        currentDifficulty = DifficultyManager.Instance.GetDifficulty((float)lastCube.y / DifficultyManager.Instance.maxDifficultyScore);
    }

    private void CreateCube(Vector3 position)
    {
        if (cubesToCreate.Count > 0)
            cubeToCreate = cubesToCreate[UnityEngine.Random.Range(0, cubesToCreate.Count)];

        GameObject newCube = Instantiate(
                cubeToCreate.cube,
                position,
                Quaternion.identity) as GameObject;
        
        newCube.transform.SetParent(allCubes.transform);
        lastCube.setVector(newCube.transform.position);
        cubesPositions.Add(lastCube.getVector());

        allCubesRb.isKinematic = true;
        allCubesRb.isKinematic = false;
        

        CubeInfo cubeInfo = new CubeInfo(newCube.transform, cubeToCreate.cubeId);
        cubeInfos.Add(cubeInfo);
        
    }
    

    [SerializeField] private LocalizeStringEvent scoreStringEvent;
    private void ChangeScore()
    {
        int currentScore = lastCube.y - 1;

        if (lastScore < currentScore)
        {
            PlayerPrefs.SetInt(LAST_SCORE, currentScore);
            lastScore = currentScore;

            if (bestScore < currentScore)
            {
               PlayerPrefs.SetInt(BEST_SCORE, currentScore);
               bestScore = currentScore;
               OnBestScoreIncrised.Invoke(currentScore);
            }

            scoreStringEvent.RefreshString();
            //scoreText.text = $"Score: {currentScore}";
        }

    }


    #region Background
    private void ChangeTargetBgColor()
    {
        float score = lastCube.y - 1f;
        
        float t = Mathf.Clamp01(score / lastColorScore);
        
        t *= backGroundGradients1.Length;
        
        int currentGradient = Mathf.Clamp(Mathf.FloorToInt(t),
            0, backGroundGradients1.Length - 1);
        
        t -= currentGradient;
        
        _targetBgColor1 = backGroundGradients1[currentGradient].Evaluate(t);
        _targetBgColor2 = backGroundGradients2[currentGradient].Evaluate(t);
    }
    
    private void SetDefaultBgColor()
    {
        _targetBgColor1 = backGroundGradients1[0].Evaluate(0f);
        _targetBgColor2 = backGroundGradients2[0].Evaluate(0f);

        _bgColor1 = _targetBgColor1;
        _bgColor2 = _targetBgColor2;

        _backgroundMaterial.SetColor("_Color1", _targetBgColor1);
        _backgroundMaterial.SetColor("_Color2", _targetBgColor2);
    }
    
    
    private void ChangeBgColor()
    {
        _bgColor1 = Color.Lerp(_bgColor1, _targetBgColor1,Time.deltaTime / 2f);
        _bgColor2 = Color.Lerp(_bgColor2, _targetBgColor2,Time.deltaTime / 2f);

        _backgroundMaterial.SetColor("_Color1", _bgColor1);
        _backgroundMaterial.SetColor("_Color2", _bgColor2);
        
        // Texture2D bg = new Texture2D(2, 2);
        // bg.SetPixel(0, 1, backgroundImage.color);
        // bg.SetPixel(0, 0, Color.green);
        // bg.SetPixel(1, 1, backgroundImage.color);
        // bg.SetPixel(1, 0, Color.green);
        //
        // bg.filterMode = FilterMode.Bilinear;
        //
        // Sprite bg3 = Sprite.Create(bg, new Rect(0f, 0f, 2, 2),
        //     new Vector2(0f, 0f));
        //
        // backgroundImage.sprite = bg3;
        /*_playerCam.backgroundColor = Color.Lerp(_playerCam.backgroundColor,
            targetBgColor, Time.deltaTime / 2f);*/
    }

    #endregion
    
    
    private void MoveCameraTargetPos()
    {
        float x, z;
        Vector3 camPos = _playerCam.transform.localPosition;

        camPos.y = cameraStartPos.y + lastCube.y - 1f;
        

        // move camera backward
        if (Mathf.Abs(lastCube.z) > cameraMoveThreshold || Mathf.Abs(lastCube.x) > cameraMoveThreshold)
            foreach (var pos in cubesPositions)
            {
                x = Mathf.Abs(pos.x);
                z = Mathf.Abs(pos.z);
                
                 if(maxZx < x || maxZx < z || isCameraMoveForward)
                    maxZx = x > z ? x : z;
            }
        
        if (maxZx > 0)
            camPos.z = cameraStartPos.z - (maxZx * cameraMoveBackMul);


        cameraTargetPos = new Vector3(0,camPos.y,camPos.z);
    }

    private void MoveCamera()
    {
        Vector3 camPos = _playerCam.transform.localPosition;
        camPos = Vector3.MoveTowards(camPos, cameraTargetPos, Time.deltaTime * camMovSpeed);
        _playerCam.transform.localPosition = camPos;
    }

    private void MoveCameraTiltDirection()
    {
        cameraViewDirection.localPosition = new Vector3(0, 0, 9f + lastCube.y - 1f);
    }

    private void TiltCamera()
    {
        Transform camTransform = _playerCam.transform;
        Vector3 viewDirrection = cameraViewDirection.position - camTransform.position;
        camTransform.rotation = Quaternion.Lerp(camTransform.rotation, Quaternion.LookRotation
                (viewDirrection, Vector3.up), Time.deltaTime / 3f);
    }

    private void MoveSpawner()
    {
        List<Vector3> positions = new List<Vector3>(); //opt некешированный лист
        Vector3 newPosition = Vector3.zero;

        if (!isTestModOnlyUp)
        {
            //-floats comparasion
            if (IsPositionEmpty(new Vector3(lastCube.x + 1, lastCube.y, lastCube.z)) && lastSpawnerVector.x != 1)
                positions.Add(new Vector3(lastCube.x + 1, lastCube.y, lastCube.z));
            
            if (IsPositionEmpty(new Vector3(lastCube.x - 1, lastCube.y, lastCube.z)) && lastSpawnerVector.x != -1)
                positions.Add(new Vector3(lastCube.x - 1, lastCube.y, lastCube.z));

            if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y, lastCube.z + 1)) && lastSpawnerVector.z != 1)
                positions.Add(new Vector3(lastCube.x, lastCube.y, lastCube.z + 1));
            if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y, lastCube.z - 1)) && lastSpawnerVector.z != -1)
                positions.Add(new Vector3(lastCube.x, lastCube.y, lastCube.z - 1));
        }
        
        //the top place is always free
        /*if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y + 1, lastCube.z)))*/       
        if (lastSpawnerVector.y != 1 || isTestModOnlyUp)
        {
            positions.Add(new Vector3(lastCube.x, lastCube.y + 1, lastCube.z)); //20%
            positions.Add(new Vector3(lastCube.x, lastCube.y + 1, lastCube.z)); //33%
        }
        
        //the down placement is a boring gameplay
        //if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y - 1, lastCube.z))
        //    && cubeToPlace.position.y != lastCube.y - 1)
        //    positions.Add(new Vector3(lastCube.x, lastCube.y - 1, lastCube.z));
        
        
        if (positions.Count > 1)
            newPosition = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            LoseGame();
        else
            newPosition = positions[0];

        lastSpawnerVector = newPosition - lastCube.getVector();
        cubeSpawner.position = newPosition;
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y <= 0)
            return false;

        foreach (var position in cubesPositions)
            if (targetPos == position)
                return false;
       
        return true;
    }


    
#if UNITY_EDITOR
    
    [MenuItem("Developer/Switch Test Mod Up Only &#w")]
    private static void SwitchTestMod()
    {
        Instance.isTestModOnlyUp = !Instance.isTestModOnlyUp;
    }
    
#endif
    
    #endregion;
    
    
    struct CubePos
    {
        public int x, y, z;

        public CubePos(int x,int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void setVector (Vector3 pos)
        {
            x = Convert.ToInt32(pos.x);
            y = Convert.ToInt32(pos.y);
            z = Convert.ToInt32(pos.z);
        }
        public Vector3 getVector ()
        {
            return new Vector3(x, y, z);
        }
    }

    public class CubeInfo
    {
        public Transform cube;
        public int cubeId;

        public CubeInfo(Transform transform, int id)
        {
            cube = transform;
            cubeId = id;
        }
    }
    
    

}


