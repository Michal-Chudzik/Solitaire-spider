using System;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Solitaire_spider
{
    public class stackcard : GameData
    {
        /// <summary>
        /// Card colour
        /// </summary>
        public char colour { get; set; }
        /// <summary>
        /// Name of card 
        /// for example Ace of Spades will be stored as SA
        /// </summary>
        public string card { get; set; }
        /// <summary>
        /// Object for storing card which is Rectangle
        /// </summary>
        public Rectangle Card_Object { get; set; }
        /// <summary>
        /// Bool checking if card is flipped or not
        /// </summary>
        public bool IsVisible { get; set; } 
        /// <summary>
        /// Checking if Card is in order with card lower
        /// </summary>
        public bool Is_in_order { get; set; }


        public stackcard(bool isVisible = false, bool is_in_order = false)
        {
            IsVisible = isVisible;
            Is_in_order = is_in_order;
        }

        /// <summary>
        /// Filling card back 
        /// </summary>
        public void CardFill()
        {
            Card_Object.Width = Card_width;
            Card_Object.Height = Card_height;
            string tmp = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\images\\");
            tmp += @"\suit.png";
            Card_Object.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(tmp)) };
        }

        /// <summary>
        /// Flipping card face up
        /// </summary>
        public void CardFlip()
        {
            IsVisible = true;
            string tmp = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\images\\");
            tmp += @"\";
            tmp += card;
            tmp += ".png";
            Card_Object.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(tmp)) };
        }
    }
}
