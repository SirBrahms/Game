using System.Reflection;

namespace Game.Items;

class ItemManager
{
    public static List<IItem> GetRandomItems()
    {
        var All = GetAllItems();
        List<IItem> Items = new List<IItem>();
        Random r = new Random();
        for (int i = 0; i < r.Next(2, 5); i++)
        {
            var CurrentType = All[r.Next(All.Count - 1)];

            var EnemyInstance = (IItem?)Activator.CreateInstance(CurrentType) ?? throw new Exception("Somehow got Null: GetRandomEnemy");
            Items.Add(EnemyInstance);
        }
        return Items;
    }

    public static List<Type> GetAllItems()
    {
        var RetList = new List<Type>();

        var InterfaceType = typeof(IItem);
        var asm = Assembly.GetAssembly(InterfaceType) ?? throw new Exception("Somehow got null");
        RetList = asm.GetTypes()
            .Where(p => InterfaceType.IsAssignableFrom(p)).ToList();

        RetList.Remove(InterfaceType);
        return RetList;
    }
}