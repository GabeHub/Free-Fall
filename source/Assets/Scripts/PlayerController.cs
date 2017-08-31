using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public float force;
    public float torque;
    public float glassToBreak;
    public Texture2D scoreTexture;
    public Texture2D bubbleTexture;
    public GUIStyle restartButtonStyle;
    public ParallaxScroll parallax;

    private Rigidbody2D rb;
    private Animator animator;
    private int score = 0;
    private bool isDead = false;
    private bool isImmortal = false;
    private Collider2D bubble;
    [SerializeField]
    private int lvl;

    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            lvl = PlayerPrefs.GetInt("Level");
        }
        else lvl = 1;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(rb.velocity.y);
        parallax.offset = transform.position.y * 0.5f;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.AddForce(new Vector2(-force, 0));
            rb.AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {

            rb.AddForce(new Vector2(force, 0));
            rb.AddTorque(-torque);
        }

        if (Input.GetButton("Fire1") && !isDead)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float mouseToPlayer = transform.position.x - mousePosition.x;
            if (mouseToPlayer > 0)
            {
                rb.AddForce(new Vector2(-force, 0));
                rb.AddTorque(torque);
            }
            if (mouseToPlayer < 0)
            {
                rb.AddForce(new Vector2(force, 0));
                rb.AddTorque(-torque);
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
        if (collision.gameObject.CompareTag("BadBonus"))
        {
            if (isImmortal)
            {
                SwitchSpikes(collision);
                Destroy(bubble.gameObject);
            }
            else
            {
                Death(collision.gameObject.GetComponent<Collider2D>());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isDead)
        {
            if (collider.gameObject.CompareTag("Bonus"))
            {
                SetIsEmptyToTrue(collider);
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
                    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + 10);
                    RowDestroy(collider);
                    Destroy(collider.gameObject);
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
        score += lvl;
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
        Rect button1Rect = new Rect(Screen.width * 0.1f, Screen.height * 0.3f, Screen.width * 0.8f, Screen.height * 0.15f);
        if (GUI.Button(button1Rect, "Tap to restart", restartButtonStyle))
        {
            SceneManager.LoadScene("FreeFall");
        }

        Rect button2Rect = new Rect(Screen.width * 0.1f, Screen.height * 0.5f, Screen.width * 0.8f, Screen.height * 0.15f);
        if (GUI.Button(button2Rect, "Back to menu", restartButtonStyle))
        {
            SceneManager.LoadScene("MainScreen");
        }
    }

    void SetIsEmptyToTrue(Collider2D collider)
    {
        bool flag = false;
        for (int i = 0; i < RoomGenerator.raw; i++)
        {
            if (flag) break;
            for (int j = 0; j < RoomGenerator.column; j++)
            {
                if (RoomGenerator.playerMatrix[i, j].xPosition == collider.gameObject.transform.position.x && RoomGenerator.playerMatrix[i, j].yPosition == collider.gameObject.transform.position.y)
                {
                    RoomGenerator.playerMatrix[i, j].isEmpty = true;
                    flag = true;
                    break;
                }
            }
        }
    }

    void SwitchSpikes(Collision2D collision)
    {
        int triggerHash = Animator.StringToHash("switch");
        animator = collision.gameObject.GetComponent<Animator>();
        animator.SetTrigger(triggerHash);
        collision.gameObject.tag = "Untagged";
    }

    void RowDestroy(Collider2D collider)
    {
        bool flag = false;
        for (int i = 0; i < RoomGenerator.raw; i++)
        {
            if (flag) break;
            for (int j = 0; j < RoomGenerator.column; j++)
            {
                if (RoomGenerator.playerMatrix[i, j].xPosition == collider.gameObject.transform.position.x && RoomGenerator.playerMatrix[i, j].yPosition == collider.gameObject.transform.position.y)
                {
                    RoomGenerator.playerMatrix[i, j].isEmpty = true;

                    if (!RoomGenerator.playerMatrix[i + 1, j].isEmpty && RoomGenerator.playerMatrix[i + 1, j].obstacle.gameObject.name == "glassBlock(Clone)" && rb.velocity.y > glassToBreak)
                    {
                        for (int k = 0; k < RoomGenerator.column; k++)
                        {
                            if (RoomGenerator.playerMatrix[i, k].obstacle)
                            {
                                RoomGenerator.playerMatrix[i, k].isEmpty = true;
                                Destroy(RoomGenerator.playerMatrix[i, k].obstacle.gameObject);
                            }
                        }
                    }

                    flag = true;
                    break;
                }
            }
        }
    }
}
