using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject UI;
    bool CanRestart;
    private float timeElapsed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        timeElapsed = 0;
        CanRestart = false;
        UI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (CanRestart)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
