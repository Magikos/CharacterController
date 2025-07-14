using UnityEngine;

public class ErrorState : BaseState<CharacterContext>
{
    private readonly string _errorMessage;

    public ErrorState(string errorMessage = "Unknown state machine error")
    {
        _errorMessage = errorMessage;
    }

    public override bool IsBlocking => true; // Prevents further transitions

    public override void Enter(CharacterContext context)
    {
        Logwin.LogError("[ErrorState]", $"Entered ErrorState: {_errorMessage}");
    }

    public override void Update(CharacterContext context)
    {
        Debug.LogError($"ErrorState Update: {_errorMessage}");
        base.Update(context);
    }
}