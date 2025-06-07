using UnityEngine;
using System.Collections;

public class TrapTrigger : MonoBehaviour
{
    [Header("Trap Animation Settings")]
    [Tooltip("Array of trap GameObjects to rotate")]
    public GameObject[] trapsToRotate;
    
    [Tooltip("Target rotation angle (90 for standing up)")]
    public float targetRotationZ = 90f;
    
    [Tooltip("Speed of rotation animation")]
    public float rotationSpeed = 180f; // degrees per second
    
    [Tooltip("Delay before traps start rotating")]
    public float activationDelay = 0.1f;
    
    [Header("Audio & Effects")]
    [Tooltip("Sound effect when traps activate")]
    public AudioClip trapActivationSound;
    
    [Tooltip("Particle effect when traps activate")]
    public GameObject activationEffect;
    
    private bool hasTriggered = false;
    private AudioSource audioSource;
    
    public Transform[] traps; // Kéo các trap Spike vào đây
    public float targetX = 5f; // vị trí X muốn tụ về
    public float spacingY = 1.2f; // khoảng cách giữa mỗi trap theo trục Y
    public float baseY = 0f; // điểm bắt đầu từ trap đầu tiên

    void Start()
    {
        ValidateComponents();
        SetupAudioSource();
    }

    private void ValidateComponents()
    {
        if (!GetComponent<Collider2D>())
        {
            Debug.LogError("TrapTrigger requires a Collider2D component set as trigger!");
            enabled = false;
            return;
        }

        if (trapsToRotate == null || trapsToRotate.Length == 0)
        {
            Debug.LogWarning("No traps assigned to TrapTrigger!");
        }
    }

    private void SetupAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && trapActivationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < traps.Length; i++)
            {
                traps[i].rotation = Quaternion.Euler(0, 0, 90); // xoay dọc
                float newY = baseY + i * spacingY; // tăng Y dần theo i
                traps[i].position = new Vector3(targetX, newY, traps[i].position.z);
            }
        }
    }

    private IEnumerator ActivateTraps()
    {
        // Play sound effect
        if (audioSource != null && trapActivationSound != null)
        {
            audioSource.PlayOneShot(trapActivationSound);
        }

        // Spawn activation effect
        if (activationEffect != null)
        {
            Instantiate(activationEffect, transform.position, Quaternion.identity);
        }

        // Wait for activation delay
        yield return new WaitForSeconds(activationDelay);

        // Start rotating all traps
        foreach (GameObject trap in trapsToRotate)
        {
            if (trap != null)
            {
                StartCoroutine(RotateTrap(trap));
            }
        }
    }

    private IEnumerator RotateTrap(GameObject trap)
    {
        Vector3 startRotation = trap.transform.eulerAngles;
        Vector3 targetRotation = new Vector3(startRotation.x, startRotation.y, targetRotationZ);
        
        float rotationProgress = 0f;
        float rotationDuration = Mathf.Abs(targetRotationZ - startRotation.z) / rotationSpeed;

        while (rotationProgress < 1f)
        {
            rotationProgress += Time.deltaTime / rotationDuration;
            
            Vector3 currentRotation = Vector3.Lerp(startRotation, targetRotation, rotationProgress);
            trap.transform.eulerAngles = currentRotation;
            
            yield return null;
        }

        // Ensure final rotation is exact
        trap.transform.eulerAngles = targetRotation;
        
        Debug.Log($"Trap {trap.name} rotated to {targetRotationZ} degrees");
    }

    // // Optional: Reset traps (useful for testing or reusable traps)
    public void ResetTraps()
    {
        hasTriggered = false;
        
        foreach (GameObject trap in trapsToRotate)
        {
            if (trap != null)
            {
                trap.transform.eulerAngles = Vector3.zero;
            }
        }
    }
    
    public static void ResetAllTraps()
    {
        foreach (var trap in FindObjectsOfType<TrapTrigger>())
        {
            trap.ResetTraps();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw trigger area
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.color = hasTriggered ? Color.red : Color.yellow;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }

        // Draw lines to connected traps
        if (trapsToRotate != null)
        {
            Gizmos.color = Color.cyan;
            foreach (GameObject trap in trapsToRotate)
            {
                if (trap != null)
                {
                    Gizmos.DrawLine(transform.position, trap.transform.position);
                }
            }
        }
    }
}