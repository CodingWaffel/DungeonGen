using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class TextureGenerator
{
    static Color a = Color.grey;
    static Color b = Color.green;
    static Color c = Color.yellow;
    public static Texture2D CreateHeightBasedTexture(float[,] heightMap, Texture2D top, Texture2D mid, Texture2D bot, Vector2 tilingTop, Vector2 tilingMid, Vector2 tilingBot, Vector4 thresholds){
        Texture2D tex = new Texture2D(heightMap.GetLength(0), heightMap.GetLength(1));
        Texture2D topTiled = Tile(top, tilingTop);
        topTiled.Apply();
        Texture2D midTiled = Tile(mid, tilingMid);
        midTiled.Apply();
        Texture2D botTiled = Tile(bot, tilingBot);
        botTiled.Apply();

        float maxValue = GetMaxValue(heightMap);

        float current;
        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int y = 0; y < heightMap.GetLength(1); y++)
            {
                current = heightMap[x, y] < 0 ? 0 : heightMap[x, y];
                current /= maxValue;

                if(current >= thresholds.x){
                    tex.SetPixel(x, y, topTiled.GetPixel(x, y));
                }else if(current < thresholds.x && current > thresholds.y){
                    tex.SetPixel(x, y, Color.Lerp(midTiled.GetPixel(x, y), topTiled.GetPixel(x, y), ((current - thresholds.y) /(thresholds.x - thresholds.y))));
                }
                else if(current >= thresholds.z){
                    tex.SetPixel(x, y, midTiled.GetPixel(x, y));
                }else if(current < thresholds.z && current > thresholds.w){
                    tex.SetPixel(x, y, Color.Lerp(botTiled.GetPixel(x, y), midTiled.GetPixel(x, y), ((current - thresholds.w) /(thresholds.z - thresholds.w))));
                }
                else{
                    tex.SetPixel(x, y, botTiled.GetPixel(x, y));
                }
                
            }
            
        }
        tex.Apply();
        return tex;
    }

    static float GetMaxValue(float[,] input){
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

    static Texture2D Tile(Texture2D texture, Vector2 tiling){
        Texture2D tex = Clone(texture);
        Texture2D tile = Clone(texture);
        TextureScaler.scale(tile, (int)(texture.width/tiling.x), (int)(texture.height/tiling.y));

        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y ++)
            {
                tex.SetPixel(x, y, tile.GetPixel(x % tile.width, y % tile.height));        
            }
        }

        return tex;
    }

    static Texture2D Clone(Texture2D texture){
        Texture2D tex = new Texture2D(texture.width, texture.height);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y ++)
            {
                tex.SetPixel(x, y, texture.GetPixel(x, y));        
            }
        }
        return tex;
    }
    
    
}
