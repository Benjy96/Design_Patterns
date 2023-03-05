namespace CQRS;

/// <summary>
/// Ensures implementer has a way to Register clients & Send them messages
/// </summary>
public interface IMediator
{
    TReturn Send<TQuery, TReturn>(TQuery query) where TQuery : IQuery<TReturn>;
    void Send<TCommand>(TCommand command) where TCommand : ICommand;
    void Register<TCommand>(ICommandHandler<TCommand> commandHandler) where TCommand : ICommand;
    void Register<TQuery, TReturn>(IQueryHandler<TQuery, TReturn> commandHandler) where TQuery : IQuery<TReturn>;
}

/// <summary>
/// Ensures implementer has a way to send messages to a chatroom
/// </summary>
public interface IMessageWriter
{
    void Write(IChatRoom chatRoom, ChatMessage message);
}

#region Chatroom
/// <summary>
/// Ensures join, leave, and messaging functionality
/// </summary>
public interface IParticipant
{
    string Name { get; }
    void Join(IChatRoom chatRoom);
    void Leave(IChatRoom chatRoom);
    void SendMessageTo(IChatRoom chatRoom, string message);
    void NewMessageReceivedFrom(IChatRoom chatRoom, ChatMessage message);
    IEnumerable<IParticipant> ListParticipantsOf(IChatRoom chatRoom);
    IEnumerable<ChatMessage> ListMessagesOf(IChatRoom chatRoom);
}

/// <summary>
/// Ensures participant management and messaging functionality
/// </summary>
public interface IChatRoom
{
    string Name { get; }

    // Participant related
    void Add(IParticipant participant);
    void Remove(IParticipant participant);
    IEnumerable<IParticipant> ListParticipants();

    // Message related
    void Add(ChatMessage message);
    IEnumerable<ChatMessage> ListMessages();
}
#endregion

#region Command & Query

/// <summary>
/// Command: Something which changes state of app
/// </summary>
public interface ICommand { }
/// <summary>
/// Ensures implementer is set up to handle any Command
/// </summary>
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    void Handle(TCommand command);
}

/// <summary>
/// Query: Something which reads the state of the app and doesn't change anything
/// </summary>
/// <typeparam name="TReturn">Some type of state information</typeparam>
public interface IQuery<TReturn> { }
/// <summary>
/// Ensures implementer is set up to handle any Query which returns something
/// </summary>
/// <typeparam name="TQuery">Type of query to take in</typeparam>
/// <typeparam name="TReturn">Type of data to return</typeparam>
public interface IQueryHandler<TQuery, TReturn> where TQuery : IQuery<TReturn>
{
    TReturn Handle(TQuery query);
}
#endregion