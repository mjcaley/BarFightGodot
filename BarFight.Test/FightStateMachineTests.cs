namespace BarFight.Test;

public class UnitTest1
{
    [Fact]
    public void TestAttack()
    {
        var machine = new FighterStateMachine();

        var changed = false;
        var state = FighterState.Idle;
        machine.StateChanged += (newState) => { changed = true; state = newState; };

        Assert.True(changed);
        Assert.Equal(FighterState.Idle, state);
    }
}
