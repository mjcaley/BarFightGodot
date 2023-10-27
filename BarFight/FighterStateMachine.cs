using Godot;

namespace BarFight;

public partial class FighterStateMachine : Node
{
    private FighterState _state = FighterState.Idle;
    public FighterState State
    {
        get
        {
            return _state;
        }
        private set
        {
            _state = value;
            EmitSignal(SignalName.StateChanged, Variant.From(State));
        }
    }

    [Signal]
    public delegate void StateChangedEventHandler(FighterState state);

    #region Attack

    [Export(hintString: "Time to recover in seconds")]
    public float AttackRecoveryTime { get; set; } = .5f;
    
    private Godot.Timer _attackRecoverTimer;

    public void Attack() => _attackRecoverTimer.Start(AttackRecoveryTime);

    private void OnAttackRecoveryTimeout()
    {
        State = FighterState.Idle;
    }

    #endregion

    #region Hit

    [Export(hintString: "Time to recover from being hit (seconds)")]
    public float HitRecoveryTime { get; set; } = 0.2f;

    private Godot.Timer _hitTimer;

    public void Hit()
    {
        _attackRecoverTimer.Stop();
        State = FighterState.Hit;
        _hitTimer.Start(HitRecoveryTime);
    }

    private void OnHitRecoveryTimeout()
    {

    }

    #endregion


    public override void _Ready()
    {
        base._Ready();

        _attackRecoverTimer = new Godot.Timer()
        {
            OneShot = true
        };
        _attackRecoverTimer.Timeout += OnAttackRecoveryTimeout;
        AddChild(_attackRecoverTimer);

        _hitTimer = new Godot.Timer()
        {
            OneShot = true
        };
        _hitTimer.Timeout += OnHitRecoveryTimeout;
        AddChild(_hitTimer);

        State = FighterState.Idle;
    }
}
