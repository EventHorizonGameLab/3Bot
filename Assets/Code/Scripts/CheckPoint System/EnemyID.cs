using UnityEngine;

public class EnemyID : MonoBehaviour
{
    public int enemyID { get; private set; }

    private void Start()
    {
        GenerateObjectID();
    }

    private void GenerateObjectID()
    {
        string combinedString = transform.position.ToString() + transform.rotation.ToString() + gameObject.name;
        enemyID = combinedString.GetHashCode();
    }
}
