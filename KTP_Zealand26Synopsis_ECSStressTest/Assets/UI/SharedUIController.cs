using UnityEngine;
using Unity.Entities;
using UnityEngine.UIElements;

public class SharedUIController : MonoBehaviour
{
    public static SharedUIController Instance { get; private set; }
    private float[] fpsBuffer = new float[100];
    private int fpsBufferIndex = 0;
    [SerializeField] private float timeBetweenFPSUpdates = 0.1f;
    private float fpsCoolDown = 0f;

    [SerializeField]
    private UIDocument uiDocument;
    private Button OOPSceneBtn, ECSSceneBtn, OOPPhysicsSceneBtn, EcsPhysicsSceneBtn, CustomECSBtn, ExitBtn;
    private Label FPSCounter;
    private Label SquareCounter;
    private Label Scenario;

    private SliderInt SquaresPerSecondSlider;
    private Label SquaresPerSecondFeedback;

    private OOPSquareSpawner spawner;
    private OOPPhysicsSquareSpawner physicsSpawner;
    //ECS will be handled by the bridge system.

    public int ReportedNumOfSquares { get; set; } = -1;

    public void Awake()
    {
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
        FPSCounter = root.Q<Label>("FPS_Counter");
        SquareCounter = root.Q<Label>("Square_Counter");
        Scenario = root.Q<Label>("Scenario");


        SquaresPerSecondSlider = root.Q<SliderInt>("SquaresPerSecondSlider");
        SquaresPerSecondFeedback = root.Q<Label>("SquaresPerSecondFeedback");

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

        if(SquaresPerSecondSlider != null)
        {
            SquaresPerSecondSlider.RegisterValueChangedCallback(ChangeSpawnRates);
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
        if(SquaresPerSecondSlider != null)
        {
            SquaresPerSecondSlider.UnregisterValueChangedCallback(ChangeSpawnRates);
        }
    }

    public void LoadScene(string sceneName)
    {
        ReportedNumOfSquares = -1;
        ReportedNumOfSquares = 0;
        Scenario.text = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    private void LoadOOP2DScene() => LoadScene("OOP2DScene");
    private void LoadECS2DScene() => LoadScene("ECS2DScene");
    private void LoadECSCustomScene() => LoadScene("ECS_Custom");
    private void LoadOOP2DPhysicsScene() => LoadScene("OOP2DPhysicsScene");
    private void LoadECSFake2DPhysicsScene() => LoadScene("ECSFake2DPhysicsScene");

    private void ChangeSpawnRates(ChangeEvent<int> evt)
    {
        int newSpawnRate = evt.newValue;

        OOPBridgeSystem bridge = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<OOPBridgeSystem>();
        //https://docs.unity3d.com/Packages/com.unity.entities@6.5/api/Unity.Entities.World.GetExistingSystemManaged.html
        //GetExistingSystemManaged might not be safe (But I think that is for threads only and the null check might be enough.).
        //I could not figure out how else to set values in ECS (at the time of writing and specifically for the purpose of making it happen only in the event.).
        if (bridge != null)
        {
            bridge.SetSpawnRate(newSpawnRate);
        } else Debug.LogError("OOPBridgeSystem not found.");

        if(spawner != null)
        {
            spawner.spawnRate = newSpawnRate;
        }
        if(physicsSpawner != null)
        {
            physicsSpawner.spawnRate = newSpawnRate;
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
