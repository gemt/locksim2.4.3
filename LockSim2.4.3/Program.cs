using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockSim2._4._3
{
    class Program
    {
        static List<(Item, Item)> RingPairs_Static = new List<(Item, Item)>();
        static List<(Item, Item)> TrinketPairs_Static = new List<(Item, Item)>();
        static Stat[] GloveEnchants = new Stat[] { new Stat(Stat.Hit, 15), new Stat(Stat.Sp, 20) };
        static long expectedCount = 0;
        static long processCount = 0;

        public class Worker
        {
            public Thread Thread { get; }
            double minDps = 9999999;
            int minIdx = 0;
            int charIdx = 0;
            (Item, Item) RingPair;

            public Character[] characters = new Character[100];
            List<Item>[] ItemBySlot;
            public Worker((Item,Item) ringPair, List<Item>[] ItemBySlot) {
                RingPair = ringPair;
                this.ItemBySlot = ItemBySlot;
                Thread = new Thread(Run);
                Thread.Start();
            }

            HashSet<string> charIdentifiers = new HashSet<string>();
            void Run() {
                Foo(new Item[12], Item.Head);
            }

            void Foo(Item[] selectedItems, int itemSlot) {
                var thisGroup = ItemBySlot[itemSlot];
                for (int i = 0; i < thisGroup.Count; i++) {
                    selectedItems[itemSlot] = thisGroup[i];

                    if (itemSlot == Item.Wep) {
                        foreach (var trinketPair in TrinketPairs_Static) {
                            foreach (var gloveEnchant in GloveEnchants) {
                                var character = new Character(selectedItems, RingPair.Item1, RingPair.Item2, trinketPair.Item1, trinketPair.Item2, gloveEnchant);
                                /*
                                if (!charIdentifiers.Add(character.Hash)) {
                                    throw new Exception();
                                }
                                */
                                if (charIdx < 100) {
                                    characters[charIdx] = character;
                                    if (character.DPS < minDps) {
                                        minDps = character.DPS;
                                        minIdx = charIdx;
                                    }
                                    charIdx++;
                                } else {
                                    if (character.DPS > minDps) {
                                        characters[minIdx] = character;
                                        double min = 99999;

                                        for (int z = 0; z < characters.Length; z++) {
                                            if (characters[z].DPS < min) {
                                                min = characters[z].DPS;
                                                minIdx = z;
                                            }
                                        }

                                        minDps = min;
                                    }
                                }
                                Interlocked.Increment(ref processCount);
                            }
                        }
                    } else {
                        Foo(selectedItems, itemSlot+1);
                    }
                }
            }
        }


        static void Main(string[] args) {
            var lines = File.ReadAllLines(@"C:\Users\G3m7\Documents\git\LockSim2.4.3\items2.csv");
            List<Item> items = new List<Item>();
            for(int i = 2; i < lines.Length; i++) {
                var line = lines[i]; 
                var stats = line.Split(';');
                var itemCombs = Create(stats);
                items.AddRange(itemCombs);
                /*
                Console.WriteLine();
                foreach(var itm in itemCombs) {
                    Console.WriteLine(itm.ToString());
                }
                */
            }
            /*
            foreach(var itm in items) {
                Console.WriteLine($"{itm.Name}");
            }
            */
            var asd = items.GroupBy(x => x.Id).Where(x => x.Count() > 1);
            var distinctItems = items.Select(x => x.Id).Distinct();
            var nonDistinct = items.Where(x => !distinctItems.Contains(x.Id));
            Debug.Assert(items.Select(x => x.Id).Distinct().Count() == items.Count);

            List<Item>[] ItemBySlot = new List<Item>[12];
            ItemBySlot[Item.Head] = items.Where(x => x.Slot == Item.Head).ToList();
            ItemBySlot[Item.Neck] = items.Where(x => x.Slot == Item.Neck).ToList();
            ItemBySlot[Item.Shoulder] = items.Where(x => x.Slot == Item.Shoulder).ToList();
            ItemBySlot[Item.Back] = items.Where(x => x.Slot == Item.Back).ToList();
            ItemBySlot[Item.Chest] = items.Where(x => x.Slot == Item.Chest).ToList();
            ItemBySlot[Item.Wrist] = items.Where(x => x.Slot == Item.Wrist).ToList();
            ItemBySlot[Item.Hand] = items.Where(x => x.Slot == Item.Hand).ToList();
            ItemBySlot[Item.Waist] = items.Where(x => x.Slot == Item.Waist).ToList();
            ItemBySlot[Item.Leg] = items.Where(x => x.Slot == Item.Leg).ToList();
            ItemBySlot[Item.Feet] = items.Where(x => x.Slot == Item.Feet).ToList();
            ItemBySlot[Item.Wand] = items.Where(x => x.Slot == Item.Wand).ToList();
            ItemBySlot[Item.Wep] = items.Where(x => x.Slot == Item.Wep).ToList();

            var fingers = items.Where(x => x.Slot == Item.Finger).ToList();
            var fingerCombos = new Combinations<Item>(fingers, 2);
            List<(Item, Item)> ringPairs = new List<(Item, Item)>();
            foreach (var c in fingerCombos) {
                RingPairs_Static.Add((c[0], c[1]));
            }

            var trinkets = items.Where(x => x.Slot == Item.Trinket).ToList();
            var trinketCombos = new Combinations<Item>(trinkets, 2);
            foreach(var c in trinketCombos) {
                TrinketPairs_Static.Add((c[0], c[1]));
            }
            expectedCount = 
                  (long)GloveEnchants.Length
                * (long)RingPairs_Static.Count 
                * (long)TrinketPairs_Static.Count
                * (long)ItemBySlot[Item.Head].Count
                * (long)ItemBySlot[Item.Neck].Count
                * (long)ItemBySlot[Item.Shoulder].Count
                * (long)ItemBySlot[Item.Back].Count
                * (long)ItemBySlot[Item.Chest].Count
                * (long)ItemBySlot[Item.Wrist].Count
                * (long)ItemBySlot[Item.Hand].Count
                * (long)ItemBySlot[Item.Waist].Count
                * (long)ItemBySlot[Item.Leg].Count
                * (long)ItemBySlot[Item.Feet].Count
                * (long)ItemBySlot[Item.Wand].Count
                * (long)ItemBySlot[Item.Wep].Count;
            Console.WriteLine($"Starting processing of {expectedCount} combinations");
            Worker[] threads = new Worker[RingPairs_Static.Count];
            for(int i = 0; i < RingPairs_Static.Count; i++) {
                threads[i] = new Worker(RingPairs_Static[i], ItemBySlot);
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while(threads.Any(x => x.Thread.IsAlive)) {
                for (int i = 0; i < RingPairs_Static.Count; i++) {
                    threads[i].Thread.Join(1);
                }
                Thread.Sleep(1000);
                var pct = 100.0 / expectedCount * processCount;

                double nPercentage = (double)Math.Max(processCount,1) / (double)expectedCount;
                double nElapsed = sw.Elapsed.TotalSeconds;
                double nTotalTime = (1.0 / nPercentage) * nElapsed;
                double nRemaining = nTotalTime - nElapsed;
                var ts = TimeSpan.FromSeconds(nRemaining);
                Console.WriteLine(
                    String.Format("{0:0.000}% {1}/{2} {3}",
                    pct,
                    processCount,
                    expectedCount,
                    ts.ToString(@"hh\:mm\:ss")));
            }
            List<Character> resultChars = new List<Character>();
            foreach(var worker in threads) {
                resultChars.AddRange(worker.characters.Where(x => x != null));
            }
            var orderedCharacters = resultChars.OrderByDescending(x => x.DPS);
            using (StreamWriter outputFile = new StreamWriter("result2.txt")) {
                foreach (var character in orderedCharacters) {
                    outputFile.WriteLine(character.ToString());
                    //Console.WriteLine(character.ToString());
                }
            }
            var a = orderedCharacters.ElementAt(0);
            var b = orderedCharacters.ElementAt(1);
            Console.WriteLine(orderedCharacters.ElementAt(0).ToString());
            Console.WriteLine(orderedCharacters.ElementAt(1).ToString());
            //Foo(ItemBySlot, new Dictionary<Item.ESlot, Item>(), Item.ESlot.Head, characters);
            //var ordered = characters.OrderByDescending(x => x.DPS).ToList();
            //for(int i = 0; i < 10; i++) {
            //}
        }


        enum Index
        {
            slot = 0,
            name = 1,
            intellect = 2,
            stam = 3,
            sp = 4,
            shadowsp = 5,
            fire = 6,
            hit = 7,
            crit = 8,
            haste = 9,
            spirit = 10,
            mp5 = 11,
            gems = 12,
            meta = 13,
            gembonusType = 14,
            gemBonusVal = 15,
            set = 16,
            statValue = 17,
            dpsDiff = 18,
            ignoreHitVal = 19,
            ignoreHitDpsDiff = 20,
            source = 21,
            itemId = 22,
            unkn=23,
            include=24
        };

        public static List<Item> Create(string[] stats) {
            var name = stats[1];
            bool include = stats[(int)Index.include] == "1";
            if (!include)
                return new List<Item>();
            if(!Enum.TryParse<Item.SlotEnum>(stats[0], out var slotEnum)) {
                throw new Exception();
            }
            int slot = (int)slotEnum;
            int.TryParse(stats[2], out int intellect);
            int.TryParse(stats[3], out int stam);
            int.TryParse(stats[4], out int sp);
            int.TryParse(stats[5], out int shadow);
            int.TryParse(stats[7], out int hit);
            int.TryParse(stats[8], out int crit);
            int.TryParse(stats[9], out int haste);
            int.TryParse(stats[10], out int spirit);
            int.TryParse(stats[11], out int mp5);

            List<eColor> sockets = new List<eColor>();
            var gems = stats[12];
            foreach(var g in gems) {
                switch (g) {
                    case 'R': sockets.Add(eColor.Red); break;
                    case 'B': sockets.Add(eColor.Blue); break;
                    case 'Y': sockets.Add(eColor.Yellow); break;
                    case 'P': sockets.Add(eColor.Purple); break;
                    case 'G': sockets.Add(eColor.Green); break;
                    case 'O': sockets.Add(eColor.Orange); break;
                }
            }

            var gemBonusType = stats[14];
            if (!int.TryParse(stats[15], out int gemBonusVal))
                gemBonusVal = 0;

            Debug.Assert(gemBonusType.Length > 0 ? gemBonusVal > 0 : gemBonusVal == 0);

            Stat socketBonus = null;
            switch (gemBonusType) {
                case "dmg": socketBonus = new Stat(Stat.Sp, gemBonusVal); break;
                case "hit": socketBonus = new Stat(Stat.Hit, gemBonusVal); break;
                case "stam": socketBonus = new Stat(Stat.Stam, gemBonusVal); break;
                case "int": socketBonus = new Stat(Stat.Intel, gemBonusVal); break;
                case "crit": socketBonus = new Stat(Stat.Crit, gemBonusVal); break;
                case "resi":
                case "": break;
                default:
                    throw new NotImplementedException($"Missing handle for type {gemBonusType}");
            }

            var setStr = stats[16];
            Item.ESet? set = null;
            switch (setStr) {
                case "T4": set = Item.ESet.T4; break;
                case "Spellstrike": set = Item.ESet.Spellstrike;  break;
            }

            var itemId = int.Parse(stats[(int)Index.itemId]);
            
            var items = new List<Item>();   
            if (sockets.Count == 0) {
                var itemStats = new Stat[9];
                itemStats[0] = new Stat(Stat.Intel, intellect);
                itemStats[1] = new Stat(Stat.Stam, stam);
                itemStats[2] = new Stat(Stat.Sp, sp);
                itemStats[3] = new Stat(Stat.ShadSP, shadow);
                itemStats[4] = new Stat(Stat.Hit, hit);
                itemStats[5] = new Stat(Stat.Crit, crit);
                itemStats[6] = new Stat(Stat.Haste, haste);
                itemStats[7] = new Stat(Stat.Spirit, spirit);
                itemStats[8] = new Stat(Stat.Mp5, mp5);
                items.Add(new Item(slot, name, itemStats, new Socket[0], socketBonus, set, itemId));
                return items;
            }

            bool hasMeta = (stats[13].Length > 0);

            //Combinations<Gem> combs = new Combinations<Gem>(Gem.Gems, sockets.Count);
            /*
            Variations<Gem> combs = new Variations<Gem>(Gem.Gems, sockets.Count, GenerateOption.WithRepetition);
            for(int combNum = 0; combNum < combs.Count; combNum++) {
                var comb = combs.ElementAt(combNum);
                
                List<Socket> chosenSockets = new List<Socket>();
                if (hasMeta)
                    chosenSockets.Add(new Socket(eColor.Meta, Gem.MetaGem));
                Debug.Assert(sockets.Count == comb.Count);
                for(int i = 0; i < sockets.Count; i++) {
                    chosenSockets.Add(new Socket(sockets[i], comb[i]));
                }
                items.Add(new Item(slot, name, itemStats, chosenSockets.ToArray(), socketBonus, set, itemId*100+combNum));
            }
            */
            //socket bonus 
            List<Socket> chosenSocketsBonus = new List<Socket>();
            {
                var itemStats = new Stat[9];
                itemStats[0] = new Stat(Stat.Intel, intellect);
                itemStats[1] = new Stat(Stat.Stam, stam);
                itemStats[2] = new Stat(Stat.Sp, sp);
                itemStats[3] = new Stat(Stat.ShadSP, shadow);
                itemStats[4] = new Stat(Stat.Hit, hit);
                itemStats[5] = new Stat(Stat.Crit, crit);
                itemStats[6] = new Stat(Stat.Haste, haste);
                itemStats[7] = new Stat(Stat.Spirit, spirit);
                itemStats[8] = new Stat(Stat.Mp5, mp5);
                for(int i = 0; i < sockets.Count; i++) { 
                    switch (sockets[i]) {
                        case eColor.Blue:
                            chosenSocketsBonus.Add(new Socket(sockets[i], Gem.Gems[0]));
                            break;
                        case eColor.Yellow:
                            chosenSocketsBonus.Add(new Socket(sockets[i], Gem.Gems[1]));
                            break;
                        case eColor.Red:
                            chosenSocketsBonus.Add(new Socket(sockets[i], Gem.Gems[2]));
                            break;
                        default:
                            throw new Exception();
                    }
                }
                if (hasMeta)
                    chosenSocketsBonus.Add(new Socket(eColor.Meta, Gem.MetaGem));
                items.Add(new Item(slot, name, itemStats, chosenSocketsBonus.ToArray(), socketBonus, set, itemId * 100));
            }

            // red socket
            {
                var itemStats = new Stat[9];
                itemStats[0] = new Stat(Stat.Intel, intellect);
                itemStats[1] = new Stat(Stat.Stam, stam);
                itemStats[2] = new Stat(Stat.Sp, sp);
                itemStats[3] = new Stat(Stat.ShadSP, shadow);
                itemStats[4] = new Stat(Stat.Hit, hit);
                itemStats[5] = new Stat(Stat.Crit, crit);
                itemStats[6] = new Stat(Stat.Haste, haste);
                itemStats[7] = new Stat(Stat.Spirit, spirit);
                itemStats[8] = new Stat(Stat.Mp5, mp5);
                List<Socket> chosenSockets = new List<Socket>();
                bool anyNewGems = false;
                for (int i = 0; i < sockets.Count; i++) {
                    chosenSockets.Add(new Socket(sockets[i], Gem.Gems[2]));
                    if (chosenSockets[i].EquippedGem != chosenSocketsBonus[i].EquippedGem)
                        anyNewGems = true;
                }

                // avoid duplicating the item when the socketing done when going for socketbonus is the same as when going full red...
                if (anyNewGems) {
                    if (hasMeta)
                        chosenSockets.Add(new Socket(eColor.Meta, Gem.MetaGem));
                    items.Add(new Item(slot, name, itemStats, chosenSockets.ToArray(), socketBonus, set, itemId * 100+1));
                }
            }
            
            return items;
        }
    }
}


