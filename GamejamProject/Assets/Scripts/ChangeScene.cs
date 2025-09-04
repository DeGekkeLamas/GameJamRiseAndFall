using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads a scene by its exact name as defined in Build Settings.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load</param>
    public void LoadSceneByName(string sceneName)
    {
        // Check if the scene exists in Build Settings
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene \"{sceneName}\" not found. Make sure it's added to Build Settings and the name is correct.");
        }
    }
}
