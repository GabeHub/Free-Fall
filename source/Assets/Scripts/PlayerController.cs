using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public float force;
    public Texture2D scoreTexture;
    public Texture2D bubbleTexture;
    public float glassToBreak;
    public GUIStyle restartButtonStyle;

    private Rigidbody2D rb;
    private Animator animator;
    private float score = 0f;
    private bool isDead = false;
    private bool isImmortal = false;
    private Collider2D bubble;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.Log(rb.velocity.y);
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
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int triggerHash = Animator.StringToHash("onHit");
        animator.SetTrigger(triggerHash);
        if (collision.gameObject.CompareTag("BadBonus") && !isImmortal)
        {
            isDead = true;
            animator.SetBool("isDead", isDead);
        }
        else if (collision.gameObject.CompareTag("Glass"))
        {
            if (rb.velocity.y <= glassToBreak)
            {
                Destroy(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BadBonus") && isImmortal)
        {
            Destroy(bubble.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Bonus"))
        {
            CollectDiamond(collision);
        }
        if (collision.gameObject.CompareTag("Bubble"))
        {
            CollectBubble(collision);
        }
    }

    void CollectDiamond(Collider2D collision)
    {
        score++;
        Destroy(collision.gameObject);
    }

    void CollectBubble(Collider2D collision)
    {
        if (bubble)
        {
            Destroy(bubble.gameObject);
        }
        isImmortal = true;
        bubble = collision;
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
}
