using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        byte[] _ImageByteArray;

        public byte[] ImageByteArray
        {
            get
            {
                return _ImageByteArray;
            }
            set
            {
                _ImageByteArray = value;
                ImageBitmap = BitmapImageConverter.ByteArrayToImage(_ImageByteArray);
                Source = ImageBitmap;
            }
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
            get;
            private set;
        }

        public string JsonFile
        {
            get;
            set;
        }

        public void DrawOnImage()
        {
            ObjectField1.Children.Clear();
            if (PersonsList.Count != 0)
            {
                double resizeFactor = ResizeFactor;

                foreach (var person in PersonsList)
                {
                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(Colors.Yellow);
                    rect.Fill = new SolidColorBrush(Colors.Transparent);
                    rect.Width = person.faceRectangle.width * resizeFactor;
                    rect.Height = person.faceRectangle.height * resizeFactor;
                    Canvas.SetLeft(rect, person.faceRectangle.left * resizeFactor);
                    Canvas.SetTop(rect, person.faceRectangle.top * resizeFactor);
                    ObjectField1.Children.Add(rect);

                    Label label = new Label();
                    label.Content = "age: " + person.faceAttributes.age.ToString() +
                        "\ngender: " + person.faceAttributes.gender;
                    Canvas.SetLeft(label, person.faceRectangle.left * resizeFactor);
                    Canvas.SetTop(label, person.faceRectangle.top * resizeFactor +
                        person.faceRectangle.height * resizeFactor);
                    //label.Background = Brushes.White;
                    label.Foreground = Brushes.Yellow;
                    ObjectField1.Children.Add(label);
                }
            }
        }

        bool error_state = false;
        bool result = false;
        public bool Result
        {
            get
            {
                return result;
            }
            set
            {
                if (value == true)
                {
                    //ErrorBorder.Visibility = Visibility.Visible;
                    //ErrorBorder.BorderBrush = Brushes.Green;
                    DetectedDock.Visibility = Visibility.Visible;
                }
                else
                {
                    ErrorState = true;
                    DetectedDock.Visibility = Visibility.Collapsed;
                }
                result = value;
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
                    ErrorStack.Visibility = Visibility.Visible;
                    ErrorCodeText.Text = ErrorResult.error.code;
                    ErrorMessageText.Text = ErrorResult.error.message;

                }
                else
                {
                    ErrorBorder.Visibility = Visibility.Collapsed;
                    ErrorStack.Visibility = Visibility.Collapsed;
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
