using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class SharedUIController : MonoBehaviour
{
    public static SharedUIController Instance { get; private set; }
    private float[] fpsBuffer = new float[100];
    private int fpsBufferIndex = 0;
    [SerializeField] private float timeBetweenFPSUpdates = 0.1f;
    private float fpsCoolDown = 0f;

    [SerializeField]
    private UIDocument uiDocument;
    private Button ToggleJobsBtn;

    private Button OOPSceneBtn, ECSSceneBtn, OOPPhysicsSceneBtn, EcsPhysicsSceneBtn, CustomECSBtn, ExitBtn;
    private Label Scenario;

    private Label FPSCounter;
    private Label SquareCounter;

    private SliderInt SquaresPerSecondSlider;
    private Label SquaresPerSecondFeedback;

    private OOPSquareSpawner spawner;
    private OOPPhysicsSquareSpawner physicsSpawner;
    private FakeEcsSpawner fakeEcsSpawner;
    //ECS will be handled by the bridge system.
    OOPBridgeSystem bridge;

    public string JobIndicatorLabel = "";
    public bool JobsEnabled = false;

    public int ReportedNumOfSquares { get; set; } = -1;

    private bool initialized = false;

    public void Awake()
    {
        //Alright... so mostly by accident I have learned that Start is only once, while Awake is called whenever the scene is changed.
        //TODO - Because of this I should reconsider where I put the following, but for now "initialized" should be enough.

        if (initialized) return;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this)
        {
            if(gameObject != null) Destroy(gameObject);
            return;
        }

        if (uiDocument == null)
        {
            if (TryGetComponent(out UIDocument doc))
            {
                uiDocument = doc;
            }
            else
            {
                Debug.LogError("UIDocument component not found on the GameObject.");
                return;
            }
        }

        VisualElement root = uiDocument.rootVisualElement;

        ECSSceneBtn = root.Q<Button>("ECS_MovingSquares");
        OOPSceneBtn = root.Q<Button>("OOP_MovingSquares");
        CustomECSBtn = root.Q<Button>("ECS_Custom");
        OOPPhysicsSceneBtn = root.Q<Button>("OOP_2D_Physics");
        EcsPhysicsSceneBtn = root.Q<Button>("ECS_Constrained3D_Physics");
        ExitBtn = root.Q<Button>("ExitBtn");

        ToggleJobsBtn = root.Q<Button>("JobsToggle");
        if(ToggleJobsBtn != null)
        {
            if(JobsEnabled)
            {
                ToggleJobsBtn.style.backgroundColor = Color.green;
            } else
            {
                ToggleJobsBtn.style.backgroundColor = Color.red;
            }
        }
            

        Scenario = root.Q<Label>("Scenario");

        FPSCounter = root.Q<Label>("FPS_Counter");
        SquareCounter = root.Q<Label>("Square_Counter");


        SquaresPerSecondSlider = root.Q<SliderInt>("SquaresPerSecondSlider");
        SquaresPerSecondFeedback = root.Q<Label>("SquaresPerSecondFeedback");
        PopulateReferences();
        SetSpawnRate(SquaresPerSecondSlider.value);


        initialized = true;
    }

    public void Update()
    {
        fpsBuffer[fpsBufferIndex] = 1f / Time.unscaledDeltaTime;
        fpsBufferIndex = (fpsBufferIndex + 1) % fpsBuffer.Length;


        fpsCoolDown -= Time.deltaTime;
        if (fpsCoolDown <= 0f)
        {
            fpsCoolDown = timeBetweenFPSUpdates;
            float averageFPS = 0f;
            foreach (float fps in fpsBuffer)
            {
                averageFPS += fps;
            }
            averageFPS /= fpsBuffer.Length;
            FPSCounter.text = $"FPS: {averageFPS:0.0}";
            SquareCounter.text = $"Squares: {(ReportedNumOfSquares >= 0 ? ReportedNumOfSquares.ToString() : "N/A")}";
        }
    }

    //https://docs.unity3d.com/6000.1/Documentation/Manual/execution-order.html
    //As far as I can tell OnEnable happens before Start.
    public void OnEnable()
    {
        if (Instance != this) return;

        ExitBtn.clicked += ExitGame;
        OOPSceneBtn.clicked += LoadOOP2DScene;
        ECSSceneBtn.clicked += LoadECS2DScene;
        CustomECSBtn.clicked += LoadECSCustomScene;
        OOPPhysicsSceneBtn.clicked += LoadOOP2DPhysicsScene;
        EcsPhysicsSceneBtn.clicked += LoadECSFake2DPhysicsScene;
        ToggleJobsBtn.clicked += ToggleJobSystem;

        if (SquaresPerSecondSlider != null)
        {
            SquaresPerSecondSlider.RegisterValueChangedCallback(OnSpawnRateSliderChange);
        }
    }

    public void OnDisable()
    {
        if (Instance != this) return;

        ExitBtn.clicked -= ExitGame;
        OOPSceneBtn.clicked -= LoadOOP2DScene;
        ECSSceneBtn.clicked -= LoadECS2DScene;
        CustomECSBtn.clicked -= LoadECSCustomScene;
        OOPPhysicsSceneBtn.clicked -= LoadOOP2DPhysicsScene;
        EcsPhysicsSceneBtn.clicked -= LoadECSFake2DPhysicsScene;
        ToggleJobsBtn.clicked -= ToggleJobSystem;

        if(SquaresPerSecondSlider != null)
        {
            SquaresPerSecondSlider.UnregisterValueChangedCallback(OnSpawnRateSliderChange);
        }
    }

    public void LoadScene(string sceneName, string sceneTitle)
    {
        StartCoroutine(LoadSceneAsync(sceneName, sceneTitle));
    }

    //https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.LoadSceneAsync.html
    //Took a bit of time for my foggy brain, but thankfully there is unity documentation.
    //I try to document it here and there in the code, because these are not directly relevant for the assignment, but still indicative of what I have done.
    //There are however many cases where I have forgotten or otherwise not done so.
    //Anyways, it is needed only to really ensure that there are handles for the spawners so I can use the slider to set their rates and so I can set the spawn rate "text".
    public IEnumerator LoadSceneAsync(string sceneName, string sceneTitle)
    {
        ReportedNumOfSquares = 0;
        if (sceneName == "CleanScene") ReportedNumOfSquares = -1;
        AsyncOperation loadingNextScene = SceneManager.LoadSceneAsync(sceneName);
        while (!loadingNextScene.isDone)
        {
            yield return null;
        }
        Scenario.text = sceneTitle;
        PopulateReferences();
        SetSpawnRate(SquaresPerSecondSlider.value);
        System.GC.Collect(); //Forcing the garbage collector to collect garbage. I hope this will result in more accurate tests.


    }

    public void PopulateReferences()
    {
        spawner = FindFirstObjectByType<OOPSquareSpawner>();
        physicsSpawner = FindFirstObjectByType<OOPPhysicsSquareSpawner>();
        fakeEcsSpawner = FindFirstObjectByType<FakeEcsSpawner>();
        bridge = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<OOPBridgeSystem>();
    }

    private void LoadOOP2DScene() => LoadScene("OOP2DScene", "Spawning traditional game objects");
    private void LoadECS2DScene() => LoadScene("ECS2DScene", $"Spawning ECS game objects {JobIndicatorLabel}");
    private void LoadECSCustomScene() => LoadScene("CustomECS", "Spawning custom \"ECS\" game objects");
    private void LoadOOP2DPhysicsScene() => LoadScene("OOP2DPhysicsScene", "Spawning traditional game objects with physics");
    private void LoadECSFake2DPhysicsScene() => LoadScene("ECSFake2DPhysicsScene", "Spawning ECS game objects with fake physics");
    private void OnSpawnRateSliderChange(ChangeEvent<int> evt)
    {
        int newSpawnRate = evt.newValue;
        SetSpawnRate(newSpawnRate);
    }

    private void ToggleJobSystem()
    {
        World ecsWorld = World.DefaultGameObjectInjectionWorld;

        SystemHandle StandardMoverSystem_Handle = ecsWorld.GetExistingSystem<SquareMoverSystem>();
        SystemHandle JobMoverSystem_Handle = ecsWorld.GetExistingSystem<JobSquareMoverSystem>();

        //I turns out if you try to do the below it does not "resolve".*
        /*
        SystemState standardMoverSystem_State = ecsWorld.Unmanaged.ResolveSystemStateRef(StandardMoverSystem_Handle);
        SystemState jobMoverSystem_State = ecsWorld.Unmanaged.ResolveSystemStateRef(JobMoverSystem_Handle);
        */

        JobsEnabled = !JobsEnabled;

        //*I must say it is quite alien to me, that I cannot just "reference" an object outside of this "instant", but the more you know.
        ecsWorld.Unmanaged.ResolveSystemStateRef(StandardMoverSystem_Handle).Enabled = !JobsEnabled;
        ecsWorld.Unmanaged.ResolveSystemStateRef(JobMoverSystem_Handle).Enabled = JobsEnabled;

        if(JobsEnabled)
        {
            JobIndicatorLabel = "(Jobs)";
            ToggleJobsBtn.style.backgroundColor = Color.green;
        }
        else
        {
            JobIndicatorLabel = "";
            ToggleJobsBtn.style.backgroundColor = Color.red;
        }

        ECSSceneBtn.text = $"ECS 2D {JobIndicatorLabel}";
    }

    private void SetSpawnRate(int newSpawnRate)
    {
        
        //https://docs.unity3d.com/Packages/com.unity.entities@6.5/api/Unity.Entities.World.GetExistingSystemManaged.html
        //GetExistingSystemManaged might not be safe (But I think that is for threads only and the null check might be enough.).
        //I could not figure out how else to set values in ECS (at the time of writing and specifically for the purpose of making it happen only in the event.).


        if (bridge != null)
        {
            bridge.SetSpawnRate(newSpawnRate);
        }

        if (spawner != null)
        {
            spawner.spawnRate = newSpawnRate;
        }
        if (physicsSpawner != null)
        {
            physicsSpawner.spawnRate = newSpawnRate;
        }
        if (fakeEcsSpawner != null)
        {
            fakeEcsSpawner.spawnRate = newSpawnRate;
        }

        if (SquaresPerSecondFeedback != null)
        {
            SquaresPerSecondFeedback.text = $"{newSpawnRate}";
        }
    }
    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
