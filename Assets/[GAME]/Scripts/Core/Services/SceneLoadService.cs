using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadService : IService
{
    private SceneType _currentSceneType;
    private SceneType _sceneType;

    public FloatParameter ProggressParam { get; private set; }

    public SceneLoadService(SceneType currentSceneType)
    {
        _currentSceneType = currentSceneType;
    }

    public void Initialize()
    {
        ProggressParam = new FloatParameter(100, 0);
    }

    public void Cleanup()
    {
        ProggressParam.OverrideValue(0);
    }

    public SceneType GetSceneType()
    {
        return _currentSceneType;
    }

    public async UniTask LoadScene(SceneType sceneType)
    {
        ProggressParam.SetToMin();
        _sceneType = sceneType;
        var currentScene = SceneManager.GetActiveScene();
        var asyncTask = SceneManager.LoadSceneAsync($"{_sceneType}", LoadSceneMode.Additive);
        asyncTask.completed += Completed;
        while (!asyncTask.isDone)
        {
            ProggressParam.OverrideValue(asyncTask.progress);
            await UniTask.Yield();
        }

        await UniTask.DelayFrame(20);
        await SceneManager.UnloadSceneAsync(currentScene);
    }

    private void Completed(AsyncOperation operation)
    {
        operation.completed -= Completed;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName($"{_sceneType}"));
    }
}