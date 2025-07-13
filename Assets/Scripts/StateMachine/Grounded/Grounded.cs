public class Grounded : CompositeState<CharacterContext>
{
    public override void Enter(CharacterContext context)
    {
        // If we're entering grounded and don't have a current state, 
        // use the existing GroundedTransitionBuilder logic to choose the appropriate state
        if (StateMachine.CurrentState == null)
        {
            var initialState = GroundedTransitionBuilder.PickMovementState(context);
            StateMachine.SetInitialState(context, initialState);
            Logwin.Log("[Grounded]", $"Entering with state: {initialState.Name}");
        }
        base.Enter(context);
    }
}
