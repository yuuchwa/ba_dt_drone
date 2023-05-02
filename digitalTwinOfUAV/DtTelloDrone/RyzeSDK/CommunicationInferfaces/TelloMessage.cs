using System;
using System.Collections.Generic;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.RyzeSDK.CommunicationInferfaces;

public class TelloMessage
{
    private readonly TelloTopic _topic;
    private readonly MessageSender _source;
    private readonly Tuple<TelloAction, string> _command;

    public TelloMessage(TelloTopic topic, MessageSender source, Tuple<TelloAction, string> command)
    {
        _topic = topic;
        _source = source;
        _command = command;
    }

    public TelloTopic GetTopic()
    {
        return _topic;
    }

    public MessageSender GetSource()
    {
        return _source;
    }

    public Tuple<TelloAction, string> GetCommand()
    {
        return _command;
    }
}