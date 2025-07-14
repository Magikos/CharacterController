public class AirborneState : CompositeState<CharacterContext>
{
    public override void Enter(CharacterContext context)
    {
        var initialState = AirborneTransitionBuilder.PickMovementState(context);
        StateMachine.SetInitialState(context, initialState);
        Logwin.Log("[Airborne]", $"Entering with state: {initialState.Name}");
        base.Enter(context);
    }
}
