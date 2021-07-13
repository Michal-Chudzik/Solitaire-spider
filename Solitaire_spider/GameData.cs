using System;
using System.Collections.Generic;

namespace Solitaire_spider
{
    public class GameData : Cards
    {
        /// <summary>
        /// Random number generator
        /// </summary>
        private static Random rng = new Random();
        /// <summary>
        /// Difficulty level returned from window1
        /// </summary>
        public int Difficulty_Level { get; set; }
        /// <summary>
        /// Array of class for every card in deck
        /// </summary>
        public stackcard[] Sc { get; set; } = new stackcard[104];
        /// <summary>
        /// List of cards for shuffling
        /// </summary>
        public List<int> Card_list { get; set; }
        /// <summary>
        /// Array of Lists of Class for storring
        /// all information about cards on table
        /// </summary>
        public List<stackcard>[] Deck { get; set; } = new List<stackcard>[10];
        /// <summary>
        /// Temporary List of cards for moving cards
        /// </summary>
        public List<stackcard> Tmp_cards { get; set; } = new List<stackcard>();
        public int Card_width { get; } = 120;
        public int Card_height { get; } = 160;
        /// <summary>
        /// Coordinates of the card taken
        /// </summary>
        public int Starting_card_position_X { get; set; } = 0;
        public int Starting_card_position_Y { get; set; } = 0;
        /// <summary>
        /// Vertical distance between cards in columns
        /// </summary>
        public int Card_distance_vertical { get; } = 30;
        /// <summary>
        /// Array for storage of independent vertical distance 
        /// in each column
        /// </summary>
        public int[] Card_dis_vert { get; set; } = new int[10];
        /// <summary>
        /// Horizontal distance between cards in rows
        /// </summary>
        public int Card_distance_horizontal { get; } = 15;
        /// <summary>
        /// Vertical distance of cards from top border
        /// </summary>
        public int Initial_distance_vertical { get; } = 10;
        /// <summary>
        /// Horizontal distance of cards from left border
        /// </summary>
        public int Initial_distance_horizontal { get; } = 25;
        /// <summary>
        /// Counter for going through cards
        /// </summary>
        public int List_counter { get; set; }
        /// <summary>
        /// Amount of cards in each columns
        /// </summary>
        public int[] Rows_count { get; set; } = new int[10];
        /// <summary>
        /// Counter for Z index
        /// </summary>
        public int Z_index_counter { get; set; } = 1;
        /// <summary>
        /// Counter for built foundation
        /// </summary>
        public int Stack_removed { get; set; } = 0;
        /// <summary>
        /// Coordinates of the stock pile
        /// X coordinate is distance from right border
        /// </summary>
        public int Stack_addons_x_coordinate { get; set; } = 30;
        /// <summary>
        /// Coordinates of the stock pile
        /// Y coordinate is distance from bottom border
        /// </summary>
        public int Stack_addons_y_coordinate { get; set; } = 10;
        /// <summary>
        /// Counter for amount of stock pile left
        /// default is 5
        /// </summary>
        public int Stack_addons_counter { get; set; } = 5;
        /// <summary>
        /// Variable for user points
        /// </summary>
        public int Points { get; set; } = 500;

        public GameData()
        {
            for (int i = 0; i < Card_dis_vert.Length; i++)
            {
                Card_dis_vert[i] = Card_distance_vertical;
            }

        }

        /// <summary>
        /// Shuffling cards and adding them to class
        /// </summary>
        public void CardShuffle()
        {
            List<int> cards = new List<int>(104);
            for (int i = 0; i < 52; i++)
            {
                cards.Add(i);
            }
            for (int i = 0; i < 52; i++)
            {
                cards.Add(i);
            }
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                // Next() method of System.Random class in C# is used to 
                // get random integer number. In this case number from 0 to n
                int k = rng.Next(n + 1);  
                int value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
            int count = 0;
            foreach (int i in cards)
            {
                stackcard tmp = new stackcard();
                tmp.card = Enum.GetName(typeof(Card_Type), i % Difficulty_Level);
                tmp.colour = tmp.card[0];
                Sc[count++] = tmp;

            }
        }


        /// <summary>
        /// Checks in which column user clicked or release mouse
        /// </summary>
        /// <param name="position"> Mouse position </param>
        /// <returns> Column number </returns>
        public int ColumnCheck(double position)
        {
            int column = 0;
            int tmp = Card_distance_horizontal + Initial_distance_horizontal + Card_width;
            for (int i = 1; i <= 10; i++)
            {
                if (position < tmp)
                {
                    column = i;
                    break;
                }
                tmp += Card_width + Card_distance_horizontal;
            }
            if (column == 0)
            {
                column = 10;
            }
            return column;
        }

        public int RowCheck(double position, int column)
        {
            int row = 0;
            int tmp = Initial_distance_vertical + Card_dis_vert[column];
            int i = 1;
            while (row == 0)
            {
                if (position < tmp)
                {
                    row = i;
                }
                tmp += Card_dis_vert[column];
                i++;
            }
            if (row > Rows_count[column])
            {
                row = Rows_count[column];
            }
            return row;
        }
        public void DiffLv(int dif)
        {
            Difficulty_Level = dif;
        }
    }

}

