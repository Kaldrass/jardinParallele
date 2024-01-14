using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;

// Script that describes the growth of a tree.
public class growthScript : MonoBehaviour
{
    private Tree t;
    public GameObject tree;
    public TreeData tData;
    public TreeGroupBranch tronc;
    public Material[] m;
    // Start is called before the first frame update
    void Start()
    {
        // Au début, il n'y a rien dans l'arbre
        
        t = tree.GetComponent<Tree>();
        tData = t.data as TreeData;
        // On ajoute une branche
        //tData.branchGroups = new TreeGroupBranch[1];
        tronc = tData.branchGroups[0];
        tData.root.seed = Random.Range(0, 999999);
        tData.root.rootSpread = Random.Range(1.0f, 5.0f);
        // On va vouloir set la hauteur du tronc à vec2 (0,0) au début
        tronc.seed = Random.Range(0, 999999);
        tronc.radius = Random.Range(0.5f, 1.0f);
        tronc.height = new Vector2(3.0f, 20.0f);
        tronc.distributionFrequency = Random.Range(1, 2);
        tronc.flareHeight = Random.Range(0.5f, 1.0f);

        // On applique les mêmes transformations à toutes les branches
        //for (int i = 1; i < tData.branchGroups.Length; i++)
        //{
        //    TreeGroupBranch branch = tData.branchGroups[i];
        //    branch.seed = Random.Range(0, 999999);
        //    branch.radius = Random.Range(0.5f, 1.0f);
        //    branch.height = new Vector2(1.0f, 1.0f);
        //    branch.distributionFrequency = Random.Range(1, 2);
        //    branch.flareHeight = Random.Range(0.5f, 1.0f);
        //    branch.visible = false;
        //}
        // On ne s'occupe pas des feuilles au début
        tData.UpdateMesh(tree.transform.worldToLocalMatrix, out m);
        Debug.Log("Current Seed: " + tData.root.seed);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
