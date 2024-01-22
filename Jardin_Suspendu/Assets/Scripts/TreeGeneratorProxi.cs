using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;

public class TreeGeneratorProxi : MonoBehaviour
{
    public int seed;
    public int width;
    public int height;

    [Range(0.01f, 10f)]
    public float scale;
    public GameObject treePrefab;
    //public Tree arbre1;
    //public Tree arbre2;
    //public Tree arbre3;

    [Range(0.01f, 1f)]
    public float acceptancePoint;

    public LayerMask groundLayer;
    void Start()
    {
        for (int y = 185; y < height; y++)
        {
            for (int x = 335; x < width; x++)
            {
                float xCoord = x / scale + seed;
                float yCoord = y / scale + seed;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                if (sample >= acceptancePoint)
                {
                    if (Physics.Raycast(new Vector3(x, 100, y), Vector3.down, out RaycastHit hit, 200f, groundLayer))
                    {
                        float yPoint;
                        yPoint = hit.point.y;
                        GameObject tree = Instantiate(treePrefab, new Vector3(x, yPoint, y), Quaternion.identity);
                        tree.transform.SetParent(this.transform);
                    }




                    //tree.GetComponent<growthScript>().seed = Random.Range(0, 999999);
                    //tree.GetComponent<growthScript>().width = Random.Range(1, 5);
                    //tree.GetComponent<growthScript>().height = Random.Range(1, 5);
                    //tree.GetComponent<growthScript>().scale = Random.Range(0.5f, 1.0f);
                    //tree.GetComponent<growthScript>().acceptancePoint = Random.Range(0.5f, 1.0f);
                }
            }
        }
    }

}
