using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameOver : MonoBehaviour
{
    public GameObject UI;
    bool CanRestart;
    private float timeElapsed = 0f;
    public bool SpawnEnemy = false;
    public Terrain terrain;
    public GameObject ChomperPrefab = null;
    public int nChompers = 5;
    public GameObject SpitterPrefab = null;
    public int nSpitters = 5;
    public GameObject GrenadierPrefab = null;
    public int nGrenadiers = 1;

    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0;
        CanRestart = false;
        UI.SetActive(false);
        if(SpawnEnemy == true)
        {
            SpawnMobsRandomPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (CanRestart)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene("StartMenu");
            }
        }
    }

    public void Over()
    {
        //Debug.Log("오버오버");
        UI.GetComponentInChildren<TextMeshProUGUI>().text = "GAME OVER\n\n" + timeElapsed.ToString("F2") + "s\n\nPress Enter To Begin";
        UI.SetActive(true);
        CanRestart = true;
    }
    public void Complete()
    {
        UI.GetComponentInChildren<TextMeshProUGUI>().text = "Complete\n\n" + timeElapsed.ToString("F2") + "s\n\nPress Enter To Begin";
        UI.SetActive(true);
        CanRestart = true;
    }

    void SpawnMobsRandomPosition()
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPosition = terrain.transform.position;

        for (int i = 0; i < nChompers+ nSpitters+ nGrenadiers; i++)
        {
            // 랜덤 XZ 좌표 생성
            float randomX = Random.Range(0, terrainData.size.x);
            float randomZ = Random.Range(0, terrainData.size.z);

            // 높이 계산
            float terrainHeight = terrainData.GetHeight(
                Mathf.FloorToInt(randomX / terrainData.size.x * terrainData.heightmapResolution),
                Mathf.FloorToInt(randomZ / terrainData.size.z * terrainData.heightmapResolution)
            );

            // 소환 위치 계산
            Vector3 spawnPosition = new Vector3(
                randomX + terrainPosition.x,
                terrainHeight + terrainPosition.y,
                randomZ + terrainPosition.z
            );

            Object mobPrefab = null;
            if (i < nChompers)
            {
                mobPrefab = ChomperPrefab;
            }
            if(i >=nChompers && i < nChompers+nSpitters)
            {
                mobPrefab = SpitterPrefab;
            }
            if (i >= nChompers + nSpitters)
            {
                mobPrefab = GrenadierPrefab;

            }
            // 몹 소환
            Instantiate(mobPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
