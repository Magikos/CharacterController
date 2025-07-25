public class Grounded : CompositeState<CharacterContext>
{
    public override void Enter(CharacterContext context)
    {
        var initialState = GroundedTransitionBuilder.PickMovementState(context);
        StateMachine.SwitchState(context, initialState, true);
        Logwin.Log("[Grounded]", $"Entering with state: {initialState.Name}");
        base.Enter(context);
    }
}
