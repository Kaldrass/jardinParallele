using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFPS : MonoBehaviour
{
    public Camera playerCam;
    CharacterController characterController;
    public float vitesseDeplacement = 7.5f;
    public float vitesseSaut = 8f;
    float gravity = 20f;
    Vector3 mouvementDirection;

    float rotationX = 0;
    public float vitesseRotation = 2.0f;
    public float rotationXLimit = 45.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        characterController = GetComponent<CharacterController> ();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float speedZ = Input.GetAxis("Vertical");
        float speedX = Input.GetAxis("Horizontal");
        

        speedX = speedX * vitesseDeplacement;
        speedZ = speedZ * vitesseDeplacement;

        mouvementDirection = forward*speedZ+right*speedX;

        if(characterController.isGrounded)
        {
            mouvementDirection.y -= gravity* Time.deltaTime;
            if (Input.GetButton("Jump"))
            {
                mouvementDirection.y = vitesseSaut;
            }
        }
        
        characterController.Move(mouvementDirection*Time.deltaTime);

        rotationX += -Input.GetAxis("Mouse Y") * vitesseRotation;
        rotationX = Mathf.Clamp(rotationX,-rotationXLimit, rotationXLimit);

        playerCam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * vitesseRotation, 0);
    }
}
