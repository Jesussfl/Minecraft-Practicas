using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeData{

    /* 
     * Esta clase es la encargada de contener los atributos de los bloques
     */
     
   public static readonly int TextureAtlasSizeInBlocks = 4;  //Tamaño del sprite atlas que contiene las texturas ejem: 4x4

    public static readonly float NormalizedBlockTextureSize = 1f / TextureAtlasSizeInBlocks;


    public static readonly Vector3[] CubeVerts = new Vector3[8] //Contiene la posicion de cada vertice
    {
        new Vector3(0.0f, 0.0f, 0.0f), //Inferior Izquierda
        new Vector3(1.0f, 0.0f, 0.0f), //Inferior Derecha
        new Vector3(1.0f, 1.0f, 0.0f), //Superior Derecha
        new Vector3(0.0f, 1.0f, 0.0f), //Superior Izquierda
        new Vector3(0.0f, 0.0f, 1.0f), //Inferior Izquierda - Parte trasera
        new Vector3(1.0f, 0.0f, 1.0f), //Inferior Derecha - Parte Trasera
        new Vector3(1.0f, 1.0f, 1.0f), //Superior Derecha - Parte Trasera
        new Vector3(0.0f, 1.0f, 1.0f)  //Superior Izquierda - Parte Trasera
    };

    public static readonly Vector3[] FaceChecks = new Vector3[6] //Contiene la ubicacion de las caras del cubo
    {
        new Vector3(0.0f, 0.0f, -1.0f), //Cara Frontal
        new Vector3(0.0f, 0.0f, 1.0f),  //Cara Trasera
        new Vector3(0.0f, 1.0f, 0.0f),  //Cara Superior
        new Vector3(0.0f, -1.0f, 0.0f), //Cara Inferior
        new Vector3(-1.0f, 0.0f, 0.0f), //Cara Izquierda
        new Vector3(1.0f, 0.0f, 0.0f),  //Cara Derecha

    };

    public static readonly int[,] TrianglesInCube = new int[6, 4] //Contiene la unión de los vertices que conforman las caras de un cubo
    {
        {0, 3, 1, 2}, //Cara Frontal
        {5, 6, 4, 7}, //Cara Trasera
        {3, 7, 2, 6}, //Cara Superior
        {1, 5, 0, 4}, //Cara Inferior
        {4, 7, 0, 3}, //Cara Izquierda
        {1, 2, 5, 6}  //Cara Derecha
    };

    public static readonly Vector2[] UVsInCube = new Vector2[4] //Contiene los vertices de la cara de un cubo. esto es utilizado para colocar texturas
    {
        new Vector2(0.0f,0.0f),  //Inferior Izquierdo
        new Vector2(0.0f, 1.0f), //Inferior Derecho
        new Vector2(1.0f, 0.0f), //Superior Derecho
        new Vector2(1.0f, 1f),   //Superior Izquierdo

    };
}
