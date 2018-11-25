using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
using System.Collections.ObjectModel;
using System.Threading;

namespace berezin_lab_2
{
    
    using static BitmapImageConverter;
    using static Json;

    
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataBase db = new DataBase();
        public MainWindow()
        {
            InitializeComponent();
                foreach (PersonControlData obj in db.PersonControlDatas)
                {
                    PersonControl pc = new PersonControl();
                    pc.Data = obj;
                    persons_list.Add(pc);
                    //MessageBox.Show(pc.ToString());
                }
            PersonListBox.ItemsSource = persons_list;
        }

        private void CanSaveImage(object sender, CanExecuteRoutedEventArgs e)
        {
            if (PersonListBox.SelectedIndex != -1)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
            
        }

        private void SaveImage(object sender, ExecutedRoutedEventArgs e)
        {
            
            SaveFileDialog save_diag = new SaveFileDialog();
            if (save_diag.ShowDialog() == true)
            {
                try
                {
                    var bmp = new RenderTargetBitmap(
                        (int)cs.ActualWidth, (int)cs.ActualHeight, 96, 96, PixelFormats.Default);
                    bmp.Render(cs);
                    var enc = new PngBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(bmp));
                    using (var s = File.Create(save_diag.FileName))
                        enc.Save(s);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private void CanOpenImage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        ObservableCollection<PersonControl> persons_list = new ObservableCollection<PersonControl>();

        private void OpenImages(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog open_diag = new OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            open_diag.Filter =
                "Images (*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG)|*.BMP;*.JPG;*.GIF;*.PNG;*.JPEG|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            open_diag.Multiselect = true;
            if (open_diag.ShowDialog() == true)
            {
                try
                {
                    string[] file_names = open_diag.FileNames;
                    persons_list.Clear();
                    foreach (var name in file_names)
                    {
                        PersonControl obj = new PersonControl();
                        //BitmapImage img = new BitmapImage(new Uri(name, UriKind.RelativeOrAbsolute));
                        //BitmapImage img = ByteArrayToImage(GetImageAsByteArray(name));
                        //obj.Source = img;
                        obj.ImageByteArray = GetImageAsByteArray(name);
                        obj.FileName = name;
                        obj.FileNameShort = System.IO.Path.GetFileName(name);
                        persons_list.Add(obj);
                    }
                    ObjectField.Children.Clear();
                    PersonListBox.ItemsSource = persons_list;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DrawInfoOnObjectField(PersonControl obj)
        {
            
            ObjectField.Children.Clear();
            if (obj.PersonsList.Count != 0)
            {
                double resizeFactor = obj.ResizeFactor;

                foreach (var person in obj.PersonsList)
                {
                    Rectangle rect = new Rectangle();
                    rect.Stroke = new SolidColorBrush(Colors.Yellow);
                    rect.Fill = new SolidColorBrush(Colors.Transparent);
                    rect.Width = person.faceRectangle.width * resizeFactor;
                    rect.Height = person.faceRectangle.height * resizeFactor;
                    Canvas.SetLeft(rect, person.faceRectangle.left * resizeFactor);
                    Canvas.SetTop(rect, person.faceRectangle.top * resizeFactor);
                    ObjectField.Children.Add(rect);

                    Label label = new Label();
                    label.Content = "age: " + person.faceAttributes.age.ToString() +
                        "\ngender: " + person.faceAttributes.gender;
                    Canvas.SetLeft(label, person.faceRectangle.left * resizeFactor);
                    Canvas.SetTop(label, person.faceRectangle.top * resizeFactor +
                        person.faceRectangle.height * resizeFactor);
                    //label.Background = Brushes.White;
                    label.Foreground = Brushes.Yellow;
                    ObjectField.Children.Add(label);
                }
            }
        }

        private void SelectedPersonControlEvent(object sender, RoutedEventArgs e)
        {
            PersonControl obj = (PersonControl)PersonListBox.SelectedItem;
            if (PersonListBox.SelectedIndex != -1)
            {
                DrawInfoOnObjectField((PersonControl)PersonListBox.SelectedItem);
            }
        }

        
        CancellationTokenSource TokenSource = new CancellationTokenSource();

        private void DetectFacesAsync(object sender, ExecutedRoutedEventArgs e)
        {
            //List<Task> task_list = new List<Task>();
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            foreach (var person_control in persons_list)
            {
                CancellationToken token = TokenSource.Token;

                var task = Task.Factory.StartNew(async () =>
                {
                    void check_token_cancel()
                    {
                        if (token.IsCancellationRequested)
                        {
                            person_control.ProgressBar.Visibility = Visibility.Collapsed;
                            token.ThrowIfCancellationRequested();
                        }
                    }
                    person_control.ProgressBar.Visibility = Visibility.Visible;
                    check_token_cancel();
                    string contentString = await GetJsonAsync(person_control.ImageByteArray);
                    check_token_cancel();
                    person_control.JsonFile = contentString;
                    // Display the JSON response.
                    //MessageBox.Show(JsonPrettyPrint(contentString));
                    check_token_cancel();
                    object obj = ConvertToPersons(contentString);
                    check_token_cancel();
                    if (obj is ErrorResult)
                    {
                        var error = (ErrorResult)obj;
                        person_control.ErrorResult = error;
                        //person_control.ErrorState = true;
                        person_control.Result = false;
                    }
                    if (obj is List<Person>)
                    {
                        var PersonsList = (List<Person>)obj;
                        person_control.PersonsList = PersonsList;
                        person_control.DetectedNum = PersonsList.Count;
                        person_control.Result = true;
                        using (DataBase db = new DataBase())
                        {
                            var data = person_control.Data;
                            db.PersonControlDatas.Add(data);
                            db.SaveChanges();
                            person_control.Id=data.Id;
                           
                        }                        
                    }
                    person_control.ProgressBar.Visibility = Visibility.Collapsed;
                    if ((persons_list.IndexOf(person_control) == PersonListBox.SelectedIndex) &&
                    (PersonListBox.SelectedIndex != -1))
                    {
                        DrawInfoOnObjectField((PersonControl)PersonListBox.SelectedItem);
                    }
                    person_control.DrawOnImage();

                    //using (DataBase db = new DataBase())
                    //{
                    //    var users = db.PersonControlDatas;
                    //    String str = "Список объектов:";
                    //    foreach (PersonControlData u in users)
                    //    {
                    //        str += u.Id + "\n";
                    //    }
                    //    MessageBox.Show(str);
                    //}
                }, token, TaskCreationOptions.None, context);               
            }
        }

        private void CanDetectFaces(object sender, CanExecuteRoutedEventArgs e)
        {
            if (persons_list.Count != 0)
            {
                e.CanExecute = true;
            }
        }

        private void CancelTasks(object sender, ExecutedRoutedEventArgs e)
        {
            TokenSource.Cancel();
        }

        private void CanCancelTasks(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DetectNewOrError(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CanDetectNewOrError(object sender, CanExecuteRoutedEventArgs e)
        {
            if (persons_list.Count != 0)
            {
                e.CanExecute = true;
            }
        }

        private void ClearDataBase(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CanClearDataBase(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void DO_remove_obj(object sender, RoutedEventArgs e)
        {

        }
    }
}
