using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    public void MakeTransparent(Vector3[] cutPoints)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors = new Color[vertices.Length];
        
        // 기존 정점에 색상을 설정합니다 (초기화)
        for (int i = 0; i < vertices.Length; i++)
        {
            colors[i] = Color.white;
        }

        // cutPoints 주위의 정점을 투명하게 설정합니다.
        foreach (Vector3 cutPoint in cutPoints)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (Vector3.Distance(transform.TransformPoint(vertices[i]), cutPoint) < 0.1f)
                {
                    colors[i] = new Color(1, 1, 1, 0); // 투명
                }
            }
        }

        // 메쉬에 색상을 적용합니다.
        mesh.colors = colors;
    }
}
