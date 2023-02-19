using UnityEngine;


public class Map : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField] private GameObject _firstBg;
    [SerializeField] private GameObject _secondBg;
    [SerializeField] private GameObject _thridBg;
    [SerializeField] private GameObject _background;
    [SerializeField] private Camera _camera;
    private Player _player;
    private GameManager _gameManager;

    [Header("Action")]
    bool phase1;
    bool phase2;
    public bool phase3;

    [Header("Transform")]
    private Vector3 _beginLocation;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        _camera = GameObject.Find("Main Camera").gameObject.GetComponent<Camera>();
    }

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        _beginLocation = new Vector3(18.9200001f, 0, -0.2654979f);
        _beginLocation.x += 9;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager.GetBeginPlay()) return;
        if(_player == null) return;

        float speed = _player.GetSpeed();
        transform.position += new Vector3(-speed, 0,0) * Time.deltaTime;

        if (transform.position.x < -3.5f && !phase1)
        {
            phase1 = true;
            Instantiate(this.gameObject, new Vector3(50.0200005f, 0, -0.265497923f), new Quaternion(0, 0, 0, 1));
        }

        if (transform.position.x < -63.17)
        {
            Destroy(this.gameObject);
        }

        if (IsObjectVisible(_camera, _firstBg.GetComponent<SpriteRenderer>()))
        {
            phase2 = true;
        }

    }

    public static bool IsObjectVisible(Camera camera, Renderer renderer)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
    }
}
