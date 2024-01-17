using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;

// Script that describes the growth of a tree.
public class CroissanceBuisson : MonoBehaviour
{
    public Tree t;
    public TreeData tData;
    public Material[] m;

    [Range(0.01f, 10f)]
    public float growthDelay;
    private float croissTime;
    public float growthSpeed;

    public float xTrunk;
    public float yTrunk;

    [Range(0.1f, 1.0f)]
    public float radTrunk;

    public float xBranch;
    public float yBranch;
    public float radBranch;

    [Range(0.0f, 0.5f)]
    public float xLeaf;
    [Range(0.0f, 0.5f)]
    public float yLeaf;
    // Start is called before the first frame update
    void Start()
    {
        // On initialise les valeurs des variables
        croissTime = Time.time + growthDelay;
        //growthSpeed = 0.2f;

        xTrunk = 1.0f;
        yTrunk = 2.0f;
        radTrunk = Random.Range(0.5f, 1.0f); ;

        xBranch = 0.0f;
        yBranch = 0.2f;
        radBranch = Random.Range(0.0f, 0.2f);

        xLeaf = 0.0f;
        yLeaf = 0.1f;

        tData = t.data as TreeData;
        var tronc = tData.branchGroups[0];
        var branche = tData.branchGroups[1];
        var feuille = tData.leafGroups[0];

        // On va vouloir set la hauteur du tronc à vec2 (0,0) au début

        tronc.height = new Vector2(xTrunk, yTrunk);
        tronc.radius = radTrunk;
        tronc.seed = 1234;



        branche.height = new Vector2(xBranch, yBranch); // from 0 - 10
        branche.radius = radBranch;
        branche.distributionFrequency = 45;
        //  branche.distributionMode = "Random"
        branche.seed = 181957;

        feuille.seed = 1234;
        feuille.distributionFrequency = 55;
        //feuille.distributionMode = "Opposite";
        //feuille.geometryMode = "2";
        feuille.size = new Vector2(xLeaf, yLeaf);

        tData.UpdateSeed(Random.Range(0, 999999));
        tData.UpdateMesh(transform.worldToLocalMatrix, out m);

        //Debug.Log("Current Seed: " + tData.root.seed);
        InvokeRepeating("MyUpdate", 1, 0.1f);

    }
    public void MyUpdate()
    {
        if (yLeaf <= 0.75)
        {
            if (Time.time >= croissTime)
            {
                TrunkLength();
                TrunkRadius();
                if (Time.time >= 2.00f)
                {
                    BranchLength();
                    BranchRadius();
                    if (Time.time >= 6.00f)
                    {
                        LeafSize();
                    }
                }
                croissTime = Time.time + growthDelay;

                tData.UpdateMesh(transform.worldToLocalMatrix, out m);
            }
        }
    }

    
    void TrunkLength()
    {
        xTrunk += growthSpeed;
        yTrunk += growthSpeed;
        xTrunk = Mathf.Clamp(xTrunk, 4.0f, 4.3f);
        yTrunk = Mathf.Clamp(yTrunk, 5.0f, 5.3f);

        tData.branchGroups[0].height = new Vector2(xTrunk, yTrunk);
    }
    void TrunkRadius()
    {
        radTrunk += growthSpeed;
        radTrunk = Mathf.Clamp(radTrunk, 0.3f, 0.6f);

        tData.branchGroups[0].radius = radTrunk;
    }
    void BranchLength()
    {
        if (xBranch <= 5.0f && yBranch <= 5.2)
        {
            xBranch += growthSpeed;
            yBranch += growthSpeed;

            xBranch = Mathf.Clamp(xBranch, 4.5f, 5.0f);
            yBranch = Mathf.Clamp(yBranch, 4.7f, 5.2f);
            tData.branchGroups[1].height = new Vector2(xBranch, yBranch);
        }
    }
    void BranchRadius()
    {
        radBranch += growthSpeed;
        radBranch = Mathf.Clamp(radBranch, 0.0f, 0.1f);

        tData.branchGroups[1].radius = radBranch;
    }

    void LeafSize()
    {
        xLeaf += growthSpeed;
        yLeaf += growthSpeed;

        xLeaf = Mathf.Clamp(xLeaf, 0.3f, 0.75f);
        yLeaf = Mathf.Clamp(yLeaf, 0.5f, 1.0f);

        tData.leafGroups[0].size = new Vector2(xLeaf, yLeaf);
    }
}
