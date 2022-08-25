using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PutBlocks : MonoBehaviour
{
    #region Atributos
    //Referencias
    public Transform shootingPoint; //Punto de donde sale el raycast
    public GameObject blockObject; //Bloque que se pondrá
    public Transform parent; //Padre en donde se almacenarán los bloques colocados
    public GameObject lastHightlightedBlock;
    public Color normalColor;
    public Color highlightedColor;

    #endregion


    #region Metodos
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            BuildBlock(blockObject);
        }
        if (Input.GetMouseButtonDown(0))
        {
            DestroyBlock();
        }
        HighlightBlock();
    }
    private void BuildBlock(GameObject block) //Instancia un nuevo bloque en la posicion colocada
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
        {

            if (hitInfo.transform.tag == "Block" || hitInfo.transform.tag == "Chunk")
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x + hitInfo.normal.x / 2), Mathf.RoundToInt(hitInfo.point.y + hitInfo.normal.y / 2), Mathf.RoundToInt(hitInfo.point.z + hitInfo.normal.z / 2));
                //Se suma y se divide entre dos para que se puedan colocar bloques fuera de la cara en la que apunta el raycast y no dentro del cubo
                Instantiate(block, spawnPosition, Quaternion.identity, parent);
            }
            else
            {
                Vector3 spawnPosition = new Vector3(Mathf.RoundToInt(hitInfo.point.x), Mathf.RoundToInt(hitInfo.point.y), Mathf.RoundToInt(hitInfo.point.z));
                Instantiate(block, spawnPosition, Quaternion.identity, parent);
            }
        }
    }
    void DestroyBlock() //Destruye el bloque en la posicion colocada
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Block")
            {
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }
    void HighlightBlock() //Ilumina la textura del bloque si es asignada con un tipo que acepta colores, de lo contrario no funciona
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo))
        {
            if (hitInfo.transform.tag == "Block")
            {
                if (lastHightlightedBlock == null)
                {
                    lastHightlightedBlock = hitInfo.transform.gameObject;
                    hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = highlightedColor;
                }
                else if (lastHightlightedBlock != hitInfo.transform.gameObject)
                {
                    lastHightlightedBlock.GetComponent<Renderer>().material.color = normalColor;
                    hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = highlightedColor;
                    lastHightlightedBlock = hitInfo.transform.gameObject;
                }
            }
        }
    } 
    #endregion
}
