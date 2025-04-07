using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _creditsPanel;
    public void StartGame() => SceneManager.LoadScene("GamePlay");
    public void QuitGame() => Application.Quit();
    public void ShowCredits() => _creditsPanel.SetActive(true);
    public void HideCredits() => _creditsPanel.SetActive(false);
}
