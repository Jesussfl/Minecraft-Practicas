using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    #region Atributos
    //Referencias
    private PlayerInputs playerInputs;
    public CharacterController characterController;
    public Transform groundCheck;
    public LayerMask groundMask;

    //Variables
    private Vector3 direction;
    private Vector3 velocity;
    private bool isGrounded;

    public float groundDistance = 0.4f;
    public float gravity = -9.8f;
    public float speed = 10f;
    public float jumpHeight = 3f;
    public float sprintSpeed = 10f;

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

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        Move();
        Jump();
        ApplyGravity();
    }


    private void GetMovementPosition()
    {
        Vector2 inputValues = playerInputs.Player.Move.ReadValue<Vector2>();

        direction = Vector3.ClampMagnitude((transform.right * inputValues.x + transform.forward * inputValues.y), 1f);
    }
    private void ApplyGravity() //Aplica constantemente gravedad al personaje
    {

        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    private void Move() //Mueve el personaje
    {
        GetMovementPosition();
        characterController.Move(direction * speed * Time.deltaTime);
    }
    private void Jump() //Salta el personaje
    {
        if (playerInputs.Player.Jump.IsPressed() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    #endregion

}
