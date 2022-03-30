
using UnityEngine;

public class Cylindre : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GenerateColor();
    }

    public void GenerateColor()
    {
        GetComponent<MeshRenderer>().sharedMaterial.color = Random.ColorHSV();
    }

    public void Reset()
    {
        GetComponent<MeshRenderer>().sharedMaterial.color = Color.white;
    }
}
