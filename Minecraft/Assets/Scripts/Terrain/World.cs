using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    //Referencias
    Chunk[,] chunks = new Chunk[worldSizeInChunks, worldSizeInChunks]; //Contiene los chunks generados en el mundo
    public Transform player; //Referencia del jugador
    public Vector3 spawnPosition; 
    public Material material;
    public BlockType[] blocktypes;
    public BiomeAttributes biome;

    //Atributos
    public static readonly int worldSizeInChunks = 100; //Tamano del mundo
    public static readonly int viewDistanceInChunks = 5; //Distancia de renderizado de chunks
    public int seed; //Semilla del terreno

    List<Chunk.ChunkCoord> activeChunks = new List<Chunk.ChunkCoord>(); //Lista de los chunks que estan activos/renderizados

    Chunk.ChunkCoord playerLastChunkCoord;
    Chunk.ChunkCoord playerChunkCoord;

    #region Encapsulamiento
    public static int worldSizeInCubes
    {
        get { return worldSizeInChunks * Chunk.ChunkWidth; }
    }

    #endregion


    #region Metodos
    private void Start()
    {
        Random.InitState(seed);

        //Con el spawn position, el jugador aparecera en medio del terreno, Importante: no es la coordenada 0,0
        spawnPosition = new Vector3((worldSizeInChunks * Chunk.ChunkWidth) / 2f, Chunk.ChunkHeight + 10f, (worldSizeInChunks * Chunk.ChunkWidth) / 2f); //250, 15, 250
        GenerateTerrain();
        playerLastChunkCoord = GetChunkCoordFromVector3(player.position);

    }
    private void Update()
    {
        playerChunkCoord = GetChunkCoordFromVector3(player.position);

        //if (!playerChunkCoord.Equals(playerLastChunkCoord)) //Si el jugador se mueve fuera del chunk en el que estaba, se generan nuevos chunks de acuerdo a la posicion del jugador y la distancia de renderizado
        //    CheckViewDistance();
    
    }
    private void GenerateTerrain()//Este metodo itera segun la variable worldSize para contruir el mundo en base a la cantidad de chunks
    {
        //Con estas iteraciones se obtiene el resultado de ver 5 chunks a la izquierda, 5 chunks a la derecha, al frente y atras
        for (int x = (worldSizeInChunks / 2) - viewDistanceInChunks; x < (worldSizeInChunks / 2) + viewDistanceInChunks; x++) //45, 55 = 10
        {
            for (int z = (worldSizeInChunks / 2) - viewDistanceInChunks; z < (worldSizeInChunks / 2) + viewDistanceInChunks; z++) //45 , 55 = 10
            {
                CreateChunk(x, z);
            }
        }

        player.position = spawnPosition;
    }
    Chunk.ChunkCoord GetChunkCoordFromVector3(Vector3 pos) //Retorna una referencia con atributos de las coordenadas ya asignadas a partir de un vector enviado por parametro
    {
        int x = Mathf.FloorToInt(pos.x) / Chunk.ChunkWidth;
        int z = Mathf.FloorToInt(pos.z) / Chunk.ChunkWidth;
        return new Chunk.ChunkCoord(x, z); 
    }
    public Chunk GetChunkFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x) / Chunk.ChunkWidth;
        int z = Mathf.FloorToInt(pos.z) / Chunk.ChunkWidth;
        return chunks[x, z];
    }
    private void CheckViewDistance() //Almacena nuevos chunks activos y elimina los que no estan cargados o activos segun la distancia de renderizado del jugador
    {
        Chunk.ChunkCoord coord = GetChunkCoordFromVector3(player.position); //Se instancia como coordenada la posicion del jugador 
        //50, 50

        List<Chunk.ChunkCoord> previouslyActiveChunks = new List<Chunk.ChunkCoord>(activeChunks);

        for (int x = coord.x - viewDistanceInChunks; x < coord.x + viewDistanceInChunks; x++) //45 //55 = 10 iteraciones
        {
            for (int z = coord.z - viewDistanceInChunks; z < coord.z + viewDistanceInChunks; z++) //45 //55 = 10 iteraciones
            {
                if (CheckIfChunkIsInWorld(new Chunk.ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null) //Si el chunk aun no ha sido creado
                        CreateChunk(x, z);
                    else if (!chunks[x, z].IsActive) //Si el chunk esta inactivo
                    {
                        chunks[x, z].IsActive = true;
                        activeChunks.Add(new Chunk.ChunkCoord(x, z));
                    }
                }
                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals (new Chunk.ChunkCoord(x, z)))
                    {
                        previouslyActiveChunks.RemoveAt(i);
                    }
                }
            }
        }
            foreach (Chunk.ChunkCoord c in previouslyActiveChunks)
                chunks[c.x, c.z].IsActive = false;
    }
    private void CreateChunk(int x, int z) //Metodo para crear un chunk
    {
        chunks[x, z] = new Chunk(new Chunk.ChunkCoord(x, z), this);
        activeChunks.Add(new Chunk.ChunkCoord(x, z));
    }
    private bool CheckIfChunkIsInWorld(Chunk.ChunkCoord coord) //Valida que el chunk este dentro del tamano del terreno
    {
        if (coord.x > 0 && coord.x < worldSizeInChunks - 1 && coord.z > 0 && coord.z < worldSizeInChunks - 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool CheckChunkInWorld(Vector3 pos) //Comprueba si el chunk esta dentro del terreno para no dibujar sus caras
    {
        if (pos.x >= 0 && pos.x < worldSizeInCubes && pos.y >= 0 && pos.y < Chunk.ChunkHeight && pos.z >= 0 && pos.z < worldSizeInCubes)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public byte GetCubeType(Vector3 pos) //Retorna el codigo del bloque
    {
        int yPos = Mathf.FloorToInt(pos.y); //Transforma la altura a int
        
        //Si esta fuera del mundo, retorna aire
        if (!CheckChunkInWorld(pos))
            return 0;

        //Si esta en la parte de abajo del chunk, retorna bedrock
        if (yPos == 0)
        {
            return 1;
        }

        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biome.terrainScale)) + biome.solidGroundHeight;
        byte voxelValue = 0;
        
        if (yPos == terrainHeight) //Si esta en la parte mas alta del terreno genera cesped
        {
            voxelValue = 4;
        }
        else if (yPos < terrainHeight && yPos > terrainHeight - 4) //Si esta en por debajo de la altura maxima, genera tierra 3 capas hacia abajo
        {
            voxelValue = 3;
        }
        else if (yPos > terrainHeight) //Si la altura es mayor a la altura del terreno, genera aire
            return 0;
        else //Si no, genera piedra
            voxelValue = 2;


        if (voxelValue == 2) //Si hay piedra se ejecuta un scriptable object que contiene los datos de biomas
        {
            foreach (Lode lode in biome.lode) 
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.treshold))
                        voxelValue = lode.blockID;
            }
        }

            return voxelValue;
        

        

    }


    #endregion
}




[System.Serializable] //Esto es utilizado para que aparezca en el editor y podamos crear tantos tipos de bloques como queramos
public class BlockType
{

    //Atributos

    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]

    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    
    public int GetTextureID (int faceIndex) //Retorna un ID del sprite atlas dependiendo de la cara, estos valores son asignados desde el editor
    {

        //De esta forma se pueden asignar distintas texturas para cada cara
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID");
                return 0;
        }
    }
}
