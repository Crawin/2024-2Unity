using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AllenInput : MonoBehaviour
{
    public float MAXHP;
    private float currHP;
    public GameObject Camera;
    public GameObject Gameover;

    Animator animator;
    int rotationSpeed = 5;
    public float distanceFromPlayer = 5f; // 플레이어로부터의 거리
    public float height = 2f; // 카메라의 높이
    private float mouseX;
    private float mouseY;
    private float PlayerHalfHeight = 0.895f;
    public LayerMask enemyLayer; // 적 레이어
    public float attackRange = 1f; // 공격 범위
    private bool Aiming;
    private bool Punching;
    bool CanCombo;
    bool CanDamage;
    bool isDying;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Aiming = false;
        Punching = false;
        CanCombo = false;
        currHP = MAXHP;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(UI.GetComponentInChildren<TextMeshProUGUI>().text);
        if (isDying == false)
        {
            MouseInput();
            KeyboardInput();
            if (CanDamage)
            {
                // 공격 범위 내의 적을 감지
                Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
                foreach (Collider enemy in hitEnemies)
                {
                    // 적에게 피해를 주는 로직
                    // 예를 들어, 적의 Health 스크립트에 접근하여 피해를 주는 방식
                    //enemy.GetComponent<Enemy>().TakeDamage(10); // 예: 10의 피해를 줌
                    enemy.GetComponent<ControllAnimator>().GetDamage(1);
                }
                if (hitEnemies.Length > 0)
                {
                    CanDamage = false;
                }
            }
        }
    }
    private void KeyboardInput()
    {
        // WASD 입력 처리
        float horizontal = Input.GetAxis("Horizontal"); // A, D 키 입력
        float vertical = Input.GetAxis("Vertical"); // W, S 키 입력

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        // 걷기 상태 확인
        bool isWalking = horizontal != 0 || vertical != 0;
        animator.SetBool("Walk?", isWalking); // 걷기 애니메이션 파라미터 설정

        // 달리기 상태 확인
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift); // Shift 키가 눌렸을 때
        animator.SetBool("Run?", isRunning); // 달리기 애니메이션 파라미터 설정

        Vector3 cameraForward_xz = Camera.transform.forward;
        cameraForward_xz.y = 0;
        cameraForward_xz.Normalize();

        Vector3 cameraRight_xz = Camera.transform.right;
        cameraRight_xz.y = 0;
        cameraRight_xz.Normalize();

        Vector3 moveDirection = cameraForward_xz * direction.z + cameraRight_xz * direction.x;
        if (moveDirection.magnitude > 0)
        {
            // 목표 회전 생성
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // 부드럽게 회전
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
        // 마우스 이동 감지
        mouseX += Input.GetAxis("Mouse X") * rotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // y축 회전 (좌우)
        mouseY = Mathf.Clamp(mouseY, -30f, 30f); // 상하 회전 제한

        // 카메라 위치 계산

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
        // 카메라 위치와 회전 적용
        Camera.transform.position = position;
        Camera.transform.LookAt(PlayerCenter); // 플레이어를 바라보도록 설정

        if (Aiming)
        {
            Vector3 CameraForward = Camera.transform.forward;
            CameraForward.x = CameraForward.x + Camera.transform.right.x * 0.5f;
            CameraForward.y = 0;
            CameraForward.z = CameraForward.z + Camera.transform.right.z * 0.5f;
            Quaternion playerRotation = Quaternion.LookRotation(CameraForward); // 카메라의 forward보다 살짝 오른쪽 대각
            transform.rotation = playerRotation;

            Vector3 CamForwardOffset = Camera.transform.forward;
            CamForwardOffset.y = 0; // xz 평면 벡터로
            CamForwardOffset = CamForwardOffset.normalized;
            Vector3 CamRightOffset = Camera.transform.right;
            CamRightOffset.y = 0; // xz 평면 벡터로
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
        // 애니메이션 실행
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
            Debug.Log(gameObject.name + "이 " + damage + "피해를 입음");
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
        Debug.Log("겜오버");
        Gameover.GetComponent<GameOver>().Over();
    }
}
