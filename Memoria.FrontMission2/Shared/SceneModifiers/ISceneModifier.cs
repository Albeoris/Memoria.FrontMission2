using UnityEngine.SceneManagement;

namespace Memoria.FrontMission2;

public interface ISceneModifier
{
    void OnSceneLoaded(Scene scene, LoadSceneMode mode);
}