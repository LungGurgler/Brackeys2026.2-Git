using System.Collections;
using UnityEngine;

public class KingController : MonoBehaviour
{

    public static KingController Instance; 
    public Vector2 kingPosition { get { return transform.position; } }
    public Rigidbody2D rb;

    [SerializeField] private float maxHealth;
    private float currentHealth; 

    [SerializeField] private float moveSpeed = 7.5f;

    [SerializeField] private Transform orbit; 

    

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; 
        }

        initPlayer();
    }

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();   
    }

    private void Update()
    {
       
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");
        rb.MovePosition((Vector2) transform.position + move * moveSpeed * Time.deltaTime);

    }

    private void FixedUpdate()
    {
      
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //StartCoroutine(StartCameraShake()); 
        currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
        if(currentHealth <= 0)
        {
            print("Dead");
            Time.timeScale = 0f; 
        }
    }

    void initPlayer()
    {
        currentHealth = maxHealth; 
    }

    private IEnumerator StartCameraShake()
    {
        Camera cam = Camera.main;
        Vector2 pos = cam.transform.position;
        float elapsedTime = 0f; 
        while(elapsedTime < 0.25)
        {
            elapsedTime += Time.deltaTime;
            cam.transform.position = pos + Random.insideUnitCircle * 0.15f; 
            yield return new WaitForEndOfFrame();
        }
        cam.transform.position = pos; 
    }


}
