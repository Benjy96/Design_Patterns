/**
Command Query Responsibility Segregation

Command - change state of app
Query - read state of app

Basically: Split requests into commands & queries
Why bother? Separation of Concerns.
*/

using System.Diagnostics;
using Xunit;

namespace CQRS.Tests;

public class ChatRoomTest
{
    private readonly IMediator _mediator = new Mediator();
    private readonly TestMessageWriter _bobMessageWriter = new();
    private readonly TestMessageWriter _aliceMessageWriter = new();
    private readonly TestMessageWriter _numptyMessageWriter = new();

    private readonly IChatRoom _room1 = new ChatRoom("Room 1");
    private readonly IChatRoom _room2 = new ChatRoom("Room 2");

    private readonly IParticipant _bob;
    private readonly IParticipant _alice;
    private readonly IParticipant _numpty;

    public ChatRoomTest()
    {
        _mediator.Register(new JoinChatRoom.Handler());
        _mediator.Register(new LeaveChatRoom.Handler());
        _mediator.Register(new SendChatMessage.Handler());
        _mediator.Register(new ListParticipants.Handler());
        _mediator.Register(new ListMessages.Handler());

        _bob = new Participant(_mediator, "Bob", _bobMessageWriter);
        _alice = new Participant(_mediator, "Alice", _aliceMessageWriter);
        _numpty = new Participant(_mediator, "Numpty", _numptyMessageWriter);
    }

    [Fact]
    public void A_participant_should_be_able_to_list_the_participants_that_joined_a_chatroom()
    {
        _bob.Join(_room1);
        _alice.Join(_room1);

        _bob.Join(_room2);
        _numpty.Join(_room2);

        var room1Participants = _bob.ListParticipantsOf(_room1);
        
        Assert.Collection(room1Participants,
            p => Assert.Same(_bob, p),
            p => Assert.Same(_alice, p)
        );
    }

    [Fact]
    public void A_participant_should_be_able_to_send_chat_messages_and_others_should_be_able_to_retrives_them_after_joining()
    {
        _bob.Join(_room1);
        _bob.SendMessageTo(_room1, "Hello!");
        _alice.Join(_room1);

        var messages = _alice.ListMessagesOf(_room1);
        Assert.Collection(messages,
            message =>
            {
                Assert.Same(_bob, message.Sender);
                Assert.Equal("Hello!", message.Message);
            }
        );
    }

    [Fact]
    public void A_participant_should_receive_new_messages()
    {
        _bob.Join(_room1);
        _alice.Join(_room1);
        _alice.Join(_room2);
        _bob.SendMessageTo(_room1, "Hello!");

        Assert.Collection(_aliceMessageWriter.Output,
            line =>
            {
                Assert.Equal(_room1, line.chatRoom);
                Assert.Equal(_bob, line.message.Sender);
                Assert.Equal("Hello!", line.message.Message);
            }
        );
    }

    [Fact]
    public void A_participant_should_not_receive_messages_after_leaving_a_chatroom()
    {
        _bob.Join(_room1);
        _alice.Join(_room1);
        _numpty.Join(_room1);
        _bob.SendMessageTo(_room1, "Hello!");
        _alice.Leave(_room1);
        _bob.SendMessageTo(_room1, "This should not reach Garner");

        Assert.Collection(_aliceMessageWriter.Output,
            line =>
            {
                Assert.Equal(_room1, line.chatRoom);
                Assert.Equal(_bob, line.message.Sender);
                Assert.Equal("Hello!", line.message.Message);
            }
        );
        Assert.Collection(_numptyMessageWriter.Output,
            line =>
            {
                Assert.Equal(_room1, line.chatRoom);
                Assert.Equal(_bob, line.message.Sender);
                Assert.Equal("Hello!", line.message.Message);
            },
            line =>
            {
                Assert.Equal(_room1, line.chatRoom);
                Assert.Equal(_bob, line.message.Sender);
                Assert.Equal("This should not reach Garner", line.message.Message);
            }
        );
    }

    [Fact]
    public void Another_component_could_send_commands_using_the_mediator()
    {
        _bob.Join(_room1);
        _alice.Join(_room1);
        _alice.Join(_room2);
        _mediator.Send(new SendChatMessage.Command(_room1, new ChatMessage(_bob, "Hello!")));

        Assert.Collection(_aliceMessageWriter.Output,
            line =>
            {
                Assert.Equal(_room1, line.chatRoom);
                Assert.Equal(_bob, line.message.Sender);
                Assert.Equal("Hello!", line.message.Message);
            }
        );
    }

    [Fact]
    public void Multiple_handlers_can_react_to_one_command()
    {
        var moderatorChatRoom = new ChatRoom("Moderator Room");
        var moderator = new ChatModerator(_mediator, moderatorChatRoom, new string[] { "bad" });
        _mediator.Register(moderator);

        _bob.Join(_room1);
        _alice.Join(_room1);
        _alice.Join(_room2);
        _bob.SendMessageTo(_room1, "Hello bad!");
        _alice.SendMessageTo(_room2, "bad Hello!");

        var messages = moderatorChatRoom.ListMessages();
        Assert.Collection(messages,
            message =>
            {
                Assert.Equal(moderator, message.Sender);
                Assert.StartsWith("Bad word found", message.Message);
            },
            message =>
            {
                Assert.Equal(moderator, message.Sender);
                Assert.StartsWith("Bad word found", message.Message);
            }
        );

    }

    private sealed class ChatModerator : Participant, IParticipant, ICommandHandler<SendChatMessage.Command>
    {
        private readonly IEnumerable<string> _badWords;
        private readonly IMediator _mediator;
        private readonly IChatRoom _moderatorChatRoom;

        public ChatModerator(IMediator mediator, IChatRoom moderatorChatRoom, IEnumerable<string> badWords)
            : base(
                mediator: mediator,
                name: nameof(ChatModerator),
                messageWriter: new EmptyMessageWriter()
            )
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _moderatorChatRoom = moderatorChatRoom ?? throw new ArgumentNullException(nameof(moderatorChatRoom));
            _badWords = badWords ?? throw new ArgumentNullException(nameof(badWords));
        }

        public void Handle(SendChatMessage.Command command)
        {
            if (command.Message.Sender == this || command.ChatRoom == _moderatorChatRoom)
            {
                return;
            }

            var containsAnyBadWord = _badWords.Any(x => command.Message.Message.Contains(x));
            if (containsAnyBadWord)
            {
                var message = $"Bad word found in message sent by '{command.Message.Sender.Name}' on '{command.Message.Date}' in room '{command.ChatRoom.Name}'\n> {command.Message.Message}";
                _mediator.Send(new SendChatMessage.Command(
                    _moderatorChatRoom,
                    new ChatMessage(this, message)
                ));
            }
        }

        private class EmptyMessageWriter : IMessageWriter
        {
            public void Write(IChatRoom chatRoom, ChatMessage message) { }
        }
    }

    /// <summary>
    /// Stores Message & Chatroom pairs in a List
    /// </summary>
    private class TestMessageWriter : IMessageWriter
    {
        public List<(IChatRoom chatRoom, ChatMessage message)> Output { get; } = new();

        public void Write(IChatRoom chatRoom, ChatMessage message)
        {
            // TODO: Console output in test?
            Console.WriteLine($"({nameof(TestMessageWriter)}) - ChatRoom {chatRoom}: {message}");
            Output.Add((chatRoom, message));
        }
    }
}