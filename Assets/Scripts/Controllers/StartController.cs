using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{

    public void Play()
    {
        if (DataController.saveData.IsLoaded)
        {
            LoadingScreenController.LoadingScreen.OpenLoadingScreen((() =>
            {
                SceneManager.LoadScene("HomeScene");
            }));
        }
        else
        {
            LoadingScreenController.LoadingScreen.OpenLoadingScreen((() =>
            {
                SceneManager.LoadScene("TutorialScene");
            }));
        }
    }
    
}
