using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;
using Unity.VisualScripting;

public class CroissanceBoul : MonoBehaviour
{
    public Tree t;
    public TreeData tData;
    public Material[] m;
    public WindZone windZone;


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

    private float height; // min of the height, = xTrunk but not updated in TrunkLength()
    private float maxHeight; // Max of the height 
    private float maxRadius;
    private float maxLeafSize;

    private TreeGroupBranch branches;
    private TreeGroupBranch tronc;
    private TreeGroupLeaf feuilles;

    private bool growing = false;
    private bool tGrown, bGrown, lGrown = false;

    // Start is called before the first frame update
    void Start()
    {
        // On initialise les valeurs des variables
        croissTime = Time.time + growthDelay;
        tData = t.data as TreeData;
        var tronc = tData.branchGroups[0];
        var branche = tData.branchGroups[1];
        var feuille = tData.leafGroups[0];
        
        //Partie Tronc
        xTrunk = 1.0f;
        yTrunk = 2.0f;
        height = xTrunk;
        //maxHeight = 22.2f;
        radTrunk = Random.Range(0.5f, 1.0f);
        tronc = tData.branchGroups[0];
        tronc.height = new Vector2(xTrunk, yTrunk);
        tronc.radius = radTrunk;
        tronc.seed = 1234;
        tronc.crinklyness = 0.297f;
        //maxRadius = maxHeight / 20.0f;

        //Partie Branche
        xBranch = 0.2f;
        yBranch = 0.8f;
        radBranch = Random.Range(0.0f, 0.2f);
        
        branche.height = new Vector2(xBranch, yBranch); // from 0 - 10
        branche.radius = radBranch;
        branche.distributionFrequency = 60;
        branche.seed = 1234;
        branche.crinklyness = 0.566f;
        branche.distributionMode = TreeGroup.DistributionMode.Whorled;
        branche.distributionPitch = 0.3f; // Inclinaison des branches
        branche.seekBlend = 0.1f; // cherche le soleil à 10%

        //Partie Feuilles
        //maxLeafSize = 10.3f;
        xLeaf = 0.0f;
        yLeaf = 0.1f;
        feuille.size = new Vector2(xLeaf, yLeaf);
        feuille.seed = 1234;
        feuille.distributionFrequency = 19;
        feuille.distributionMode = TreeGroup.DistributionMode.Whorled;


        //Mise à jour des donnees de l'arbre
        tData.UpdateFrequency(1);
        tData.UpdateSeed(Random.Range(0, 999999));
        tData.UpdateMesh(transform.worldToLocalMatrix, out m);

        //Debug.Log("Current Seed: " + tData.root.seed);
        

    }
    void Update()
    {
        
        
        if (Time.time >= croissTime && growing == false)
        {
            float gs = growthSpeed;
            growing = true;
            tGrown = UpdateTrunk(gs);
            if (Time.time >= 3.00f)
            {
                bGrown = UpdateBranch(gs);
                if (Time.time >= 5.00f)
                 {
                    lGrown = UpdateLeaf(gs);
                }
            }
            croissTime = Time.time + growthDelay;

            tData.UpdateFrequency(1);
            tData.UpdateMesh(transform.worldToLocalMatrix, out m);
            growing = (tGrown && bGrown && lGrown); // Fonctionne car Update est en séquentiel. True quand tout est grown

        }

    }
    
    void TrunkLength(float growthSpeed)
    {
        xTrunk += growthSpeed;
        yTrunk += growthSpeed;
        xTrunk = Mathf.Clamp(xTrunk, 19.5f, 20.2f);
        yTrunk = Mathf.Clamp(yTrunk, 21.8f, 22.2f);

        tData.branchGroups[0].height = new Vector2(xTrunk, yTrunk);
    }
    void TrunkRadius(float growthSpeed)
    {
        radTrunk += growthSpeed;
        radTrunk = Mathf.Clamp(radTrunk, 1.5f, 1.7f);

        tData.branchGroups[0].radius = radTrunk;
    }
    void BranchLength(float growthSpeed)
    {
        if (xBranch <= 5.0f && yBranch <= 5.2)
        {
            xBranch += growthSpeed;
            yBranch += growthSpeed;

            xBranch = Mathf.Clamp(xBranch, 14.5f, 14.9f);
            yBranch = Mathf.Clamp(yBranch, 19.1f, 19.5f);
            tData.branchGroups[1].height = new Vector2(xBranch, yBranch);
        }
    }
    void BranchRadius(float growthSpeed)
    {
        radBranch += growthSpeed;
        radBranch = Mathf.Clamp(radBranch, 1.6f, 1.9f);

        tData.branchGroups[1].radius = radBranch;
    }

    void LeafSize(float growthSpeed)
    {
        xLeaf += growthSpeed;
        yLeaf += growthSpeed;

        xLeaf = Mathf.Clamp(xLeaf, 0.0f, 0.1f);
        yLeaf = Mathf.Clamp(yLeaf, 0.1f, 0.2f);

        tData.leafGroups[0].size = new Vector2(xLeaf, yLeaf);
    }
    bool UpdateTrunk(float growthSpeed)
    {
        var lengthValue = xTrunk;
        var radiusValue = radTrunk;
        TrunkLength(growthSpeed);
        TrunkRadius(growthSpeed);
        return (lengthValue == xTrunk && radiusValue == radTrunk); // true if trunk has grown
    }
    bool UpdateBranch(float growthSpeed)
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
        BranchLength(growthSpeed);
        BranchRadius(growthSpeed);
        return (lengthValue == xBranch && radiusValue == radBranch && numberValue == branches.distributionFrequency);
    }
    bool UpdateLeaf(float growthSpeed)
    {
        var sizeValue = xLeaf;
        var numberValue = feuilles.distributionFrequency;
        feuilles.distributionFrequency += 2; // On ajoute 2 feuilles          
        feuilles.distributionFrequency = ((int)Mathf.Clamp(feuilles.distributionFrequency, 1.0f, 40.0f)); // MAXIMUM 100 FEUILLES PAR BRANCHE
        LeafSize(growthSpeed);
        return (sizeValue == xLeaf && numberValue == feuilles.distributionFrequency);
    }
}
