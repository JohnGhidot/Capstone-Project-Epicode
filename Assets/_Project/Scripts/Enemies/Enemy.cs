using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyMoverKinematic))]
[RequireComponent(typeof(EnemyAttackMelee))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour
{
    private enum EnemyState { Idle, Chase, Attack, Dead }

    [Header("References")]
    [SerializeField] private EnemyArchetypeSO _archetype;
    [SerializeField] private Transform _player;
    [SerializeField] private Animator _anim;

    private EnemyMoverKinematic _mover;
    private EnemyAttackMelee _attack;

    [Header("Gravity")]
    [SerializeField] private float _gravityForce = -9.81f;
    [SerializeField] private float _terminalVelocity = -50f;

    private float _verticalSpeed;
    private bool _isGrounded;

    [Header("Level / Scaling")]
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private bool _useSceneIndexAsLevel = false;

    [Header("Per-Prefab Level Controls")]
    [SerializeField] private int _levelOffset = 0;
    [SerializeField] private bool _ignoreSceneLevel = false;
    [SerializeField] private int _fixedLevel = 1;

    private int _effectiveDamage;
    private float _effectiveMoveSpeed;
    private float _effectiveCooldown;

    [Header("Parry")]
    [SerializeField] private bool _canParry = true;
    [SerializeField] private float _parryCooldown = 2.0f;
    [SerializeField] private float _parryWindow = 0.30f;
    [SerializeField] private float _parryAngleTolerance = 70f;
    [SerializeField] private float _parryChancePerSecond = 0.35f;

    private float _nextParryTime = 0f;
    private float _parryActiveUntil = 0f;

    private float _nextAttackTime = 0f;
    private CharacterController _cc;

    private EnemyState _state = EnemyState.Idle;

    public bool IsParryActive
    {
        get
        {
            if (Time.time < _parryActiveUntil)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void Awake()
    {
        _mover = GetComponent<EnemyMoverKinematic>();
        _attack = GetComponent<EnemyAttackMelee>();
        _cc = GetComponent<CharacterController>();

        if (_anim == null)
        {
            _anim = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (_archetype == null)
        {
            return;
        }

        ApplyDifficulty();

        if (_player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                _player = p.transform;
            }
        }

        if (_player != null)
        {
            InitWithPlayer();
            _state = EnemyState.Chase;
        }
        else
        {
            _state = EnemyState.Idle;
        }
    }

    private void Update()
    {
        if (_state == EnemyState.Dead)
        {
            return;
        }

        if (_anim != null && _cc != null)
        {
            Vector3 v = _cc.velocity;
            v.y = 0f;
            float speed = v.magnitude;
            _anim.SetFloat("Speed", speed);
        }

        _isGrounded = _cc.isGrounded;

        if (_isGrounded == true && _verticalSpeed < 0f)
        {
            _verticalSpeed = -2f;
        }

        _verticalSpeed += _gravityForce * Time.deltaTime;

        if (_verticalSpeed < _terminalVelocity)
        {
            _verticalSpeed = _terminalVelocity;
        }

        if (_player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                _player = p.transform;
                InitWithPlayer();
                _state = EnemyState.Chase;
            }
            else
            {
                _state = EnemyState.Idle;
            }
        }

        switch (_state)
        {
            case EnemyState.Idle:
                {
                    MaybeOpenParryWindow();
                    break;
                }

            case EnemyState.Chase:
                {
                    _mover.Tick();

                    MaybeOpenParryWindow();

                    bool inRange = IsInAttackRange(_archetype.AttackRange);
                    if (inRange == true)
                    {
                        _state = EnemyState.Attack;
                    }
                    break;
                }

            case EnemyState.Attack:
                {
                    bool stillInRange = IsInAttackRange(_archetype.AttackRange);
                    if (stillInRange == false)
                    {
                        _state = EnemyState.Chase;
                        break;
                    }

                    MaybeOpenParryWindow();

                    if (Time.time >= _nextAttackTime)
                    {
                        bool facing = IsFacingTarget(_player, _archetype.AttackAngleTolerance);
                        if (facing == true)
                        {
                            _nextAttackTime = Time.time + _effectiveCooldown;

                            if (_anim != null)
                            {
                                _anim.SetTrigger("Attack");
                            }
                            else
                            {
                                _attack.PerformAttackWithWindupFallback(_archetype.WindupTime);
                            }
                        }
                    }
                    break;
                }
        }

        _cc.Move(new Vector3(0f, _verticalSpeed, 0f) * Time.deltaTime);
    }

    public void ApplyDifficulty()
    {
        if (_archetype == null)
        {
            return;
        }

        int effectiveLevel = GetEffectiveLevel();
        _archetype.EvaluateAtLevel(
            effectiveLevel,
            out _effectiveDamage,
            out _effectiveMoveSpeed,
            out _effectiveCooldown
        );
    }

    public void SetupForLevel(int level)
    {
        if (level < 1)
        {
            level = 1;
        }
        _currentLevel = level;

        ApplyDifficulty();

        float stopDistance = _archetype.AttackRange * 0.9f;
        if (stopDistance < 0.1f)
        {
            stopDistance = 0.1f;
        }
        _mover.SetMoveParams(_effectiveMoveSpeed, _archetype.TurnSpeed, stopDistance);

        if (_player != null)
        {
            _attack.SetupRuntime(
                _effectiveDamage,
                _effectiveCooldown,
                _archetype.WindupTime,
                _archetype.HitRadius,
                _archetype.AttackAngleTolerance,
                _player
            );
        }
    }

    private int GetEffectiveLevel()
    {
        if (_ignoreSceneLevel == true)
        {
            int lvlFixed = _fixedLevel;
            if (lvlFixed < 1)
            {
                lvlFixed = 1;
            }
            return lvlFixed;
        }

        int baseLevel = _currentLevel;
        if (baseLevel < 1)
        {
            baseLevel = 1;
        }

        int lvl = baseLevel + _levelOffset;

        if (lvl < 1)
        {
            return 1;
        }
        else
        {
            return lvl;
        }
    }

    private void MaybeOpenParryWindow()
    {
        if (_canParry == false)
        {
            return;
        }

        if (_player == null)
        {
            return;
        }

        if (Time.time < _nextParryTime)
        {
            return;
        }

        bool inMelee = IsInAttackRange(_archetype.AttackRange + 0.35f);
        if (inMelee == false)
        {
            return;
        }

        bool facing = IsFacingTarget(_player, _parryAngleTolerance);
        if (facing == false)
        {
            return;
        }

        float chanceThisFrame = _parryChancePerSecond * Time.deltaTime;
        float roll = Random.value;

        bool doParry = false;
        if (roll <= chanceThisFrame)
        {
            doParry = true;
        }

        if (doParry == true)
        {
            TryBeginParry();
        }
    }

    public bool TryBeginParry()
    {
        if (_canParry == false)
        {
            return false;
        }

        if (Time.time < _nextParryTime)
        {
            return false;
        }

        _parryActiveUntil = Time.time + _parryWindow;
        _nextParryTime = Time.time + _parryCooldown;

        if (_anim != null)
        {
            _anim.SetTrigger("Parry");
        }

        return true;
    }

    public void OnSuccessfullParry(GameObject source)
    {
        if (_anim != null)
        {
            _anim.SetTrigger("ParrySuccess");
        }
    }

    public bool WillParryIncomingFrom(GameObject source)
    {
        if (_canParry == false)
        {
            return false;
        }

        if (source == null)
        {
            return false;
        }

        if (IsParryActive == false)
        {
            return false;
        }

        bool facing = IsFacingTarget(source.transform, _parryAngleTolerance);
        if (facing == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsFacingTarget(Transform t, float toleranceDeg)
    {
        if (t == null)
        {
            return false;
        }

        Vector3 to = t.position - transform.position;
        to.y = 0f;

        if (to.sqrMagnitude < 0.0001f)
        {
            return true;
        }

        Vector3 forward = transform.forward;
        Vector3 toNorm = to.normalized;
        float angle = Vector3.Angle(forward, toNorm);

        if (angle <= toleranceDeg)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void OnDeath()
    {
        if (_state == EnemyState.Dead)
        {
            return;
        }

        _state = EnemyState.Dead;

        if (_anim != null)
        {
            _anim.SetTrigger("Die");
        }
    }


    private void InitWithPlayer()
    {
        _mover.SetTarget(_player);

        float stopDistance = _archetype.AttackRange * 0.9f;
        if (stopDistance < 0.1f)
        {
            stopDistance = 0.1f;
        }
        _mover.SetMoveParams(_effectiveMoveSpeed, _archetype.TurnSpeed, stopDistance);

        _attack.SetupRuntime(
            _effectiveDamage,
            _effectiveCooldown,
            _archetype.WindupTime,
            _archetype.HitRadius,
            _archetype.AttackAngleTolerance,
            _player
        );
    }

    private bool IsInAttackRange(float range)
    {
        if (_player == null)
        {
            return false;
        }

        Vector3 to = _player.position - transform.position;
        to.y = 0f;

        float sqr = to.sqrMagnitude;
        float threshold = range * range;

        if (sqr <= threshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
