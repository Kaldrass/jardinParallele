using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
public class Croissance : MonoBehaviour
{
    private Tree t;
    public GameObject tree;
    public TreeData tData;
    public Material[] m;
    // Start is called before the first frame update
    void Start()
    {
        t = tree.GetComponent<Tree>();
        tData = t.data as TreeEditor.TreeData;

        tData.root.seed = Random.Range(0, 999999);
        tData.root.rootSpread = Random.Range(1.0f, 5.0f);
        tData.branchGroups[0].radius = Random.Range(0.5f, 1.0f);
        tData.branchGroups[0].seed = Random.Range(0, 999999);
        tData.branchGroups[0].flareHeight = Random.Range(0.5f, 1.0f);
        tData.branchGroups[0].distributionFrequency = Random.Range(1, 2);
        tData.UpdateMesh(tree.transform.worldToLocalMatrix, out m);
        Debug.Log("Current Seed: " + tData.root.seed);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
