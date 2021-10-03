
using System.Collections.Generic;
using UnityEngine;

namespace MapGen
{
    public  class MapGenerator
    {
        public  int mapWidth = 1;
        public  int mapHeight = 1;
        int[] lengthValues = new int[10]{64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768};
        public  int MapWidth => lengthValues[mapWidth-1];
        public  int MapHeight => lengthValues[mapHeight-1];
        public  float frequency = 10f;
        public float amplitude = 1f;
        public float persistance = 1f;
        public float lacunarity = 1f;
        public int octaves = 2;
        public  Vector2 offset = Vector2.zero;
        public  float heightMultiplier = 100f;
        public  AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public  Texture2D islandShape;
        public Shader shader;
        public string baseMapReference, baseColorReference;
      
        public  int polyReduceFactor = 0;
        int[] polyRecuction = new int[10]{1, 2, 4, 8, 16, 32, 64, 128, 256, 512};
        public  bool archipel = false;
        public  bool autoUpdate = false;
        public  Material material;

        public  MaterialOption materialOption = MaterialOption.SingleMaterial;
        public  Material currentMat;

        //3 Tex Height Material------------------------------------------------------
        public  Texture2D topTexture, midTexture, botTexture, topNormal, midNormal, botNormal;
        public  Vector2 tilingTop = Vector2.one;
        public  Vector2 tilingMid = Vector2.one;
        public  Vector2 tilingBot = Vector2.one;
        public  float metallic, smoothness, occlusion;
        public  float topThreshold = .8f;
        public  float topMidBlendThreshold = .7f;
        
       
        public  float midThreshold = .5f;
        public  float midBottomBlendThreshold = .3f;

        public bool generateLODs = false;

        public  void GenerateMap(float[,] map, int polyReduceFactor, Material mat)
        {
            // float[,] noiseMap = Noise.CalcNoise(MapWidth, MapHeight, offset, octaves, amplitude, frequency, persistance, lacunarity);
            // Texture2D islandShape = this.islandShape;
            // TextureScaler.scale(islandShape, MapWidth, MapHeight);
            // for (int x = 0; x < map.GetLength(0); x++)
            // {
            //     for (int y = 0; y < map.GetLength(1); y++)
            //     {
            //         map[x, y] = heightCurve.Evaluate(map[x, y])* (1f - islandShape.GetPixel(x,y).grayscale) * heightMultiplier;
            //     }
            // }


                List<MeshData> meshes = MeshGenerator.GenerateTerrainMesh(map, Vector2.zero, Vector2.zero, new Vector2(MapWidth, MapHeight),polyRecuction[polyReduceFactor], generateLODs);
                MeshCreator.DrawMeshes(meshes, mat, generateLODs);
                // switch(materialOption){
                //     case MaterialOption.HeightBasedTextures:
                //         MeshCreator.DrawMeshes(meshes, SetHeightBasedTextureParameter(map), generateLODs);
                //         break;
                //     case MaterialOption.SingleMaterial:
                //         MeshCreator.DrawMeshes(meshes, material, generateLODs);
                //         break;
                //     case MaterialOption.CreateSingleTexture:
                //         MeshCreator.DrawMeshes(meshes, CreateTexMaterial(map, shader, baseMapReference, baseColorReference), generateLODs);
                //         break;
                //     default:
                //         break;
                // }
                Resources.UnloadUnusedAssets();

        }
        
        Material CreateTexMaterial(float[,] heightMap, Shader shader, string baseMapName, string baseColorReference){
            Texture2D tex = TextureGenerator.CreateHeightBasedTexture(heightMap, topTexture, midTexture, botTexture, tilingTop, tilingMid, tilingBot, new Vector4(topThreshold, topMidBlendThreshold, midThreshold, midBottomBlendThreshold));
        
            Material mat = new Material(shader);
            mat.SetColor(baseColorReference, Color.white);
            mat.SetTexture(baseMapName, tex);
            return mat;
            }
        Material SetHeightBasedTextureParameter(float[,] heightMap){
            Material material = new Material(Shader.Find("Shader Graphs/MapGenShader"));

            material.SetFloat("_MaxHeight", GetMaxValue(heightMap));

            material.SetTexture("_TopTex", topTexture);
            material.SetTexture("_MidTex", midTexture);
            material.SetTexture("_BotTex", botTexture);

            material.SetTexture("_NormalTop", topNormal);
            material.SetTexture("_NormalMid", midNormal);
            material.SetTexture("_NormalBot", botNormal);

            material.SetFloat("_TopThreshold", topThreshold);
            material.SetFloat("_TopBlendThreshold", topMidBlendThreshold);
            material.SetFloat("_MidThreshold", midThreshold);
            material.SetFloat("_MidBlendThreshold", midBottomBlendThreshold);

            material.SetVector("_TilingTop", tilingTop);   
            material.SetVector("_TilingMid", tilingMid);   
            material.SetVector("_TilingBot", tilingBot);  

            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Occlusion", occlusion);

            currentMat = material;
        return material; 
        }

        private  void OnValidate()
        {
            if(mapWidth < 1)
            {
                mapWidth = 1;
            }
            if (mapHeight < 1)
            {
                mapHeight = 1;
            }

        }


        float GetMaxValue(float[,] input){
            float max = 0f;
            for (int x = 0; x < input.GetLength(0); x++)
            {
                for (int y = 0; y < input.GetLength(1); y++)
                {
                    if(input[x, y] > max) max = input[x, y]; 
                }
            }
        return max;
        }
    }

    public enum MaterialOption{
        HeightBasedTextures, CreateSingleTexture, SingleMaterial
    }
}