using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    [Header("Ground Settings")]
    public DisappearingGround targetGround;
    
    [Header("Trigger Settings")]
    public bool triggerOnce = true;
    
    private bool hasTriggered = false;
    
    void Start()
    {
        Debug.Log("GroundTrigger Start - Checking setup...");
        
        // Kiểm tra có Collider2D không
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("No Collider2D found on TriggerZone!");
        }
        else
        {
            Debug.Log($"Collider2D found: {col.GetType()}, IsTrigger: {col.isTrigger}");
        }
        
        // Kiểm tra target ground
        if (targetGround == null)
        {
            Debug.LogError("Target Ground is not assigned!");
        }
        else
        {
            Debug.Log($"Target Ground assigned: {targetGround.name}");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"=== TRIGGER ENTERED ===");
        Debug.Log($"Object: {other.name}");
        Debug.Log($"Tag: {other.tag}");
        Debug.Log($"Has Rigidbody2D: {other.GetComponent<Rigidbody2D>() != null}");
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ PLAYER DETECTED!");
            
            if (triggerOnce && hasTriggered) 
            {
                Debug.Log("❌ Already triggered, ignoring...");
                return;
            }
            
            if (targetGround != null)
            {
                Debug.Log("🚀 Starting ground disappearing...");
                targetGround.StartDisappearing();
                hasTriggered = true;
            }
            else
            {
                Debug.LogError("❌ Target Ground is null!");
            }
        }
        else
        {
            Debug.Log($"❌ Not player, tag was: '{other.tag}'");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"TRIGGER EXITED by: {other.name}");
    }
    
    // Hiển thị trigger zone trong Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}