using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.Agent;

public struct CoreMessage
{
    private TelloAction _action;
    
    CoreMessage(TelloAction message)
    {
        _action = message;
    }

    public TelloAction GetAction()
    {
        return _action;
    }
}