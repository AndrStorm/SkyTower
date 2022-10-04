using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class GameController : MonoBehaviour
{
    public static event Action<int> OnDifficultyChanged;
    public static event Action<int> OnScoreIncrised;

    [Header("Game property")]
    public float camMovSpeed = 2f;
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
    public Text score;
    public Transform cubeSpawner;
    public Transform cameraViewDirection;
    public GameObject allCubes, restartButton, exploseOnSpawn;
    public GameObject cubeToCreate;
    public GameObject[] vfxsOnSpawn;
    public List<GameObject> cubesToCreate;
    public GameObject[] canvasMenu;


    private CubePos lastCube = new CubePos(0, 1, 0);
    private Color targetBGColor;
    private Vector3 cameraTargetPos,cameraStartPos, lastSpawnerVector = Vector3.up;
    private float maxZX = 0f, timerAutoPlace;
    private bool gameLost,gameStart,achievmentsOpened;
    private Rigidbody allCubesRB;
    private Coroutine showCubePlace;

    public static Camera playerCam;
    string lastScore = "lastScore";
    string bestScore = "bestScore";

    List<Vector3> cubesPositions = new List<Vector3>
    {
        new Vector3(0, 1, 0)
    };

    private void OnEnable()
    {
        AchievmentsButtons.OnAchievmentsWindowOpen += SetAchievmentsOpened;  
    }
    private void OnDisable()
    {
        AchievmentsButtons.OnAchievmentsWindowOpen -= SetAchievmentsOpened;
    }

    private void Awake()
    {
        playerCam = Camera.main;
    }

    private void Start()
    {
        if (timeToAutoPlace.Length != timeToPlace.Length || timeToAutoPlace.Length != difLevelsThresholds.Length)
            Debug.LogError("Warning: timeToAutoPlace.Length != timeToPlace.Length || timeToAutoPlace.Length != difLevelsThresholds.Length");

        
        for (int i = cubesToCreate.Count - 1; i >= 0; i--)
        {
            if (PlayerPrefs.GetInt($"Cube{i + 1}") == 0)
                cubesToCreate.RemoveAt(i);
        }

        ResetTimer(curLevel);

        targetBGColor = playerCam.backgroundColor;
        cameraStartPos = playerCam.transform.localPosition;
        cameraTargetPos = cameraStartPos;

        allCubesRB = allCubes.GetComponent<Rigidbody>();


        CreateCube(new Vector3(0, 1, 0));
        showCubePlace = StartCoroutine(ShowCubePlace());  
    }

    

    IEnumerator ShowCubePlace()
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

    private void Update()
    {
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

    #region Methods;
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
        cameraTargetPos = new Vector3(cameraTargetPos.x, -cameraTargetPos.z * 1.5f, cameraTargetPos.z * 2f);     
    }

    private void StartGame()
    {
        gameStart = true;
        PlayerPrefs.SetInt("lastScore", 0);
        score.text = $"Score: {PlayerPrefs.GetInt("lastScore")}";
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
        ResetTimer(curLevel);

        CreateCube(cubeSpawner.position);

        foreach (var vfx in vfxsOnSpawn)
        {
            PlayVFX(vfx,lastCube.getVector(),3f);
        }
        
        //PlayVFX(exploseOnSpawn,lastCube.getVector(),3f);
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
                cubeToCreate,
                position,
                Quaternion.identity) as GameObject;

        newCube.transform.SetParent(allCubes.transform);
        lastCube.setVector(newCube.transform.position);
        cubesPositions.Add(lastCube.getVector());

        allCubesRB.isKinematic = true;
        allCubesRB.isKinematic = false;

    }

    private void PlayVFX(GameObject vfxSample, Vector3 pos, float timeToDestroy)
    {
        GameObject vfx = Instantiate(vfxSample, pos, Quaternion.identity) as GameObject;
        Destroy(vfx, timeToDestroy);
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
        

        if (Mathf.Abs(lastCube.z) > 2 || Mathf.Abs(lastCube.x) > 2)
            foreach (var pos in cubesPositions)
            {
                x = Mathf.Abs(pos.x);
                z = Mathf.Abs(pos.z);
                if(maxZX < x || maxZX < z)
                    maxZX = x > z ? x : z;
            }
        if (maxZX > 0)
            camPos.z = cameraStartPos.z - ((maxZX * maxZX) / (maxZX*0.6f));

        cameraTargetPos = new Vector3(camPos.x,camPos.y,camPos.z);
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

}
