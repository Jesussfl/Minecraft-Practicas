using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{

    //Un perlin noise es un mapa en donde los puntos oscuros serian cercanos o iguales a cero y los puntos blancos cercanos o iguales a 1
    public static float Get2DPerlin (Vector2 position, float offset, float scale) 
    {
        /*
         * Position = a la posicion del mapa de perlin en dos dimensiones
         * Offset = al tamano del area de acuerdo a la posicion enviada
         * Scale = al zoom realizado al mapa de perlin
         */

        return Mathf.PerlinNoise((position.x + 0.1f) / Chunk.ChunkWidth * scale + offset, (position.y + 0.1f) / Chunk.ChunkWidth * scale + offset);
    }

    //Este metodo convierte el mapa de perlin 2D en un entorno 3D, de esta manera se pueden generar elementos dentro de los chunks, como los minerales
    public static bool Get3DPerlin (Vector3 position, float offset, float scale, float threshold)
    {
        float x = (position.x + offset + 0.1f) * scale;
        float y = (position.y + offset + 0.1f) * scale;
        float z = (position.z + offset + 0.1f) * scale;

        float AB = Mathf.PerlinNoise(x, y);
        float BC = Mathf.PerlinNoise(y, z);
        float AC = Mathf.PerlinNoise(x, z);
        float BA = Mathf.PerlinNoise(y, x);
        float CB = Mathf.PerlinNoise(z, y);
        float CA = Mathf.PerlinNoise(z, x);

        if ((AB + BC + AC + BA+ CB+ CA) / 6F > threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
