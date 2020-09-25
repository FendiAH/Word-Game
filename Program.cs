using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace FinalProject
{

    [Serializable]
    public class Room
    {
        public Boolean Current = false;
        public String Name { get; set; }
        public String Description { get; set; }
        public List<Item> Items { get; set; }
        public bool FinalRoom { get; set; }
        public Dictionary<string, Room> Direction { get; set; }
        public bool ContainsItem(String itemName)
        {
            foreach (Item item in Items)
            {
                if (itemName.StartsWith(item.Name))
                {

                    return true;
                }
            }
            return false;
        }
        public Item TransferItem(String itemName)
        {
            foreach (Item item in Items)
            {
                if (itemName.StartsWith(item.Name))
                    return item;
            }
            return null;
        }
        public void PrintRoom()
        {
            Console.WriteLine(this.Description);

            if (Items.Count > 0)
            {
                foreach (Item item in Items)
                    Console.WriteLine(item.ToString());
            }
        }

    }

    [Serializable]
    public class Item : Attackable, IsWeapon
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public uint Worth { get; set; }
        public bool Weapon = false;

        public void IsWeapon()
        {
            Weapon = true;
        }

        public void IsAttacked()
        {
            if (Name == "mirror")
            {
                Name = "broken mirror";
                Worth = 0;
                Description = "A broken mirror (might be used as a weapon)";
                IsWeapon();
                return;
            }
            else
            {
                Worth = 0;
                Description = "A Destroyed " + Name;
            }
        }

        public override string ToString()
        {
            return $"{Name}: ({Worth} gold){Description}";
        }

    }

    [Serializable]
    public class Bagpack
    {
        public List<Item> Items;

        public bool CheckItem(string itemName)
        {
            foreach (Item item in Items)
            {
                if (itemName.Contains(item.Name))
                    return true;
            }
            return false;
        }

        public Item Transferred(string itemName)
        {
            foreach (Item item in Items)
            {
                if (itemName.Contains(item.Name))
                    return item;
            }
            return null;
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
        }

        public void PrintBag()
        {
            if (Items.Count == 0)
            {
                Console.WriteLine("----------------------------------------------------------------\n" +
                    "Your bagpack is empty" +
                    "----------------------------------------------------------------");
            }
            else if (Items.Count > 0)
            {
                Console.WriteLine("----------------------------------------------------------------\n" +
                    "Your bagpack contains:");
                foreach (Item item in Items)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine("----------------------------------------------------------------");
            }
        }
    }

    [Serializable]
    public class Enemy : Item, Attackable
    {
        public Room Location { get; set; }

        [NonSerialized]
        protected Random rand = new Random();

        public void Delete()
        {
            Name = null;
            Location = null;
            Worth = 0;
            Description = null;
        }

        public void Attack()
        {
            Console.WriteLine("you has been killed by " + Description);
            Location.FinalRoom = true;
        }

        public void IsAttacked()
        {
            Name = "dead " + Name;
            Worth = 1000;
            Description = "A " + Name + " lays on the ground dead!";
        }

        public void PrintLocation()
        {
            Console.WriteLine(Name + " is in " + Location.Name);
        }

        public void ChangeLocation()
        {
            Dictionary<int, Room> random = new Dictionary<int, Room> { };
            int x = 0;
            if (Location.Direction.ContainsKey("east"))
            {
                random.Add(x, Location.Direction.GetValueOrDefault("east"));
                x++;
            }
            if (Location.Direction.ContainsKey("west"))
            {
                random.Add(x, Location.Direction.GetValueOrDefault("west"));
                x++;
            }
            if (Location.Direction.ContainsKey("south"))
            {
                random.Add(x, Location.Direction.GetValueOrDefault("south"));
                x++;
            }
            if (Location.Direction.ContainsKey("north"))
            {
                random.Add(x, Location.Direction.GetValueOrDefault("north"));
                x++;
            }
            if (Location.Direction.ContainsKey("up"))
            {
                random.Add(x, Location.Direction.GetValueOrDefault("up"));
                x++;
            }
            if (Location.Direction.ContainsKey("down"))
            {
                random.Add(x, Location.Direction.GetValueOrDefault("down"));
                x++;
            }

            Location = random.GetValueOrDefault(rand.Next(0, x));
        }
    }

    public class Scoreboard
    {
        public uint Score { get; set; }

        public void PrintScore()
        {
            Console.WriteLine("Game Over: \nYour score is: " + Score);
        }
    }

    [Serializable]
    public class SavedGame
    {
        public String UserName;
        public List<Room> Setup;
        public Bagpack UserBagpack;
        public Enemy enemy;
    }

    [Serializable]
    public class Game
    {
        protected Scoreboard scoreboard = new Scoreboard();

        public Room currentRoom;

        public Enemy enemy;

        public List<Room> rooms;

        protected string introduction =
        "Weeks ago, you received a mysterious letter claiming that your late\n" +
        "grandfather (who you don't know anything about) left you his house\n" +
        "and land in the mountains. Having no property yourself, this is a \n" +
        "substantial inheritance. After a few days of hiking into the\n" +
        "countryside, you come upon the house, opulent and imperial, standing\n" +
        "proudly against the hills leading into the mountain behind it. You are\n" +
        "carrying a bagpack that you can use to store Items you pick up along\n" +
        "the journey.";

        public void StartGame()
        {
            
            SetupRoom();
            InputLoop();
            scoreboard.PrintScore();

        }

        private void SetupRoom()
        {
            const string NORTH = "north";
            const string SOUTH = "south";
            const string EAST = "east";
            const string WEST = "west";
            const string UP = "up";
            const string DOWN = "down";

            entranceHall.Direction = new Dictionary<string, Room>
            {
                {SOUTH, outside },
                {NORTH, livingRoom }
            };

            livingRoom.Direction = new Dictionary<string, Room>
            {
                {SOUTH, entranceHall},
                {NORTH, fancyBedroom},
                {EAST, kitchen},
                {WEST, paintingRoom},
                {DOWN, cellar},
            };

            fancyBedroom.Direction = new Dictionary<string, Room>
            {
                { SOUTH, livingRoom},
            };

            kitchen.Direction = new Dictionary<string, Room>
            {
                { WEST, livingRoom},
            };

            paintingRoom.Direction = new Dictionary<string, Room>
            {
                { EAST, livingRoom},
            };

            cellar.Direction = new Dictionary<string, Room>
            {
                { UP, livingRoom},
                { NORTH, library},
            };

            library.Direction = new Dictionary<string, Room>
            {
                { SOUTH, cellar},
            };

            laboratory.Direction = new Dictionary<string, Room>
            {
                { NORTH, skeletonRoom},
                { SOUTH, library},
            };

            skeletonRoom.Direction = new Dictionary<string, Room>
            {
                {SOUTH, laboratory },
                {NORTH, outside }
            };

            outside.Direction = new Dictionary<string, Room>
            {
                {SOUTH, skeletonRoom },
                {NORTH, entranceHall }
            };
        }

        public Bagpack bagpack = new Bagpack
        {
            Items = new List<Item> { }
        };

        public Room outside = new Room
        {
            Name = "Outside",
            Description =
            "You decide to take what you've already found and leave. Something\n" +
            "about this place unnerves you, and you never return.",
            Items = new List<Item>(),
            FinalRoom = true
        };

        public Room entranceHall = new Room
        {
            Name = "Entrance Hall",
            Description =
            "You are in the entrance hall. There is a door leading further into\n" +
            "the house to the north, and you can go back outside to the south.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "picture frame",
                    Description ="An old picture frame with your grandparent's picture",
                    Worth = 5,
                    Weapon = false
                }, // picture frame
                new Item
                {
                    Name = "wooden cross",
                    Description = "a wooden cross with a sharp bottom, your grandfather\n" +
                    "must have been out to hunt Vampires",
                    Worth = 14,
                    Weapon = true
                } // wooden cross
            }
        };

        public Room livingRoom = new Room
        {
            Name = "Living Room",
            Description =
            "You are in the living room. There are doors to the north, south\n" +
            "(towards the entrance hall), east, and west. There is a staircase \n" +
            "going down.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "trophy case",
                    Description = "A trophy case containing a massive golden cup",
                    Worth = 150,
                    Weapon = false
                }, //trophy case
                new Item
                {
                    Name = "elven sword",
                    Description = "A leaf-bladed longsword, elven crafted.",
                    Worth = 150,
                    Weapon = true
                }, //elven sword
                new Item
                {
                    Name = "fancy rug",
                    Description = "A large, oriental-style rug with exceptional craftsmanship.",
                    Worth = 100,
                    Weapon = false
                } //fancy rug
            }
        };

        public Room paintingRoom = new Room
        {
            Name = "Painting Room",
            Description =
            "You are in the painting room. There is a Harpsichord. A painting \n" +
            "depicts a skeleton holding open a gateway to an underground passage.\n" +
            "A male elf is entering the passage. A female elf is holding a strange\n" +
            "orb. A human man stands to the side observing.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "harpsichord",
                    Description = "An incredibly heavy harpsichord.",
                    Worth = 300,
                    Weapon = false
                }, //harpsichord
                new Item
                {
                    Name = "oil painting",
                    Description =
                    "The painting depicts a skeleton holding open a gateway\n" +
                    " to an underground passage. A male elf is entering the passage.\n" +
                    "A female elf is holding a strange orb. A human man stands to \n" +
                    "the side observing.",
                    Worth = 150,
                    Weapon = false
                }, //oil painting
            }
        };

        public Room kitchen = new Room
        {
            Name = "Kitchen",
            Description =
            "You are in the kitchen. A table seems to have been used recently for\n" +
            "the preparation of food. A passage leads to the west.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "kitchen knife",
                    Description = "A sharp knife used for Kitchen purpose.",
                    Worth = 30,
                    Weapon = true
                }, //kitchen knife
                new Item
                {
                    Name = "sack of peppers",
                    Description = "A brown sack containing spicy green peppers.",
                    Worth = 1,
                    Weapon = false
                }, //sack of peppers
                new Item
                {
                    Name = "glass of water",
                    Description = "A refreshing glass of cold water.",
                    Worth = 1,
                    Weapon = false
                }, //glass of water
            }
        };

        public Room fancyBedroom = new Room
        {
            Name = "Fancy Bedroom",
            Description =
            "You are in the fancy bedroom. There is a four-poster bed with red\n" +
            "sheets. There is a closed chest at the foot of the bed.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "mirror",
                    Description = "A mirror on the wall (might be useful if broken)",
                    Worth = 20,
                    Weapon = false
                }, //mirror
                new Item
                {
                    Name = "boots",
                    Description = "Tough boots with spikes for climbing.",
                    Worth = 10,
                    Weapon = false
                }, //boots
                new Item
                {
                    Name = "sheets",
                    Description = "Fancy silk sheets",
                    Worth = 50,
                    Weapon = false
                }, //sheets
                new Item
                {
                    Name = "gold",
                    Description = "Shiny gold coins.",
                    Worth = 100,
                    Weapon = false
                }, //gold
            }
        };

        public Room cellar = new Room
        {
            Name = "Cellar",
            Description =
            "You are in a tidy cellar. There are barrels of wine here. A door \n" +
            "leads to the north, and a staircase goes up.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "wine",
                    Description = "A bottle of fine wine.",
                    Worth = 50,
                    Weapon = false
                }, //wine
            }
        };

        public Room library = new Room
        {
            Name = "Library",
            Description =
            "You are in a large library. There are many books about anatomy, \n" +
            "history, and alchemy. Some of the books are written in Elven. There \n" +
            "is a door to the south that takes you to the cellar. ",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item
                {
                    Name = "necklace",
                    Description = "A ruby necklace.",
                    Worth = 125,
                    Weapon = false
                }, //necklace
                new Item
                {
                    Name = "elven book",
                    Description = "A tome written in the indecipherable elven dialect.",
                    Worth = 100,
                    Weapon = false
                }, //elven book
            },
        };

        public Room laboratory = new Room
        {
            Name = "Laboratory",
            Description =
            "You find yourself in a strange laboratory. A lamp with a red filter \n" +
            "lights the room. There is a secret passage to the south. There is a \n" +
            "door with a skull to the north.",
            FinalRoom = false,
            Items = new List<Item>
            {
                new Item    //Orb
                {
                    Name = "orb",
                    Description =
                    "The Orb of Yendor, an ancient artifact that has been missing \n" +
                    "for many years.",
                    Worth = 500,
                    Weapon = false
                },
                new Item
                {
                    Name = "flask",
                    Description = "A flask encrusted with gems.",
                    Worth = 200,
                    Weapon = false
                },
                new Item
                {
                    Name = "lamp",
                    Description = "A lamp with a ruby-tinted filter.",
                    Worth = 30,
                    Weapon = false
                },
            }
        };

        public Room skeletonRoom = new Room
        {
            Name = "Skeleton Room",
            Description =
            "The strange door opens into darkness. You peer in, and a pair of \n" +
            "skeletal hands reach out and drags you in! The last thing you see \n" +
            "is a strange underground passage before the last of the light disappears.",
            Items = new List<Item>(),
            FinalRoom = true,
        };

        public void InputLoop()
        {
            int move = 0;
            enemy = new Enemy
            {
                Name = "tiger",
                Location = laboratory,
                Description = "a wild large scary tiger",
                Worth = 0,
            }; //Creates new enemy

            while (true)
            {
                currentRoom.PrintRoom();

                if (currentRoom.FinalRoom)
                {
                    if (currentRoom == skeletonRoom)
                    {
                        scoreboard.Score = 0;
                    }

                    break;
                }

                string input = Console.ReadLine().ToLower();

                string subInput;

                if (input.Equals("quit"))
                {
                    while (true)
                    {
                        Console.WriteLine("Would you like to save the game?(Y/N)");
                        string save = Console.ReadLine().ToLower();
                        if (save.Equals("y"))
                        {
                            SaveGame();
                            break;
                        }
                        else if (save.Equals("n"))
                            break;
                        else
                        {
                            Console.WriteLine("Invalid input");
                        }
                    }
                    break;
                } //when user wants to quit asks if the user wants to save before quitting

                if (input.StartsWith("pick up"))
                {
                    subInput = input.Substring(8);
                    if (subInput.Equals("elven book") && !library.Direction.ContainsKey("north"))
                    {
                        library.Direction.Add("north", laboratory);
                        Console.WriteLine("You have found a new Secret Door to the north");
                        library.Description += ", and a door to the north";
                    }

                    if (currentRoom.ContainsItem(subInput))
                    {
                        Item transferred = currentRoom.TransferItem(subInput);
                        bagpack.AddItem(transferred);
                        currentRoom.Items.Remove(transferred);
                        scoreboard.Score += transferred.Worth;
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Item is not available in " + currentRoom.Name);
                        continue;
                    }
                } //checks if item available in room and picks up

                else if (input.StartsWith("drop"))
                {
                    subInput = input.Substring(5);
                    if (bagpack.CheckItem(subInput))
                    {
                        Item transfer = bagpack.Transferred(subInput);
                        currentRoom.Items.Add(transfer);
                        bagpack.RemoveItem(transfer);
                    }
                } //checks if item available in bagpack and drops it

                else if (input.StartsWith("attack"))
                {
                    Item weapon;
                    Item itemAttacked;
                    subInput = input.Substring(7);
                    if (currentRoom.ContainsItem(subInput) & subInput != null)
                    {
                        itemAttacked = currentRoom.TransferItem(subInput);
                        subInput = subInput.Substring(itemAttacked.Name.Length);
                        if (subInput.Length > 6)
                        {
                            if (bagpack.CheckItem(subInput.Substring(6)))
                            {
                                weapon = bagpack.Transferred(subInput.Substring(6));
                                if (weapon.Weapon)
                                {
                                    itemAttacked.IsAttacked();
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("The Object you are trying to attack with is not a Weapon.");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You don't have this item in your bagpack");
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input.");
                            continue;
                        }
                    }
                    else if (subInput.StartsWith(enemy.Name.ToLower()))
                    {
                        if (subInput.Length > (enemy.Name.Length + 6))
                        {
                            if (bagpack.CheckItem(subInput.Substring(enemy.Name.Length + 6)))
                            {
                                weapon = bagpack.Transferred(subInput.Substring(enemy.Name.Length + 6));
                                if (weapon.Weapon)
                                {
                                    enemy.IsAttacked();
                                    Item deadEnemy = new Item
                                    {
                                        Name = enemy.Name,
                                        Description = enemy.Description,
                                        Worth = enemy.Worth,
                                        Weapon = false
                                    };
                                    currentRoom.Items.Add(deadEnemy);
                                    enemy.Delete();
                                    continue;
                                }
                                else
                                {
                                    Console.WriteLine("The Object you are trying to attack with is not a Weapon.");
                                    continue;
                                }
                            }
                            else
                            {
                                Console.WriteLine("You do not have a weapon with that name");
                                continue;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong input");
                            continue;
                        }
                    }

                    else if (!subInput.StartsWith(enemy.Name.ToLower()))
                    {
                        Console.WriteLine("The " + $"{enemy.Name} you are trying to attack doesn't exist in this room.");
                        continue;
                    }

                } //check for possible attack and executes if possible

                else if (input.StartsWith("check bag"))
                {
                    bagpack.PrintBag();
                } //lists the items in bagpack

                else if (currentRoom.Direction.ContainsKey(input))
                {
                    if (currentRoom == enemy.Location)
                    {
                        enemy.Attack();
                        scoreboard.Score = 0;
                        break;
                    }

                    currentRoom = currentRoom.Direction.GetValueOrDefault(input);

                    if (move % 2 == 0 && enemy.Name != null)
                    {
                        enemy.ChangeLocation();
                        enemy.PrintLocation();
                    }

                    move++;
                    continue;
                }

                else if (!currentRoom.Direction.ContainsKey(input))
                {
                    Console.WriteLine("Invalid input, try again.");
                    continue;
                }
            }
        }

        public String TitleCreator(Room room)
        {
            String title = room.Name;
            return title;
        }

        public static List<Item> TransferItem(List<Item> items)
        {
            List<Item> newList = new List<Item>();
            foreach (Item item in items)
            {
                newList.Add(item);
            }
            return newList;
        }

        public static Room TransferRoom(Room saved)
        {
            Room loaded = new Room();
            loaded.Name = saved.Name;
            loaded.Description = saved.Description;
            loaded.Direction = saved.Direction;
            loaded.FinalRoom = saved.FinalRoom;
            loaded.Items = TransferItem(saved.Items);
            return loaded;
        }

        public void SaveGame()
        {
            currentRoom.Current = true;
            SavedGame save = new SavedGame();
            save.enemy = enemy;
            save.UserBagpack = bagpack;
            save.Setup = new List<Room>();
            save.Setup.Add(outside);
            save.Setup.Add(entranceHall);
            save.Setup.Add(livingRoom);
            save.Setup.Add(paintingRoom);
            save.Setup.Add(kitchen);
            save.Setup.Add(fancyBedroom);
            save.Setup.Add(cellar);
            save.Setup.Add(library);
            save.Setup.Add(laboratory);
            save.Setup.Add(skeletonRoom);
            save.Setup.Add(currentRoom);
            Console.WriteLine("File Name: ");
            String FileName = "../../" + Console.ReadLine() + ".bin";
            Stream SaveFileStream = File.Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, save);
            SaveFileStream.Close();

            /*System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(SavedGame));

            var path = "../../bagpack.xml";
            System.IO.FileStream File = System.IO.File.Create(path);

            writer.Serialize(File, save);

            titleReader.Serialize(File, "Bagpack");
            foreach (Item item in bagpack.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, entranceHall.Name);
            foreach (Item item in entranceHall.Items)
            {
                writer.Serialize(File, item);
            }

            titleReader.Serialize(File, outside.Name);
            foreach (Item item in outside.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, livingRoom.Name);
            foreach (Item item in livingRoom.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, paintingRoom.Name);
            foreach (Item item in paintingRoom.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, kitchen.Name);
            foreach (Item item in kitchen.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, fancyBedroom.Name);
            foreach (Item item in fancyBedroom.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, cellar.Name);
            foreach (Item item in cellar.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, library.Name);
            foreach (Item item in library.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, laboratory.Name);
            foreach (Item item in laboratory.Items)
                writer.Serialize(File, item);

            titleReader.Serialize(File, skeletonRoom.Name);
            foreach (Item item in skeletonRoom.Items)
                writer.Serialize(File, item);

            */
        }

        public static Game Loadgame(String filename)
        {
            Game loadGame = new Game();
            while (true)
            {
                try
                {
                    FileStream fs = new FileStream("../../" + filename, FileMode.Open);
                    BinaryFormatter binaryForm = new BinaryFormatter();
                    SavedGame ToLoadGame = (SavedGame)binaryForm.Deserialize(fs);
                    loadGame.SetupRoom();
                    foreach (Room room in ToLoadGame.Setup)
                    {
                        if (room.Current)
                        {
                            Console.WriteLine(room.Description);//TODO:remove print statement
                            loadGame.currentRoom = TransferRoom(room);
                        }
                        else if (room.Name == "Kitchen")
                        {
                            loadGame.kitchen = TransferRoom(room);
                        }
                        else if (room.Name == "Outside")
                        {
                            loadGame.outside = TransferRoom(room);
                        }
                        else if (room.Name == "Painting Room")
                        {
                            loadGame.paintingRoom = TransferRoom(room);
                        }
                        else if (room.Name == "Library")
                        {
                            loadGame.library = TransferRoom(room);
                        }
                        else if (room.Name == "Laboratory")
                        {
                            loadGame.laboratory = TransferRoom(room);
                        }
                        else if (room.Name == "Living Room")
                        {
                            loadGame.livingRoom = TransferRoom(room);
                        }
                        else if (room.Name == "Skeleton Room")
                        {
                            loadGame.skeletonRoom = TransferRoom(room);
                        }
                        else if (room.Name == "Fancy Bedroom")
                        {
                            loadGame.fancyBedroom = TransferRoom(room);
                        }
                        else if (room.Name == "Cellar")
                        {
                            loadGame.cellar = TransferRoom(room);
                        }
                        else if (room.Name == "Entrance Hall")
                        {
                            loadGame.entranceHall = TransferRoom(room);
                        }
                    }
                    loadGame.bagpack = ToLoadGame.UserBagpack;
                    loadGame.enemy = ToLoadGame.enemy;
                    fs.Close();
                    return loadGame;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to deserialize. Reason: " + e.Message);
                    Console.WriteLine("Enter the correct File Name:");
                    filename = Console.ReadLine()+".bin";
                }

            }
        }

        public static void Main(String[] args)
        {
            while (true)
            {
                Console.WriteLine("Would you like to load a saved game?(Y/N)");
                String answer = Console.ReadLine().ToLower();
                if (answer == 'y'.ToString())
                {
                    Console.WriteLine("What is the name of the game you saved?:");
                    String fileName = Console.ReadLine() + ".bin";
                    Game loadedgame = Loadgame(fileName);
                    loadedgame.StartGame();
                    break;
                }
                else if (answer == 'n'.ToString())
                {
                    Console.WriteLine("Starting a new Game!");
                    Game game = new Game();
                    game.currentRoom = game.entranceHall;
                    Console.WriteLine(game.introduction);
                    game.StartGame();
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid Input!");
                    continue;
                }
            }
        }
    }
}