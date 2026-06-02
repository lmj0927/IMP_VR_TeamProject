using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Color gizmosColor = new Color(0f, 1f, 0f, 0.5f);

    void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
