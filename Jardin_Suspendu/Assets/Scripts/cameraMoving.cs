using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;

    float horizontalMove;
    float forwardMove;
    float verticalMove;
    Vector3 moveDirection;

    Rigidbody rb;
    [SerializeField] private float sensX = 100f;
    [SerializeField] private float sensY = 100f;
    Camera cam;

    float mouseX;
    float mouseY;

    float multiplier = .05f;

    float xRotation;
    float yRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        KeyboardInput();
        MouseInput();
        
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
    void KeyboardInput()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        forwardMove = Input.GetAxisRaw("Vertical");
        // map CTRL to go down and space to go up
        verticalMove = Input.GetKey(KeyCode.Space) ? 1 : Input.GetKey(KeyCode.LeftControl) ? -1 : 0;

        moveDirection = transform.right * horizontalMove + transform.forward * forwardMove + transform.up * verticalMove;
            } 
    void MouseInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
    
        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
    private void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        rb.velocity = moveDirection.normalized * moveSpeed;
    }
}