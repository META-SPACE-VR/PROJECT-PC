using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class RegularPolygonCylinder : MonoBehaviour
{
    [SerializeField]
    private int polygonSides = 3; // 다각형 변의 갯수

    [SerializeField]
    private float polygonSize = 1.0f; // 다각형 사이즈

    [SerializeField]
    private float height = 1.0f; // 높이

    [SerializeField]
    private Vector3 offset = Vector3.zero; // 오프셋

    Mesh mesh; // 메시
    Vector3[] vertices; // 꼭짓점
    int[] triangles; // 삼각형 정보

    // OnValidate
    void OnValidate() {
        if(mesh == null) return;

        if(polygonSides >= 3 && polygonSize > 0 && height > 0) {
            SetMeshData();
            CreateMesh();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        SetMeshData();
        CreateMesh();
    }

    void SetMeshData()
    {
        vertices = new Vector3[6 * polygonSides + 2];
        triangles = new int[12 * polygonSides];

        // 윗면, 아랫면 꼭짓점 생성
        vertices[2 * polygonSides] = new Vector3(0, height/2.0f, 0) + offset;
        vertices[2 * polygonSides + 1] = new Vector3(0, -height/2.0f, 0) + offset;

        float theta = 2*Mathf.PI/polygonSides;
        for(int i = 0; i < polygonSides; i++) {
            vertices[i] = new Vector3(Mathf.Cos(theta*i) * polygonSize, height/2.0f, Mathf.Sin(theta*i) * polygonSize) + offset;
        }
        for(int i = 0; i < polygonSides; i++) {
            vertices[polygonSides + i] = new Vector3(Mathf.Cos(theta*i) * polygonSize, -height/2.0f, Mathf.Sin(theta*i) * polygonSize) + offset;
        }

        // 윗면 삼각형 생성
        for(int i = 0; i < polygonSides; i++) {
            triangles[3 * i] = 2 * polygonSides;
            triangles[3 * i + 1] = (i + 1) % polygonSides;
            triangles[3 * i + 2] = i;
        }

        // 아랫면 삼각형 생성
        for(int i = 0; i < polygonSides; i++) {
            triangles[3 * (polygonSides + i)] = polygonSides + i;
            triangles[3 * (polygonSides + i) + 1] = polygonSides + (i + 1) % polygonSides;
            triangles[3 * (polygonSides + i) + 2] = 2 * polygonSides + 1;
        }

        // 옆면 꼭짓점 및 옆면 삼각형 생성
        for(int i = 0; i < polygonSides; i++) {
            vertices[2 * polygonSides + 2 + 4 * i] = new Vector3(Mathf.Cos(theta*i) * polygonSize, height/2.0f, Mathf.Sin(theta*i) * polygonSize) + offset;
            vertices[2 * polygonSides + 2 + 4 * i + 1] = new Vector3(Mathf.Cos(theta*(i+1)) * polygonSize, height/2.0f, Mathf.Sin(theta*(i+1)) * polygonSize) + offset;
            vertices[2 * polygonSides + 2 + 4 * i + 2] = new Vector3(Mathf.Cos(theta*i) * polygonSize, -height/2.0f, Mathf.Sin(theta*i) * polygonSize) + offset;
            vertices[2 * polygonSides + 2 + 4 * i + 3] = new Vector3(Mathf.Cos(theta*(i+1)) * polygonSize, -height/2.0f, Mathf.Sin(theta*(i+1)) * polygonSize) + offset;

            triangles[6 * polygonSides + 6 * i] = 2 * polygonSides + 2 + 4 * i;
            triangles[6 * polygonSides + 6 * i + 1] = 2 * polygonSides + 2 + 4 * i + 1;
            triangles[6 * polygonSides + 6 * i + 2] = 2 * polygonSides + 2 + 4 * i + 2;
            triangles[6 * polygonSides + 6 * i + 3] = 2 * polygonSides + 2 + 4 * i + 1;
            triangles[6 * polygonSides + 6 * i + 4] = 2 * polygonSides + 2 + 4 * i + 3;
            triangles[6 * polygonSides + 6 * i + 5] = 2 * polygonSides + 2 + 4 * i + 2;
        }
    }

    void CreateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Destroy(this.GetComponent<MeshCollider>());
        MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();

        meshCollider.convex = true;
        meshCollider.isTrigger = true;
    }
}
