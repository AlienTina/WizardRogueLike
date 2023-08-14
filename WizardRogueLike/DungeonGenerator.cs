using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace WizardRogueLike
{

    public enum roomType
    {
        fight = 0,
        item,
        boss,
        none
    }
    public class DungeonGenerator
    {
        public Vector2 start;

        public Room[,] rooms = new Room[0,0];
        public char[,] GenerateDungeon(int width, int height)
        {
            char[,] dungeon = new char[width, height];
            rooms = new Room[width, height];

            // Initialize the dungeon with walls
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    dungeon[x, y] = '#';
                }
            }

            // Start the random walk from the center of the dungeon
            int startX = width / 2;
            int startY = height / 2;
            dungeon[startX, startY] = ' ';
            start = new Vector2(startX, startY);
            rooms[startX, startY] = new Room(startX, startY, roomType.fight);

            Random rand = new Random();
            int numSteps = width * height; // Adjust the number of steps based on the size of the dungeon

            int previousX = startX;
            int previousY = startY;

            for (int i = 0; i < numSteps; i++)
            {
                // Choose a random direction (up, down, left, or right)
                int direction = rand.Next(4);

                // Move in the chosen direction
                if (direction == 0 && startY > 0) // Up
                {
                    startY--;
                }
                else if (direction == 1 && startY < height - 1) // Down
                {
                    startY++;
                }
                else if (direction == 2 && startX > 0) // Left
                {
                    startX--;
                }
                else if (direction == 3 && startX < width - 1) // Right
                {
                    startX++;
                }
                
                // Carve a path in the dungeon
                dungeon[startX, startY] = ' ';
                rooms[startX, startY] = new Room(startX, startY);
                previousX = startX;
                previousY = startY;
            }

            rooms[startX, startY].myType = roomType.boss;
            
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    if (rooms[x, y] != null)
                        rooms[x, y].SetNeighbour(dungeon, rooms, width, height);
                }
            }

            return dungeon;
        }
    }

    public class Room
    {
        public int x;
        public int y;

        public int[] neighbours = new int[4];

        public roomType myType;
        public Room(int x, int y)
        {
            this.x = x; 
            this.y = y;
            for(int i = 0; i < neighbours.Length;i++) { neighbours[i] = 0;}

            this.myType = roomType.fight;
        }

        public Room(int x, int y, roomType myType)
        {
            this.x = x;
            this.y = y;
            for (int i = 0; i < neighbours.Length; i++) { neighbours[i] = 0; }

            this.myType = myType;
        }

        public void SetNeighbour(char[,] grid, Room[,] rooms, int width, int height)
        {
            Debug.WriteLine(x.ToString() + " " + y.ToString());
            //neighbours[direction] = 1;
            if (y - 1 > 0)
            {
                Debug.WriteLine("Yup");
                if (grid[x, y - 1] == ' ')
                {
                    neighbours[0] = 1;
                    if (rooms[x, y - 1] != null)
                        rooms[x, y - 1].neighbours[1] = 1;
                }
            }
            if (y + 1 < height)
            {
                Debug.WriteLine("Yup");
                if (grid[x, y + 1] == ' ')
                {
                    neighbours[1] = 1;
                    if (rooms[x, y + 1] != null)
                        rooms[x, y + 1].neighbours[0] = 1;
                }
            }
            if (x - 1 > 0)
            {
                Debug.WriteLine("Yup");
                if (grid[x - 1, y] == ' ')
                {
                    neighbours[2] = 1;
                    if (rooms[x - 1, y] != null)
                        rooms[x - 1, y].neighbours[3] = 1;
                }
            }
            if (x + 1 < width)
            {
                Debug.WriteLine("Yup");
                if (grid[x + 1, y] == ' ')
                {
                    neighbours[3] = 1;
                    if (rooms[x + 1, y] != null)
                        rooms[x + 1, y].neighbours[2] = 1;
                }
            }
        }
    }
}
