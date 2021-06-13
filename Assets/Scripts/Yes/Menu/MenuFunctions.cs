using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    [SerializeField] private int _toLoadSceneIndex;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void LoadGame()
    {
        SceneManager.LoadScene(_toLoadSceneIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
