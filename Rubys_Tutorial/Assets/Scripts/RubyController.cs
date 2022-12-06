using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public GameObject projectilePrefab;
    public TextMeshProUGUI ammoText;
    private int cogCount = 4;
    public int health { get { return currentHealth; }}
    int currentHealth;
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    public float timeBoosting = 4.0f;
    float speedBoostTimer;
    bool isBoosting;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    AudioSource audioSource;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    public AudioClip cogpickupSound;
    public AudioClip backgroundSound;
    public ParticleSystem gainHealth;
    public ParticleSystem loseHealth;
    public TextMeshProUGUI scoreText;
    public GameObject winText;
    private int totalScore = 0;
    public GameObject loseText;
    public static int level;
    bool gameOver;
    
    
    // Start is called before the first frame update
    void Start()
    {
        winText.SetActive(false);
        loseText.SetActive(false);
        gameOver = false;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        audioSource= GetComponent<AudioSource>();
        ChangeScore(totalScore);

        ammoText.text = "Cogs: " + cogCount;

        audioSource.clip = backgroundSound;
        audioSource.Play();

    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                if (totalScore >= 4)
                {
                SceneManager.LoadScene("SecondScene");
                }
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (currentHealth <= 0)
        {
            loseText.SetActive(true);
            gameOver = true;

            audioSource.clip = backgroundSound;
            audioSource.Stop();

            audioSource.clip = LoseSound;
            audioSource.Play();
            Debug.Log("Played");

            speed = 0;
            
            
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }
        }

    if (isBoosting == true)
        {
            speedBoostTimer -= Time.deltaTime;
            speed = 5;
        
            if (speedBoostTimer < 0)
            {
                isBoosting = false;
                speed = 3; 
            }
        }
    }
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);
            Instantiate(loseHealth, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
        }

        if (amount > 0)
        {
            Instantiate(gainHealth, rigidbody2d.position + Vector2.up * 1.5f, Quaternion.identity);
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }

    public void ChangeScore(int score)
    {
        
        totalScore += score;
        scoreText.text = "Robots Fixed: " + totalScore.ToString() + "/4";

        if (totalScore >= 4)
        {
            winText.SetActive(true);
            gameOver = true;

            audioSource.clip = backgroundSound;
            audioSource.Stop();

            audioSource.clip = WinSound;
            audioSource.Play();
        }    
    }

    void Launch()
    {
        if (cogCount > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
        cogCount -= 1;
        SetCogText();
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void SetCogText()
    {
        ammoText.text = "Cogs: " + cogCount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ammo")
        {
            cogCount += 4;
            SetCogText();
            Destroy(other.gameObject);

            PlaySound(cogpickupSound);
        }
    }

    public void SpeedBoost(int amount)
    {
        if (amount > 0)
        {
            speedBoostTimer = timeBoosting;
            isBoosting = true;
        }
    }

}
