using System;
using DtTelloDrone.Model.Attributes;

namespace DtTelloDrone.MessageBroker;

/// <summary>
/// Represents a message sent between the ground stationg and the drone.
/// </summary>
public class DroneMessage
{
    private readonly MessageTopic _topic;
    private readonly MessageSender _source;
    private readonly Tuple<DroneAction, string> _command;

    /// <summary>
    /// Instantiate a DroneMessage
    /// </summary>
    /// <param name="topic">An enum representing the topic of the message.</param>
    /// <param name="source">An enum representingthe sender of the message.</param>
    /// <param name="command">A tuple containing a DroneAction enum representing the action to be executed
    /// and a string representing any additional parameters required for the action.</param>
    public DroneMessage(MessageTopic topic, MessageSender source, Tuple<DroneAction, string> command)
    {
        _topic = topic;
        _source = source;
        _command = command;
    }

    /// <summary>
    /// Returns the topic of the DroneMessage instance.
    /// </summary>
    /// <returns>The topic.</returns>
    public MessageTopic GetTopic()
    {
        return _topic;
    }

    /// <summary>
    /// Returns the source of the DroneMessage instance.
    /// </summary>
    /// <returns>The source.</returns>
    public MessageSender GetSource()
    {
        return _source;
    }

    /// <summary>
    /// Returns a tuple containinthe droneAction and a string representing a value.
    /// </summary>
    /// <returns>The topic.</returns>
    public Tuple<DroneAction, string> GetCommand()
    {
        return _command;
    }
}