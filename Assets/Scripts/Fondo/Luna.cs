using UnityEngine;

public class Luna : MonoBehaviour
{
    private Transform camara;
    private SpriteRenderer spriteRenderer;
    private float duracionNoche;
    private float tiempoTranscurrido = 0f;
    private bool moverLuna = false;
    
    // Puntos de la parábola
    private Vector3 puntoInicio;
    private Vector3 puntoControl;
    private Vector3 puntoFinal;

    void Awake()
    {
        camara = Camera.main.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (moverLuna)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = Mathf.Clamp01(tiempoTranscurrido / duracionNoche);
            
            // Curva cuadrática de Bézier para la parábola
            transform.position = (1 - t) * (1 - t) * puntoInicio + 
                                 2 * (1 - t) * t * puntoControl + 
                                 t * t * puntoFinal;
            
            if (t >= 1f)
            {
                moverLuna = false;
                gameObject.SetActive(false);
            }
        }
    }

    public void IniciarMovimiento(float duracion)
    {
        duracionNoche = duracion;
        tiempoTranscurrido = 0f;
        
        // Calcular puntos en espacio mundial basado en la cámara
        float alturaCamara = camara.position.y;
        float anchoMundo = camara.GetComponent<Camera>().orthographicSize * 2 * camara.GetComponent<Camera>().aspect;
        float alturaMundo = camara.GetComponent<Camera>().orthographicSize * 2;
        
        // Punto de inicio: fuera de la pantalla por la derecha, arriba
        puntoInicio = new Vector3(
            camara.position.x + anchoMundo/2 + 2f,
            alturaCamara + alturaMundo/3,
            0
        );
        
        // Punto de control: arriba en el centro
        puntoControl = new Vector3(
            camara.position.x,
            alturaCamara + alturaMundo/2 + 2f,
            0
        );
        
        // Punto final: fuera de la pantalla por la izquierda
        puntoFinal = new Vector3(
            camara.position.x - anchoMundo/2 - 2f,
            alturaCamara - alturaMundo/4,
            0
        );
        
        transform.position = puntoInicio;
        gameObject.SetActive(true);
        moverLuna = true;
    }
}
