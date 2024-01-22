using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeEditor;
using System.Data;
using Unity.VisualScripting;
using UnityEngine.Assertions.Must;

// Script that describes the growth of a tree.
public class growthScript : MonoBehaviour
{
    public Tree t;
    public TreeData tData;
    public Material[] m;
    public WindZone windZone;

    [Range(0.01f,10f)]
    public float growthDelay;
    public float growthSpeed;

    [Range(0.1f, 1.0f)]
    public float radTrunk;

    [Range(0.0f, 0.5f)]
    public float xLeaf;
    [Range(0.0f, 0.5f)]
    public float yLeaf;
    public int maxNbFeuilles = 40;
    [Range(1.0f, 2.0f)]
    public float windMainForce;

    private float croissTime;

    private float height; // min of the height, = xTrunk but not updated in TrunkLength()
    private float maxHeight; // Max of the height 
    private float maxRadius;
    private float maxLeafSize;
    private int maxNbBranches = 30;
    private int nbFeuilles = 2;

    private TreeGroupBranch branches;
    private TreeGroupBranch tronc;
    private TreeGroupLeaf feuilles;
    private bool growing = false;
    private bool tGrown, bGrown, lGrown = false;

    float xTrunk;
    float yTrunk;

    float xBranch;
    float yBranch;
    float radBranch;
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
        maxRadius = maxHeight / 20.0f;
        // PARTIE BRANCHES
        
        xBranch = 0.0f;
        yBranch = 0.2f;
        radBranch = Random.Range(0.0f, 0.2f);
        branches = tData.branchGroups[1];
        branches.height = new Vector2(xBranch, yBranch); // from 0 - 10
        branches.radius = radBranch;
        branches.distributionMode = TreeGroup.DistributionMode.Opposite; // Distribution en oppos�
        branches.distributionTwirl = 0.4f; // Rotation des branches
        branches.distributionFrequency = 1; // Nbr de branches
        branches.distributionPitch = 0.3f; // Inclinaison des branches
        branches.seekBlend = 0.1f; // cherche le soleil � 10%

        // PARTIE FEUILLES
        maxLeafSize = 1.0f;
        xLeaf = 0.0f;
        yLeaf = 0.1f;
        feuilles = tData.leafGroups[0];
        feuilles.size = new Vector2(xLeaf, yLeaf); //mini feuilles au d�but
        feuilles.distributionFrequency = 1; // Une unique feuille par branches, jusqu'� 50 (100 est bien mais plus co�teux)

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
            windZone.windMain = windMainForce; // windMainForce entre 1.0f et 2.0f
            float gs = PetitSoufflet() ? (0.5f * 1.0f / windZone.windMain) * growthSpeed : growthSpeed; 
            // Si le vent souffle, on la croissance est moiti� plus lente
            // c'est la thigmomorphogen�se pour faire plus savant
            reduceMax(gs);
            growing = true;
            tGrown = UpdateTrunk(gs);
            if (Time.time >= 3.00f)
            {
                bGrown = UpdateBranch(gs);
                if(Time.time >= 5.00f)
                {
                    lGrown = UpdateLeaf(gs);
                }
            }
            croissTime = Time.time + growthDelay;
            tData.UpdateFrequency(1); // UpdateFrequency(id) mais id = 1 fonctionne tout le temps
            tData.UpdateMesh(transform.worldToLocalMatrix, out m);
            growing = (tGrown && bGrown && lGrown); // Fonctionne car Update est en s�quentiel. True quand tout est grown
        }
    }
    void TrunkLength(float growthSpeed)
    {
        xTrunk += growthSpeed;
        yTrunk += growthSpeed;
        xTrunk = Mathf.Clamp(xTrunk, 0.0f, maxHeight);
        yTrunk = Mathf.Clamp(yTrunk, 1.0f, maxHeight * 1.2f); // Not just +1, we want to let it grow more 
        //Debug.Log("Trunk Size : " + xTrunk + " " + yTrunk);
        tData.branchGroups[0].height = new Vector2(xTrunk, yTrunk);
    }   
    void TrunkRadius(float growthSpeed)
    {
        radTrunk += (2.0f - growthSpeed) / 20.0f;
        radTrunk = Mathf.Clamp(radTrunk, 0.0f, maxRadius);

        tData.branchGroups[0].radius = radTrunk;
    }
    void BranchLength(float growthSpeed)
    {
        xBranch += growthSpeed;
        yBranch += growthSpeed;
        xBranch = Mathf.Clamp(xBranch, 0.0f, xTrunk * 0.25f);
        yBranch = Mathf.Clamp(yBranch, 0.0f, maxHeight * 0.5f);
        //Debug.Log("Branch size : " + xBranch + " " + yBranch);
        branches.height = new Vector2(xBranch, yBranch);
    }
    void BranchRadius(float growthSpeed)
    {
        radBranch += growthSpeed;
        radBranch = Mathf.Clamp(radBranch, 0.0f, 1.0f);

        branches.radius = radBranch;
    }

    void LeafSize(float growthSpeed)
    {
        xLeaf += growthSpeed * 0.1f;
        yLeaf += growthSpeed * 0.1f;

        xLeaf = Mathf.Clamp(xLeaf, 0.0f, maxLeafSize - 0.2f);
        yLeaf = Mathf.Clamp(yLeaf, 0.0f, maxLeafSize + 0.2f);

        tData.leafGroups[0].size = new Vector2(xLeaf, yLeaf);
    }
    bool UpdateTrunk(float growthSpeed)
    {
        var lengthValue = xTrunk;
        var radiusValue = radTrunk;
        TrunkLength(growthSpeed);
        TrunkRadius(growthSpeed);
        return (lengthValue==xTrunk && radiusValue == radTrunk); // true if trunk has grown
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
            branches.distributionFrequency = (int)(Mathf.Clamp(branches.distributionFrequency, 1.0f, (float)maxNbBranches)); // 30 branches maximum, question de ressources
        }
        BranchLength(growthSpeed);
        BranchRadius(growthSpeed);
        return (lengthValue == xBranch && radiusValue == radBranch && numberValue == branches.distributionFrequency);
    }
    bool UpdateLeaf(float growthSpeed)
    {
        var sizeValue = xLeaf;
        var numberValue = feuilles.distributionFrequency;
        feuilles.distributionFrequency += nbFeuilles; // On ajoute 2 feuilles          
        feuilles.distributionFrequency = ((int)Mathf.Clamp(feuilles.distributionFrequency, 1.0f, (float)maxNbFeuilles)); // GRAND MAXIMUM 100 FEUILLES PAR BRANCHE
        LeafSize(growthSpeed);
        return (sizeValue == xLeaf && numberValue == feuilles.distributionFrequency);
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
        if(gs != growthSpeed)
        {
            // windZone.windMain est un float donnant la force globale du vent. A utiliser pour faire varier les valeurs.
            // Max 2 pour le r�alisme, 1 est la valeur par d�faut -> valeur minimale

            // HAUTEUR DU TRONC
            maxHeight -= (growthSpeed - gs);
            maxHeight = Mathf.Clamp(maxHeight, xTrunk, maxHeight); // min(max) = xTrunk
            // LARGEUR DU TRONC
            maxRadius += (growthSpeed - gs) / 20.0f; // Le tronc grossit quand il y a du vent pour mieux r�sister
            maxRadius = Mathf.Clamp(maxRadius, 0.0f, 2.0f);
            // NB DE BRANCHES
            maxNbBranches -= 1;
            maxNbBranches = Mathf.Clamp(maxNbBranches, branches.distributionFrequency, maxNbBranches); // min(max) = branches.distributionFrequency
            // NB DE FEUILLES
            maxNbFeuilles -= 1;
            maxNbFeuilles = Mathf.Clamp(maxNbFeuilles, feuilles.distributionFrequency, maxNbFeuilles); // min(max) = feuilles.distributionFrequency
        }
    }
}
