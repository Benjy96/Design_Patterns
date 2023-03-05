namespace CQRS;

/// <summary>
/// Contains Command & Handler for joining a chat room
/// </summary>
public class JoinChatRoom
{
    /// <summary>
    /// Stores JoinChatRoom Command Info: relevant Chatroom & Requester
    /// </summary>
    public class Command : ICommand
    {
        public IChatRoom ChatRoom { get; }
        public IParticipant Requester { get; }
        
        public Command(IChatRoom chatRoom, IParticipant requester)
        {
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
            Requester = requester ?? throw new ArgumentNullException(nameof(requester));
        }
    }

    /// <summary>
    /// Takes in a JoinChatRoom Command (info about chatroom & requester) and uses that Command's info
    /// to add the requester to the chatroom
    /// </summary>
    public class Handler : ICommandHandler<Command>
    {
        public void Handle(Command command)
        {
            command.ChatRoom.Add(command.Requester);
        }
    }
}

/// <summary>
/// Contains Command & Handler for leaving a chat room
/// </summary>
public class LeaveChatRoom
{
    /// <summary>
    /// Stores LeaveChatRoom Command info: Relevant Chatroom & requester
    /// </summary>
    public class Command : ICommand
    {
        public IChatRoom ChatRoom { get; }
        public IParticipant Requester { get; }
        public Command(IChatRoom chatRoom, IParticipant requester)
        {
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
            Requester = requester ?? throw new ArgumentNullException(nameof(requester));
        }
    }

    /// <summary>
    /// Takes in a LeaveChatRoom Command (info about chatroom & requester) and uses that context to remove requester from the room
    /// </summary>
    public class Handler : ICommandHandler<Command>
    {
        public void Handle(Command command)
        {
            command.ChatRoom.Remove(command.Requester);
        }
    }
}

/// <summary>
/// Contains Command & Handler for sending a chat message
/// </summary>
public class SendChatMessage
{
    /// <summary>
    /// Stores SendChatMessage Command info: Relevant Chatroom & message
    /// </summary>
    public class Command : ICommand
    {
        public IChatRoom ChatRoom { get; }
        public ChatMessage Message { get; }
        public Command(IChatRoom chatRoom, ChatMessage message)
        {
            ChatRoom = chatRoom ?? throw new ArgumentNullException(nameof(chatRoom));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }

    /// <summary>
    /// Takes in a SendChatMessage Command (info about the chatroom & message) 
    /// and uses that to send the Command's message to everyone in the chatroom
    /// </summary>
    public class Handler : ICommandHandler<Command>
    {
        public void Handle(Command command)
        {
            command.ChatRoom.Add(command.Message);
            foreach (var participant in command.ChatRoom.ListParticipants())
            {
                participant.NewMessageReceivedFrom(command.ChatRoom, command.Message);
            }
        }
    }
}