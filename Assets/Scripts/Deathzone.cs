using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Vector3 respawnPoint = Vector3.zero;
    
    [Tooltip("Optional particle effect for respawn")]
    public GameObject respawnEffect;

    private void Start()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (!GetComponent<Collider2D>())
        {
            Debug.LogError("DeathZone requires a Collider2D component!");
            enabled = false;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        try
        {
            HandlePlayerRespawn(other);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during player respawn: {e.Message}");
        }
    }

    private void HandlePlayerRespawn(Collider2D player)
    {
        // Reset position
        player.transform.position = respawnPoint;

        // Reset velocity
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        // Spawn effect if available
        if (respawnEffect != null)
        {
            Instantiate(respawnEffect, respawnPoint, Quaternion.identity);
        }

        foreach (var ground in FindObjectsOfType<DisappearingGround>())
        {
            ground.ResetGround();
        }
        TrapTrigger.ResetAllTraps();

        Debug.Log($"Player respawned at {respawnPoint}");
    }
}