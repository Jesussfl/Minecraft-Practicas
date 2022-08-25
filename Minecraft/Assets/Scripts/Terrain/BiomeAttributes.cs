using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*El ScriptableObject es una clase que permite almacenar grandes cantidades de datos compartidos independientes de instancias de script
 * Esto permite crear un nuevo objeto desde el menu en el editor
*/
[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Minecraft/Biome Attribute")]
public class BiomeAttributes : ScriptableObject
{

    //Aqui se guardan las caracteristicas de los biomas
    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;

    public Lode[] lode;
}

[System.Serializable] //Esto permite construir los atributos de la clase desde el editor
public class Lode //Contiene los valores de generacion de vetas de minerales dentro de los chunks
{
    public string nodeName;
    public byte blockID;
    public int minHeight;   //La altura minima en la que se genera
    public int maxHeight;   //La altura maxima en la que se encuentra la veta
    public float scale;     //El tamaño de la veta
    public float treshold;
    public float noiseOffset;
}
