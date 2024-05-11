using System.Dynamic;
using System.Runtime.Serialization;
using Game.Enemies;
using Game.Items;

namespace Game.Types;

class Room
{
    public List<Type> Enemies { get; set; } = new List<Type>();
    public List<IItem> Rewards { get; set; } = new List<IItem>();

    public string Name { get; set; } = string.Empty;
    public string Lore { get; set; } = string.Empty;
    // Flags
    public bool IsBossRoom { get; set; } = false;
    public bool ShowGeneratedInfo { get; set; } = true;
    
    public Delegate EnterAction { get; set; }
    public Delegate LoopAction { get; set; }
    public Delegate LeaveAction { get; set; }

    public Room(Delegate EnterAction, Delegate LoopAction, Delegate LeaveAction)
    {
        this.EnterAction = EnterAction;
        this.LoopAction = LoopAction;
        this.LeaveAction = LeaveAction;
    }
}