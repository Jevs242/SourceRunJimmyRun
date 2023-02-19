using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Sprite")]
    [SerializeField]private Sprite[] _sprite;

    [Header("Component Reference")]
    [SerializeField] private GameObject _saw;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    private GameManager _gameManager;

    [Header("Actions")]
    private int _random;
    private bool _playerDead;

    [Header("Time")]
    private float desiredDuration = 0.05f;
    private float elapsedTime;

    [Header("Transform")]
    private Vector3 _startPosition;
    private Vector3 _endPosition = new Vector3(0, 1, 0);

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _boxCollider.size = new Vector2(0.825307846f, 0.49619019f);
        transform.localPosition = new Vector3(0, 1.27683139f, 0);
        _saw.transform.localPosition = new Vector3(0, 1.27683139f, 0);
        transform.localRotation = Quaternion.identity;
        SetRandom(_gameManager.GetLastRandom());
        _gameManager.SetLastRandom(_random);
        _saw.GetComponent<SpriteRenderer>().sprite = null;
        _spriteRenderer.sprite = _sprite[_random];
        _startPosition = transform.position;
        _startPosition.x = 0;
        switch (_random)
        {
            case 0:
                break;
            case 1:
                _spriteRenderer.sprite = null;
                _saw.GetComponent<SpriteRenderer>().sprite = _sprite[_random];
                _saw.transform.position += new Vector3(0 , .7f, 0);
                transform.position += new Vector3(0, .7f, 0);
                _boxCollider.size = new Vector2(0.5f, 1.5f);
                break;
            case 2:
                transform.position += new Vector3(0, 2, 0);
                _boxCollider.size = new Vector2(_boxCollider.size.x - 0.5f, 5);
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_random == 1)
        {
            _saw.transform.eulerAngles += new Vector3(0, 0, 40 * Time.deltaTime);
        }

        if(_random == 2 && _playerDead)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / desiredDuration;
            percentageComplete = Mathf.Clamp(percentageComplete, 0, 1);

            transform.position = Vector3.Lerp(_endPosition, _startPosition, Mathf.SmoothStep(0, 1, percentageComplete));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "Player")
        {
            Player player = other.GetComponent<Player>();

            if(_random == 1 && player.GetCrouch() && player.GetGrounded() || _random == 2 && player.GetDash() && player.GetGrounded() && !player.GetEmpty())
            {
                return;
            }

        }

        if(_random == 2)
        {
            _startPosition.x = transform.position.x;
            _endPosition.y = transform.position.y;
            _endPosition.x = transform.position.x;
            _playerDead = true;
        }

        if(_gameManager)
        {
            _gameManager.IsDead();
        }

    }

    private void SetRandom(int lastRandom)
    {
        _random = Random.Range(0, _sprite.Length);
        if(lastRandom == _random)
        {
            SetRandom(lastRandom);
        }
    }


}
