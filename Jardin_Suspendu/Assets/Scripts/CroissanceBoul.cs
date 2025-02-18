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
    public GameObject water;
    

    public float distWater = 2.0f;
    

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

    public float maxNbFeuilles = 40.0f;
    [Range(1.0f, 2.0f)]
    public float windMainForce;
    private int nbFeuilles = 2;

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
        var tronc = tData.branchGroups[0];
        var branche = tData.branchGroups[1];
        var feuille = tData.leafGroups[0];
        
        //Partie Tronc
        xTrunk = 1.0f;
        yTrunk = 2.0f;
        height = xTrunk;
        maxHeight = 22.2f;
        radTrunk = Random.Range(0.5f, 1.0f);
        tronc = tData.branchGroups[0];
        tronc.height = new Vector2(xTrunk, yTrunk);
        tronc.radius = radTrunk;
        tronc.seed = 1234;
        tronc.crinklyness = 0.297f;
        maxRadius = maxHeight / 20.0f;

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
        branche.seekBlend = 0.1f; // cherche le soleil � 10%

        //Partie Feuilles
        maxLeafSize = 0.3f;
        xLeaf = 0.0f;
        yLeaf = 0.1f;
        feuille.size = new Vector2(xLeaf, yLeaf);
        feuille.seed = 1234;
        feuille.distributionFrequency = 1;
        feuille.distributionMode = TreeGroup.DistributionMode.Whorled;


        //Mise � jour des donnees de l'arbre
        tData.UpdateFrequency(1);
        tData.UpdateSeed(Random.Range(0, 999999));
        tData.UpdateMesh(transform.worldToLocalMatrix, out m);

        //Debug.Log("Current Seed: " + tData.root.seed);
        

    }
    void Update()
    {
        
        
        if (Time.time >= croissTime && growing == false)
        {
            float alti = Altitude() ? 0.6f : 1.0f;
            float proxi = ProxiWater() ? 1.5f : 1.0f;
            float gs = PetitSoufflet() ? (0.5f * 1.0f / windZone.windMain) * growthSpeed : growthSpeed;
            windZone.windMain = windMainForce;
            reduceProxi(proxi);
            reduceAlti(alti);
            reduceMax(gs);
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
            growing = (tGrown && bGrown && lGrown); // Fonctionne car Update est en s�quentiel. True quand tout est grown

        }

    }
    
    void TrunkLength(float growthSpeed)
    {
        xTrunk += growthSpeed;
        yTrunk += growthSpeed;
        xTrunk = Mathf.Clamp(xTrunk, maxHeight-2.7f, maxHeight-2.0f);
        yTrunk = Mathf.Clamp(yTrunk, maxHeight-0.4f, maxHeight);

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
        xBranch += growthSpeed;
        yBranch += growthSpeed;

        xBranch = Mathf.Clamp(xBranch, maxHeight-7.7f, maxHeight-7.3f);
        yBranch = Mathf.Clamp(yBranch, maxHeight-3.1f, maxHeight-2.7f);
        branche.height = new Vector2(xBranch, yBranch);
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

        xLeaf = Mathf.Clamp(xLeaf, maxLeafSize-0.3f, maxLeafSize - 0.2f);
        yLeaf = Mathf.Clamp(yLeaf,maxLeafSize-0.2f, maxLeafSize);

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
        var numberValue = branche.distributionFrequency;
        if (xTrunk - height > 0.5f)
        {
            height = xTrunk;
            branche.distributionFrequency++; // On ajoute une branche
            branche.distributionFrequency = (int)(Mathf.Clamp(branche.distributionFrequency, 1.0f, 30.0f)); // 30 branches maximum, question de ressources
        }
        BranchLength(growthSpeed);
        BranchRadius(growthSpeed);
        return (lengthValue == xBranch && radiusValue == radBranch && numberValue == branche.distributionFrequency);
    }
    bool UpdateLeaf(float growthSpeed)
    {
        var sizeValue = xLeaf;
        var numberValue = feuille.distributionFrequency;
        feuille.distributionFrequency += nbFeuilles; // On ajoute 2 feuilles          
        feuille.distributionFrequency = ((int)Mathf.Clamp(feuille.distributionFrequency, maxNbFeuilles-5.0f, maxNbFeuilles)); // MAXIMUM 100 FEUILLES PAR BRANCHE
        LeafSize(growthSpeed);
        return (sizeValue == xLeaf && numberValue == feuille.distributionFrequency);
    }
    bool ProxiWater()
    {

        Mesh mesh = tData.mesh;
        // meshw = GameObject.Mesh(water).GetComponent<Mesh>();

        foreach (Vector3 point in mesh.vertices)
        {

            if (water.transform.InverseTransformPoint(transform.TransformPoint(point)).magnitude < distWater)
            {
                Debug.Log("Proxi");
                return true;
            }
        }
        Debug.Log("Pas proxi");
        return false;
    }
    bool Altitude()
    {
        Mesh mesh = tData.mesh;

        foreach (var point in mesh.vertices)
        {
            if (t.transform.position.y >= 24.0f)
            {
                Debug.Log("Altitude");
                return true;
            }
        }
        Debug.Log("Pas Altitude");
        return false;
    }
    void reduceAlti(float alti)
    {
        if (alti != 1.0f)
        {
            maxHeight = maxHeight * alti;
            maxHeight = Mathf.Clamp(maxHeight, 8.0f, maxHeight);

            maxNbFeuilles -= 5.0f;
            maxNbFeuilles = Mathf.Clamp(maxNbFeuilles, feuille.distributionFrequency, maxNbFeuilles);
        }
    }

    void reduceProxi(float proxi)
    {
        if (proxi != 1.0f)
        {
            maxHeight = maxHeight * proxi;
            maxHeight = Mathf.Clamp(maxHeight, 8.0f, maxHeight);

            maxNbFeuilles += 5.0f;
            maxNbFeuilles = Mathf.Clamp(maxNbFeuilles, feuille.distributionFrequency, maxNbFeuilles);
        }
    }

    bool PetitSoufflet()
    {
        // Fonction qui retourne si l'arbre se prend une petite brise
        Mesh mesh = tData.mesh;
        // check for all points of the mesh if it's inside the spherical windzone
        foreach (Vector3 point in mesh.vertices)
        {
            // Pas opti, on check tous les points en non juste ceux en p�riph�rie mais fait le caf�
            // Cependant, le caf� de cette machine est plut�t bon
            if (windZone.transform.InverseTransformPoint(transform.TransformPoint(point)).magnitude < windZone.radius)
            {
                Debug.Log("Petit Soufflet");
                return true;
            }
        }
        Debug.Log("Calme Plat");
        return false;
    }
    void reduceMax(float gs)
    {
        // Fonction qui va r�duire les max de l'arbre en fonction du vent (pour l'instant)
        nbFeuilles = gs != growthSpeed ? 1 : 2; // Si le vent souffle, on r�duit le nombre de feuilles
        if (gs != growthSpeed)
        {
            // windZone.windMain est un float donnant la force globale du vent. A utiliser pour faire varier les valeurs.
            // Max 2 pour le r�alisme, 1 est la valeur par d�faut -> valeur minimale

            // HAUTEUR DU TRONC
            maxHeight -= (growthSpeed - gs);
            maxHeight = Mathf.Clamp(maxHeight, xTrunk, maxHeight); // min(max) = xTrunk
            // LARGEUR DU TRONC
            maxRadius += (growthSpeed - gs) / 20.0f; // Le tronc grossit quand il y a du vent pour mieux r�sister
            maxRadius = Mathf.Clamp(maxRadius, 0.0f, 2.0f);

            // NB DE FEUILLES
            maxNbFeuilles -= 1;
            maxNbFeuilles = Mathf.Clamp(maxNbFeuilles, feuille.distributionFrequency, maxNbFeuilles); // min(max) = feuilles.distributionFrequency
        }
    }
}
