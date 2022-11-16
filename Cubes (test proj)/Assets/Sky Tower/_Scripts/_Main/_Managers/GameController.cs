using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using TMPro;


public class GameController : Singleton<GameController>
{
    public static event Action<int> OnDifficultyChanged;
    public static event Action<int> OnScoreIncrised;

    public static bool isGamePause;

    [Header("Cinematic Game property")]
    [SerializeField]private float camMovSpeed = 2f;
    [Tooltip("How far will the camera move backward when the tower will become wider")]
    [SerializeField]private float cameraMoveBackMul =1.7f;
    [SerializeField]private int cameraMoveThreshold = 2;
    [SerializeField]private float slowMotionScale = 0.55f, slowMotionDuration=1.5f;
    [SerializeField]private float loseCamYMul =1.5f, loseCamZMul =2f, loseCamMovSpeedMul=2f;

    [Header("Spawn Game property")] 
    [SerializeField]private float spawnGamePauseDuration = 0.01f;
    [SerializeField]private float spawnPauseDuration = 0.1f;
    [SerializeField]private float spawnShakeAmount = 0.01f;
    [SerializeField]private float spawnShakeDur = 0.05f;

    [Header("Sound Game property")]
    [SerializeField]private float actionMusicVolumeMul = 0.5f;
    [SerializeField]private int maxWindTowerHeight = 100;
    [Range(0f,1f)] [Tooltip("Add value to Tower Height mul and dot Product mul")]
    [SerializeField]private float minWindFactor=0.25f;
    [SerializeField]private float windVolumeMul=0.5f, windFadingSpeed=0.7f;
    [SerializeField]private float ambColorIntensity = 0.65f;
    
    [Header("Game property")]
    [SerializeField]private int colorScoreStep = 5;
    [SerializeField]private Color[] bgColors;
    
    [SerializeField]private float[] timeToAutoPlace, timeToPlace;
    [SerializeField]private int[] difLevelsThresholds;
    
    [SerializeField]private Color[] phaseColors;
    [SerializeField]private Vector3 phaseLightIntensity;
    [SerializeField]private Vector3 phaseSpawnerFlicker;

    
    private int curLevel = 0;


    [Header("References")]
    public GameObject allCubes;
    [SerializeField]private TextMeshProUGUI scoreText;
    [SerializeField]private Transform cubeSpawner;
    [SerializeField]private Transform cameraViewDirection;
    [SerializeField]private GameObject restartButton;
    [SerializeField]private CubeScriptable cubeToCreate;
    [SerializeField]private GameObject[] canvasMenu;

    private AudioSource windSource;

    private List<CubeInfo> cubeInfos = new List<CubeInfo>();
    private List<CubeScriptable> cubesToCreate;
    private Rigidbody allCubesRb;
    
    
    private CubePos lastCube = new CubePos(0, 1, 0);
    private Color targetBgColor;
    private Vector3 cameraTargetPos,cameraStartPos;
    private Vector3 lastSpawnerVector = Vector3.up;
    private float timerAutoPlace;
    private float maxZx;
    private bool isGameLost,isGameStart;
    private bool achievmentsOpened;
    private bool isSpawnPause;
    
    private List<Vector3> cubesPositions = new List<Vector3>
    {
        new Vector3(0, 1, 0)
    };
    
    
    private Coroutine showCubePlace;
    private static Camera _playerCam;
    
    
    private const string LastScore = "lastScore";
    private const string BestScore = "bestScore";
    
    
    
    private void OnEnable()
    {
        AchievmentsButtons.OnAchievmentsWindowOpen += SetAchievmentsOpened;  
    }
    private void OnDisable()
    {
        AchievmentsButtons.OnAchievmentsWindowOpen -= SetAchievmentsOpened;
    }

    protected override void Awake()
    {
        base.Awake();
        _playerCam = Camera.main;
    }

    private void Start()
    {
        windSource = SoundManager.Instance.GetSoundSource("Wind");
        allCubesRb = allCubes.GetComponent<Rigidbody>();
        
        
#if UNITY_EDITOR
        if (timeToAutoPlace.Length != timeToPlace.Length || timeToAutoPlace.Length != difLevelsThresholds.Length)
                   Debug.LogError("Warning arrays(haven't make custom class yet): timeToAutoPlace.Length != timeToPlace.Length || timeToAutoPlace.Length != difLevelsThresholds.Length"); 
#endif
        
        
        cubesToCreate = new List<CubeScriptable>();
        foreach (var cube in CubesManager.Instance.cubesDict)
        {
            if (cube.Value.active)
            {
                cubesToCreate.Add(cube.Value);
            }
        }

        
        ResetAutoPlaceTimer(curLevel);

        targetBgColor = _playerCam.backgroundColor;
        cameraStartPos = _playerCam.transform.localPosition;
        cameraTargetPos = cameraStartPos;
        
        
        CreateCube(new Vector3(0, 1, 0));
        showCubePlace = StartCoroutine(ShowCubePlace());  
    }
    
    private void Update()
    {
        if(isGamePause || isSpawnPause) return;
        
        if(isGameLost)
            TiltCamera();

        MoveCamera();
        ChangeBgColor();
        ChangeAmbientLight(_playerCam.backgroundColor, ambColorIntensity);


        bool isTowerDestroyed = allCubes == null;
        bool isGameContinue = !achievmentsOpened && !isGameLost && !isTowerDestroyed && cubeSpawner != null;
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && isGameContinue && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif

            if (!isGameStart)
                StartGame();

            InitializeCubeCreation();
            InitializeSpawnerMovement();
            
        }


        
        if (isGameStart && !isGameLost && allCubesRb.velocity.magnitude >= 0.18f)
            LoseGame();


        if (!isTowerDestroyed && isGameLost)
        {
            //
            float dotProduct = Vector3.Dot(Vector3.down, allCubesRb.velocity.normalized);
            float windFactor = Mathf.Clamp01(dotProduct + minWindFactor);
            float towerHeightMul = Mathf.Clamp01(minWindFactor + (float) lastCube.y / maxWindTowerHeight);
            windSource.volume = windFactor * windVolumeMul * towerHeightMul;
            windSource.pitch = 0.75f + dotProduct;
        }
        else if(isTowerDestroyed)
        {
            //
            windSource.volume = Mathf.MoveTowards(windSource.volume, 0f, windFadingSpeed * Time.deltaTime);
        }
    }

    
    
    private IEnumerator ShowCubePlace()
    {
        while (true)
        {
            if (isGameStart)
            {
                if(timerAutoPlace <= 0f)
                {
                    InitializeCubeCreation();
                }
                else if (timerAutoPlace > 0f)
                {
                    timerAutoPlace -= timeToPlace[curLevel];

                    if (timerAutoPlace < 2f * timeToPlace[curLevel])
                    {
                        LampColorManager.Instance.ChangeLampColor(phaseColors[2], phaseLightIntensity.z,phaseSpawnerFlicker.z);
                    }
                    else if (timerAutoPlace < 4f * timeToPlace[curLevel])
                    {
                        LampColorManager.Instance.ChangeLampColor(phaseColors[1], phaseLightIntensity.y, phaseSpawnerFlicker.y);
                    }
                    else
                    {
                        LampColorManager.Instance.ChangeLampColor(phaseColors[0], phaseLightIntensity.x, phaseSpawnerFlicker.x);
                    }
                }
            }
            
            InitializeSpawnerMovement();
            yield return new WaitForSeconds(timeToPlace[curLevel]);
        }
    }
    
    private IEnumerator PauseGame(float t)
    {
        Time.timeScale = 0f;
        //AudioListener.pause = true;
        isGamePause = true;
        
        yield return new WaitForSecondsRealtime(t);
        
        Time.timeScale = 1f;
        //AudioListener.pause = false;
        isGamePause = false;
    }
    
    private IEnumerator PauseSpawn(float t)
    {
        isSpawnPause = true;
        
        yield return new WaitForSecondsRealtime(t);

        isSpawnPause = false;
    }
    
    private IEnumerator SlowDownGame(float scale, float t)
    {
        Time.timeScale = scale;

        yield return new WaitForSecondsRealtime(t);
        
        Time.timeScale = 1f;
    }
    
    
    
    #region Methods;
    
    

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
        SoundManager.Instance.PlaySound("Wind");
        SoundManager.Instance.ResetMusicVolume();
        
        isGameLost = true;

        Destroy(LampColorManager.Instance.gameObject);
        Destroy(cubeSpawner.gameObject);
        StopCoroutine(showCubePlace);
        restartButton.SetActive(true);
        
        cameraTargetPos = new Vector3(cameraTargetPos.x, -cameraTargetPos.z * loseCamYMul, cameraTargetPos.z * loseCamZMul);
        camMovSpeed *= loseCamMovSpeedMul;
        
        allCubesRb.velocity *= 1.1f;
    }

    private void StartGame()
    {
        var curMusVol = SoundManager.Instance.GetMusicVolume();
        SoundManager.Instance.SetMusicVolume(actionMusicVolumeMul * curMusVol);
        isGameStart = true;
        
        PlayerPrefs.SetInt("lastScore", 0);
        scoreText.text = $"Score: 0";
        
        foreach (GameObject obj in canvasMenu)
            Destroy(obj);
    }

    private void SetAchievmentsOpened(bool windowState)
    {
        achievmentsOpened = windowState;
    }

    private void ResetAutoPlaceTimer(int curLevel)
    {
        timerAutoPlace = timeToAutoPlace[curLevel];
    }

    private void ChangeAmbientLight(Color ambientColor, float ambColorIntensity = 1f)
    {
        ambientColor = new Color(ambientColor.r * ambColorIntensity, ambientColor.g * ambColorIntensity, ambientColor.b * ambColorIntensity, ambientColor.a);
        RenderSettings.ambientLight = ambientColor;
    }

    private void InitializeSpawnerMovement()
    {
        MoveSpawner();
        LampColorManager.Instance.gameObject.transform.position = new Vector3(cubeSpawner.transform.position.x, cubeSpawner.transform.position.y + 2, cubeSpawner.transform.position.z);
    }

    private void InitializeCubeCreation()
    {
        StartCoroutine(PauseGame(spawnGamePauseDuration));
        StartCoroutine(PauseSpawn(spawnPauseDuration));
        ResetAutoPlaceTimer(curLevel);
        
        
        Vector3 vfxDir = cubeSpawner.position - lastCube.getVector();
        
        CreateCube(cubeSpawner.position);

        
        Vector3 vfxPos = lastCube.getVector();
        vfxPos.x -= vfxDir.x/2;
        vfxPos.z -= vfxDir.z/2;
        if (vfxDir.y < 0.1f) vfxPos.y += 0.5f;
 
        Quaternion vfxRotation = Quaternion.LookRotation(vfxDir, Vector3.up) * Quaternion.Euler(90, 0, 0);
        VfxManager.Instance.PlaySpawnVfx(vfxPos, vfxRotation, cubeToCreate.cubeId);
        
        
        
        CameraShaker.Instance.ShakeCamera(spawnShakeAmount,spawnShakeDur);
        SoundManager.Instance?.PlaySound("CubeSpawn");
        LampColorManager.Instance.ChangeLampColor(phaseColors[0], phaseLightIntensity.x, phaseSpawnerFlicker.x);

        
        ChangeScore();
        ChangeTargetBgColor();
        MoveCameraTargetPos();
        MoveCameraTiltDirection();
        IncriseDifficulty();
    }

    private void IncriseDifficulty()
    {
        for (int i = difLevelsThresholds.Length - 1; i >= 0; i--)
        {
            if (lastCube.y > difLevelsThresholds[i])
            {
                if(curLevel != i)
                {
                    OnDifficultyChanged?.Invoke(i);
                    curLevel = i;
                }
                return;
            }
        }

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

    private void ChangeScore()
    {
        int currentScore = PlayerPrefs.GetInt(LastScore);
        int totalScore = PlayerPrefs.GetInt(BestScore);
        int currentPos = lastCube.y - 1;

        if (currentScore < currentPos)
        {
            PlayerPrefs.SetInt(LastScore, currentPos);
            OnScoreIncrised.Invoke(currentPos);
        }    
            

        if (totalScore < currentPos)
            PlayerPrefs.SetInt(BestScore, currentPos);

        scoreText.text = $"Score: {PlayerPrefs.GetInt(LastScore)}";
    }

    private void ChangeTargetBgColor()
    {
            targetBgColor = bgColors[Mathf.Clamp((lastCube.y - 1) / colorScoreStep, 0, bgColors.Length - 1)];
    }

    private void ChangeBgColor()
    {
        _playerCam.backgroundColor = Color.Lerp(_playerCam.backgroundColor, targetBgColor, Time.deltaTime / 2f);
    }

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
                if(maxZx < x || maxZx < z)
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
        Vector3 viewDirrection = cameraViewDirection.position - _playerCam.transform.position;
        _playerCam.transform.rotation = Quaternion.Lerp(_playerCam.transform.rotation, Quaternion.LookRotation(viewDirrection, Vector3.up), Time.deltaTime / 3f);
    }

    private void MoveSpawner()
    {
        List<Vector3> positions = new List<Vector3>();
        Vector3 newPosition = Vector3.zero;

        if (IsPositionEmpty(new Vector3(lastCube.x + 1, lastCube.y, lastCube.z)) && lastSpawnerVector.x != 1)
            positions.Add(new Vector3(lastCube.x + 1, lastCube.y, lastCube.z));
        if (IsPositionEmpty(new Vector3(lastCube.x - 1, lastCube.y, lastCube.z)) && lastSpawnerVector.x != - 1)
            positions.Add(new Vector3(lastCube.x - 1, lastCube.y, lastCube.z));

        /*if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y + 1, lastCube.z)))*/       //the top place is always free
        if (lastSpawnerVector.y != 1)
        {
            positions.Add(new Vector3(lastCube.x, lastCube.y + 1, lastCube.z)); //20%
            positions.Add(new Vector3(lastCube.x, lastCube.y + 1, lastCube.z)); //33%
        }
        

        //if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y - 1, lastCube.z))          //the down placement is a boring gameplay
        //    && cubeToPlace.position.y != lastCube.y - 1)
        //    positions.Add(new Vector3(lastCube.x, lastCube.y - 1, lastCube.z));

        if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y, lastCube.z + 1)) && lastSpawnerVector.z != 1)
            positions.Add(new Vector3(lastCube.x, lastCube.y, lastCube.z + 1));
        if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y, lastCube.z - 1)) && lastSpawnerVector.z != - 1)
            positions.Add(new Vector3(lastCube.x, lastCube.y, lastCube.z - 1));

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
