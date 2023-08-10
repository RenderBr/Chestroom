using System.Text;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace ChestroomPlugin
{
    public static class Utils
    {
        public static void ConvertToAutoRefill(int s, int e)
        {
            var items = new StringBuilder();
            for (int i = s; i < s + e; i++)
            {
                Terraria.Chest c = Main.chest[i];
                if (c != null)
                {
                    for (int j = 0; j < 40; j++)
                        items.Append(c.item[j].netID).Append(",").Append(c.item[j].stack).Append(",").Append(c.item[j].prefix).Append(",");
                    main.Database.Query("INSERT INTO Chests (X, Y, Account, Items, Flags, WorldID) VALUES (@0, @1, '', @2, @3, @4)",
                        c.x, c.y, items.ToString(0, items.Length - 1), 5, Main.worldID);
                    items.Clear();
                    Main.chest[i] = null;
                }
            }
        }

        public static void informplayers(bool hard = false)
        {
            foreach (TSPlayer ts in TShock.Players)
            {
                if ((ts != null) && (ts.Active))
                {
                    for (int i = 0; i < 255; i++)
                    {
                        for (int j = 0; j < Main.maxSectionsX; j++)
                        {
                            for (int k = 0; k < Main.maxSectionsY; k++)
                            {
                                Netplay.Clients[i].TileSections[j, k] = false;
                            }
                        }
                    }
                }
            }
        }


        public static int ExcludedIndex = Chestroom.ExcludedItems.Length - 1;
        public static bool ExcludeItem(int id)
        {
            if(ExcludedIndex <0)
                ExcludedIndex = Chestroom.ExcludedItems.Length - 1;

            if (id == Chestroom.ExcludedItems[ExcludedIndex])
            {
                ExcludedIndex--;
                return true;
            }             
            return false;
        }
    }
}
