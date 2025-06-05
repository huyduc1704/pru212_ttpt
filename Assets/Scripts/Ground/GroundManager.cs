using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GroundManager : MonoBehaviour
{
    [Header("Ground Management")]
    public List<DisappearingGround> allDisappearingGrounds = new List<DisappearingGround>();
    
    [Header("Global Settings")]
    public bool resetAllOnPlayerDeath = true;
    
    void Start()
    {
        // Tự động tìm tất cả DisappearingGround trong scene
        if (allDisappearingGrounds.Count == 0)
        {
            allDisappearingGrounds.AddRange(FindObjectsOfType<DisappearingGround>());
        }
    }
    
    public void ResetAllGrounds()
    {
        foreach (DisappearingGround ground in allDisappearingGrounds)
        {
            if (ground != null)
            {
                // Reset ground về trạng thái ban đầu
                ground.StopAllCoroutines();
                TilemapRenderer renderer = ground.GetComponent<TilemapRenderer>();
                // Reset về material gốc
                if (renderer != null)
                {
                    renderer.material = ground.GetComponent<DisappearingGround>().originalMaterial;
                }
                ground.GetComponent<TilemapCollider2D>().enabled = true;
            }
        }
    }
    
    public void TriggerAllGrounds()
    {
        foreach (DisappearingGround ground in allDisappearingGrounds)
        {
            if (ground != null)
            {
                ground.StartDisappearing();
            }
        }
    }
}