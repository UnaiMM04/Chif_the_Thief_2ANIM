using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] Transform playerTransform;
    //[SerializeField] Transform playerRotate;

    [SerializeField] float distance = 0f;
    [SerializeField] float verticalOffset = 0f;



    void Start()
    {
        
        

    }

    
    void Update()
    {
        
    }

    void LateUpdate()
    {
        Vector3 offset = new Vector3(0f, verticalOffset, distance);

        transform.position = playerTransform.position + offset;
        //transform.rotation = playerRotate.rotation;
    }
}
