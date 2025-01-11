using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Necesario para usar TextMeshPro

public class Taxi : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    public float speed = 10f;
    public float rotationSpeed = 200f;
    public int lifeTaxi = 100;
    public float umbral = 1f;
    public bool estaViaje = false;
    [SerializeField] private GameObject persona;
    [SerializeField] private TextMeshProUGUI vidaText;  

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Estabilizar el centro de masa
        ActualizarTextoVida();
    }

    void FixedUpdate()
    {
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        Vector3 forwardMovement = transform.forward * moveVertical * speed;
        rb.linearVelocity = new Vector3(forwardMovement.x, rb.linearVelocity.y, forwardMovement.z);

        if (moveHorizontal != 0)
        {
            float rotation = moveHorizontal * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, rotation, 0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            lifeTaxi -= 5; // Resta vida al taxi
            ActualizarTextoVida(); // Actualiza el texto de vida en pantalla

            if (lifeTaxi <= 0)
            {
                Debug.Log("El taxi ha sido destruido.");
                Destroy(gameObject); // Elimina el taxi si la vida llega a 0
                SceneManager.LoadScene(2); 
            }
        }
    }

    public void SetVelocity(float Speed)
    {
        speed = Speed;
    }

    public float GetVelocity()
    {
        return rb.linearVelocity.magnitude;
    }

    public void ComenzarViaje()
    {
        if (rb.linearVelocity.magnitude == 0 && Vector3.Distance(transform.position, persona.transform.position) < umbral && !estaViaje)
        {
            Debug.Log($"El taxi comienza un viaje");
            SetVelocity(12f);
            estaViaje = true;
            Destroy(persona);
        }
        else
        {
            Debug.Log($"En estas condiciones no puede iniciar un viaje");
        }
    }

    private void ActualizarTextoVida()
    {
        if (vidaText != null)
        {
            vidaText.text = $"Vida del taxi: {lifeTaxi}";
        }
        else
        {
            Debug.LogWarning("TextMeshPro no asignado en el Inspector.");
        }
    }
}
