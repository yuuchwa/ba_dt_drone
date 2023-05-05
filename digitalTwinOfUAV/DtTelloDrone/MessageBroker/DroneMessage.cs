using System;
using System.Collections.Generic;
using DtTelloDrone.MessageBroker;
using DtTelloDrone.Model.Attributes;
using DtTelloDrone.RyzeSDK.Attribute;

namespace DtTelloDrone.RyzeSDK.CommunicationInferfaces;

public class DroneMessage
{
    private readonly MessageTopic _topic;
    private readonly MessageSender _source;
    private readonly Tuple<DroneAction, string> _command;

    public DroneMessage(MessageTopic topic, MessageSender source, Tuple<DroneAction, string> command)
    {
        _topic = topic;
        _source = source;
        _command = command;
    }

    public MessageTopic GetTopic()
    {
        return _topic;
    }

    public MessageSender GetSource()
    {
        return _source;
    }

    public Tuple<DroneAction, string> GetCommand()
    {
        return _command;
    }
}