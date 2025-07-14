public class AirborneState : CompositeState<CharacterContext>
{
    public override void Enter(CharacterContext context)
    {
        context.Motor.FallStartHeight = context.References.Transform.position.y;
        base.Enter(context);
    }

    public override void Exit(CharacterContext context)
    {
        context.Sensor.LastGroundedHeight = context.References.Transform.position.y;
        base.Exit(context);
    }
}
