using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;

// Script that describes the growth of a tree.
public class growthScript : MonoBehaviour
{
    public Tree t;
    public TreeData tData;
    public Material[] m;

    [Range(0.01f,10f)]
    public float growthDelay;
    private float croissTime;
    public float growthSpeed;

    public float xTrunk;
    public float yTrunk;
    private float height; // min of the height, =xTrunk but not updated in TrunkLength()

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
        height = xTrunk;

        xBranch = 0.0f;
        yBranch = 0.2f;
        radBranch = Random.Range(0.0f, 0.2f);

        xLeaf = 0.0f;
        yLeaf = 0.1f;

        tData = t.data as TreeData;
        var tronc = tData.branchGroups[0];

        // On va vouloir set la hauteur du tronc à vec2 (0,0) au début
        
        tronc.height = new Vector2(xTrunk, yTrunk);
        tronc.radius = radTrunk;
        tData.branchGroups[1].height = new Vector2(xBranch, yBranch); // from 0 - 10
        tData.branchGroups[1].radius = radBranch;
        tData.branchGroups[1].distributionFrequency = 1;
        tData.leafGroups[0].size = new Vector2(xLeaf, yLeaf);
        
        tData.UpdateFrequency(1);
        tData.UpdateSeed(Random.Range(0, 999999));
        tData.UpdateMesh(transform.worldToLocalMatrix, out m);

        //Debug.Log("Current Seed: " + tData.root.seed);

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= croissTime) 
        {
            UpdateTrunk();
            if (Time.time >= 5.00f)
            {
                UpdateBranch();
                if(Time.time >= 7.00f)
                {
                    UpdateLeaf();
                }
            }
            croissTime = Time.time + growthDelay;
            tData.UpdateFrequency(1);
            tData.UpdateMesh(transform.worldToLocalMatrix, out m);
        }
    }
    void TrunkLength()
    {
        xTrunk += growthSpeed;
        yTrunk += growthSpeed;
        tData.branchGroups[0].height = new Vector2(xTrunk, yTrunk);
    }   
    void TrunkRadius()
    {
        radTrunk += growthSpeed;
        radTrunk = Mathf.Clamp(radTrunk, 0.0f, 1.0f);

        tData.branchGroups[0].radius = radTrunk;
    }
    void BranchLength()
    {
        xBranch += growthSpeed;
        yBranch += growthSpeed;
        xBranch = Mathf.Clamp(xBranch, 0.0f, 10.0f);
        yBranch = Mathf.Clamp(yBranch, 0.0f, 10.0f);
        tData.branchGroups[1].height = new Vector2(xBranch, yBranch);
    }
    void BranchRadius()
    {
        radBranch += growthSpeed;
        radBranch = Mathf.Clamp(radBranch, 0.0f, 1.0f);

        tData.branchGroups[1].radius = radBranch;
    }

    void LeafSize()
    {
        xLeaf += growthSpeed;
        yLeaf += growthSpeed;

        xLeaf = Mathf.Clamp(xLeaf, 0.0f, 0.75f);
        yLeaf = Mathf.Clamp(yLeaf, 0.0f, 1.0f);

        tData.leafGroups[0].size = new Vector2(xLeaf, yLeaf);
    }
    void UpdateTrunk()
    {
        TrunkLength();
        TrunkRadius();
        if (xTrunk - height > 0.5f)
        {
            height = xTrunk;
            tData.branchGroups[1].distributionFrequency++;
        }

    }
    void UpdateBranch()
    {
        BranchLength();
        BranchRadius();
    }
    void UpdateLeaf()
    {
        LeafSize();
    }   
}
