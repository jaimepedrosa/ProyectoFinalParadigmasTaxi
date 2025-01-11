using UnityEngine;

public class SpeedRadar : MonoBehaviour
{
    [SerializeField] private bool alert = false;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float maximumvelocity = 10f; 
    public Taxi taxi;

    void Start()
    {
        if (taxi == null)
        {
            taxi = FindAnyObjectByType<Taxi>();
            if (taxi == null)
            {
                Debug.LogError("No se encontró ningún objeto de tipo Taxi.");
            }
        }
    }
    public bool GetAlert()
    {
        return alert; 
    }

    void Update()
    {
        if (taxi != null)
        {
            float distancia = Vector3.Distance(transform.position, taxi.transform.position);
            if (distancia <= detectionRadius)
            {
                float velocidad = taxi.GetVelocity();
                Debug.Log($"El taxi está dentro del radio del radar. Velocidad: {velocidad} m/s");
                if (velocidad >= maximumvelocity)
                {
                    alert = true;
                    Debug.Log($"Comienza la persecucion entre el taxi y el coche de policia"); 
                }

            }
        }
    }
}
