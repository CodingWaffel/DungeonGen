using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen{
        
    public class MeshData
    {
        public List<MeshData> lodMeshes;
        public Vector3[] vertices;
        public List<int> triangles;
        public Vector2[] uvs;

        public Vector3 position;

        public MeshData(int meshWidth, int meshHeight, Vector3 position)
        {
            this.lodMeshes = new List<MeshData>();
            this.position = position;
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            //triangles = new List<List<int>>();
            triangles = new List<int>();
        }
        public void AddTriangle(int subMesh, int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }


        public Mesh CreateMesh()
        {

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.subMeshCount = 1;
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            return mesh;
        }
    }
}