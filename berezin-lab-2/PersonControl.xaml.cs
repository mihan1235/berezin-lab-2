using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace berezin_lab_2
{
    /// <summary>
    /// Логика взаимодействия для PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl, INotifyPropertyChanged
    {
        public PersonControl()
        {
            InitializeComponent();
            PersonsList = new List<Person>();
        }

        public string FileName
        {
            get;
            set;
        }

        public string FileNameShort{
            get;
            set;
        }

        public BitmapImage ImageBitmap
        {
            get;
            private set;
        }

        public double ResizeFactor
        {
            get
            {
                double dpi = ImageBitmap.DpiX;
                return (dpi > 0) ? 96 / dpi : 1;
            }
        }
        public ImageSource Source {
            get
            {
                return ImageBitmap;
            }
            set
            {
                ImageBitmap = (BitmapImage)value;
            }
        }

        public String JsonFile
        {
            get;
            set;
        }

        bool error_state = false;

        public bool Result
        {
            set
            {
                if (value == true)
                {
                    ErrorBorder.Visibility = Visibility.Visible;
                    ErrorBorder.BorderBrush = Brushes.Green;
                }
                else
                {
                    ErrorState = true;
                }
            }
        }

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
                    ErrorBorder.BorderBrush = Brushes.Red;
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

        double detected_num = 0;
        public double DetectedNum
        {
            get
            {
                return detected_num;
            }
            set
            {
                detected_num = value;
                OnPropertyChanged("DetectedNum");
            }
        }

        public  ErrorResult ErrorResult
        {
            get;
            set;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
