using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{

    public SlimeIA slime;

    [Header("Movimento")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Gravidade")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Câmeras")]
    [SerializeField] private GameObject camNormal;
    [SerializeField] private GameObject camTop;

    [Header("Attack")]
    [SerializeField] private ParticleSystem fxAtack;
    [SerializeField] private bool isAtacking = false;

    [Range(0.2f, 1f)]
    public float hitRange = 0.5f;
    public Transform hitBox;
    public Collider[] hitEnemies;
    public LayerMask hitMask;
    public int attackDamage = 10;

    //Não aparecem no inspector
    private CharacterController characterController;
    private Animator anim;
    private Vector3 velocity;
    private Vector2 moveInput;
    private bool isRunning;

    //Para receber dano/ter vida
    [Header("Player Stats")]
    public int maxHp = 100;
    public int currentHp;
    public bool isDead = false;

    public bool isDead2 = false;

    [SerializeField] public GameObject gameOverUI; //Tela de GameOver

    [SerializeField] private GameManager gameManager; //Para parar os personagens


    private void Start()
    {
        currentHp = maxHp;
        isDead = false;

        gameOverUI.SetActive(false);

        isRunning = false;

        characterController = GetComponent<CharacterController>();

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead) {
            if (!isDead2)
            {
                Die();
            }
        return;
    }

        MovimentacaoPlayer();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            AtaquePlayer();
        }
    }

    private void MovimentacaoPlayer()
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        bool isMoving = direction.magnitude > 0.1f;
        float speed = isRunning ? runSpeed : walkSpeed;

        isRunning = Keyboard.current.leftShiftKey.isPressed;



        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

            transform.rotation =
                Quaternion.Lerp(transform.rotation,
                targetRotation, rotationSpeed * Time.deltaTime);

            characterController.Move(direction.normalized * speed * Time.deltaTime);

        }
        AplicarGravidade();
        AtualizarAnimacao(isMoving);

    }

    private void AplicarGravidade()
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Pequena força para manter o personagem no chão
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void AtualizarAnimacao(bool estaMovendo)
    {
        float targetSpeed = 0f;

        if (estaMovendo)
        {
            targetSpeed = isRunning ? 1f : 0.5f;
        }
        float smoothSpeed =
            Mathf.Lerp(anim.GetFloat("Speed"),
            targetSpeed, Time.deltaTime * 10f);

        anim.SetFloat("Speed", smoothSpeed);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CamTrigger"))
        {
            camNormal.SetActive(false);
            camTop.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CamTrigger"))
        {
            camNormal.SetActive(true);
            camTop.SetActive(false);
        }
    }

    private void AtaquePlayer()
    {
        if (isAtacking) return; // Evita ataques consecutivos
        anim.SetTrigger("Attack");
        fxAtack.Emit(1);
        isAtacking = true;

        hitEnemies = Physics.OverlapSphere
            (hitBox.position, hitRange, hitMask);

        foreach (Collider enemy in hitEnemies)
        {
            enemy.gameObject.SendMessage(
                "GetHit", attackDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AtackIsDone()
    {
        isAtacking = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (hitBox == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere
            (hitBox.position, hitRange);
    }

    public void GetHit(int damage)
    {
        if (isDead) return;

        currentHp -= damage;

        Debug.Log("Player recebeu dano! HP: " + currentHp);

        anim.SetTrigger("GetHit");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        
        isDead = true;

        slime.StopEnemy();
  


        anim.SetTrigger("Die");
        isDead2 = true;

        characterController.enabled = false;
        gameOverUI.SetActive(true);
        gameManager.isGameOver = true;
        this.enabled = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}