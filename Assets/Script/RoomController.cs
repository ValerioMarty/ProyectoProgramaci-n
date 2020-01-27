using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RoomController : MonoBehaviour
{
    [Header("Room Configuration")]
    public GameObject[] doors;
    public GameObject[] directions;
    public Transform[] regions;

    [Header("Debug")]
    public Vector2Int position;
    public bool isBossRoom;
    public bool isSecret;
    public string whatIsSecret;
    public bool generate;

    [Header("Asset Generation")]
    [Range(0, 100)]
    public float chanceOfGeneratingAsset;
    [Space(5)]
    [Range(0, 100)]
    public float chestChance;
    [Range(0, 100)]
    public float rockChance;
    [Space(5)]
    public GameObject chestPrefab;
    public GameObject rockPrefab;
    public GameObject decorationPrefab;
    [Space(5)]
    public Transform assetParent;
    [Space(10)]
    public string usedCellTag = "Used";
    //la decoracion es el resto

    [Header("Hazard Generation")]
    [Range(0, 100)]
    public float emptyRegionChance;
    [Space(5)]
    [Range(0, 100)]
    public float partiallyEmptyRegionChance;
    [Range(0, 100)]
    public int percentageOfEmptyCells = 50;
    //el resto es que no pase nada

    [Header("Enemy Generation")]
    [Range(0, 100)]
    public float chanceEnemyInCell;
    [Range(0, 100)]
    public int chanceOfRanger;
    [Space(5)]
    public GameObject meleePrefab;
    public GameObject rangerPrefab;
    public GameObject bossPrefab;

    private Transform enemyParent;

    private void Awake()
    {
        foreach (var item in doors)
        {
            item.SetActive(true);
        }


    }
    //al instanciarse la habitacion, inmediatamente genera los assets y los peligros
    private void Start()
    {
        enemyParent = GameObject.FindGameObjectWithTag("Enemies").transform;
        if (generate)
        {
            //generar assets en el centro
            GenerateAssets(regions[0]);
            GenerateEnemies(regions[0]);
            //genera peligros y assets en el resto de regiones
            for (int i = 1; i < regions.Length; i++)
            {
                //si la puerta esta abierta no genera peligros
                if (doors[i - 1].activeInHierarchy)
                {
                    GenerateHazards(regions[i]);

                }
                //solo lo genera si la region entera no esta desactivada
                if (regions[i].gameObject.activeInHierarchy)
                {
                    GenerateAssets(regions[i]);
                    GenerateEnemies(regions[i]);
                }
            }

        }
    }

    public void GenerateHazards(Transform parent)
    {
        float whatEvent = Random.Range(0, 100f);
        //completamente vacio
        if (whatEvent < emptyRegionChance)
        {
            parent.gameObject.SetActive(false);
        }
        //medio vacio
        else if (whatEvent < emptyRegionChance + partiallyEmptyRegionChance)
        {
            foreach (Transform item in parent)
            {
                if (Random.Range(0, 101) < percentageOfEmptyCells)
                {
                    item.gameObject.SetActive(false);
                }
            }
        }
    }

    //generacion de assets encima de cada celula de suelo que no haya sido desactivada
    public void GenerateAssets(Transform parent)
    {
        foreach (Transform item in parent)
        {
            if (item.gameObject.activeInHierarchy)
            {
                if (Random.Range(0, 100f) < chanceOfGeneratingAsset)
                {
                    float whatAsset = Random.Range(0, 100f);
                    GameObject asset;
                    if (whatAsset < chestChance)
                    {
                        asset = Instantiate(chestPrefab);
                    }
                    else if (whatAsset < rockChance + chestChance)
                    {
                        asset = Instantiate(rockPrefab);
                    }
                    else
                    {
                        asset = Instantiate(decorationPrefab);
                    }
                    asset.transform.SetParent(assetParent);
                    asset.transform.position = item.transform.position + Vector3.up;
                    item.tag = usedCellTag;
                }
            }
        }
    }

    public void GenerateEnemies(Transform parent)
    {
        foreach (Transform item in parent)
        {
            if (item.gameObject.activeInHierarchy)
            {
                if (!item.CompareTag(usedCellTag))
                {
                    if (Random.Range(0, 100f) < chanceEnemyInCell)
                    {
                        GameObject asset;
                        if (Random.Range(0, 101) < chanceOfRanger)
                        {
                            asset = Instantiate(rangerPrefab);
                        }
                        else
                        {
                            asset = Instantiate(meleePrefab);
                        }

                        asset.transform.SetParent(enemyParent);
                        asset.transform.position = item.transform.position + Vector3.up * 2;
                    }
                }
            }
        }
    }

    //funcion para abrir o cerrar puertas
    public void ToggleDoor(int direction, bool value, bool turnSecret = false)
    {
        doors[direction].SetActive(value);
        if (turnSecret)
        {
            doors[direction].layer = LayerMask.NameToLayer(whatIsSecret);
        }
    }

    public void TurnSecret()
    {
        Debug.Log("secret room spawned");
        isSecret = true;
        foreach (var item in doors)
        {
            item.layer = LayerMask.NameToLayer(whatIsSecret);
            item.SetActive(true);
        }
    }

    public void TurnBossRoom()
    {
        // Debug.Log("boss room spawned");
        isBossRoom = true;
        Instantiate(bossPrefab, this.transform.position + Vector3.up*4, Quaternion.identity, EnemiesInfo.instance.enemyParent);
    }
}
