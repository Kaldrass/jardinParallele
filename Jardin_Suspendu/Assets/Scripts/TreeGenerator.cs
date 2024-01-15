using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;

public class TreeGenerator : MonoBehaviour
{
    public GameObject treeObject;
    public GameObject treesInJardinObject;

    public int treeNb;

    List<GameObject> treesList = new List<GameObject>();
    GameObject[] treesArray;
    // Start is called before the first frame update
    void Start()
    {
       for (int i = 0;i<=treeNb;i++)
        {
            treesList.Add(Instantiate<GameObject>(treeObject));
            treesArray = treesList.ToArray();
            treesArray[i].transform.position = new Vector3(Random.Range(0,500), 0, Random.Range(0,500));
            treesArray[i].transform.parent = treesInJardinObject.transform;
        }
    }


    // Update is called once per frame
    
}
