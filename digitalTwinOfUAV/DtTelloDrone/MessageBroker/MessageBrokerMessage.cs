using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.Model.Agent;

public struct MessageBrokerMessage
{
    private TelloAction _action;
    
    public MessageBrokerMessage(TelloAction message)
    {
        _action = message;
    }

    public TelloAction GetAction()
    {
        return _action;
    }
}