using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Magazynierzy_pomylki.Helpers
{
    public class InputBox
    {

        Window Box = new Window();//window for the inputbox
        System.Windows.Media.FontFamily font = new System.Windows.Media.FontFamily("Tahoma");//font for the whole inputbox
        int FontSize = 30;//fontsize for the input
        StackPanel sp1 = new StackPanel();// items container
        string title = "InputBox";//title as heading
        string boxcontent;//title
        string defaulttext = string.Empty;//default textbox content
        string errormessage = "Niepoprawna wiadomość";//error messagebox content
        string errortitle = "Błąd";//error messagebox heading title
        string okbuttontext = "OK";//Ok button content
        string CancelButtonText = "Anuluj";
        System.Windows.Media.Brush BoxBackgroundColor = System.Windows.Media.Brushes.White;// Window Background
        System.Windows.Media.Brush InputBackgroundColor = System.Windows.Media.Brushes.Ivory;// Textbox Background
        bool clicked = false;
        bool clickedOk = false;
        TextBox input = new TextBox();
        Button ok = new Button();
        Button cancel = new Button();
        bool inputreset = false;

        public InputBox(string content)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            windowdef();
        }

        public InputBox(string content, string Htitle, string DefaultText)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }
            try
            {
                defaulttext = DefaultText;
            }
            catch
            {
                DefaultText = "Error!";
            }
            windowdef();
        }

        public InputBox(string content, string Htitle, string Font, string defaultText, int Fontsize)
        {
            try
            {
                boxcontent = content;
            }
            catch { boxcontent = "Error!"; }
            try
            {
                font = new System.Windows.Media.FontFamily(Font);
            }
            catch { font = new System.Windows.Media.FontFamily("Tahoma"); }
            try
            {
                title = Htitle;
            }
            catch
            {
                title = "Error!";
            }

            defaulttext = defaultText;

            if (Fontsize >= 1)
                FontSize = Fontsize;
            windowdef();
        }

        private void windowdef()// window building - check only for window size
        {
            Box.Height = 250;// Box Height
            Box.Width = 400;// Box Width
            Box.Background = BoxBackgroundColor;
            Box.Title = title;
            Box.Content = sp1;
            Box.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            TextBlock content = new TextBlock();
            content.TextWrapping = TextWrapping.Wrap;
            content.Background = null;
            content.HorizontalAlignment = HorizontalAlignment.Center;
            content.Text = boxcontent;
            content.FontFamily = font;
            content.FontSize = FontSize;
            sp1.Children.Add(content);

            input.TextWrapping = TextWrapping.Wrap;
            input.FontFamily = font;
            input.FontSize = FontSize;
            input.HorizontalAlignment = HorizontalAlignment.Center;
            input.Text = defaulttext;
            input.MinWidth = 350;
            input.MaxWidth = 400;
            input.MinHeight = 100;
            input.KeyDown += input_KeyDown;
            input.Margin = new Thickness(0, 20, 0, 20);

            sp1.Children.Add(input);


            ok.Width = 70;
            ok.Height = 30;
            ok.Click += ok_Click;
            ok.Content = okbuttontext;
            ok.HorizontalAlignment = HorizontalAlignment.Center;


            WrapPanel gboxContent = new WrapPanel();
            gboxContent.HorizontalAlignment = HorizontalAlignment.Center;

            sp1.Children.Add(gboxContent);
            gboxContent.Children.Add(ok);

            input.Focus();

        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && clickedOk == false)
            {
                e.Handled = true;
                ok_Click(input, null);
            }

            if (e.Key == Key.Escape)
            {
                cancel_Click(input, null);
            }
        }

        void ok_Click(object sender, RoutedEventArgs e)
        {
            clickedOk = true;
            Box.Close();
            clickedOk = false;
        }

        void cancel_Click(object sender, RoutedEventArgs e)
        {
            Box.Close();
        }

        public string ShowDialog()
        {
            Box.ShowDialog();
            return input.Text;
        }
    }
}
