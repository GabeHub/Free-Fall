using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void DisplayMainMenu()
    {
        Rect button1Rect = new Rect(Screen.width * 0.1f, Screen.height * 0.3f, Screen.width * 0.8f, Screen.height * 0.15f);
        if (GUI.Button(button1Rect, "Block1"))
        {
            SceneManager.LoadScene("FreeFall");
            RoomGenerator.levelPath = "Data/LevelBlock";
            SceneManager.UnloadSceneAsync("MainScreen");
        }

        Rect button2Rect = new Rect(Screen.width * 0.1f, Screen.height * 0.7f, Screen.width * 0.8f, Screen.height * 0.15f);
        if (GUI.Button(button2Rect, "Block2"))
        {
            SceneManager.LoadScene("FreeFall");
            RoomGenerator.levelPath = "Data/LevelG2Block";
            SceneManager.UnloadSceneAsync("MainScreen");
        }
    }

    private void OnGUI()
    {
        DisplayMainMenu();
    }
}
