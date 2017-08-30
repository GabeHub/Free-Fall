﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float force;
    public float glassToBreak;
    public Texture2D scoreTexture;
    public Texture2D bubbleTexture;
    public GUIStyle restartButtonStyle;

    private Rigidbody2D rb;
    private Animator animator;
    private int score = 0;
    private bool isDead = false;
    private bool isImmortal = false;
    private Collider2D bubble;
    [SerializeField]
    private int lvl;

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey("Level"))
        {
            lvl = PlayerPrefs.GetInt("Level");
        }
        else lvl = 1;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.Log(rb.velocity.y);
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(new Vector2(-force, 0));
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.AddForce(new Vector2(force, 0));
        }

        if (Input.GetButton("Fire1") && !isDead)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mouseToPlayer = transform.position.x - mousePosition.x;
            if (mouseToPlayer > 0)
            {
                rb.AddForce(new Vector2(-force, 0));
            }
            if (mouseToPlayer < 0)
            {
                rb.AddForce(new Vector2(force, 0));
            }
        }

        if (bubble == null)
        {
            isImmortal = false;
        }

        if (isDead)
        {
            Destroy(gameObject.GetComponent<Collider2D>());
            rb.AddForce(new Vector2(0, force * 3));
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        int triggerHash = Animator.StringToHash("onHit");
        animator.SetTrigger(triggerHash);
        if (collision.gameObject.CompareTag("BadBonus") && !isImmortal)
        {
            Death(collision.gameObject.GetComponent<Collider2D>());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BadBonus") && isImmortal)
        {
            Destroy(bubble.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isDead)
        {
            if (collider.gameObject.CompareTag("Bonus"))
            {
                CollectDiamond(collider);
            }
            else if (collider.gameObject.CompareTag("Bubble"))
            {
                CollectBubble(collider);
            }
            else if (collider.gameObject.CompareTag("Glass"))
            {
                if (rb.velocity.y <= glassToBreak)
                {
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 2);
                    Destroy(collider.gameObject);
                }
                else
                {
                    if (collider.gameObject.name == "glassBlock")
                    {
                        DestroyRow(collider);
                    }
                }
            }
            else if (collider.gameObject.CompareTag("Laser"))
            {
                Death(collider);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetInt("Score", score + PlayerPrefs.GetInt("Score"));
        PlayerPrefs.Save();
    }

    void Death(Collider2D collider)
    {
        isDead = true;
        animator.SetBool("isDead", isDead);
        if (collider.gameObject.CompareTag("Laser"))
        {
            //laser death animation
        }
        else
        {            
            //spike death animation
        }
    }

    void CollectDiamond(Collider2D collider)
    {
        score += lvl * 5;
        Destroy(collider.gameObject);
    }

    void CollectBubble(Collider2D collider)
    {
        if (bubble)
        {
            Destroy(bubble.gameObject);
        }
        isImmortal = true;
        bubble = collider;
    }

    void DisplayScore()
    {
        Rect rect = new Rect(20, 20, 60, 80);
        GUI.DrawTexture(rect, scoreTexture);

        GUIStyle style = new GUIStyle()
        {
            fontSize = 80,
            fontStyle = FontStyle.Italic
        };

        style.normal.textColor = Color.green;

        Rect labelRect = new Rect(rect.xMax, rect.y, 60, 32);
        GUI.Label(labelRect, score.ToString(), style);
    }

    private void OnGUI()
    {
        DisplayScore();
        if (isDead)
        {
            DisplayGameOver();
        }
    }

    void DisplayGameOver()
    {
        Rect buttonRect = new Rect(Screen.width * 0.1f, Screen.height * 0.45f, Screen.width * 0.8f, Screen.height * 0.15f);
        if(GUI.Button(buttonRect, "Tap to restart", restartButtonStyle))
        {
            //SceneManager.LoadScene(SceneManager.GetSceneByName("FreeFall").ToString());
            Application.LoadLevel(Application.loadedLevelName);
        }
    }

    void DestroyRow(Collider2D collider)
    {
        float posY = collider.gameObject.transform.position.y + RoomGenerator.deltaY;
        float posX = collider.transform.position.x;
        bool flag = false;
        for (int i = 0; i < RoomGenerator.raw; i++)
        {
            if (flag) break;
            for (int j = 0; j < RoomGenerator.column; j++)
            {
                if (RoomGenerator.playerMatrix[i, j].xPosition == posX && RoomGenerator.playerMatrix[i, j].yPosition == posY)
                {
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (!RoomGenerator.playerMatrix[i, k].isEmpty)
                        {
                            if(RoomGenerator.playerMatrix[i, k].obstacle)
                            {
                                if (RoomGenerator.playerMatrix[i, k].obstacle.CompareTag("Glass"))
                                {
                                    Destroy(RoomGenerator.playerMatrix[i, k].obstacle.gameObject);
                                    RoomGenerator.playerMatrix[i, k].isEmpty = true;
                                }
                                else break;
                            }
                            else
                            {
                                RoomGenerator.playerMatrix[i, k].isEmpty = true;
                                break;
                            }
                        }
                        else break;
                    }
                    for (int k = j + 1; k < RoomGenerator.column; k++)
                    {
                        if (!RoomGenerator.playerMatrix[i, k].isEmpty)
                        {
                            if(RoomGenerator.playerMatrix[i, k].obstacle)
                            {
                                if (RoomGenerator.playerMatrix[i, k].obstacle.CompareTag("Glass"))
                                {
                                    Destroy(RoomGenerator.playerMatrix[i, k].obstacle.gameObject);
                                    RoomGenerator.playerMatrix[i, k].isEmpty = true;
                                }
                                else break;
                            }
                            else
                            {
                                RoomGenerator.playerMatrix[i, k].isEmpty = true;
                                break;
                            }
                        }
                        else break;
                    }
                    flag = true;
                    break;
                }
            }
        }
    }
}
