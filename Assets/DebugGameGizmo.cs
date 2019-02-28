using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGameGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector3 vecTop = Camera.main.ViewportToWorldPoint(new Vector3(Camera.main.rect.xMin, Camera.main.rect.yMax, Camera.main.transform.position.z));
        Vector3 vecDown = Camera.main.ViewportToWorldPoint(new Vector3(Camera.main.rect.xMin, Camera.main.rect.yMin, Camera.main.transform.position.z));
        Gizmos.DrawRay(vecTop, Vector3.right * 1000);
        Gizmos.DrawRay(vecDown, Vector3.right * 1000);
    }
}
