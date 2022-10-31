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

    public static bool gamePause;

    [Header("Cinematic Game property")]
    [SerializeField]private float camMovSpeed = 2f;
    [SerializeField]private float cameraMoveBackMul =1.7f;
    [SerializeField]private int cameraMoveThreshold = 2;
    [SerializeField]private float slowMotionScale = 0.55f, slowMotionDuration=1.5f;
    [SerializeField]private float loseCamYMul =1.5f, loseCamZMul =2f, loseCamMovSpeedMul=2f;

    [Header("Spawn Game property")] 
    [SerializeField]private float spawnGamePauseDuration = 0.01f;
    [SerializeField]private float spawnPauseDuration = 0.1f;
    [SerializeField]private float spawnShakeAmount = 0.01f;
    [SerializeField]private float spawnShakeDur = 0.05f;
    
    [Header("Game property")] 
    public float ambColorIntensity;
    public int colorScoreStep = 10;
    public Color[] bgColors;
    
    public float[] timeToAutoPlace, timeToPlace;
    public int[] difLevelsThresholds;
    
    public Color[] phaseColors;
    public Vector3 phaseLightIntensity;
    public Vector3 phaseSpawnerFlicker;

    
    private int curLevel = 0;


    [Header("References")]
    public TextMeshProUGUI score;
    public Transform cubeSpawner;
    public Transform cameraViewDirection;
    public GameObject allCubes, restartButton;
    public CubeScriptable cubeToCreate;
    public GameObject[] canvasMenu;

    
    
    private List<CubeInfo> cubeInfos = new List<CubeInfo>();
    private List<CubeScriptable> cubesToCreate;
    private Rigidbody allCubesRB;

    
    
    private CubePos lastCube = new CubePos(0, 1, 0);
    private Color targetBGColor;
    private Vector3 cameraTargetPos,cameraStartPos;
    private Vector3 lastSpawnerVector = Vector3.up;
    private float timerAutoPlace;
    private float maxZX;
    private bool gameLost,gameStart;
    private bool achievmentsOpened;
    private bool spawnPause;
    
    List<Vector3> cubesPositions = new List<Vector3>
    {
        new Vector3(0, 1, 0)
    };
    
    
    private Coroutine showCubePlace;
    public static Camera playerCam;
    
    
    string lastScore = "lastScore";
    string bestScore = "bestScore";
    
    
    
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
        playerCam = Camera.main;
    }

    private void Start()
    {
        spawnPause = false; //just in case
        
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

        
        ResetTimer(curLevel);

        targetBGColor = playerCam.backgroundColor;
        cameraStartPos = playerCam.transform.localPosition;
        cameraTargetPos = cameraStartPos;

        allCubesRB = allCubes.GetComponent<Rigidbody>();


        CreateCube(new Vector3(0, 1, 0));
        showCubePlace = StartCoroutine(ShowCubePlace());  
    }
    
    private void Update()
    {
        if(gamePause || spawnPause) return;
        
        if(gameLost)
            TiltCamera();

        MoveCamera();
        ChangeBGColor();
        ChangeAmbientLight(playerCam.backgroundColor, ambColorIntensity);
        

        bool gameContinue = !achievmentsOpened && !gameLost && allCubes != null && cubeSpawner != null;
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && gameContinue && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif

            if (!gameStart)
                StartGame();

            InitializeCubeCreation();
            InitializeSpawnerMovement();
            
        }

        if (gameStart && !gameLost && allCubesRB.velocity.magnitude >= 0.18f)
            LoseGame();
    }

    
    
    private IEnumerator ShowCubePlace()
    {
        while (true)
        {
            if (gameStart)
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
                        ColorManager.Instance.ChangeLampColor(phaseColors[2], phaseLightIntensity.z,phaseSpawnerFlicker.z);
                    }
                    else if (timerAutoPlace < 4f * timeToPlace[curLevel])
                    {
                        ColorManager.Instance.ChangeLampColor(phaseColors[1], phaseLightIntensity.y, phaseSpawnerFlicker.y);
                    }
                    else
                    {
                        ColorManager.Instance.ChangeLampColor(phaseColors[0], phaseLightIntensity.x, phaseSpawnerFlicker.x);
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
        gamePause = true;
        
        yield return new WaitForSecondsRealtime(t);
        
        Time.timeScale = 1f;
        //AudioListener.pause = false;
        gamePause = false;
    }
    
    private IEnumerator PauseSpawn(float t)
    {
        spawnPause = true;
        
        yield return new WaitForSecondsRealtime(t);

        spawnPause = false;
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
    
    public bool IsLoose()
    {
        if (gameLost)
            return true;
        return false;
    }

    public void LoseGame()
    {
        gameLost = true;

        Destroy(ColorManager.Instance.gameObject);
        Destroy(cubeSpawner.gameObject);
        StopCoroutine(showCubePlace);
        restartButton.SetActive(true);
        cameraTargetPos = new Vector3(cameraTargetPos.x, -cameraTargetPos.z * loseCamYMul, cameraTargetPos.z * loseCamZMul);
        camMovSpeed *= loseCamMovSpeedMul;
        allCubesRB.velocity *= 1.1f;
    }

    private void StartGame()
    {
        gameStart = true;
        PlayerPrefs.SetInt("lastScore", 0);
        score.text = $"Score: 0";
        foreach (GameObject obj in canvasMenu)
            Destroy(obj);
    }

    private void SetAchievmentsOpened(bool windowState)
    {
        achievmentsOpened = windowState;
    }

    private void ResetTimer(int curLevel)
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
        ColorManager.Instance.gameObject.transform.position = new Vector3(cubeSpawner.transform.position.x, cubeSpawner.transform.position.y + 2, cubeSpawner.transform.position.z);
    }

    private void InitializeCubeCreation()
    {
        StartCoroutine(PauseGame(spawnGamePauseDuration));
        StartCoroutine(PauseSpawn(spawnPauseDuration));
        ResetTimer(curLevel);
        
        
        Vector3 vfxDir = cubeSpawner.position - lastCube.getVector();
        CreateCube(cubeSpawner.position);

        
        Vector3 vfxPos = lastCube.getVector();
        vfxPos.x -= vfxDir.x/2;
        vfxPos.z -= vfxDir.z/2;
        if (vfxDir.y < 0.1f) vfxPos.y += 0.5f;
 
        Quaternion vfxRotation = Quaternion.LookRotation(vfxDir, Vector3.up) * Quaternion.Euler(90, 0, 0);
        VfxManager.Instance.PlaySpawnVfx(vfxPos, vfxRotation, cubeToCreate.cubeId);
        
        
        CameraShaker.Instance.ShakeCamera(spawnShakeAmount,spawnShakeDur);
        SoundManager.Instance.PlayCubeSpawnSound();
        ColorManager.Instance.ChangeLampColor(phaseColors[0], phaseLightIntensity.x, phaseSpawnerFlicker.x);

        
        ChangeScore();
        ChangeTargetBGColor();
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

        allCubesRB.isKinematic = true;
        allCubesRB.isKinematic = false;
        

        CubeInfo cubeInfo = new CubeInfo(newCube.transform, cubeToCreate.cubeId);
        cubeInfos.Add(cubeInfo);
        
    }

    private void ChangeScore()
    {
        int currentScore = PlayerPrefs.GetInt(lastScore);
        int totalScore = PlayerPrefs.GetInt(bestScore);
        int currentPos = lastCube.y - 1;

        if (currentScore < currentPos)
        {
            PlayerPrefs.SetInt(lastScore, currentPos);
            OnScoreIncrised.Invoke(currentPos);
        }    
            

        if (totalScore < currentPos)
            PlayerPrefs.SetInt(bestScore, currentPos);

        score.text = $"Score: {PlayerPrefs.GetInt(lastScore)}";
    }

    private void ChangeTargetBGColor()
    {
            targetBGColor = bgColors[Mathf.Clamp((lastCube.y - 1) / colorScoreStep, 0, bgColors.Length - 1)];
    }

    private void ChangeBGColor()
    {
        playerCam.backgroundColor = Color.Lerp(playerCam.backgroundColor, targetBGColor, Time.deltaTime / 2f);
    }

    private void MoveCameraTargetPos()
    {
        float x, z;
        Vector3 camPos = playerCam.transform.localPosition;

        camPos.y = cameraStartPos.y + lastCube.y - 1f;
        

        // move camera backward
        if (Mathf.Abs(lastCube.z) > cameraMoveThreshold || Mathf.Abs(lastCube.x) > cameraMoveThreshold)
            foreach (var pos in cubesPositions)
            {
                x = Mathf.Abs(pos.x);
                z = Mathf.Abs(pos.z);
                if(maxZX < x || maxZX < z)
                    maxZX = x > z ? x : z;
            }
        if (maxZX > 0)
            camPos.z = cameraStartPos.z - (maxZX * cameraMoveBackMul);


        cameraTargetPos = new Vector3(0,camPos.y,camPos.z);
    }

    private void MoveCamera()
    {
        Vector3 camPos = playerCam.transform.localPosition;
        camPos = Vector3.MoveTowards(camPos, cameraTargetPos, Time.deltaTime * camMovSpeed);
        playerCam.transform.localPosition = camPos;
    }

    private void MoveCameraTiltDirection()
    {
        cameraViewDirection.localPosition = new Vector3(0, 0, 9f + lastCube.y - 1f);
    }

    private void TiltCamera()
    {
        Vector3 viewDirrection = cameraViewDirection.position - playerCam.transform.position;
        playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, Quaternion.LookRotation(viewDirrection, Vector3.up), Time.deltaTime / 3f);
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
