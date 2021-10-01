using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MapGen
{
    
    public static class MeshGenerator 
    {
        
        /// <summary>
        /// Recursive Function to Generate the meshes
        /// if too many vertices would be needed, it splits in 4 and goes recusively deeper
        /// </summary>
        /// <param name="heightMap"> the noise to generate from </param>
        /// <param name="position"> where should the mesh be generated </param>
        /// <param name="origin"> origin for uv calculation (should start by Vector2.zero) </param>
        /// <param name="originalBounds"> original height/width for uv calculation </param>
        /// <param name="polyReduceFactor"> how much should polygons be stripped? </param>
        /// <param name="generateLODs"> generates 3 LOD Meshes if set </param>
        /// <param name="inLodGeneration"> should always be set to default false </param>
        /// <returns></returns>
        public static List<MeshData> GenerateTerrainMesh(float[,] heightMap, Vector3 position, Vector2 origin, Vector2 originalBounds, int polyReduceFactor = 1, bool generateLODs = true, bool inLodGeneration = false){
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);
            float topLeftX = (width - 1) / -2f;
            float topLeftZ = (height - 1) / 2f;

            List<MeshData> meshes = new List<MeshData>();
            if(width%polyReduceFactor != 0) width += polyReduceFactor - width%polyReduceFactor;
            if(height%polyReduceFactor != 0) height += polyReduceFactor - height % polyReduceFactor;
            if(width/polyReduceFactor * height  > 65535 && !inLodGeneration){
                List<float[,]> quarterHeights = Quarter(heightMap);
                meshes.AddRange(GenerateTerrainMesh(quarterHeights[0],
                 new Vector3(position.x - quarterHeights[0].GetLength(0)/2f +.5f, 0, position.z + quarterHeights[0].GetLength(1)/2f -.5f), origin, originalBounds, polyReduceFactor, generateLODs));

                meshes.AddRange(GenerateTerrainMesh(quarterHeights[1],
                 new Vector3(position.x + quarterHeights[1].GetLength(0)/2f -.5f, 0, position.z + quarterHeights[1].GetLength(1)/2f -.5f), new Vector2(origin.x + width/2, origin.y), originalBounds, polyReduceFactor, generateLODs));

                meshes.AddRange(GenerateTerrainMesh(quarterHeights[2],
                 new Vector3(position.x - quarterHeights[2].GetLength(0)/2f +.5f, 0, position.z - quarterHeights[2].GetLength(1)/2f +.5f), new Vector2(origin.x, origin.y + height/2), originalBounds, polyReduceFactor, generateLODs));

                meshes.AddRange(GenerateTerrainMesh(quarterHeights[3],
                 new Vector3(position.x + quarterHeights[3].GetLength(0)/2f -.5f,0, position.z - quarterHeights[3].GetLength(1)/2f +.5f), new Vector2(origin.x + width/2, origin.y + height/2), originalBounds, polyReduceFactor, generateLODs));

            }else{
                
                MeshData meshData = new MeshData(width/polyReduceFactor, height, position);
                int vertexIndex = 0;

                
                
                for (int y = 0; y < height; y+=polyReduceFactor)
                {
                    for (int x = 0; x < width; x+=polyReduceFactor)
                    {
                        

                        meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightMap[x,y], topLeftZ - y);
                        meshData.uvs[vertexIndex] = new Vector2((x + origin.x) / (float)originalBounds.x, (y + origin.y) / (float)originalBounds.y);

                        
                        if(((x%polyReduceFactor == 0 && y%polyReduceFactor == 0)||(y == height - polyReduceFactor - 1 && x%polyReduceFactor == 0) || (x == width - polyReduceFactor - 1 && y%polyReduceFactor == 0) || (x == width - polyReduceFactor - 1 && y == height - polyReduceFactor - 1))
                         && x<width -polyReduceFactor&&y<height - polyReduceFactor)
                        {
                            meshData.AddTriangle(0, vertexIndex, vertexIndex + width/polyReduceFactor + 1, vertexIndex + width/polyReduceFactor );
                            meshData.AddTriangle(0, vertexIndex + width/polyReduceFactor + 1, vertexIndex, vertexIndex + 1);              
                            
                        }
                        vertexIndex++;   
                    }
                }
                if(!inLodGeneration && generateLODs){
                    meshData.lodMeshes.Add(GenerateTerrainMesh(heightMap, position, origin, originalBounds, polyReduceFactor*2, true, true).First());
                    meshData.lodMeshes.Add(GenerateTerrainMesh(heightMap, position, origin, originalBounds, polyReduceFactor*2*2, true, true).First());
               
                }
                meshes.Add(meshData);

            }
            return meshes;
        }
        
        /// <summary>
        /// split a heightMap in 4
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static List<float[,]> Quarter(float[,] input){
            List<float[,]> result = new List<float[,]>();

            int width = input.GetLength(0);
            int height = input.GetLength(1);

            int widthHalf1, widthHalf2, heightHalf1, heightHalf2;

            if(width%2 == 0){
                
                widthHalf1 = (width/2)+1;
                widthHalf2 = (width/2)-1;
            }else{
                widthHalf1 = (int)(((float)width / 2f) + .5f);
                widthHalf2 = (int)(((float)width / 2f) - .5f);

            }

            if(height%2 == 0){
                heightHalf1 = (height/2)+1;
                heightHalf2 = (height/2)-1;
            }else{
                heightHalf1 = (int)(((float)height / 2f) + .5f);
                heightHalf2 = (int)(((float)height / 2f) - .5f);
            }

            result.Add(GetQuarter(input, 0, widthHalf1, 0, heightHalf1));
            result.Add(GetQuarter(input, widthHalf2, width, 0, heightHalf1));
            result.Add(GetQuarter(input, 0, widthHalf1, heightHalf2, height));
            result.Add(GetQuarter(input, widthHalf2, width, heightHalf2, height));
            
            //set border values the same
            float value;
            for(int y = 0; y < result[0].GetLength(1); y++){
                value = result[0][result[0].GetLength(0)-1, y] + result[1][0,y];
                value += result[0][result[0].GetLength(0)-2, y] + result[1][1,y];
                value /= 4;
                result[0][result[0].GetLength(0)-1, y] = value;
                result[1][0,y] = value;
            }
            for(int x = 0; x < result[0].GetLength(0); x++){
                value = result[0][x, result[0].GetLength(1)-1] + result[2][x,0];
                value += result[0][x, result[0].GetLength(1)-2] + result[2][x,1];
                value /= 4;
                result[0][x, result[0].GetLength(1)-1] = value;
                result[2][x,0] = value;
            }
            for(int x = 0; x < result[1].GetLength(0); x++){
                value = result[1][x, result[1].GetLength(1)-1] + result[3][x,0];
                value += result[1][x, result[1].GetLength(1)-2] + result[3][x,1];
                value /= 4;
                result[1][x, result[1].GetLength(1)-1] = value;
                result[3][x,0] = value;
            }
            for(int y = 0; y < result[2].GetLength(1); y++){
                value = result[2][result[2].GetLength(0)-1, y] + result[3][0,y];
                value += result[2][result[2].GetLength(0)-2, y] + result[3][1,y];
                value /= 4;
                result[2][result[2].GetLength(0)-1, y] = value;
                result[3][0,y] = value;
            }

            //correct mid top vertex from first quarter
            result[0][result[0].GetLength(0)-1, result[0].GetLength(1)-1] = result[1][0, result[1].GetLength(1)-1];

            return result;
        }
        
        /// <summary>
        /// Helperfunction for Quarter()
        /// </summary>
        /// <param name="input"></param>
        /// <param name="widthStart"></param>
        /// <param name="widthEnd"></param>
        /// <param name="heightStart"></param>
        /// <param name="heightEnd"></param>
        /// <returns></returns>
        static float[,] GetQuarter(float[,] input, int widthStart, int widthEnd, int heightStart, int heightEnd){
            float[,] result = new float[widthEnd - widthStart, heightEnd - heightStart];
            for(int i = widthStart; i < widthEnd; i++)
                for(int j = heightStart; j < heightEnd; j++)
                    result[i - widthStart,j - heightStart] = input[i,j];
            return result;
        }
        
        public static float[,] ToGrayScale(Texture2D input){
            float[,] result = new float[input.width, input.height];
            for(int x = 0; x < input.width; x++)
                for (int y = 0; y < input.height; y++)
                    result[x,y] = input.GetPixel(x,y).grayscale;

            return result;
        }

    }

}