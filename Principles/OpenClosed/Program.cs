using AttackStructures;

namespace Game;

public class Program
{
    public static void Main()
    {
        var ninja = new NinjaNoOCP.Ninja("The Blue Phantom");
        var target = new NinjaNoOCP.Ninja("The Unseen Mirage");

        ExecuteSequence(ninja, target);
        
        Console.WriteLine("*** SEQUENCE 1 COMPLETE ***");
        Console.WriteLine("*** SEQUENCE 2 ***\n");

        // We can now alter program behaviour without changing the actual class's code
        var ninja2 = new NinjaWithOCP.Ninja("The Blue Phantom", new Sword(), new Pistol());
        var target2 = new NinjaWithOCP.Ninja("The Unseen Mirage", new Sword(), new Pistol());

        ExecuteSequence(ninja, target);
    }

    static void ExecuteSequence<T>(T theBluePhantom, T theUnseenMirage) where T : IAttackable, IAttacker
    {
        // The Blue Phantom attacks The Unseen Mirage with a first attack
        var result = theBluePhantom.Attack(theUnseenMirage);
        PrintAttackResult(result);

        // The Unseen Mirage moves away from The Blue Phantom
        theUnseenMirage.MoveTo(5, 5);
        PrintMovement(theUnseenMirage);

        // The Blue Phantom attacks The Unseen Mirage with a second attack
        var result2 = theBluePhantom.Attack(theUnseenMirage);
        PrintAttackResult(result2);

        // The Unseen Mirage moves further away from The Blue Phantom
        theUnseenMirage.MoveTo(20, 20);
        PrintMovement(theUnseenMirage);

        // The Blue Phantom attacks The Unseen Mirage with a third attack
        var result3 = theBluePhantom.Attack(theUnseenMirage);
        PrintAttackResult(result3);

        // The Unseen Mirage strikes back at The Blue Phantom from a distance
        var result4 = theUnseenMirage.Attack(theBluePhantom);
        PrintAttackResult(result4);

        // Output
        void PrintAttackResult(AttackResult attackResult)
        {
            if (attackResult.Succeeded)
            {
                Console.WriteLine($"{attackResult.Attacker} hits {attackResult.Target} using {attackResult.Weapon} at distance {attackResult.Distance}!{Environment.NewLine}");
            }
            else
            {
                Console.WriteLine($"{attackResult.Attacker} misses {attackResult.Target} using {attackResult.Weapon} at distance {attackResult.Distance}...{Environment.NewLine}");
            }
        }
        void PrintMovement(IAttackable ninja)
        {
            Console.WriteLine($"{ninja.Name} moved to {ninja.Position}.{Environment.NewLine}");
        }
    }

}