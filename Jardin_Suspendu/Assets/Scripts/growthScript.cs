using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;
using Unity.VisualScripting;

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

    [Range(0.1f, 1.0f)]
    public float radTrunk;

    public float xBranch;
    public float yBranch;
    public float radBranch;

    [Range(0.0f, 0.5f)]
    public float xLeaf;
    [Range(0.0f, 0.5f)]
    public float yLeaf;


    private float height; // min of the height, = xTrunk but not updated in TrunkLength()
    private float maxHeight; // Max of the height 

    private TreeGroupBranch branches;
    private TreeGroupBranch tronc;
    private TreeGroupLeaf feuilles;

    private bool growing = false;
    private bool tGrown, bGrown, lGrown = false;
    // Start is called before the first frame update
    void Start()
    {
        croissTime = Time.time + growthDelay;
        tData = t.data as TreeData;

        // PARTIE TRONC
     
        xTrunk = 1.0f;
        yTrunk = 2.0f;
        height = xTrunk;
        maxHeight = 33.0f;
        radTrunk = Random.Range(0.5f, 1.0f); ;
        tronc = tData.branchGroups[0];
        tronc.height = new Vector2(xTrunk, yTrunk);
        tronc.radius = radTrunk;
        
        // PARTIE BRANCHES
        
        xBranch = 0.0f;
        yBranch = 0.2f;
        radBranch = Random.Range(0.0f, 0.2f);
        branches = tData.branchGroups[1];
        branches.height = new Vector2(xBranch, yBranch); // from 0 - 10
        branches.radius = radBranch;
        branches.distributionMode = TreeGroup.DistributionMode.Opposite; // Distribution en opposé
        branches.distributionTwirl = 0.4f; // Rotation des branches
        branches.distributionFrequency = 1; // Nbr de branches
        branches.distributionPitch = 0.3f; // Inclinaison des branches
        branches.seekBlend = 0.1f; // cherche le soleil à 10%
        
        // PARTIE FEUILLES
        
        xLeaf = 0.0f;
        yLeaf = 0.1f;
        feuilles = tData.leafGroups[0];
        feuilles.size = new Vector2(xLeaf, yLeaf); //mini feuilles au début
        feuilles.distributionFrequency = 1; // Une unique feuille par branches, jusqu'à 50 (100 est bien mais plus coûteux)

        //MISE A JOUR DES DONNEES DE L'ARBRE DE VIE
        tData.UpdateFrequency(1);
        tData.UpdateSeed(Random.Range(0, 999999));
        tData.UpdateMesh(transform.worldToLocalMatrix, out m);

        //Debug.Log("Current Seed: " + tData.root.seed);

    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time >= croissTime && growing == false) 
        {
            growing = true;
            tGrown = UpdateTrunk();
            if (Time.time >= 3.00f)
            {
                bGrown = UpdateBranch();
                if(Time.time >= 5.00f)
                {
                    lGrown = UpdateLeaf();
                }
            }
            croissTime = Time.time + growthDelay;
            tData.UpdateFrequency(1);
            tData.UpdateMesh(transform.worldToLocalMatrix, out m);
            growing = (tGrown && bGrown && lGrown); // Fonctionne car Update est en séquentiel. True quand tout est grown
        }
    }
    void TrunkLength()
    {
        xTrunk += growthSpeed;
        yTrunk += growthSpeed;
        xTrunk = Mathf.Clamp(xTrunk, 0.0f, maxHeight);
        yTrunk = Mathf.Clamp(yTrunk, 1.0f, maxHeight * 1.2f); // Not just +1, we want to let it grow more 
        //Debug.Log("Trunk Size : " + xTrunk + " " + yTrunk);
        tData.branchGroups[0].height = new Vector2(xTrunk, yTrunk);
    }   
    void TrunkRadius()
    {
        radTrunk += growthSpeed / 20.0f;
        radTrunk = Mathf.Clamp(radTrunk, 0.0f, maxHeight/20.0f);

        tData.branchGroups[0].radius = radTrunk;
    }
    void BranchLength()
    {
        xBranch += growthSpeed;
        yBranch += growthSpeed;
        xBranch = Mathf.Clamp(xBranch, 0.0f, xTrunk * 0.25f);
        yBranch = Mathf.Clamp(yBranch, 0.0f, maxHeight * 0.5f);
        //Debug.Log("Branch size : " + xBranch + " " + yBranch);
        branches.height = new Vector2(xBranch, yBranch);
    }
    void BranchRadius()
    {
        radBranch += growthSpeed;
        radBranch = Mathf.Clamp(radBranch, 0.0f, 1.0f);

        branches.radius = radBranch;
    }

    void LeafSize()
    {
        xLeaf += growthSpeed * 0.1f;
        yLeaf += growthSpeed * 0.1f;

        xLeaf = Mathf.Clamp(xLeaf, 0.0f, 0.8f);
        yLeaf = Mathf.Clamp(yLeaf, 0.0f, 1.2f);

        tData.leafGroups[0].size = new Vector2(xLeaf, yLeaf);
    }
    bool UpdateTrunk()
    {
        var lengthValue = xTrunk;
        var radiusValue = radTrunk;
        TrunkLength();
        TrunkRadius();
        return (lengthValue==xTrunk && radiusValue == radTrunk); // true if trunk has grown
    }
    bool UpdateBranch()
    {
        var lengthValue = xBranch;
        var radiusValue = radBranch;
        var numberValue = branches.distributionFrequency;
        if (xTrunk - height > 0.5f)
        {
            height = xTrunk;
            branches.distributionFrequency++; // On ajoute une branche
            branches.distributionFrequency = (int)(Mathf.Clamp(branches.distributionFrequency, 1.0f, 30.0f)); // 30 branches maximum, question de ressources
        }
        BranchLength();
        BranchRadius();
        return (lengthValue == xBranch && radiusValue == radBranch && numberValue == branches.distributionFrequency);
    }
    bool UpdateLeaf()
    {
        var sizeValue = xLeaf;
        var numberValue = feuilles.distributionFrequency;
        feuilles.distributionFrequency+=2; // On ajoute 2 feuilles          
        feuilles.distributionFrequency = ((int)Mathf.Clamp(feuilles.distributionFrequency, 1.0f, 40.0f)); // MAXIMUM 100 FEUILLES PAR BRANCHE
        LeafSize();
        return (sizeValue == xLeaf && numberValue == feuilles.distributionFrequency);
    }   
}
