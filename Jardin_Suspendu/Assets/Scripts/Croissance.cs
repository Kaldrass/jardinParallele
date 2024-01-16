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
    public GameObject treeObject;
    public GameObject treesInJardinObject;

    public float CroissDelay = 1f;
    public float DebutBranche = 5f;
    public float DebutFeuille = 10f;

    public float xTronc;
    public float yTronc;
    public float radTronc;

    public float xBranche;
    public float yBranche;
    public float radBranche;

    public float xFeuille;
    public float yFeuille;

    public int treeNb;
    List<GameObject> treesList = new List<GameObject>();
    GameObject[] treesArray;
    public float CroissTime;
    
    // Start is called before the first frame update
    void Start()
    {
        this.CroissTime = Time.time + this.CroissDelay;
       

        for (int i = 0; i <= treeNb; i++)
        {
            treesList.Add(Instantiate<GameObject>(treeObject));
            treesArray = treesList.ToArray();
            treesArray[i].transform.position = new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
            treesArray[i].transform.parent = treesInJardinObject.transform;
            t = tree.GetComponent<Tree>();
            tData = t.data as TreeEditor.TreeData;

            tData.root.seed = Random.Range(0, 999999);
            tData.root.rootSpread = Random.Range(1.0f, 5.0f);
            tData.branchGroups[0].radius = Random.Range(0.5f, 1.0f);
            tData.branchGroups[0].seed = Random.Range(0, 999999);
            //tData.branchGroups[0].flareHeight = Random.Range(0.5f, 1.0f);
            tData.branchGroups[0].distributionFrequency = Random.Range(1, 2);
            tData.branchGroups[0].height = new Vector2(5.0f, 10.0f);
            tData.branchGroups[1].height = new Vector2(10.0f, 20.0f);
            tData.UpdateMesh(tree.transform.worldToLocalMatrix, out m);
            Debug.Log("Current Seed: " + tData.root.seed);
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (Time.time >= this.CroissTime)
        {
            LongueurTronc();
            RadiusTronc();
           // if ()
            //{
                LongueurBranche();
                RadiusBranche();

              //  if()
               // {
                    SizeFeuille();
                //}
          //  }
        }
    }

    public void LongueurTronc()
    {

    }
    public void RadiusTronc()
    {

    }
    public void LongueurBranche()
    {

    }
    public void RadiusBranche()
    {

    }
    public void SizeFeuille()
    {

    }

}