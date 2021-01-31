using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Terrain terrain;
    public Transform water;

    public GameObject fish;
    public int fishCount = 100;
    public GameObject trap;
    public int trapCount = 100;
    public GameObject[] foods;
    public int foodsCount = 1000;
    public GameObject hunter;
    public int hunterCount = 100;


    private float terrainWidth;
    private float terrainLength;

    private float xTerrainPos;
    private float zTerrainPos;


    // Start is called before the first frame update
    void Start()
    {
        //Get terrain size
        terrainWidth = terrain.terrainData.size.x;
        terrainLength = terrain.terrainData.size.z;

        //Get terrain position
        xTerrainPos = terrain.transform.position.x;
        zTerrainPos = terrain.transform.position.z;

        for (int i = 0; i < fishCount; i++)
        {
            generateObjectNearWater(fish);
        }

        for (int i = 0; i < fishCount; i++)
        {
            generateObjectOnTerrain(trap);
        }

        for (int i = 0; i < foods.Length; i++)
        {
            for (int j = 0; j < foodsCount; j++)
            {
                generateObjectOnTerrain(foods[i]);
            }
        }

        for (int i = 0; i < hunterCount; i++)
        {
            generateObjectOnTerrain(hunter);
        }
    }

    void generateObjectOnTerrain(GameObject prefab)
    {
        Vector3 spawn;

        do
        {
            //Generate random x,z,y position on the terrain
            float randX = Random.Range(xTerrainPos, xTerrainPos + terrainWidth);
            float randZ = Random.Range(zTerrainPos, zTerrainPos + terrainLength);
            float yVal = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0, randZ));

            //Apply Offset if needed
            yVal = yVal + prefab.transform.localScale.y / 2;

            spawn = new Vector3(randX, yVal, randZ);
        } while (spawn.y - prefab.transform.localScale.y / 2 < water.transform.position.y);

        //Generate the Prefab on the generated position
        GameObject objInstance = Instantiate(prefab, spawn, Quaternion.identity);
        objInstance.transform.parent = this.transform;
    }

    void generateObjectNearWater(GameObject prefab)
    {
        Vector3 spawn;

        do
        {
            //Generate random x,z,y position on the terrain
            float randX = Random.Range(xTerrainPos, xTerrainPos + terrainWidth);
            float randZ = Random.Range(zTerrainPos, zTerrainPos + terrainLength);
            float yVal = Terrain.activeTerrain.SampleHeight(new Vector3(randX, 0, randZ));

            //Apply Offset if needed
            yVal = yVal + prefab.transform.localScale.y / 2;

            spawn = new Vector3(randX, yVal, randZ);
        } while (Mathf.Abs(spawn.y - water.transform.position.y) > 3);

        //Generate the Prefab on the generated position
        GameObject objInstance = Instantiate(prefab, spawn, Quaternion.identity);
        objInstance.transform.parent = this.transform;
        objInstance.name = objInstance.name + spawn;
    }
}
