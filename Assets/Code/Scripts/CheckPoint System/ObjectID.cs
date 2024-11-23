using UnityEngine;

public class ObjectID : MonoBehaviour
{
    public int objectID { get; private set; }

    private void Start()
    {
        GenerateObjectID();
    }

    private void GenerateObjectID()
    {
        string combinedString = transform.position.ToString() + transform.rotation.ToString() + gameObject.name;
        objectID = combinedString.GetHashCode();
    }
}
