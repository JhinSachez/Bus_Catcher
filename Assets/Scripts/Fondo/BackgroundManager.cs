using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundLayer
    {
        public Sprite backgroundSprite;
        public float transitionDuration = 2f;
        public float displayTime = 10f;
        [Range(0, 1)] public float startOpacity = 0f;
        public Color tint = Color.white;
    }

    [Header("Background Configuration")]
    public List<BackgroundLayer> backgrounds = new List<BackgroundLayer>();
    public bool loop = true;
    public int startIndex = 0;

    [Header("Transition Settings")]
    public TransitionMode transitionMode = TransitionMode.CrossFade;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool autoStart = true;
    public int sortingOrder = -1000;
    
    public GameObject lunaPrefab;
    public GameObject solPrefab;
   

    public enum TransitionMode
    {
        CrossFade,          // Fundido cruzado clásico
        SmoothStep,         // Fundido con suavizado
        Linear,            // Transición lineal
        WipeLeft,          // Cortinilla de izquierda
        WipeRight,         // Cortinilla de derecha
        WipeUp,            // Cortinilla arriba
        WipeDown,          // Cortinilla abajo
        FadeThroughColor,  // Fundido a color sólido
        SlideLeft,         // Deslizamiento izquierda
        SlideRight,        // Deslizamiento derecha
        SlideUp,           // Deslizamiento arriba
        SlideDown          // Deslizamiento abajo
    }

  
    private Image backgroundImage;
    private Image overlayImage;
    private Canvas backgroundCanvas;
    private Canvas overlayCanvas;
    private Luna luna;
    private Sol sol;
    
    
    private int currentIndex = 0;
    private int nextIndex = 0;
    private bool isTransitioning = false;
    private Coroutine transitionCoroutine;
    
   
    public System.Action<int> OnBackgroundChanged;
    public System.Action<int> OnTransitionComplete;

    void Awake()
    {
        CreateBackgroundCanvas();
        CreateOverlayCanvas();
        
        if (lunaPrefab != null)
        {
            GameObject lunaObj = Instantiate(lunaPrefab);
            luna = lunaObj.GetComponent<Luna>();
            luna.gameObject.SetActive(false);
        }
        
        if (solPrefab != null)
        {
            GameObject solObj = Instantiate(solPrefab);
            sol = solObj.GetComponent<Sol>();
            sol.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (autoStart && backgrounds.Count > 0)
        {
            SetBackground(startIndex);
            
            if (startIndex == 0) // Cambia el 1 por el índice de tu fondo de noche
            {
                if (luna != null)
                {
                    luna.IniciarMovimiento(backgrounds[startIndex].displayTime);
                }
            }
            
            if (backgrounds.Count > 1)
            {
                StartBackgroundCycle();
            }
        }
    }

    private void CreateBackgroundCanvas()
{
    // Crear GameObject para el fondo principal
    GameObject bgObject = new GameObject("Background_Main");
    bgObject.transform.SetParent(transform);
    bgObject.transform.localPosition = Vector3.zero;
    bgObject.transform.localScale = Vector3.one;
    
    // Configurar Canvas en World Space
    backgroundCanvas = bgObject.AddComponent<Canvas>();
    backgroundCanvas.renderMode = RenderMode.WorldSpace;
    
    // Configurar la cámara principal
    Camera mainCamera = Camera.main;
    if (mainCamera != null)
    {
        backgroundCanvas.worldCamera = mainCamera;
    }
    
    backgroundCanvas.sortingOrder = sortingOrder;
    
    // Configurar Canvas Scaler para World Space
    CanvasScaler canvasScaler = bgObject.AddComponent<CanvasScaler>();
    canvasScaler.dynamicPixelsPerUnit = 100;
    canvasScaler.referencePixelsPerUnit = 100;
    
    // Desactivar raycasting
    GraphicRaycaster raycaster = bgObject.AddComponent<GraphicRaycaster>();
    raycaster.enabled = false;
    
    // Configurar Image
    GameObject imageObject = new GameObject("Background_Image");
    imageObject.transform.SetParent(bgObject.transform);
    imageObject.transform.localPosition = Vector3.zero;
    imageObject.transform.localRotation = Quaternion.identity;
    imageObject.transform.localScale = Vector3.one;
    
    backgroundImage = imageObject.AddComponent<Image>();
    backgroundImage.raycastTarget = false;
    backgroundImage.preserveAspect = false;
    
    // Configurar RectTransform para que coincida con el tamaño de la cámara
    RectTransform rectTransform = backgroundImage.GetComponent<RectTransform>();
    
    // Calcular el tamaño del mundo basado en la cámara
    if (mainCamera != null)
    {
        float cameraHeight = mainCamera.orthographic ? mainCamera.orthographicSize * 2 : 10f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        rectTransform.sizeDelta = new Vector2(cameraWidth, cameraHeight);
    }
    else
    {
        rectTransform.sizeDelta = new Vector2(1920, 1080); // Tamaño por defecto
    }
    
    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    rectTransform.pivot = new Vector2(0.5f, 0.5f);
    rectTransform.anchoredPosition = Vector2.zero;
}

private void CreateOverlayCanvas()
{
    // Crear GameObject para el overlay
    GameObject overlayObject = new GameObject("Background_Overlay");
    overlayObject.transform.SetParent(transform);
    overlayObject.transform.localPosition = Vector3.zero;
    overlayObject.transform.localScale = Vector3.one;
    
    // Configurar Canvas en World Space
    overlayCanvas = overlayObject.AddComponent<Canvas>();
    overlayCanvas.renderMode = RenderMode.WorldSpace;
    
    Camera mainCamera = Camera.main;
    if (mainCamera != null)
    {
        overlayCanvas.worldCamera = mainCamera;
    }
    
    overlayCanvas.sortingOrder = sortingOrder + 1;
    
    // Canvas Scaler
    CanvasScaler canvasScaler = overlayObject.AddComponent<CanvasScaler>();
    canvasScaler.dynamicPixelsPerUnit = 100;
    canvasScaler.referencePixelsPerUnit = 100;
    
    // Desactivar raycasting
    GraphicRaycaster raycaster = overlayObject.AddComponent<GraphicRaycaster>();
    raycaster.enabled = false;
    
    // Configurar Image
    GameObject imageObject = new GameObject("Transition_Image");
    imageObject.transform.SetParent(overlayObject.transform);
    imageObject.transform.localPosition = Vector3.zero;
    imageObject.transform.localRotation = Quaternion.identity;
    imageObject.transform.localScale = Vector3.one;
    
    overlayImage = imageObject.AddComponent<Image>();
    overlayImage.raycastTarget = false;
    overlayImage.preserveAspect = false;
    overlayImage.color = Color.clear;
    
    // Configurar RectTransform
    RectTransform rectTransform = overlayImage.GetComponent<RectTransform>();
    
    // Calcular el tamaño del mundo basado en la cámara
    if (mainCamera != null)
    {
        float cameraHeight = mainCamera.orthographic ? mainCamera.orthographicSize * 2 : 10f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        rectTransform.sizeDelta = new Vector2(cameraWidth, cameraHeight);
    }
    else
    {
        rectTransform.sizeDelta = new Vector2(1920, 1080);
    }
    
    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    rectTransform.pivot = new Vector2(0.5f, 0.5f);
    rectTransform.anchoredPosition = Vector2.zero;
    
    // Desactivar por defecto
    overlayObject.SetActive(false);
}

    public void StartBackgroundCycle()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(BackgroundCycleCoroutine());
    }

    private IEnumerator BackgroundCycleCoroutine()
    {
        while (loop || currentIndex < backgrounds.Count - 1)
        {
            yield return new WaitForSeconds(backgrounds[currentIndex].displayTime);
            
            nextIndex = (currentIndex + 1) % backgrounds.Count;
            yield return StartCoroutine(TransitionToBackground(nextIndex));
        }
    }

    public void TransitionToIndex(int index)
    {
        if (index < 0 || index >= backgrounds.Count) return;
        
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        nextIndex = index;
        transitionCoroutine = StartCoroutine(TransitionToBackground(index));
    }

    private IEnumerator TransitionToBackground(int targetIndex)
    {
        isTransitioning = true;
        overlayCanvas.gameObject.SetActive(true);
        
        BackgroundLayer targetLayer = backgrounds[targetIndex];
        
        // Configurar overlay con la imagen objetivo
        overlayImage.sprite = targetLayer.backgroundSprite;
        overlayImage.color = new Color(1, 1, 1, 0);
        
        float elapsedTime = 0f;
        float duration = targetLayer.transitionDuration;
        
        // Ejecutar transición según el modo
        switch (transitionMode)
        {
            case TransitionMode.CrossFade:
                yield return StartCoroutine(CrossFadeTransition(duration));
                break;
            case TransitionMode.SmoothStep:
                yield return StartCoroutine(SmoothStepTransition(duration));
                break;
            case TransitionMode.Linear:
                yield return StartCoroutine(LinearTransition(duration));
                break;
            case TransitionMode.WipeLeft:
                yield return StartCoroutine(WipeTransition(duration, -1, 0));
                break;
            case TransitionMode.WipeRight:
                yield return StartCoroutine(WipeTransition(duration, 1, 0));
                break;
            case TransitionMode.WipeUp:
                yield return StartCoroutine(WipeTransition(duration, 0, 1));
                break;
            case TransitionMode.WipeDown:
                yield return StartCoroutine(WipeTransition(duration, 0, -1));
                break;
            case TransitionMode.FadeThroughColor:
                yield return StartCoroutine(FadeThroughColorTransition(duration, Color.black));
                break;
            case TransitionMode.SlideLeft:
                yield return StartCoroutine(SlideTransition(duration, -1, 0));
                break;
            case TransitionMode.SlideRight:
                yield return StartCoroutine(SlideTransition(duration, 1, 0));
                break;
            case TransitionMode.SlideUp:
                yield return StartCoroutine(SlideTransition(duration, 0, 1));
                break;
            case TransitionMode.SlideDown:
                yield return StartCoroutine(SlideTransition(duration, 0, -1));
                break;
        }
        
        // Actualizar fondo principal
        backgroundImage.sprite = targetLayer.backgroundSprite;
        backgroundImage.color = targetLayer.tint;
        
        if (targetIndex == 0) // Cambia el 1 por el índice que tenga tu fondo de noche
        {
            if (luna != null)
            {
                luna.IniciarMovimiento(targetLayer.displayTime);
            }
        }
        
        if (targetIndex == 2)
        {
            if (sol != null)
            {
                float duracionDia = backgrounds[2].displayTime;
                float duracionAtardecer = backgrounds[3].displayTime;
                sol.IniciarMovimiento(duracionDia, duracionAtardecer);
            }
        }
        
        // Limpiar overlay
        overlayImage.sprite = null;
        overlayImage.color = Color.clear;
        overlayCanvas.gameObject.SetActive(false);
        
        // Actualizar índice
        currentIndex = targetIndex;
        isTransitioning = false;
        
        // Eventos
        OnBackgroundChanged?.Invoke(currentIndex);
        OnTransitionComplete?.Invoke(currentIndex);
    }

    #region Transitions
    
    private IEnumerator CrossFadeTransition(float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float smoothT = transitionCurve.Evaluate(t);
            
            overlayImage.color = new Color(1, 1, 1, smoothT);
            
            yield return null;
        }
        
        overlayImage.color = Color.white;
    }

    private IEnumerator SmoothStepTransition(float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            overlayImage.color = new Color(1, 1, 1, smoothT);
            
            yield return null;
        }
        
        overlayImage.color = Color.white;
    }

    private IEnumerator LinearTransition(float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            
            overlayImage.color = new Color(1, 1, 1, t);
            
            yield return null;
        }
        
        overlayImage.color = Color.white;
    }

    private IEnumerator WipeTransition(float duration, float directionX, float directionY)
    {
        float elapsedTime = 0f;
        RectTransform rectTransform = overlayImage.GetComponent<RectTransform>();
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = new Vector2(directionX * Screen.width, directionY * Screen.height);
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float smoothT = transitionCurve.Evaluate(t);
            
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            overlayImage.color = Color.white;
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = Vector2.zero;
    }

    private IEnumerator FadeThroughColorTransition(float duration, Color fadeColor)
    {
        float elapsedTime = 0f;
        float halfDuration = duration / 2;
        
        // Fundido a color
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            
            overlayImage.color = Color.Lerp(Color.clear, fadeColor, t);
            
            yield return null;
        }
        
        elapsedTime = 0f;
        backgroundImage.sprite = overlayImage.sprite;
        
        // Fundido desde color
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / halfDuration;
            
            overlayImage.color = Color.Lerp(fadeColor, Color.clear, t);
            
            yield return null;
        }
        
        overlayImage.color = Color.clear;
    }

    private IEnumerator SlideTransition(float duration, float directionX, float directionY)
    {
        float elapsedTime = 0f;
        RectTransform rectTransform = overlayImage.GetComponent<RectTransform>();
        Vector2 startPos = new Vector2(directionX * -Screen.width, directionY * -Screen.height);
        Vector2 endPos = Vector2.zero;
        
        rectTransform.anchoredPosition = startPos;
        overlayImage.color = Color.white;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float smoothT = transitionCurve.Evaluate(t);
            
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothT);
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = Vector2.zero;
    }

    #endregion

    #region Public Methods

    public void SetBackground(int index)
    {
        if (index >= 0 && index < backgrounds.Count)
        {
            backgroundImage.sprite = backgrounds[index].backgroundSprite;
            backgroundImage.color = backgrounds[index].tint;
            currentIndex = index;
        }
        
        if (index == 0) // Cambia el 1 por el índice de tu fondo de noche
        {
            if (luna != null)
            {
                luna.IniciarMovimiento(backgrounds[index].displayTime);
            }
        }
        
        if (startIndex == 2)
        {
            if (sol != null)
            {
                float duracionDia = backgrounds[2].displayTime;
                float duracionAtardecer = backgrounds[3].displayTime;
                sol.IniciarMovimiento(duracionDia, duracionAtardecer);
            }
        }
    }

    public void SetBackgroundImmediate(int index)
    {
        if (isTransitioning && transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            isTransitioning = false;
            overlayCanvas.gameObject.SetActive(false);
        }
        
        SetBackground(index);
    }

    public void PauseTransition()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }
    }

    public void ResumeTransition()
    {
        if (!isTransitioning && backgrounds.Count > 1)
        {
            transitionCoroutine = StartCoroutine(BackgroundCycleCoroutine());
        }
    }

    #endregion

    #region Properties

    public int CurrentIndex => currentIndex;
    public bool IsTransitioning => isTransitioning;
    public BackgroundLayer CurrentBackground => backgrounds.Count > 0 ? backgrounds[currentIndex] : null;

    #endregion

    void OnDestroy()
    {
        StopAllCoroutines();
    }
}