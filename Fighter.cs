namespace BarFight;

using Godot;
using System;
using System.Collections.Generic;


public partial class Fighter : CharacterBody2D
{
    private FighterState State { get; set; } = FighterState.Idle;

    private Vector2 _direction = new();

    private Area2D _attackBody;
    private readonly HashSet<Area2D> _attackedBodies = new();

    [Export(hintString: "Time to recover in seconds")]
    public float AttackRecoveryTime { get; set; } = .5f;
    private Timer _attackRecover;

    [Export]
    public int Speed { get; set; } = 100;

    [Export]
    public int Health { get; set; } = 100;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _attackBody = GetNode<Area2D>("Attack");
        _attackBody.AreaEntered += OnBodyEntered;
        _attackBody.AreaExited += OnBodyExited;

        _attackRecover = new()
        {
            OneShot = true,
            WaitTime = AttackRecoveryTime
        };
        AddChild(_attackRecover);
        _attackRecover.Timeout += OnAttackRecovered;
    }

    private void OnBodyEntered(Area2D body)
    {
        _attackedBodies.Add(body);
        #if DEBUG
        GD.Print($"Entering body {body}");
        #endif
    }

    private void OnBodyExited(Area2D body)
    {
        _attackedBodies.Remove(body);
        #if DEBUG
        GD.Print($"Exiting body {body}");
        #endif
    }

    private void OnAttackRecovered()
    {
        State = FighterState.Idle;
        GD.Print("Attack recovered");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _direction = Vector2.Zero;
        if (Input.IsActionPressed("move_up"))
            _direction.Y = -1;
        if (Input.IsActionPressed("move_down"))
            _direction.Y = 1;
        if (Input.IsActionPressed("move_left"))
            _direction.X = -1;
        if (Input.IsActionPressed("move_right"))
            _direction.X = 1;
        _direction = _direction.Normalized();

        Position += _direction * Speed * (float)delta;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (State == FighterState.Idle && Input.IsActionPressed("attack"))
        {
            State = FighterState.Punching;
            GD.Print("Fighter punching");
            _attackRecover.Start();
            GD.Print(_attackRecover.IsStopped());

            foreach (var body in _attackedBodies)
            {
                GD.Print($"Area overlapping: {body.Name}");
            }
        }
    }
}
