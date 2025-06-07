using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class DisappearingGround : MonoBehaviour
{
    [Header("Disappearing Settings")]
    public float disappearDelay = 0.05f; // Thời gian trước khi biến mất
    public float disappearDuration = 0.05f; // Thời gian biến mất
    
    [Header("Visual Effects")]
    public bool shakeBeforeDisappear = true;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    
    private Tilemap tilemap;
    private TilemapRenderer tilemapRenderer;
    private TilemapCollider2D tilemapCollider;
    private Vector3 originalPosition;
    private bool isDisappearing = false;
    public Material originalMaterial;
    private Material fadeMaterial;
    
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tilemapRenderer = GetComponent<TilemapRenderer>();
        tilemapCollider = GetComponent<TilemapCollider2D>();
        originalPosition = transform.position;
        
        // Lưu material gốc
        originalMaterial = tilemapRenderer.material;
        
        // Tạo fade material (sử dụng material có alpha)
        CreateFadeMaterial();
    }
    
    public void StartDisappearing()
    {
        if (!isDisappearing)
        {
            StartCoroutine(DisappearSequence());
        }
    }
    
    IEnumerator DisappearSequence()
    {
        isDisappearing = true;
        
        if (shakeBeforeDisappear)
        {
            StartCoroutine(ShakeGround());
        }
        
        // Đợi trước khi biến mất
        yield return new WaitForSeconds(disappearDelay);
        
        // Fade out effect
        yield return StartCoroutine(FadeOut());
        
        // Tắt collider để player có thể rơi qua
        if (tilemapCollider != null)
        {
            tilemapCollider.enabled = false;
        }
        

    }
    
    IEnumerator ShakeGround()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            randomOffset.z = 0f;
            transform.position = originalPosition + randomOffset;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = originalPosition;
    }
    
    void CreateFadeMaterial()
    {
        // Tạo material mới để fade
        fadeMaterial = new Material(Shader.Find("Sprites/Default"));
        fadeMaterial.color = Color.white;
    }
    
    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        
        // Chuyển sang fade material
        tilemapRenderer.material = fadeMaterial;
        
        while (elapsedTime < disappearDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / disappearDuration);
            fadeMaterial.color = new Color(1f, 1f, 1f, alpha);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Đảm bảo hoàn toàn trong suốt
        fadeMaterial.color = new Color(1f, 1f, 1f, 0f);
    }
    public void ResetGround()
    {
        isDisappearing = false;
        if (tilemapCollider != null)
            tilemapCollider.enabled = true;
        if (tilemapRenderer != null && originalMaterial != null)
            tilemapRenderer.material = originalMaterial;
        if (fadeMaterial != null)
            fadeMaterial.color = Color.white;

        // Reset all triggers that target this ground
        foreach (var trigger in FindObjectsOfType<GroundTrigger>())
        {
            if (trigger.targetGround == this)
                trigger.ResetTrigger();
        }
    }
    
}