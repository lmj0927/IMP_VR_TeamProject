using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject boxPrefab;
    public List<Transform> spawnPoints;

    private Transform lastUsedPoint;

    void Start()
    {
        SpawnNewBox();
    }

    private void SpawnNewBox()
    {
        if (boxPrefab == null || spawnPoints == null || spawnPoints.Count <= 0) return;

        Transform selectedPoint = PickSpawnPoint();
        lastUsedPoint = selectedPoint;

        GameObject newBox = Instantiate(boxPrefab, selectedPoint.position, selectedPoint.rotation);
        ItemBox itemBox = newBox.GetComponent<ItemBox>();

        if (itemBox != null)
            itemBox.OnBoxDepleted += SpawnNewBox;
    }

    private Transform PickSpawnPoint()
    {
        if (spawnPoints.Count == 1) return spawnPoints[0];

        List<Transform> available = spawnPoints.FindAll(p => p != lastUsedPoint);
        return available[Random.Range(0, available.Count)];
    }
}