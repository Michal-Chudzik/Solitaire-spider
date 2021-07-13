using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media.Animation;



namespace Solitaire_spider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameData gd = new GameData();
        UIElement dragObject = null;
        Point offset;
        private List<Rectangle> Tmp_card_list { get; set; } = new List<Rectangle>();
        private string Card_name { get; set; }
        private List<Rectangle> Add_stack { get; set; } = new List<Rectangle>();
        private List<Rectangle> Add_stack_won { get; set; } = new List<Rectangle>();

        private bool can_delete { get; set; } = false;
        private int column_glob { get; set; } = 0;
        private int amount_glob { get; set; } = 0;
        public MainWindow()
        {
            InitializeComponent();
            string path = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\Images\\");
            path += "\\tlo.jpg";
            CanvasMainWindow.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(path)) };

            DifficultyWindow();
            gd.CardShuffle();
            CardsOutPrint();
            for (int i = 0; i < 5; i++)
            {
                Stack_card_print(i);
            }
            Canvas.SetRight(Timer, 200);
            Canvas.SetBottom(Timer, 20);
            Canvas.SetRight(Points, 400);
            Canvas.SetBottom(Points, 20);
        }


        public void DifficultyWindow()
        {
            Window1 win1 = new Window1(gd);
            win1.Topmost = true;
            win1.ShowDialog();
        }


        private void CardsOutPrint()
        {
            int y = gd.Initial_distance_vertical; // initial distance from top edge
            int counter = 0; // counter how many cards has been printed 
            for (int i = 0; i < 6; i++)
            {
                int x = 25;     // initial distance from left edge
                for (int j = 0; j < 10; j++)
                {
                    if (counter != 54)  // print of correct amount of cards
                    {
                        CardPrint(x, y, j, i);
                        x += gd.Card_width + gd.Card_distance_horizontal; // width of card + distance between them
                        counter++;
                        gd.Rows_count[j]++;
                    }
                }
                y += gd.Card_distance_vertical;
            }
        }


        private void CardPrint(int x, int y, int x_counter, int y_counter)
        {
            gd.Sc[gd.List_counter].Card_Object = new Rectangle { };
            gd.Sc[gd.List_counter].CardFill();

            Canvas.SetTop(gd.Sc[gd.List_counter].Card_Object, y);
            Canvas.SetLeft(gd.Sc[gd.List_counter].Card_Object, x);

            gd.Sc[gd.List_counter].Card_Object.PreviewMouseDown += UserCTRL_PreviewMouseDown;
            CanvasMainWindow.Children.Add(gd.Sc[gd.List_counter].Card_Object);
            if (y_counter == 0)
            {
                gd.Deck[x_counter] = new List<stackcard>();
                gd.Deck[x_counter].Add(gd.Sc[gd.List_counter]);
            }
            else
            {
                gd.Deck[x_counter].Add(gd.Sc[gd.List_counter]);
                // if program is in last row, flip cards
                if (y_counter == 5 || y_counter == 4 && x_counter > 3)
                {
                    gd.Sc[gd.List_counter].CardFlip();
                }
            }
            gd.List_counter++;

        }

        private void UserCTRL_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.offset = e.GetPosition(this.CanvasMainWindow);
            int column = gd.ColumnCheck(this.offset.X) - 1;
            column_glob = gd.ColumnCheck(this.offset.X) - 1;
            // checking if mouse is clicked on the stack pile
            if (offset.X >= (CanvasMainWindow.ActualWidth - (gd.Stack_addons_x_coordinate + gd.Card_width))
                && offset.X <= CanvasMainWindow.ActualWidth - (gd.Stack_addons_x_coordinate - 5 * gd.Stack_addons_counter)
                && offset.Y >= CanvasMainWindow.ActualHeight - (gd.Stack_addons_y_coordinate + gd.Card_height)
                && offset.Y <= CanvasMainWindow.ActualHeight - gd.Stack_addons_y_coordinate && Add_stack != null)
            {
                //if pile is empty return
                if (!Row_check_stack()) return;
                Add_cards(column);
                CanvasMainWindow.Children.Remove(Add_stack[Add_stack.Count - 1]);
                Add_stack.RemoveAt(Add_stack.Count - 1);
                if (gd.List_counter >= 104) Add_stack = null;
                else gd.Stack_addons_counter--;
                return;
            }
            // row in which mouse is clicked
            int row = gd.RowCheck(offset.Y, column);
            // order check
            Is_in_order(column);
            if (gd.Deck[column][row - 1].Is_in_order)
            {
                this.dragObject = sender as UIElement;
                this.offset.Y -= Canvas.GetTop(this.dragObject);
                this.offset.X -= Canvas.GetLeft(this.dragObject);
                this.CanvasMainWindow.CaptureMouse();
            }
            else return;
            // pushing cards to tmp list thanks to which we will move cards and put down them
            amount_glob = Tmp_list_fill(row, column);
            // removing cards from card stack if any are in tmp list
            if (amount_glob != 0) can_delete = true;
            else can_delete = false;
            // saving initial card position
            gd.Starting_card_position_X = (int)Canvas.GetLeft(this.dragObject);
            gd.Starting_card_position_Y = (int)Canvas.GetTop(this.dragObject);
        }

        private bool Row_check_stack()
        {
            for (int i = 0; i < gd.Rows_count.Length; i++)
            {
                if (gd.Rows_count[i] == 0)
                {
                    MessageBox.Show("Jeżeli jakieś kolumny są puste, nie możesz rozdać nowego rzędu.");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// addon stack cards
        /// </summary>
        private void Add_cards(int column)
        {
            for (int i = 0; i < 10; i++)
            {
                int x_coordinate = i * (gd.Card_distance_horizontal + gd.Card_width) + gd.Initial_distance_horizontal;
                gd.Sc[gd.List_counter].Card_Object = new Rectangle { };
                gd.Sc[gd.List_counter].CardFill();
                Canvas.SetLeft(gd.Sc[gd.List_counter].Card_Object, x_coordinate);
                Canvas.SetTop(gd.Sc[gd.List_counter].Card_Object, (gd.Rows_count[i]++ * gd.Card_dis_vert[i] + gd.Initial_distance_vertical));
                Canvas.SetZIndex(gd.Sc[gd.List_counter].Card_Object, gd.Z_index_counter++);
                gd.Sc[gd.List_counter].Card_Object.PreviewMouseDown += UserCTRL_PreviewMouseDown;
                CanvasMainWindow.Children.Add(gd.Sc[gd.List_counter].Card_Object);
                gd.Deck[i].Add(gd.Sc[gd.List_counter]);
                gd.Sc[gd.List_counter].CardFlip();
                if (gd.Rows_count[i] > 11)
                {
                    Resize_stack(i, gd.Rows_count[i] - 11);
                }
                Win_check(i);
                gd.List_counter++;
            }
        }


        private void Is_in_order(int column)
        {
            int amount = gd.Rows_count[column] - 1;
            if (amount < 0) return;
            // last card from stack is always in order
            gd.Deck[column][amount].Is_in_order = true;
            for (int i = gd.Rows_count[column]; i > 0; i--)
            {
                // if there is any cards in column and card check returned true card is in order
                if (amount != 0 && Card_check(gd.Deck[column][amount].card, gd.Deck[column][(amount--) - 1].card))
                {
                    gd.Deck[column][amount].Is_in_order = true;
                }
                else return;
            }
        }

        private bool Card_check(string card1s, string card2s)
        {
            int card1 = (int)Enum.Parse(typeof(Cards.Card_Type), card1s);
            int card2 = (int)Enum.Parse(typeof(Cards.Card_Type), card2s);
            // if substraction of numbers of two next to each other cards is equal 1
            // that means the cards are in order, but if any of them is king then cards 
            // cant be in order
            if (card1 - card2 == 1 && card1 != 0 && card1 != 13 && card1 != 26 && card1 != 39)
            {
                return true;
            }
            return false;
        }

        private int Tmp_list_fill(int row, int column)
        {
            int amount = 0;
            int i = 0;
            foreach (stackcard item in gd.Deck[column])
            {
                // if loop is in the correct row, card is face up and
                // card is in order, then add card to temporary card list
                if (++i >= row && item.IsVisible && item.Is_in_order)
                {
                    Canvas.SetZIndex(item.Card_Object, gd.Z_index_counter++);
                    Tmp_card_list.Add(item.Card_Object);
                    gd.Tmp_cards.Add(item);
                    amount++;
                }
            }
            Card_name = gd.Tmp_cards[0].card;
            return amount;
        }

        private void Stack_remove(int column, int amount)
        {
            // remove correct amount of cards from array of list of class
            for (int i = 0; i < amount; i++)
            {
                gd.Deck[column].RemoveAt(gd.Rows_count[column] - 1);
                gd.Rows_count[column]--;
                if (gd.Rows_count[column] >= 11)
                    Resize_stack_before(column, gd.Rows_count[column] - 11);
            }
        }

        private void CanvasMainWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragObject == null) return;
            var position = e.GetPosition(sender as IInputElement);
            int counter = 0;
            // move every card in list
            foreach (var rect in Tmp_card_list)
            {
                Canvas.SetTop(rect, (position.Y - this.offset.Y) + (gd.Card_distance_vertical * counter++));
                Canvas.SetLeft(rect, position.X - this.offset.X);
            }
        }

        private void CanvasMainWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            // position of a mouse when up
            var position = e.GetPosition(sender as IInputElement);
            // getting X coordinates of the place where the card was placed 
            int column = gd.ColumnCheck(position.X) - 1;
            // getting X coordinates of the place where the card was taken 
            int column_before = gd.ColumnCheck(gd.Starting_card_position_X) - 1;
            // coordinates of a column where the card was placed
            // x coordinate = current column * distance between left edge of the card + initial distance from left edge
            int x_coordinate = (column * (gd.Card_distance_horizontal + gd.Card_width)) + gd.Initial_distance_horizontal;
            // removing cards from Deck, after mouse up becouse resize wont work
            // if this was in mousedown.
            if (can_delete == true && this.dragObject != null)
            {
                Stack_remove(column_glob, amount_glob);
            }
            // if cards are in window and if they are layed on card in correct order they
            // are put there, else cards return to starting position
            if (column >= 0 && this.dragObject != null && (gd.Rows_count[column] == 0 
                || Card_put_down_check(gd.Deck[column][gd.Rows_count[column] - 1].card)))
            {
                Correct_column_put_down(x_coordinate, column, column_before);
                Win_check(column);
                if (gd.Points > 0 && column != column_before) gd.Points--;
                Points.Content = gd.Points.ToString();
            }
            else if (this.dragObject != null)
            {
                Wrong_column_put_down(column_before);
            }
            foreach (var card in gd.Deck[column])
            {
                card.Is_in_order = false;
            }
            this.dragObject = null;
            this.CanvasMainWindow.ReleaseMouseCapture();
        }

        private bool Card_put_down_check(string card1s)
        {
            int card1 = (int)Enum.Parse(typeof(Cards.Card_Type), card1s);
            int card2 = (int)Enum.Parse(typeof(Cards.Card_Type), Card_name);
            // if cards which user put down are in order by number return true, 
            // example: if user puts Diamond 4 on a Spades 5 if will return true.
            if ((card2 % 13) - (card1 % 13) == 1)
            {

                return true;
            }
            return false;
        }

        private void Correct_column_put_down(int x_coordinate, int column, int column_before)
        {
            int i = 0;

            foreach (var card in Tmp_card_list)
            {
                Canvas.SetLeft(card, x_coordinate);
                // amount cards in column * vertical distance between cards + initial distance from top edge
                Canvas.SetTop(card, (gd.Rows_count[column] * gd.Card_dis_vert[column] + gd.Initial_distance_vertical));
                gd.Rows_count[column]++;          // adding in column where user put card
                gd.Deck[column].Add(gd.Tmp_cards[i++]); // adding cards from tmp list to main cards container
                if (gd.Rows_count[column] > 11)
                    Resize_stack(column, gd.Rows_count[column] - 11);
            }
            Tmp_card_list.Clear();
            gd.Tmp_cards.Clear();
            // if column is not empty and if last card from initial stack is not flipped, flip it
            if (gd.Rows_count[column_before] != 0
                && gd.Deck[column_before][(gd.Rows_count[column_before]) - 1].IsVisible == false)
            {
                gd.Deck[column_before][(gd.Rows_count[column_before]) - 1].CardFlip();
            }
        }

        private void Win_check(int column)
        {
            int amount = gd.Rows_count[column] - 1;
            int counter = 0;
            if (amount < 0) return;
            // last card from stack is always in order
            gd.Deck[column][amount].Is_in_order = true;
            for (int i = gd.Rows_count[column]; i > 0; i--)
            {
                // if there is any cards in column and card check returned true card is in order
                if (amount != 0 && Card_check(gd.Deck[column][amount].card, gd.Deck[column][(amount--) - 1].card))
                {
                    gd.Deck[column][amount].Is_in_order = true;
                    if (++counter == 12)
                    {
                        string tmp = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\Images\\");
                        tmp += "\\";
                        tmp += gd.Deck[column][amount + 1].colour;
                        tmp += "K.png";
                        foreach (var item in gd.Deck[column])
                        {
                            if (item.Is_in_order == true)
                            {
                                CanvasMainWindow.Children.Remove(item.Card_Object);
                            }
                        }
                        Stack_remove(column, 13);
                        if (amount != 0 && gd.Deck[column][amount - 1].IsVisible == false)
                        {
                            gd.Deck[column][amount - 1].CardFlip();
                        }
                        int left = 40 * (gd.Stack_removed + 1);
                        Add_stack_won.Add(new Rectangle
                        {
                            Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(tmp)) },
                            Width = gd.Card_width,
                            Height = gd.Card_height
                        });
                        Canvas.SetBottom(Add_stack_won[gd.Stack_removed], 5);
                        Canvas.SetLeft(Add_stack_won[gd.Stack_removed], left);
                        CanvasMainWindow.Children.Add(Add_stack_won[gd.Stack_removed]);
                        gd.Points += 101;
                        Points.Content = gd.Points.ToString();
                        if (++gd.Stack_removed == 8)
                        {
                            CanvasMainWindow.Children.Remove(Timer);
                            MessageBox.Show($" Gratulacje!\n Wynik {gd.Points}\n {currentTime}");
                            this.Close();
                        }
                        return;
                    }
                }
                else return;
            }
        }

        private void Resize_stack(int column, int amount)
        {
            int i = 0;
            if (gd.Rows_count[column] <= 15)
            {
                gd.Card_dis_vert[column]--;
            }
            else if (gd.Rows_count[column] <= 26)
            {
                gd.Card_dis_vert[column]--;
            }
            else if (gd.Rows_count[column] <= 36 && gd.Rows_count[column] % 3 == 1)
            {
                gd.Card_dis_vert[column]--;
            }
            else if (gd.Rows_count[column] <= 45 && gd.Rows_count[column] % 4 == 2)
            {
                gd.Card_dis_vert[column]--;
            }
            else if (gd.Rows_count[column] % 10 == 2)
            {
                gd.Card_dis_vert[column]--;

            }
            foreach (var item in gd.Deck[column])
            {
                Canvas.SetTop(item.Card_Object, gd.Initial_distance_vertical + i++ * gd.Card_dis_vert[column]);
            }
        }
        private void Resize_stack_before(int column, int amount)
        {
            if (gd.Rows_count[column] <= 15)
            {
                gd.Card_dis_vert[column]++;
            }
            else if (gd.Rows_count[column] <= 26)
            {
                gd.Card_dis_vert[column]++;
            }
            else if (gd.Rows_count[column] <= 36 && gd.Rows_count[column] % 3 == 1)
            {
                gd.Card_dis_vert[column]++;
            }
            else if (gd.Rows_count[column] <= 45 && gd.Rows_count[column] % 4 == 2)
            {
                gd.Card_dis_vert[column]++;
            }
            else if (gd.Rows_count[column] % 10 == 2)
            {
                gd.Card_dis_vert[column]++;

            }
            int i = 0;
            foreach (var item in gd.Deck[column])
            {
                Canvas.SetTop(item.Card_Object, gd.Initial_distance_vertical + i++ * gd.Card_dis_vert[column]);
            }
        }

        private void Wrong_column_put_down(int column_before)
        {
            int i = 0;
            foreach (var card in Tmp_card_list)
            {
                Canvas.SetLeft(card, gd.Starting_card_position_X);
                // amount cards in column * vertical distance between cards + initial distance from top edge
                Canvas.SetTop(card, gd.Rows_count[column_before] * gd.Card_dis_vert[column_before] + gd.Initial_distance_vertical);
                gd.Rows_count[column_before]++;          // adding in column where user put card
                gd.Deck[column_before].Add(gd.Tmp_cards[i++]); // adding cards from tmp list to main cards container
                if (gd.Rows_count[column_before] > 11)
                    Resize_stack(column_before, gd.Rows_count[column_before] - 11);
            }
            Tmp_card_list.Clear();
            gd.Tmp_cards.Clear();
        }
        private void Stack_card_print(int i)
        {
            int left = gd.Stack_addons_x_coordinate - 5 * i;
            var tmp = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\images\\");
            tmp += "\\suit.png";
            Add_stack.Add(new Rectangle
            {
                Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(tmp)) },
                Width = gd.Card_width,
                Height = gd.Card_height
            });
            Canvas.SetBottom(Add_stack[i], gd.Stack_addons_y_coordinate);
            Canvas.SetRight(Add_stack[i], left);
            Add_stack[i].PreviewMouseDown += UserCTRL_PreviewMouseDown;
            CanvasMainWindow.Children.Add(Add_stack[i]);
        }

        private void Timer_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 1);
            dt.Tick += Ticker;
            dt.Start();
        }

        Stopwatch sw = new Stopwatch();
        string currentTime = string.Empty;

        private void Ticker(object sender, EventArgs e)
        {
            TimeSpan ts = sw.Elapsed;
            currentTime = String.Format("Czas: {0:00}:{1:00}:{2:00}",
            ts.Hours, ts.Minutes, ts.Seconds);
            Timer.Content = currentTime;
            sw.Start();
        }
    }
}

