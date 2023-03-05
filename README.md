1. Need to link a new version of code through old code? -> ADAPTER
2. Need to have nested/traversable objects with similar properties? -> COMPOSITE
3. Need to transform objects into other types (while keeping logic centralised)? -> MAPPER
4. Need to control who gets told about something, and who tells them? -> MEDIATOR
5. Need to to let stakeholders have control over being updated upon something happening? -> OBSERVER
6. Need to choose different ways of doing something based on some logic/context? -> STRATEGY

# Analogies

## Adapter
Old, cobweb-covered server sitting humming in a dark room.
This old server can calculate your all bank transactions but shit UI.
New server implements new UI and most of everything except bank transactions.

You create an adapter class in the new server to call the old server for a couple things, but do everything else the new way.

## Composite
1 team with a name.
5 people in team, each with a name.

The object representing a team or a person is a Composite.

## Mediator
1 million Youtube channels.
Ben uploads a video.
YouTube HQ decides to send push notification to 50 randoms on homepage.

YouTube HQ is the Mediator.
Random Youtube watchers are the Clients.

## Observer
1 million Youtube channels.
Ben uploads a video.
Ben's 10 subscribers get notified by YouTube HQ.

Ben's subscribers are the Observers.
Ben's channel is the Subject.

## Strategy
You need to attack another country. IAttackStrategy interface defines Attack()
You could attack in multiple ways, so you make blueprint strategies:
- LandAttack : IAttackStrategy
- SeaAttack : IAttackStrategy
- AirAttack : IAttackStrategy

Your code will work with any IAttackStrategy, and depending on the enemy's defenses, you can inject any type of attack class, since they will all implement attack.