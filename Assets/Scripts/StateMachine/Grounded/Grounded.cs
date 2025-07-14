public class Grounded : CompositeState<CharacterContext>
{
    public override void Enter(CharacterContext context)
    {
        var initialState = GroundedTransitionBuilder.PickMovementState(context);
        StateMachine.SetInitialState(context, initialState);
        Logwin.Log("[Grounded]", $"Entering with state: {initialState.Name}");
        base.Enter(context);
    }
}
