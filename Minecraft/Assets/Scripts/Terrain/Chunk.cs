using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    #region Atributos

    //Tamaño de un Chunk
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 85;

    private readonly GameObject chunkObject;
    private readonly World world;
    private readonly ChunkCoord coord;
    private readonly MeshRenderer meshRenderer;
    private readonly MeshFilter meshFilter;
    private readonly MeshCollider meshCollider;

    private int vertexIndex = 0;


    //Listas para construir cubos
    private readonly List<Vector3> vertices = new List<Vector3>(); //Contiene los vertices de un cubo
    private readonly List<int> triangles = new List<int>(); //Almacena la posicion para crear los triangulos de un cubo
    private readonly List<Vector2> uvs = new List<Vector2>(); //Contiene las texturas de los cubos


    //Contiene los identificadores de cada bloque
    public readonly byte[,,] cubeMap = new byte[ChunkWidth, ChunkHeight, ChunkWidth];

    #endregion

    public Chunk(ChunkCoord Coord, World World) //Constructor
    {
        world = World;
        coord = Coord;

        //Instanciaciones
        chunkObject = new GameObject(); 

        //Componentes
        meshFilter = chunkObject.AddComponent<MeshFilter>();

        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = world.material;

        meshCollider = chunkObject.AddComponent<MeshCollider>();

        chunkObject.transform.SetParent(world.transform);
        chunkObject.tag = "Chunk";
        chunkObject.transform.position = new Vector3(coord.x * ChunkWidth + 0.5f, 0.5f, coord.z * ChunkWidth + 0.5f);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;

        //Llamada a métodos
        PopulateChunkMap();
        CreateChunk();
        CreateMesh();

        meshCollider.sharedMesh = meshFilter.mesh;
    }

    #region Encapsulamiento
    public bool IsActive //Permite colocar un chunk activo o inactivo
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3 Position //Almacena la posicion del chunk actual
    {
        get { return chunkObject.transform.position; }
    } 
    #endregion


    #region Metodos
    private void PopulateChunkMap() //Asigna el tipo de bloque dependiendo de la altura
    {

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {

                    cubeMap[x, y, z] = world.GetCubeType(new Vector3(x, y, z) + Position);

                }
            }
        }
    }
    private void CreateChunk() //Crea los cubos que van formando el chunk
    {
        ClearCube();
        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    //Se envia como parametro la posicion inicial en la que se empieza a crear el bloque
                    if (world.blocktypes[cubeMap[x,y,z]].isSolid) //Valida que los bloques sean solidos
                    {
                    CreateCube(new Vector3(x, y, z));

                    }

                }
            }
        }
        CreateMesh();
    }
    private void CreateCube(Vector3 pos) //Dibuja las caras de un cubo
    {

        for (int p = 0; p < 6; p++)
        {

            if (!CheckCubeSide(pos + CubeData.FaceChecks[p])) //Aqui se comprueba que haya otros cubos alrededor del actual
            {
                byte blockID = cubeMap[(int)pos.x, (int)pos.y, (int)pos.z]; //Esto busca el ID de la textura y es utilizado para construir la cara del cubo

                vertices.Add(pos + CubeData.CubeVerts[CubeData.TrianglesInCube[p, 0]]); 
                vertices.Add(pos + CubeData.CubeVerts[CubeData.TrianglesInCube[p, 1]]); 
                vertices.Add(pos + CubeData.CubeVerts[CubeData.TrianglesInCube[p, 2]]); 
                vertices.Add(pos + CubeData.CubeVerts[CubeData.TrianglesInCube[p, 3]]); 

                AddTexture(world.blocktypes[blockID].GetTextureID(p));

                triangles.Add(vertexIndex); 
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);

                vertexIndex += 4; //Se hace una sumatoria para aumentar triangulos a la malla del chunk

            }

        }
    }
    private bool CheckCubeSide(Vector3 pos)  //Valida que se dibujen solo caras externas del chunk
    {

        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);


        if (!CheckCubeInChunk(x, y, z))
        {

            return world.blocktypes[world.GetCubeType(pos+Position)].isSolid;


        }
        else
        {

        return world.blocktypes[cubeMap[x, y, z]].isSolid; //Esto es un valor de referencia que se asigna en el editor de unity, por lo general la mayoria de bloques son solidos
                                                           //Excepto el cristal por dar un ejemplo
        }
    }
    private void AddTexture(int textureID)
    {
        //Esto calcula la altura de la textura del bloque en el sprite atlas
        float y = textureID / CubeData.TextureAtlasSizeInBlocks; 
        float x = textureID - (y * CubeData.TextureAtlasSizeInBlocks); 

        x *= CubeData.NormalizedBlockTextureSize;
        y *= CubeData.NormalizedBlockTextureSize;

        y = 1f - y - CubeData.NormalizedBlockTextureSize;

        //Esto define el tamano del bloque en el sprite atlas
        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + CubeData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + CubeData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + CubeData.NormalizedBlockTextureSize, y + CubeData.NormalizedBlockTextureSize));
    }  //Anade la  textura correspondiente al bloque que se creara
    private bool CheckCubeInChunk(int x, int y, int z) //Valida que el bloque que se esta creando este en la parte interior o exterior del chunk
    {
        //Verifica el lado contrario de la cara para ver si cumple con las siguientes caracteristicas y asi solo dibujar caras externas al chunk
        if (x < 0 || x > ChunkWidth - 1 || y < 0 || y > ChunkHeight - 1 || z < 0 || z > ChunkWidth - 1)
            return false;
        else
            return true;
        
    } 
    private void CreateMesh() //Crea una malla que renderiza los cubos creados en un chunk
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    private void ClearCube()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }
    public void EditCube (Vector3 Pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(Pos.x);
        int yCheck = Mathf.FloorToInt(Pos.y);
        int zCheck = Mathf.FloorToInt(Pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);
        cubeMap[xCheck, yCheck, zCheck] = newID;
        CreateChunk();
    }

    private void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + CubeData.FaceChecks[p];

            if (!CheckCubeInChunk((int)currentVoxel.x,(int)currentVoxel.y ,(int)currentVoxel.z))
            {
                world.GetChunkFromVector3(currentVoxel + Position).CreateChunk();
            }
        }
    }
    #endregion


    public class ChunkCoord //Esta clase se encarga de asignar las coordenadas del chunk
    {

        //Atributos
        public readonly int x;
        public readonly int z;


        //Constructor
        public ChunkCoord (int _x, int _z)
        {
            x = _x;
            z = _z;
        }

        public ChunkCoord (Vector3 pos)
        {
            int xCheck = Mathf.FloorToInt(pos.x);
            int zCheck = Mathf.FloorToInt(pos.z);

            x = xCheck / ChunkWidth;
            z = zCheck / ChunkWidth;
        }
        public bool Equals(ChunkCoord other)
        {
            if (other == null)
            {
                return false;
            }
            else if(other.x == x && other.z ==z)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
