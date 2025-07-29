using UnityEngine;
using UnityEngine.Events;


namespace MyUnityPackage.Controller
{
public abstract class PersonController : MonoBehaviour
{

    [Header("Références")]
    [SerializeField] protected Transform cameraHolder;

    [Header("Paramètres")]
    [SerializeField] protected float lookSensitivity = 0.4f;
    [SerializeField] protected float maxPitch = 89f;
    [SerializeField] protected float moveSpeed = 20f;
    [SerializeField] protected float jumpForce = 5f;

    protected float pitch = 0f;
    protected float yaw = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    virtual protected void Init()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = transform.eulerAngles.y;

    }
    abstract protected void Move(Vector2 move);
    virtual protected void LookAround(Vector2 look){}
    virtual protected void Jump()
    {

    }
    virtual protected void Crouch(){}
}

}
