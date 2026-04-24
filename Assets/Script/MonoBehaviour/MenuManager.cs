using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{

    [SerializeField] private Canvas PauseMenu;
    private bool m_ShowPauseMenu = false;
    private InputSystem_Actions controls;

    private void Start()
    {
        if(PauseMenu != null) PauseMenu.enabled = false; 
    }

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Menu.PauseMenu.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        controls.Menu.Enable();
    }
    private void OnDisable()
    {
        controls.Menu.Disable();
    }

    public void ToGame()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;

    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ToParameters()
    {

    }

    public void TogglePause()
    {
        if (m_ShowPauseMenu) m_ShowPauseMenu = false;
        else m_ShowPauseMenu = true;
        ToPauseMenu();
    }

    public void ToPauseMenu()
    {
        if (PauseMenu != null)
        {
            if (m_ShowPauseMenu)
            {
                PauseMenu.enabled = true;
                Time.timeScale = 0;
            }
            else if (!m_ShowPauseMenu)
            {
                PauseMenu.enabled = false;
                Time.timeScale = 1;
            }
        }
        else { Debug.Log("Touche Pause ignorée : aucun Canvas n'est assigné dans cette scène."); }
    }

}
