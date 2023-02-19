using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Actions")]
    [SerializeField] public float speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Sprite _idleSprite;
    private bool isInDash = false;
    private float _resistence = 0;
    private float _maxResistence = 100;
    private bool empty;
    private bool _dead;
    public float nowSpeed = 0;
    
    [Header("UI")]
    [SerializeField] private GameObject _imageFill;

    [Header("Component Reference")]
    [SerializeField] private GameObject _deadParticle;
    private CapsuleCollider2D _capsuleCollider2D;
    private PlayerInput _inputPlayer;
    private Rigidbody2D _rb;
    private Animator _animator;
    private GameManager _gameManager;

    [Header("Input")]
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _crouchAction;
    private InputAction _pauseAction;


    private void Awake()
    {
        _gameManager = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        _rb = GetComponent<Rigidbody2D>();
        _inputPlayer = GetComponent<PlayerInput>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _animator= GetComponent<Animator>();

        _jumpAction = _inputPlayer.actions["Jump"];
        _dashAction = _inputPlayer.actions["Dash"];
        _crouchAction = _inputPlayer.actions["Crouch"];
        _pauseAction = _inputPlayer.actions["Pause"];
        _resistence = _maxResistence;
    }

    // Start is called before the first frame update
    void Start()
    {
        nowSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        SetAnimation();
        if (!_gameManager.GetBeginPlay()) return;
        
        IsGrounded();
        IsCrouch();
        IsDash();
        SetPause();
    }

    void SetAnimation()
    {
        _animator.SetBool("Run", _gameManager.GetBeginPlay());
        _animator.SetBool("NotGrounded" , !GetGrounded());
    }

    public void PlayParticule()
    {
        if(_dead) return;
        _dead = true;
        _gameManager.PlayDeadSound();
        Vector3 Loc = this.transform.position;
        Loc.y -= 0.4f;
        Instantiate(_deadParticle , Loc , Quaternion.identity);
    }

    void IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(_capsuleCollider2D.bounds.center, Vector2.down, _capsuleCollider2D.bounds.extents.y + .01f, _layerMask);
        Debug.DrawRay(_capsuleCollider2D.bounds.center, Vector2.down * (_capsuleCollider2D.bounds.extents.y + .01f));

        if (_jumpAction.triggered && GetGrounded() && !_crouchAction.IsPressed())
        {
            _gameManager.PlayActionSound();
            _rb.AddForce(transform.up * _jumpForce);
            isInDash = false;
            speed = nowSpeed;
        }

    }

    void IsCrouch()
    {
        if(_crouchAction.IsPressed() && GetGrounded() && Time.timeScale == 1)
        {
            if(_crouchAction.triggered)
            {
                _gameManager.PlayActionSound();
            }

            transform.localScale = new Vector3(transform.localScale.x, 0.5f, transform.localScale.z );
            _capsuleCollider2D.size = new Vector2(0.75f, 0.75f);
            transform.position = new Vector3(transform.position.x, -3.45f, transform.position.z);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.75f, transform.localScale.z );
            _capsuleCollider2D.size = new Vector2(1.50f, 1.50f);
        }

        if(!_jumpAction.IsPressed() && GetGrounded() && !_crouchAction.IsPressed())
        {
            transform.position = new Vector3(transform.position.x, -3.17545748f, transform.position.z); 
        }
    }

    void IsDash()
    {
        if(_dashAction.IsPressed() && GetGrounded() && !_crouchAction.IsPressed() && !empty)
        {
            if (_dashAction.triggered)
            {
                _gameManager.PlayActionSound();
            }
            isInDash = true;
            speed = nowSpeed + 1;
            _resistence -= 50 * Time.deltaTime;
            _resistence = Mathf.Clamp(_resistence, 0, _maxResistence);

            if(_resistence <= 0)
            {
                empty = true;
                _imageFill.GetComponent<Image>().color = new Color32(239, 84, 79 , 200);
            }

        }
        else
        {
            isInDash = false;
            speed = nowSpeed;
            _resistence += 50 * Time.deltaTime;
            _resistence = Mathf.Clamp(_resistence, 0, _maxResistence);

            if (_resistence >= _maxResistence)
            {
                empty = false;
                _imageFill.GetComponent<Image>().color = new Color32(137, 196, 251,200);
            }
        }
    }

    public void SetPause()
    {
       if(_pauseAction.IsPressed() && GetGrounded() && !GetCrouch() && !GetDash())
       {
            _gameManager.SetPause();
       }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public bool GetCrouch()
    {
        return _crouchAction.IsPressed();
    }

    public bool GetGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(_capsuleCollider2D.bounds.center, Vector2.down, _capsuleCollider2D.bounds.extents.y + .01f, _layerMask);

        return hit;
    }

    public bool GetDash()
    {
        return _dashAction.IsPressed();
    }

    public float GetPercentResistence()
    {
        return _resistence / _maxResistence;
    }

    public bool GetEmpty()
    {
        return empty;
    }

}
