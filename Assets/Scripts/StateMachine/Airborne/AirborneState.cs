public class AirborneState : CompositeState<CharacterContext>
{
    public override void Enter(CharacterContext context)
    {
        var initialState = AirborneTransitionBuilder.PickMovementState(context);
        StateMachine.SwitchState(context, initialState, true);
        Logwin.Log("[Airborne]", $"Entering with state: {initialState.Name}");
        base.Enter(context);
    }
}
