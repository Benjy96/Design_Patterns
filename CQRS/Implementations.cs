/**
Command Query Responsibility Segregation

Command - change state of app
Query - read state of app

Basically: Split requests into commands & queries
Why bother? Separation of Concerns.
*/

namespace CQRS;

#region Mediator
public class Mediator : IMediator
{
    private readonly HandlerDictionary _handlers = new();

    public void Register<TCommand>(ICommandHandler<TCommand> commandHandler) where TCommand : ICommand
    {
        _handlers.AddHandler(commandHandler);
    }

    public void Register<TQuery, TReturn>(IQueryHandler<TQuery, TReturn> commandHandler) where TQuery : IQuery<TReturn>
    {
        _handlers.AddHandler(commandHandler);
    }

    public TReturn Send<TQuery, TReturn>(TQuery query)
        where TQuery : IQuery<TReturn>
    {
        var handler = _handlers.Find<TQuery, TReturn>();
        return handler.Handle(query);
    }

    public void Send<TCommand>(TCommand command)
        where TCommand : ICommand
    {
        var handlers = _handlers.FindAll<TCommand>();
        foreach (var handler in handlers)
        {
            handler.Handle(command);
        }
    }

    private class HandlerList
    {
        private readonly List<object> _commandHandlers = new();
        private readonly List<object> _queryHandlers = new();

        public void Add<TCommand>(ICommandHandler<TCommand> handler)
            where TCommand : ICommand
        {
            _commandHandlers.Add(handler);
        }

        public void Add<TQuery, TReturn>(IQueryHandler<TQuery, TReturn> handler)
            where TQuery : IQuery<TReturn>
        {
            _queryHandlers.Add(handler);
        }

        public IEnumerable<ICommandHandler<TCommand>> FindAll<TCommand>()
            where TCommand : ICommand
        {
            foreach (var handler in _commandHandlers)
            {
                if (handler is ICommandHandler<TCommand> output)
                {
                    yield return output;
                }
            }
        }
        public IQueryHandler<TQuery, TReturn> Find<TQuery, TReturn>()
            where TQuery : IQuery<TReturn>
        {
            foreach (var handler in _queryHandlers)
            {
                if (handler is IQueryHandler<TQuery, TReturn> output)
                {
                    return output;
                }
            }
            throw new QueryHandlerNotFoundException(typeof(TQuery));
        }
    }

    private class HandlerDictionary
    {
        private readonly Dictionary<Type, HandlerList> _handlers = new();

        public void AddHandler<TCommand>(ICommandHandler<TCommand> handler) where TCommand : ICommand
        {
            var type = typeof(TCommand);
            EnforceTypeEntry(type);
            var registeredHandlers = _handlers[type];
            registeredHandlers.Add(handler);
        }

        public void AddHandler<TQuery, TReturn>(IQueryHandler<TQuery, TReturn> handler)
            where TQuery : IQuery<TReturn>
        {
            var type = typeof(TQuery);
            EnforceTypeEntry(type);
            var registeredHandlers = _handlers[type];
            registeredHandlers.Add(handler);
        }

        public IEnumerable<ICommandHandler<TCommand>> FindAll<TCommand>()
            where TCommand : ICommand
        {
            var type = typeof(TCommand);
            EnforceTypeEntry(type);
            var registeredHandlers = _handlers[type];
            return registeredHandlers.FindAll<TCommand>();
        }

        public IQueryHandler<TQuery, TReturn> Find<TQuery, TReturn>() where TQuery : IQuery<TReturn>
        {
            var type = typeof(TQuery);
            EnforceTypeEntry(type);
            var registeredHandlers = _handlers[type];
            return registeredHandlers.Find<TQuery, TReturn>();
        }

        private void EnforceTypeEntry(Type type)
        {
            if (!_handlers.ContainsKey(type))
            {
                _handlers.Add(type, new HandlerList());
            }
        }
    }
}
#endregion

#region Chatroom
/// <summary>
/// Info about message, sender, and date
/// </summary>
public class ChatMessage
{
    public DateTime Date { get; }
    public IParticipant Sender { get; }
    public string Message { get; }
    public ChatMessage(IParticipant sender, string message)
    {
        Sender = sender ?? throw new ArgumentNullException(nameof(sender));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Date = DateTime.UtcNow;
    }
}

/// <summary>
/// Stores IParticipants and ChatMessages in Lists.
/// </summary>
public class ChatRoom : IChatRoom
{
    public string Name { get; }
    private readonly List<IParticipant> _participants = new();
    private readonly List<ChatMessage> _chatMessages = new();

    public ChatRoom(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void Add(IParticipant participant)
    {
        _participants.Add(participant);
    }

    public void Add(ChatMessage message)
    {
        _chatMessages.Add(message);
    }

    public IEnumerable<ChatMessage> ListMessages()
    {
        return _chatMessages.AsReadOnly();
    }

    public IEnumerable<IParticipant> ListParticipants()
    {
        return _participants.AsReadOnly();
    }

    public void Remove(IParticipant participant)
    {
        _participants.Remove(participant);
    }
}

/// <summary>
/// Uses a Mediator to send Commands & Queries to the ChatRoom
/// </summary>
public class Participant : IParticipant
{
    public string Name { get; }
    private readonly IMediator _mediator;
    private readonly IMessageWriter _messageWriter;

    public Participant(IMediator mediator, string name, IMessageWriter messageWriter)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        _messageWriter = messageWriter ?? throw new ArgumentNullException(nameof(messageWriter));
    }

    /// <summary>
    /// Uses a mediator to send a JoinChatRoom Command (modify state)
    /// </summary>
    public void Join(IChatRoom chatRoom)
    {
        _mediator.Send(new JoinChatRoom.Command(chatRoom, this));
    }

    /// <summary>
    /// Uses a mediator to send a LeaveChatRoom Command (modify state)
    /// </summary>
    public void Leave(IChatRoom chatRoom)
    {
        _mediator.Send(new LeaveChatRoom.Command(chatRoom, this));
    }

    /// <summary>
    /// Uses a mediator to send a ListMessages Query (read-only)
    /// </summary>
    public IEnumerable<ChatMessage> ListMessagesOf(IChatRoom chatRoom)
    {
        return _mediator.Send<ListMessages.Query, IEnumerable<ChatMessage>>(new ListMessages.Query(chatRoom, this));
    }

    /// <summary>
    /// Uses a mediator to send a ListParticipants Query (read-only)
    /// </summary>
    public IEnumerable<IParticipant> ListParticipantsOf(IChatRoom chatRoom)
    {
        return _mediator.Send<ListParticipants.Query, IEnumerable<IParticipant>>(new ListParticipants.Query(chatRoom, this));
    }

    /// <summary>
    /// Uses a message writing implementation to send a ChatMessage to an IChatRoom implementation
    /// </summary>
    public void NewMessageReceivedFrom(IChatRoom chatRoom, ChatMessage message)
    {
        _messageWriter.Write(chatRoom, message);
    }

    /// <summary>
    /// Uses a mediator to send a ChatMessage Command (modify state)
    /// </summary>
    public void SendMessageTo(IChatRoom chatRoom, string message)
    {
        _mediator.Send(new SendChatMessage.Command(chatRoom, new ChatMessage(this, message)));
    }
}
#endregion


public class QueryHandlerNotFoundException : Exception
{
    public QueryHandlerNotFoundException(Type queryType)
        : base($"No handler found for query '{queryType}'.")
    {
    }
}