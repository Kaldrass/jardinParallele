using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;
using Unity.VisualScripting;

// Script that describes the growth of a tree.
public class CroissanceBuisson : MonoBehaviour
{
    public Tree t;
    public TreeData tData;
    public Material[] m;
    public WindZone windZone;
    public GameObject water;

    public float closeDistance = 15.0f;

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

    private TreeGroupBranch branche;
    private TreeGroupBranch tronc;
    private TreeGroupLeaf feuille;

    private bool growing = false;
    private bool tGrown, bGrown, lGrown = false;
    // Start is called before the first frame update
    void Start()
    {
        // On initialise les valeurs des variables
        croissTime = Time.time + growthDelay;
        tData = t.data as TreeData;

        tronc = tData.branchGroups[0];
        branche = tData.branchGroups[1];
        feuille = tData.leafGroups[0];

        //Partie Tronc
        xTrunk = 1.0f;
        yTrunk = 2.0f;
        height = xTrunk;
        maxHeight = 4.3f;
        radTrunk = Random.Range(0.5f, 1.0f);
        tronc = tData.branchGroups[0];
        tronc.height = new Vector2(xTrunk, yTrunk);
        tronc.radius = radTrunk;
        tronc.seed = 1234;
        maxRadius = maxHeight / 20.0f;

        //Partie Branche
        xBranch = 0.0f;
        yBranch = 0.2f;
        radBranch = Random.Range(0.0f, 0.2f);
        branche.height = new Vector2(xBranch, yBranch); // from 0 - 10
        branche.radius = radBranch;
        branche.distributionFrequency = 30;
        branche.distributionMode = TreeGroup.DistributionMode.Random;
        branche.seed = 181957;
        branche.distributionPitch = 0.3f; // Inclinaison des branches
        branche.seekBlend = 0.1f; // cherche le soleil à 10%


        //Partie Feuille
        xLeaf = 0.0f;
        yLeaf = 0.1f;
        feuille.seed = 1234;
        feuille.distributionFrequency = 1;
        feuille.distributionMode = TreeGroup.DistributionMode.Opposite;
        feuille.size = new Vector2(xLeaf, yLeaf);



        //Mise à jour des donnees de l'arbre
        tData.UpdateFrequency(1);
        tData.UpdateSeed(Random.Range(0, 999999));
        tData.UpdateMesh(transform.worldToLocalMatrix, out m);

        //Debug.Log("Current Seed: " + tData.root.seed);

        //Calcul Proximite à l'eau
        

    }
    void Update()
    {
        //public float distance = Vector3.Distance(t.transform.position, water.transform.position);
        
        

        if (Time.time >= croissTime && growing == false)
        {
                float gs = ProxiWater() ? 2f * growthSpeed : growthSpeed; 
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
        xTrunk = Mathf.Clamp(xTrunk, 4.8f, 5.8f);
        yTrunk = Mathf.Clamp(yTrunk, 6.0f, 6.3f);

        tronc.height = new Vector2(xTrunk, yTrunk);
    }
    void TrunkRadius(float growthSpeed)
    {
        radTrunk += growthSpeed;
        radTrunk = Mathf.Clamp(radTrunk, 0.3f, 0.6f);

        tronc.radius = radTrunk;
    }
    void BranchLength(float growthSpeed)
    {
        
        xBranch += growthSpeed;
        yBranch += growthSpeed;

        xBranch = Mathf.Clamp(xBranch, 4.8f, 5.0f);
        yBranch = Mathf.Clamp(yBranch, 5.5f, 6.0f);
        branche.height = new Vector2(xBranch, yBranch);
    }
    void BranchRadius(float growthSpeed)
    {
        radBranch += growthSpeed;
        radBranch = Mathf.Clamp(radBranch, 0.0f, 0.1f);

        branche.radius = radBranch;
    }

    void LeafSize(float growthSpeed)
    {
        xLeaf += growthSpeed;
        yLeaf += growthSpeed;

        xLeaf = Mathf.Clamp(xLeaf, 0.3f, 0.75f);
        yLeaf = Mathf.Clamp(yLeaf, 0.5f, 1.0f);

        feuille.size = new Vector2(xLeaf, yLeaf);
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
        var numberValue = branche.distributionFrequency;
        if (xTrunk - height > 0.5f)
        {
            height = xTrunk;
            branche.distributionFrequency++; // On ajoute une branche
            branche.distributionFrequency = (int)(Mathf.Clamp(branche.distributionFrequency, 40.0f, 55.0f)); // 30 branches maximum, question de ressources
        }
        BranchLength(growthSpeed);
        BranchRadius(growthSpeed);
        return (lengthValue == xBranch && radiusValue == radBranch && numberValue == branche.distributionFrequency);
    }
    bool UpdateLeaf(float growthSpeed)
    {
        var sizeValue = xLeaf;
        var numberValue = feuille.distributionFrequency;
        feuille.distributionFrequency += 2; // On ajoute 2 feuilles          
        feuille.distributionFrequency = (int)(Mathf.Clamp(feuille.distributionFrequency, 10.0f, 15.0f)); // MAXIMUM 100 FEUILLES PAR BRANCHE
        LeafSize(growthSpeed);
        return (sizeValue == xLeaf && numberValue == feuille.distributionFrequency);
    }
    bool ProxiWater()
    {
        
        Mesh mesh = tData.mesh;
       // meshw = GameObject.Mesh(water).GetComponent<Mesh>();
        
        foreach (Vector3 point in mesh.vertices)
        {
            
           if (water.transform.InverseTransformPoint(transform.TransformPoint(point)).magnitude < closeDistance)
           {
                Debug.Log("Proxi");
                return true;
           }
        }
        Debug.Log("Pas proxi");
        return false;
    }

}
