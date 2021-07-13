using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Solitaire_spider
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private GameData gd1 { get; set; }
        public Window1(GameData gd)
        {
            InitializeComponent();
            string path = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory() + "\\images\\");
            path += "\\start1.jpg";
            Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(path)) };
            gd1 = gd;
        }

        private void Latwy_Click(object sender, RoutedEventArgs e)
        {
            gd1.DiffLv(13);
            Close();
        }

        private void Sredni_Click(object sender, RoutedEventArgs e)
        {
            gd1.DiffLv(26);
            Close();
        }

        private void Trudny_Click(object sender, RoutedEventArgs e)
        {
            gd1.DiffLv(52);
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Celem gry Pasjans Pająk jest usunięcie wszystkich kart ze stołu poprzez utworzenie \"zestawów\" kart.\n" +
                "Każdy zestaw musi być ułożony w porządku malejącym, od króla do asa.\n\n" +
                "W grze Pasjans Pająk za najwyższą kartę uznawany jest król,\na za najniższą as. Poprawny zestaw kart to:\nkról, dama, walet, 10, 9, 8, 7, 6, 5, 4, 3, 2, as.\n\n" +
                "Poziomy trudności:\n" +
                "Łatwy: jeden kolor,\nŚredni: dwa kolory,\nTrudny: cztery kolory.\n\n" +
                "Jeżeli w grze występuje więcej niż jeden kolor kart, aby zestaw został uznany za kompletny i usunięty ze stołu, wszystkie karty w zestawie muszą być jednego koloru.\n\n" +
                "Jeżeli w grze występuje więcej niż jeden kolor kart, to można mieszać\nze sobą kolory kart w odpowiedniej sekwencji. Należy jednak pamiętać, że aby zestaw został usunięty " +
                "ze stołu, musi zostać złożony z kart w tym samym kolorze.\n\n" +
                "Można przesuwać kilka kart jednocześnie, jeśli są ułożone\nw odpowiedniej kolejności oraz są w jednakowym kolorze.\n\n" +
                "Jeżeli skończą się możliwości ruchów, należy nacisnąć jeden\nz kompletów kart w prawym dolnym rogu, aby rozdać nowy rząd kart. Nie można rozdać nowych kart, jeśli któraś kolumna jest pusta.");
        }
    }
}
