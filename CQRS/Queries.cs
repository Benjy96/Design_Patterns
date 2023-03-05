namespace CQRS;

/// <summary>
/// Contains a Query (contextual info) & Handler (user of context) for listing participants
/// </summary>
public class ListParticipants
{
    /// <summary>
    /// Contains info about requester & the chatroom
    /// </summary>
    public class Query : IQuery<IEnumerable<IParticipant>>
    {
        public IParticipant Requester { get; }
        public IChatRoom ChatRoom { get; }
        public Query(IChatRoom chatRoom, IParticipant requester)
        {
            Requester = requester ?? throw new ArgumentNullException(nameof(requester));
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
        }
    }

    /// <summary>
    /// Takes in a ListParticipantsQuery (info about requester/chatroom) and uses that "context" to do something
    /// </summary>
    public class Handler : IQueryHandler<Query, IEnumerable<IParticipant>>
    {
        public IEnumerable<IParticipant> Handle(Query query)
        {
            return query.ChatRoom.ListParticipants();
        }
    }
}

/// <summary>
/// Contains a Query (contextual info) & Handler (user of context) for listing messages
/// </summary>
public class ListMessages
{
    /// <summary>
    /// Contains requester & chatroom (context)
    /// </summary>
    public class Query : IQuery<IEnumerable<ChatMessage>>
    {
        public IParticipant Requester { get; }
        public IChatRoom ChatRoom { get; }
        public Query(IChatRoom chatRoom, IParticipant requester)
        {
            Requester = requester ?? throw new ArgumentNullException(nameof(requester));
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
        }
    }

    /// <summary>
    /// Uses context (requester & chatroom) contained in the Query to list messages
    /// </summary>
    public class Handler : IQueryHandler<Query, IEnumerable<ChatMessage>>
    {
        public IEnumerable<ChatMessage> Handle(Query query)
        {
            return query.ChatRoom.ListMessages();
        }
    }
}