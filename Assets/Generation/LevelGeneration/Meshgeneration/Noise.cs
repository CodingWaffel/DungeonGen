using UnityEngine;


namespace MapGen
{
    public static class Noise {
        public static float [,] CalcNoise(int mapWidth, int mapHeight,Vector2 offset, float frequency = 1, bool inverseLerp = true)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random prng = new System.Random();
            Vector2 octaveOffset = new Vector2(prng.Next(+100000,100000)+offset.x, prng.Next(+100000,100000)+offset.y);

            float maxNoiseHeight = 1;
            float minNoiseHeight = -1;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            for (int y=0;y<mapHeight;y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float noiseHeight = 0;

                        float sampleX = ((x-halfWidth) / frequency + octaveOffset.x) *.1f ;
                        float sampleY = ((y-halfHeight) / frequency + octaveOffset.y) * .1f;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;
                        noiseHeight += perlinValue;

                    if(noiseHeight > maxNoiseHeight )
                    {
                        maxNoiseHeight = noiseHeight;
                    }else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;

                }
            }
            if(inverseLerp)
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                    }
                }


            return noiseMap;
        }
    
    
        public static float [,] CalcNoise(int mapWidth, int mapHeight,Vector2 offset, int octaves, float amplitude, float frequency, float persistance, float lacunarity, bool inverseLerp = true)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random prng = new System.Random();
            Vector2 octaveOffset = new Vector2(prng.Next(+100000,100000)+offset.x, prng.Next(+100000,100000)+offset.y);

            float maxNoiseHeight = 1;
            float minNoiseHeight = -1;

            float halfWidth = mapWidth / 2f;
            float halfHeight = mapHeight / 2f;

            float originalAmplitude = amplitude;
            float originalFrequency = frequency;

            for (int y=0;y<mapHeight;y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float noiseHeight = 0;
                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = ((x-halfWidth) / frequency + octaveOffset.x) *.1f ;
                        float sampleY = ((y-halfHeight) / frequency + octaveOffset.y) * .1f;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    amplitude = originalAmplitude;
                    frequency = originalFrequency;
                        

                    if(noiseHeight > maxNoiseHeight )
                    {
                        maxNoiseHeight = noiseHeight;
                    }else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                    noiseMap[x, y] = noiseHeight;

                }
            }
            if(inverseLerp)
                for (int y = 0; y < mapHeight; y++)
                {
                    for (int x = 0; x < mapWidth; x++)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                    }
                }


            return noiseMap;
        }
    }
}