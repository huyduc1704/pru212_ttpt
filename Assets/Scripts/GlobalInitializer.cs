using UnityEngine;

public class GlobalInitializer : MonoBehaviour
{
    public GameObject pointManagerPrefab;

    void Awake()
    {
        if (PointManager.Instance == null)
        {
            GameObject newPM = Instantiate(pointManagerPrefab);
            newPM.name = "PointManager"; // tránh (Clone)
        }
    }
}
