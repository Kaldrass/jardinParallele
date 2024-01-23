using System.Collections;
using System.Collections.Generic;
using TreeEditor;
//using Unity.VisualScripting;
//using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TreeGeneration : MonoBehaviour
{
    public int seed;
    public int width;
    public int height;

    [Range(0.01f, 10f)]
    public float scale;

    public GameObject treePrefab;
    public Tree t;
    public Material[] m;
    
    [Range(0.01f, 1.0f)]
    public float acceptancePoint;
    void Start()
    {
        for(int y = 0; y < height; y++) { 
            for(int x = 0; x < width; x++)
            {
                float xCoord = x / scale + seed;
                float yCoord = y / scale + seed;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                if (sample >= acceptancePoint)
                {
                    //Debug.Log("Tree : " + x + " " + y);
                    //t = treePrefab.GetComponent<Tree>();
                    //TreeData tData = t.data as TreeData;
                    //TreeGroupRoot root = tData.root;
                    //root.seed = Random.Range(0, 9999999);
                    //Random.InitState(root.seed);
                    //tData.Initialize();
                    //tData.UpdateSeed(Random.Range(0, 9999999));
                    //tData.UpdateMesh(t.transform.worldToLocalMatrix, out m

                    //var tData = tree.data as TreeData;
                    ////tData.Initialize();
                    //var root = tData.root;
                    //root.seed = Random.Range(0, 999999);
                    ////I honestly don't know what the parameters are for but it works.
                    //////seed = Random.Range(0, 999999);
                    //tData.UpdateMesh(treePrefab.transform.worldToLocalMatrix, out m);
                    ////tData.UpdateSeed(seed);
                    ////t.transform.position= new Vector3(x, 0, y);

                    //Debug.Log("Current Seed: " + seed);

                    ////////GameObject tree = Instantiate(treePrefab, new Vector3(x, 0, y), Quaternion.identity);
                    ////////t.transform.SetParent(this.transform);
                    //Random.InitState(System.Environment.TickCount);

                    //GameObject tree = new GameObject();
                    //tree.gameObject.AddComponent<Tree>();
                    //tree.gameObject.AddComponent<MeshFilter>();
                    //tree.gameObject.AddComponent<MeshRenderer>();
                    //tree.gameObject.AddComponent<growthScript>();

                    //tree.GetComponent<MeshFilter>().mesh = treePrefab.GetComponent<MeshFilter>().sharedMesh;
                    //tree.GetComponent<MeshRenderer>().material = treePrefab.GetComponent<MeshRenderer>().sharedMaterial;

                    //tree.transform.SetParent(this.transform);
                    //tree.transform.position = new Vector3(x, 0, y);

                    //tree.GetComponent<Tree>().data = ScriptableObject.CreateInstance<TreeData>();
                    //TreeData tData = tree.GetComponent<Tree>().data as TreeData;
                    //tData.Initialize();
                    //tData.root.seed = Random.Range(0, 999999);
                    //tData.root.rootSpread = Random.Range(1.0f, 5.0f);
                    //tData.UpdateSeed();
                    // We create a new tree to which we link the growth script


                    //tree.GetComponent<growthScript>().seed = Random.Range(0, 999999);
                    //tree.GetComponent<growthScript>().width = Random.Range(1, 5);
                    //tree.GetComponent<growthScript>().height = Random.Range(1, 5);
                    //tree.GetComponent<growthScript>().scale = Random.Range(0.5f, 1.0f);
                    //tree.GetComponent<growthScript>().acceptancePoint = Random.Range(0.5f, 1.0f);
                }
            }
        }    
    }

}
