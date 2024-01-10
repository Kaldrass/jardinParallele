using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tronc : MonoBehaviour
{
    public GameObject tree;
    private Tree t;
    public Material[] m;


    // Update is called once per frame
    public void UpdateTree()
    {
        t= tree.GetComponent<Tree> ();
        var tData = t.data as TreeEditor.TreeData;
        var root = tData.root;
        root.seed = Random.Range(0, 99999);
        tData.UpdateMesh(tree.transform.worldToLocalMatrix, out m);
        Debug.Log("Current Seed: " + root.seed);
    }
}
