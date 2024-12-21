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
    public Color dayColor = new Color(1f, 0.956f, 0.839f); // �� ����
    public Color nightColor = new Color(0.1f, 0.1f, 0.2f); // �� ����
    public float dayIntensity = 1.5f; // �� ����
    public float nightIntensity = 0.1f; // �� ����
    bool beforeTime;    // false == night, true == day
    bool afterTime;
    GameObject[] StreetLamps;
    public float flickerSpeed = 10f; // �����̴� �ӵ� (��)
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
        // ���� �ð� ���� ��� (0 ~ 1)
        float timeNormalized = (Time.time % cycleDuration) / cycleDuration;

        // �¾��� ȸ�� ����
        float angle = timeNormalized * 360f;
        transform.rotation = Quaternion.Euler(new Vector3(angle - 90f, 0, 0));

        // ������ ����� ���� ��ȭ
        DirectionalLightComponent.color = Color.Lerp(nightColor, dayColor, Mathf.Sin(timeNormalized * Mathf.PI));
        DirectionalLightComponent.intensity = Mathf.Lerp(nightIntensity, dayIntensity, Mathf.Sin(timeNormalized * Mathf.PI));

        // ��/�� ���¿� ���� ���� Light Ȱ��ȭ/��Ȱ��ȭ
        afterTime = DirectionalLightComponent.intensity < 1.0f; // �� �Ǵ� ����
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
            Debug.Log("���� �Ǿ����ϴ�.");
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
        int flickerCounter = 0; // ������ Ƚ�� ī��Ʈ

        while (flickerCounter < flickerCount)
        {
            // �����̴� ȿ��: �Һ��� �Ѱ� ����
            light.enabled = !light.enabled;
            flickerCounter++; // ������ Ƚ�� ����
            flickerSpeed = Mathf.Max(flickerSpeed - 0.05f, 0.05f); // �ּ� 0.05�ʷ� ����

            yield return new WaitForSeconds(flickerSpeed); // �����̴� �ӵ���ŭ ��ٸ���
        }

        // �������� ���� �� �Һ��� ��
        light.enabled = true;
    }
}
