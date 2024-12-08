using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    Image mbackgroundImage;
    GameObject mFirstButtons;
    GameObject mSecondButtons;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        mFirstButtons = GameObject.Find("FirstButtons");
        mSecondButtons = GameObject.Find("SecondButtons");
        mSecondButtons.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClick(string ButtonType)
    {
#if UNITY_EDITOR
        Debug.Log(ButtonType + " Clicked");
#endif
        switch (ButtonType)
        {
            case "Play":
                StartCoroutine(FadeOutObject(mFirstButtons, 1, 0));
                StartCoroutine(FadeOutObject(mSecondButtons, 0, 1));
                break;
            case "Exit":
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
            case "Map1":
                SceneManager.LoadScene("2024");
                break;
            case "Map2":
                SceneManager.LoadScene("SecondMap");
                break;
            case "Return":
                StartCoroutine(FadeOutObject(mFirstButtons, 0, 1));
                StartCoroutine(FadeOutObject(mSecondButtons, 1, 0));
                break;
            default:
                Debug.Log("할당되지 않은 버튼 타입");
                break;
        }
    }

    System.Collections.IEnumerator FadeOutObject(GameObject group,uint startAlpha, uint endAlpha)
    {
        Button[] buttons = group.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons) { button.interactable = false; }
        if (startAlpha == 0)
        {
            group.SetActive(true);
        }
        float elapsed = 0f;
        float fadeDuration = 0.5f;
        while(elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            group.GetComponent<CanvasGroup>().alpha = alpha;
            yield return null;
        }
        if (endAlpha == 0)
        {
            group.SetActive(false);
        }
        foreach (Button button in buttons) { button.interactable = true; }
    }
}
