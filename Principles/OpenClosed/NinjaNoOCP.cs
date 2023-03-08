using System.Numerics;
using AttackStructures;

namespace NinjaNoOCP;

/// <summary>
/// Problem: If we want to change weapons, we must MODIFY the class.
/// Open-Closed Principle: Open for extension, CLOSED for MODIFICATION.
/// </summary>
public class Ninja : IAttackable, IAttacker
{
    private readonly Weapon _sword = new Sword();
    private readonly Weapon _pistol = new Pistol();

    public string Name { get; }
    public Vector2 Position { get; set; }

    public Ninja(string name, Vector2? position = null)
    {
        Name = name;
        Position = position ?? Vector2.Zero;
    }

    public AttackResult Attack(IAttackable target)
    {
        var distance = this.DistanceFrom(target);   // extension method for IAttackable interface, of which a Ninja is (IAttackable)

        if(_sword.CanHit(distance))
        {
            return new AttackResult(_sword, this, target);
        }
        else
        {
            return new AttackResult(_pistol, this, target);
        }
    }
}