using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using TMPro;
using UnityEditor.Animations;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AllenInput : MonoBehaviour
{
    public float MAXHP;
    private float currHP;
    public GameObject Camera;
    public GameObject UI;
    private float timeElapsed = 0f;
    Animator animator;
    int rotationSpeed = 5;
    public float distanceFromPlayer = 5f; // �÷��̾�κ����� �Ÿ�
    public float height = 2f; // ī�޶��� ����
    private float mouseX;
    private float mouseY;
    private float PlayerHalfHeight = 0.895f;
    public LayerMask enemyLayer; // �� ���̾�
    public float attackRange = 1f; // ���� ����
    private bool Aiming;
    private bool Punching;
    bool CanCombo;
    bool CanDamage;
    bool isDying;
    bool canRestart;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Aiming = false;
        Punching = false;
        CanCombo = false;
        currHP = MAXHP;
        UI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        //Debug.Log(UI.GetComponentInChildren<TextMeshProUGUI>().text);
        if (isDying == false)
        {
            MouseInput();
            KeyboardInput();
            if (CanDamage)
            {
                // ���� ���� ���� ���� ����
                Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                foreach (Collider enemy in hitEnemies)
                {
                    // ������ ���ظ� �ִ� ����
                    // ���� ���, ���� Health ��ũ��Ʈ�� �����Ͽ� ���ظ� �ִ� ���
                    //enemy.GetComponent<Enemy>().TakeDamage(10); // ��: 10�� ���ظ� ��
                    enemy.GetComponent<ControllAnimator>().GetDamage(1);
                }
                if (hitEnemies.Length > 0)
                {
                    CanDamage = false;
                }
            }
        }
        if (canRestart)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    private void KeyboardInput()
    {
        // WASD �Է� ó��
        float horizontal = Input.GetAxis("Horizontal"); // A, D Ű �Է�
        float vertical = Input.GetAxis("Vertical"); // W, S Ű �Է�

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        // �ȱ� ���� Ȯ��
        bool isWalking = horizontal != 0 || vertical != 0;
        animator.SetBool("Walk?", isWalking); // �ȱ� �ִϸ��̼� �Ķ���� ����

        // �޸��� ���� Ȯ��
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift); // Shift Ű�� ������ ��
        animator.SetBool("Run?", isRunning); // �޸��� �ִϸ��̼� �Ķ���� ����

        Vector3 cameraForward_xz = Camera.transform.forward;
        cameraForward_xz.y = 0;
        cameraForward_xz.Normalize();

        Vector3 cameraRight_xz = Camera.transform.right;
        cameraRight_xz.y = 0;
        cameraRight_xz.Normalize();

        Vector3 moveDirection = cameraForward_xz * direction.z + cameraRight_xz * direction.x;
        if (moveDirection.magnitude > 0)
        {
            // ��ǥ ȸ�� ����
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // �ε巴�� ȸ��
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Aiming = true;
            animator.SetBool("Aiming?", true);
            animator.SetTrigger("AimingStart");
            animator.ResetTrigger("AimingEnd");
        }
        if (Input.GetMouseButtonUp(1))
        {
            Aiming = false;
            Punching = false;
            animator.ResetTrigger("PunchStart");
            animator.ResetTrigger("PunchCombo");
            animator.SetBool("Aiming?", false);
            animator.SetTrigger("AimingEnd");
        }
        // ���콺 �̵� ����
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // y�� ȸ�� (�¿�)
        mouseY = Mathf.Clamp(mouseY, -30f, 30f); // ���� ȸ�� ����

        // ī�޶� ��ġ ���

        Vector3 Ndirection = new Vector3(0, 0, -1);
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        Vector3 CameraDirection = rotation * Ndirection;

        RaycastHit PlayerToCam;
        Vector3 PlayerCenter = transform.position;
        PlayerCenter.y += PlayerHalfHeight;
        if (Physics.Raycast(PlayerCenter, CameraDirection, out PlayerToCam, distanceFromPlayer))
        {
            Ndirection *= PlayerToCam.distance;
        }
        else
        {
            Ndirection *= 2;
        }
        Vector3 position = transform.position + rotation * Ndirection;
        position.y += PlayerHalfHeight;
        // ī�޶� ��ġ�� ȸ�� ����
        Camera.transform.position = position;
        Camera.transform.LookAt(PlayerCenter); // �÷��̾ �ٶ󺸵��� ����

        if (Aiming)
        {
            Vector3 CameraForward = Camera.transform.forward;
            CameraForward.x = CameraForward.x + Camera.transform.right.x * 0.5f;
            CameraForward.y = 0;
            CameraForward.z = CameraForward.z + Camera.transform.right.z * 0.5f;
            Quaternion playerRotation = Quaternion.LookRotation(CameraForward); // ī�޶��� forward���� ��¦ ������ �밢
            transform.rotation = playerRotation;

            Vector3 CamForwardOffset = Camera.transform.forward;
            CamForwardOffset.y = 0; // xz ��� ���ͷ�
            CamForwardOffset = CamForwardOffset.normalized;
            Vector3 CamRightOffset = Camera.transform.right;
            CamRightOffset.y = 0; // xz ��� ���ͷ�
            CamRightOffset = CamRightOffset.normalized;

            Vector3 aimingPosition = Camera.transform.position + CamForwardOffset*0.5f+ CamRightOffset;

            Vector3 P2CDirection = (aimingPosition - PlayerCenter).normalized;
            if (Physics.Raycast(PlayerCenter, P2CDirection, out PlayerToCam, distanceFromPlayer))
            {
                P2CDirection *= PlayerToCam.distance;
            }
            else
            {
                P2CDirection *= distanceFromPlayer;
            }
            Camera.transform.position = PlayerCenter + P2CDirection;
            //Camera.transform.position = aimingPosition;

            //Camera.transform.position = aimingpoisition;
            //+= transform.right + transform.forward * 0.8f;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        // �ִϸ��̼� ����
        if (Punching == false)
        {
            Punching = true;
            animator.SetTrigger("PunchStart");
            animator.ResetTrigger("PunchCombo");
        }
        else
        {
            if (CanCombo)
            {
                animator.SetTrigger("PunchCombo");
            }
        }
        if (Aiming) {
            Ray ray = Camera.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f))
            {
                Debug.Log(hit.collider.gameObject.name);
                ControllAnimator target = hit.collider.GetComponent<ControllAnimator>();
                if (target != null)
                {
                    target.GetDamage(3);
                }
            }
        }
    }

    void MeleeAttackStart()
    {
        CanDamage = true;
        //Debug.Log("Damage True");
    }
    void MeleeAttackEnd()
    {
        CanDamage = false;
        //Debug.Log("Damage false");

    }
    void ComboInputBegin()
    {
        CanCombo = true;
    }
    void ComboInputEnd()
    {
        CanCombo = false;
    }
    void MotionEnd()
    {
        //Debug.Log("MotionEnd");
        Punching = false;
    }

    public void Damage(float damage)
    {
        if (isDying == false)
        {
            Debug.Log(gameObject.name + "�� " + damage + "���ظ� ����");
            currHP -= damage;
            if (currHP <= 0)
            {
                isDying = true;
                animator.SetTrigger("Die");
            }
            else
            {
                animator.SetTrigger("Hurt");
            }
        }
    }
    void GameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        UI.GetComponentInChildren<TextMeshProUGUI>().text = "Game Over\n\nTime: " + timeElapsed.ToString("F2") +"s\n\n Press Enter To Begin";
        UI.SetActive(true);
        canRestart = true;
    }
}
