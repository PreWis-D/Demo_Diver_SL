using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bootstraper : MonoBehaviour
{
    [SerializeField] private AudioListener _audioListener;
    [SerializeField] private EventSystem _eventSystem;

    private SceneLoadService _sceneLoadService;

    private void Awake()
    {
        _sceneLoadService = new SceneLoadService(SceneType.Bootstrap);   
        _sceneLoadService.Initialize();
    }

    private void Start()
    {
        // Initialize SDK

        StartGame();
    }

    private async void StartGame()
    {
        DontDestroyOnLoad(_audioListener);
        DontDestroyOnLoad(_eventSystem);

        await _sceneLoadService.LoadScene(SceneType.Menu);
        _sceneLoadService.Cleanup();
    }
}
