using UnityEngine;
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
    private Label Scenario;

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

        for(int test = 0; test < 10_000_000; test++)
        {
            // Just to make sure the loading screen is visible for a while
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

        ECSSceneBtn = uiDocument.rootVisualElement.Q<Button>("ECS_MovingSquares");
        OOPSceneBtn = uiDocument.rootVisualElement.Q<Button>("OOP_MovingSquares");
        CustomECSBtn = uiDocument.rootVisualElement.Q<Button>("ECS_Custom");
        OOPPhysicsSceneBtn = uiDocument.rootVisualElement.Q<Button>("OOP_2D_Physics");
        EcsPhysicsSceneBtn = uiDocument.rootVisualElement.Q<Button>("ECS_Constrained3D_Physics");
        ExitBtn = uiDocument.rootVisualElement.Q<Button>("ExitBtn");
        FPSCounter = uiDocument.rootVisualElement.Q<Label>("FPS_Counter");
        Scenario = uiDocument.rootVisualElement.Q<Label>("Scenario");
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
    }

    public void LoadScene(string sceneName)
    {
        Scenario.text = sceneName;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
    }

    private void LoadOOP2DScene() => LoadScene("OOP2DScene");
    private void LoadECS2DScene() => LoadScene("ECS2DScene");
    private void LoadECSCustomScene() => LoadScene("ECS_Custom");
    private void LoadOOP2DPhysicsScene() => LoadScene("OOP2DPhysicsScene");
    private void LoadECSFake2DPhysicsScene() => LoadScene("ECSFake2DPhysicsScene");

    public void ExitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
