using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseLook : MonoBehaviour
{

    //Referencias
    public Transform player;
    private PlayerInputs playerInputs;
    private Vector2 lookDirection;
    public Transform highlightBlock;
    public Transform placeBlock;
   // public Transform cam;
    private World world;

    //Variables
    public float mouseSensitivity = 80f;
    private float xRotation = 0f;

    #region No usado

    public float checkIncrement = 0.1f;
    public float reach = 8f;

    //public Text selectedBlockText;

    public byte selectedBlockIndex = 1; 
    #endregion


    #region Metodos
    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }
    private void OnEnable()
    {
        playerInputs.Enable();

    }
    private void OnDisable()
    {
        playerInputs.Disable();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {

        GetLookValues();
        RotateCameraAndPlayer();
        //PlaceCursorBlocks();
    }
    private void GetLookValues() //Obtiene valores al mover la camara
    {
        Vector2 inputValues = playerInputs.Player.Look.ReadValue<Vector2>();
        lookDirection.x = inputValues.x * Time.deltaTime * mouseSensitivity;
        lookDirection.y = inputValues.y * Time.deltaTime * mouseSensitivity;


    }
    private void RotateCameraAndPlayer() //Rota la direccion del jugador
    {
        xRotation -= lookDirection.y;

        xRotation = Mathf.Clamp(xRotation, -90, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //Rota la camara


        player.Rotate(Vector3.up * lookDirection.x);
    }
   
    #endregion

}
