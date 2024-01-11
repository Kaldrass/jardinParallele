using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;

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
        var branchGroups = tData.branchGroups;
        root.seed = Random.Range(0, 99999);
       // branchGroups.Length = 0.5;
        
        tData.UpdateMesh(tree.transform.worldToLocalMatrix, out m);
        Debug.Log("Current Seed: " + root.seed);
    }
}
