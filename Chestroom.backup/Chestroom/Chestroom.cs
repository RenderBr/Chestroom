using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;


namespace ChestroomPlugin
{

    public class Chestroom
    {
        static Random rnd = new Random();
        public static int[] ExcludedItems = new int[] { 0, 2772, 2773, 2774, 2775, 2777, 2778, 2779, 2780, 2782, 2783, 2784, 2785, 2881, 3273, 3340, 3341, 3342, 3343, 3344, 3345, 3346, 3462, 3463, 3464, 3465, 3480, 3481, 3482, 3483, 3484, 3485, 3486, 3487, 3488, 3489, 3490, 3491, 3492, 3493, 3494, 3495, 3496, 3497, 3498, 3499, 3500, 3501, 3502, 3503, 3504, 3505, 3506, 3507, 3508, 3509, 3510, 3511, 3512, 3513, 3514, 3515, 3516, 3517, 3518, 3519, 3520, 3521 }; //67
        static short[] torchframey = new short[] { 0, 22, 44, 66, 88, 110, 132, 154, 176, 198, 220, 242 };
        static short[] platformframey = new short[] { 0, 18, 36, 54, 72, 90, 108, 144, 234, 228 };
        static byte[] tiles = new byte[] { 38, 39, 41, 43, 44, 45, 47, 54, 118, 119, 121, 122, 140, 145, 146, 148, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 175, 176, 177, 189, 190, 191, 193, 194, 195, 196, 197, 198, 202, 206, 208, 225, 226, 229, 230, 248, 249, 250, 251, 252, 253 };
        static byte[] walls = new byte[] { 4, 5, 6, 7, 9, 10, 11, 12, 19, 21, 22, 23, 24, 25, 26, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 41, 42, 43, 45, 46, 47, 72, 73, 74, 75, 76, 78, 82, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 109, 110, 113, 114, 115 };
        static short[] chests = new short[] { 1, 3, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 };

        public static int MaxItems { get { return Main.maxItemTypes + 47; } }
        public static int ActualMaxItems { get { return MaxItems - ExcludedItems.Length; } }
        public static int MaxChests { get { return (int)Math.Ceiling((decimal)ActualMaxItems / 40); } }
       public  static int ChestsPerRow { get { return main.config.ChestsPerRow; } }
        public static int MaxRows { get { return (int)Math.Ceiling((decimal)MaxChests / ChestsPerRow); } }
       public  static int RowWidth { get { return 4 * ChestsPerRow + 2; } }
       public  static int RowHeight { get { return MaxRows * 5 + 1; } }


        public byte TileId;
        public short ChestId;
        public byte BackWall;
        public short PlatformFrameY;
        public short TorchFrameY;

        public Chestroom(bool custom)
        {
            TileId = custom ? main.config.TileId : tiles[rnd.Next(0, tiles.Length)];
            ChestId = custom ? main.config.ChestId : chests[rnd.Next(0, chests.Length)];
            BackWall = custom ? main.config.BgWall : walls[rnd.Next(0, walls.Length)];
            PlatformFrameY = custom ? main.config.pFrameY : platformframey[rnd.Next(0, platformframey.Length)];
            TorchFrameY = custom ? main.config.tFrameY : torchframey[rnd.Next(0, torchframey.Length)];
        }


        public bool Build(TSPlayer tsPlayer, int X, int Y)
        {
            int count = 0;
            for (int ch = 0; ch < 1000; ch++)
            {
                if (Main.chest[ch] != null)
                    count++;
            }
            if (count > (1000 - MaxChests))
            {
                tsPlayer.SendInfoMessage("Creating this chestroom would exceed the chest limit, chestroom cancelled.");
                return false;
            }
            bool first = true;
            int placed = ChestsPerRow * MaxRows;
            int chestitem = Main.maxItemTypes - 1;


            for (int y = RowHeight - 1; y >= 0; y--)
            {
                for (int x = RowWidth - 1; x >= 0; x--)
                {
                    if (x > 0 && x < RowWidth - 1 && y > 0 && y < RowHeight - 1)
                    {
                        Main.tile[x + X, y + Y] = new Tile() { wall = BackWall };
                    }
                    if (x == 0 || x == RowWidth - 1 || y == 0 || y == RowHeight - 1)
                    {
                        Main.tile[x + X, y + Y] = new Tile() { type = TileId };
                        Main.tile[x + X, y + Y].active(true);
                    }
                    if (y % 5 == 0 && y > 0 && y < RowHeight - 1)
                    {
                        Main.tile[x + X, y + Y].active(true);
                        if ((x % 4 == 2 || x % 4 == 3 || x == 1 || x == RowWidth - 2))
                        {
                            Main.tile[x + X, y + Y].type = TileId;
                        }
                        else if ((x % 4 == 0 || x % 4 == 1) && x > 1 && x < RowWidth - 2)
                        {
                            Main.tile[x + X, y + Y].type = (byte)19;
                            Main.tile[x + X, y + Y].frameY =PlatformFrameY;
                        }
                    }
                    if (y % 5 == 1 && (x == 1 || x == RowWidth - 2))
                    {
                        Main.tile[x + X, y + Y].active(true);
                        Main.tile[x + X, y + Y].type = 4;
                        Main.tile[x + X, y + Y].frameY = TorchFrameY;
                    }
                    if (y % 5 == 2 && x % 4 == 3 && x > 0 && x < RowWidth - 2)
                    {
                        placed--;
                        if (placed < MaxChests)
                        {
                            WorldGen.AddBuriedChest(x + X, y + Y, 1, false, ChestId);
                            if (Main.chest[count] != null)
                            {
                                for (int i = 39; i >= 0; i--)
                                {
                                    while(Utils.ExcludeItem(chestitem))
                                        chestitem--;

                                    if (chestitem < -48)
                                        break;

                                    if (first)
                                    {
                                        i -= 40 - (ActualMaxItems + 1) % 40;
                                        first = false;
                                    }
                                    Item itm = TShock.Utils.GetItemById(chestitem);
                                    itm.stack = itm.maxStack;
                                    Main.chest[count].item[i] = itm;
                                    chestitem--;
                                }
                            }
                            count++;
                        }
                    }
                }
            }
            if (main.usinginfchests)
                Utils.ConvertToAutoRefill(count - MaxChests, MaxChests);
            return true;
        }
    }
}
