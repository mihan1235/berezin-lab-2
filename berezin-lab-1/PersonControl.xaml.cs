using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace berezin_lab_1
{
    /// <summary>
    /// Логика взаимодействия для PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl
    {
        public PersonControl()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get;
            set;
        }

        public BitmapImage ImageBitmap
        {
            get;
            set;
        }

        public double ResizeFactor
        {
            get
            {
                double dpi = ImageBitmap.DpiX;
                return (dpi > 0) ? 96 / dpi : 1;
            }
        }

        ImageSource source;
        public ImageSource Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
                //PersonImage.Source = value;
            }
        }

        public String JsonFile
        {
            get;
            set;
        }

        bool error_state = false;

        public bool ErrorState
        {
            get
            {
                return error_state;
            }
            set
            {
                error_state = value;
                if (error_state == true)
                {
                    ErrorBorder.Visibility = Visibility.Visible;
                    ErrorBorder.BorderBrush = Brushes.Green;
                }
                else
                {
                    ErrorBorder.Visibility = Visibility.Collapsed;
                }
            }
        }

        public List<Person> PersonsList
        {
            get;
            set;
        }

        public double DetectedNum
        {
            get;
            set;
        }
    }
}
