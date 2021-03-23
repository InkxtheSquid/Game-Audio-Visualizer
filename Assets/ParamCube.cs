using UnityEngine;

public class ParamCube : MonoBehaviour
{
    [HideInInspector] public int _band;
    [SerializeField] private float _startScale = 1f;
    [SerializeField] private float _scaleMultipler = 10f;

    private float startPos;
    private float previousPos;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.y;
        previousPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float resizeAmount = FMODAudioVisualizer.bandBuffer[_band] * _scaleMultipler;

        transform.position = new Vector2(transform.position.x, startPos);
        this.GetComponent<SpriteRenderer>().size = new Vector2(1, _startScale + resizeAmount);
    }
}