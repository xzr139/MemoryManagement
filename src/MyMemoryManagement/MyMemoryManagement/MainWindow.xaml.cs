using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace MyMemoryManagement
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Manager Mgr;

        

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Mgr = new Manager();
        }

        private void Auto_Click(object sender, RoutedEventArgs e)
        {
            if ((string)this.Auto.Content == "自动")
            {
                Mgr.Auto();
                this.Auto.Content = "暂停";
                this.Step.IsEnabled = false;
                this.label2.Foreground = Brushes.LightGray;
                this.label1.Foreground = Brushes.LightGray;
            }
            else
            {
                Mgr.Auto();
                this.Auto.Content = "自动";
                this.Step.IsEnabled = true;
                this.label2.Foreground = Brushes.LightGray;
                this.label1.Foreground = Brushes.LightGray;
            }
        }

        private void Step_Click(object sender, RoutedEventArgs e)
        {
            Mgr.Step();
            this.label2.Foreground = Brushes.LightGray;
            this.label1.Foreground = Brushes.LightGray;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Mgr.Clear();
            this.Auto.Content = "自动";
            this.Step.IsEnabled = true;
            this.Auto.IsEnabled = true;
            this.label2.Foreground = Brushes.Black;
            this.label1.Foreground = Brushes.Black;
        }

        private void label2_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Mgr.IsInprograss)
            {
                this.label2.Visibility = Visibility.Hidden;
                this.Method.Visibility = Visibility.Visible;
            }
        }

        private void Method_MouseLeave(object sender, MouseEventArgs e)
        {
            this.label2.Visibility = Visibility.Visible;
            this.Method.Visibility = Visibility.Hidden;
        }

        private void label1_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Mgr.IsInprograss)
            {
                this.label1.Visibility = Visibility.Hidden;
                this.Number.Visibility = Visibility.Visible;
            }
        }

        private void Number_MouseLeave(object sender, MouseEventArgs e)
        {
            this.label1.Visibility = Visibility.Visible;
            this.Number.Visibility = Visibility.Hidden;
        }

        private void mask_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender.Equals(this.Mask1))
                this.Mask1.Background = Brushes.SkyBlue;
            else
                if (sender.Equals(this.Mask2))
                    this.Mask2.Background = Brushes.SkyBlue;
                else
                    if (sender.Equals(this.Mask3))
                        this.Mask3.Background = Brushes.SkyBlue;
                    else
                        if (sender.Equals(this.Mask4))
                            this.Mask4.Background = Brushes.SkyBlue;
        }

        private void mask_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender.Equals(this.Mask1))
                this.Mask1.Background = Brushes.Transparent;
            else
                if (sender.Equals(this.Mask2))
                    this.Mask2.Background = Brushes.Transparent;
                else
                    if (sender.Equals(this.Mask3))
                        this.Mask3.Background = Brushes.Transparent;
                    else
                        if (sender.Equals(this.Mask4))
                            this.Mask4.Background = Brushes.Transparent;
        }

        private void Method_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.label2 !=null)
                switch (this.Method.SelectedIndex)
                {
                    case 0:
                        this.label2.Content = "FIFO";
                        break;
                    case 1:
                        this.label2.Content = "LRU";
                        break;
                }
        }

        private void Number_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.label1 != null)
                switch (this.Number.SelectedIndex)
                {
                    case 0:
                        this.label1.Content = "RAND";
                        break;
                    case 1:
                        this.label1.Content = "1:2:1";
                        break;
                }
        }
    }
}
