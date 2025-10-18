using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    public bool hasPowerup = false;
    private float powerupStrength = 200.0f;

    private AudioSource playerAudio;

    public AudioClip crashSound;

    public ParticleSystem explosionParticle;

    public ParticleSystem dirtParticle; 

    

    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        
        if (explosionParticle != null)
        {
            var main = explosionParticle.main;
            main.useUnscaledTime = true;
        }
        
        if (dirtParticle != null)
        {
            var main = dirtParticle.main;
            main.useUnscaledTime = false;
        }
    }
    
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Get WASD input
        float horizontal = 0f;
        float vertical = 0f;

        if (Keyboard.current.wKey.isPressed) vertical = 1f;   // W - Forward
        if (Keyboard.current.sKey.isPressed) vertical = -1f;  // S - Backward
        if (Keyboard.current.aKey.isPressed) horizontal = -1f; // A - Left
        if (Keyboard.current.dKey.isPressed) horizontal = 1f;  // D - Right

        // Calculate movement direction
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);

        // Apply movement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("powerup") || other.CompareTag("poweup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);
            Debug.Log("Powerup collected!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddPickup();
            }
        }
        
        if (other.CompareTag("Finish"))
        {
            Debug.Log("Finish line reached!");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameWon();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        dirtParticle.Play();

        if(collision.gameObject.CompareTag("car"))
        {
            explosionParticle.Play();
            if (hasPowerup)
            {
                Rigidbody carRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                Vector3 awayFromPlayer = (collision.gameObject.transform.position - transform.position);
                Debug.Log("Collided with " + collision.gameObject.name + " with powerup - pushing car away!");
                carRigidbody.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
                hasPowerup = false;
                

                
            }
            else
            {
                Debug.Log("Collided with " + collision.gameObject.name + " without powerup - Game Over!");
                TriggerGameOver();
                playerAudio.PlayOneShot(crashSound, 1.0f);
                explosionParticle.Play();
                dirtParticle.Stop();
            }
        }
    }

    private void TriggerGameOver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();



        }
        else
        {
            Debug.LogWarning("GameManager not found! Restarting scene directly.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            explosionParticle.Play();
        }
    }
    
    

}
