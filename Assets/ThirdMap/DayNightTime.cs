using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DayNightTime : MonoBehaviour
{
    // Start is called before the first frame update
    public float cycleDuration = 600f;
    Light DirectionalLightComponent;
    public Color dayColor = new Color(1f, 0.956f, 0.839f); // 낮 색상
    public Color nightColor = new Color(0.1f, 0.1f, 0.2f); // 밤 색상
    public float dayIntensity = 1.5f; // 낮 강도
    public float nightIntensity = 0.1f; // 밤 강도
    bool beforeTime;    // false == night, true == day
    bool afterTime;
    GameObject[] StreetLamps;
    public float flickerSpeed = 10f; // 깜빡이는 속도 (초)
    public int flickerCount = 5;
    public Light PlayerFlash;
    void Start()
    {
        DirectionalLightComponent = FindObjectOfType<Light>();
        StreetLamps = GameObject.FindGameObjectsWithTag("StreetLamp");
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 시간 비율 계산 (0 ~ 1)
        float timeNormalized = (Time.time % cycleDuration) / cycleDuration;

        // 태양의 회전 변경
        float angle = timeNormalized * 360f;
        transform.rotation = Quaternion.Euler(new Vector3(angle - 90f, 0, 0));

        // 조명의 색상과 강도 변화
        DirectionalLightComponent.color = Color.Lerp(nightColor, dayColor, Mathf.Sin(timeNormalized * Mathf.PI));
        DirectionalLightComponent.intensity = Mathf.Lerp(nightIntensity, dayIntensity, Mathf.Sin(timeNormalized * Mathf.PI));

        // 밤/낮 상태에 따라 하위 Light 활성화/비활성화
        afterTime = DirectionalLightComponent.intensity < 1.0f; // 밤 판단 기준
        if (beforeTime != afterTime)
        {
            SetChildLightsActive(afterTime);
            beforeTime = afterTime;
        }
    }
    void SetChildLightsActive(bool active)
    {
        if (StreetLamps == null) return;

        if (active == true)
        {
            Debug.Log("밤이 되었습니다.");
            PlayerFlash.enabled = true;
            foreach (var StreetLamp in StreetLamps)
            {
                StartCoroutine(FlickerLight(StreetLamp.GetComponentInChildren<Light>(true)));
            }
        }
        else {
            PlayerFlash.enabled = false;
            foreach (var StreetLamp in StreetLamps)
            {
                StreetLamp.GetComponentInChildren<Light>(true).enabled = false;
            }
        }
    }
    private IEnumerator FlickerLight(Light light)
    {
        int flickerCounter = 0; // 깜빡인 횟수 카운트

        while (flickerCounter < flickerCount)
        {
            // 깜빡이는 효과: 불빛을 켜고 끄기
            light.enabled = !light.enabled;
            flickerCounter++; // 깜빡인 횟수 증가
            flickerSpeed = Mathf.Max(flickerSpeed - 0.05f, 0.05f); // 최소 0.05초로 제한

            yield return new WaitForSeconds(flickerSpeed); // 깜빡이는 속도만큼 기다리기
        }

        // 깜빡임이 끝난 후 불빛을 켬
        light.enabled = true;
    }
}
