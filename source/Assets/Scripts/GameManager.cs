using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject player;

    [System.Serializable]
    public struct Score
    {
        public int level;
        public int score;

        Score(int lvl, int scr)
        {
            level = lvl;
            score = scr;
        }
    }

    public Score[] scoreTable;

    private float globalTimer = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (player)
        {
            globalTimer += Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
	}

    private void OnDestroy()
    {
        int score = 0;
        int lvl = 1;
        if (PlayerPrefs.HasKey("Score"))
        {
            score = PlayerPrefs.GetInt("Score");
        }
        for(int i=0; i < scoreTable.Length - 1; i++)
        {
            if(score >= scoreTable[i].score && score < scoreTable[i + 1].score)
            {
                lvl = scoreTable[i].level;
            }
            if (score >= scoreTable[scoreTable.Length - 1].score)
            {
                lvl = scoreTable[scoreTable.Length - 1].level;
                break;
            }
        }
        PlayerPrefs.SetInt("Level", lvl);
        PlayerPrefs.Save();
    }
}
