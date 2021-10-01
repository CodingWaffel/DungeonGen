using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapGen
{
    public static class MeshCreator
    {
        public static GameObject myTerrain;
        public static string mapName;


        public static void DrawMeshes(List<MeshData> meshData, Material materials, bool generateLODs){
            if(myTerrain != null) GameObject.DestroyImmediate(myTerrain);

            myTerrain = GameObject.Find("TerrainParent");
            if(myTerrain == null){
                myTerrain = new GameObject("TerrainParent");
            }else{
                GameObject.DestroyImmediate(myTerrain);
                myTerrain = new GameObject("TerrainParent");
            }
                
            foreach (MeshData mesh in meshData)
            {
                GameObject current = generateLODs ? NewTerrainBaseLODs(mesh, materials) : NewTerrainBase(mesh, materials);
                current.transform.position = mesh.position;
                current.transform.SetParent(myTerrain.transform);
                //OnNewGameObject(current);
            }
 
        }
        static GameObject NewTerrainBase(MeshData mesh, Material material, string objectName = "Terrain"){
            GameObject tile = new GameObject("Terrain");

            GameObject newTerrain = new GameObject(objectName);
            newTerrain.transform.SetParent(tile.transform);
            MeshFilter meshFilter = newTerrain.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newTerrain.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = newTerrain.AddComponent<MeshCollider>();
            meshFilter.sharedMesh = mesh.CreateMesh();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
            meshRenderer.sharedMaterial = material;


            return tile;
        }
        static GameObject NewTerrainBaseLODs(MeshData mesh, Material material, string objectName = "Terrain"){
            GameObject tile = new GameObject("Terrain");

            
            LODGroup lodGroup = tile.AddComponent<LODGroup>();
            lodGroup.fadeMode = LODFadeMode.SpeedTree;
            List<LOD> lods = new List<LOD>();

            GameObject newTerrain = new GameObject(objectName + "LOD0");
            newTerrain.transform.SetParent(tile.transform);
            MeshFilter meshFilter = newTerrain.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = newTerrain.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = newTerrain.AddComponent<MeshCollider>();
            meshFilter.sharedMesh = mesh.CreateMesh();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
            meshRenderer.sharedMaterial = material;


            lods.Add(new LOD(.8f, new Renderer[1]{meshRenderer}));
            
            
            //lods[i] = new LOD(1.0F / (i + 1), renderers);
            for (int i = 0; i < mesh.lodMeshes.Count; i++)
            {
                GameObject newTerrainLod = new GameObject(objectName + "LOD" + (i+1).ToString());
                newTerrainLod.transform.SetParent(tile.transform);
                MeshFilter meshFilterLod = newTerrainLod.AddComponent<MeshFilter>();
                MeshRenderer meshRendererLod = newTerrainLod.AddComponent<MeshRenderer>();
                MeshCollider meshColliderLod = newTerrainLod.AddComponent<MeshCollider>();
                meshFilterLod.sharedMesh = mesh.lodMeshes[0].CreateMesh();
                meshColliderLod.sharedMesh = meshFilterLod.sharedMesh;
                meshRendererLod.sharedMaterial = material;

                lods.Add(new LOD(1f/(i+3*(i+1)), new Renderer[1]{meshRendererLod}));
            }
            lodGroup.SetLODs(lods.ToArray());

            return tile;
        }

        //public virtual void OnNewGameObject(GameObject obj){}
    }
}