using System.Dynamic;
using System.Runtime.Serialization;
using Game.Enemies;
using Game.Items;

namespace Game.Types;

class Room
{
    public List<IEnemy> Enemies { get; set; } = new List<IEnemy>();
    public List<IItem> Rewards { get; set; } = new List<IItem>();

    public string Name { get; set; } = string.Empty;
    public string Lore { get; set; } = string.Empty;
    // Flags
    public bool IsBossRoom { get; set; } = false;
    public bool ShowGeneratedInfo { get; set; } = true;
    
    public Delegate OnEnterAction { get; set; }
    public Delegate LoopAction { get; set; }
    public Delegate OnLeaveAction { get; set; }

    public Room(Delegate OnEnterAction, Delegate LoopAction, Delegate OnLeaveAction)
    {
        this.OnEnterAction = OnEnterAction;
        this.LoopAction = LoopAction;
        this.OnLeaveAction = OnLeaveAction;
    }
}