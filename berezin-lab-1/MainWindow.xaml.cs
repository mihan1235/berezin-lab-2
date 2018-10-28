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


namespace berezin_lab_1
{
    using System.Collections.ObjectModel;
    using System.Threading;
    using static Json;
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

        //string FileName;
        //BitmapImage img;
        ObservableCollection<PersonControl> persons_list = new ObservableCollection<PersonControl>();

        private void OpenImages(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog open_diag = new OpenFileDialog();
            // Set the file dialog to filter for graphics files.
            open_diag.Filter =
                "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|" +
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
                        BitmapImage img = new BitmapImage(new Uri(name, UriKind.RelativeOrAbsolute));
                        obj.ImageBitmap = img;
                        obj.Source = img;
                        obj.FileName = name;
                        obj.FileNameShort = System.IO.Path.GetFileName(name);
                        persons_list.Add(obj);
                    }
                    //ImageObject.Source = img;
                    //ImageObject.Source = null;
                    ObjectField.Children.Clear();
                    PersonListBox.ItemsSource = persons_list;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        /// <summary>
        /// Returns the contents of the specified file as a byte array.
        /// </summary>
        /// <param name="imageFilePath">The image file to read.</param>
        /// <returns>The byte array of the image data.</returns>
        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            using (FileStream fileStream =
                new FileStream(imageFilePath, FileMode.Open, FileAccess.Read))
            {
                BinaryReader binaryReader = new BinaryReader(fileStream);
                return binaryReader.ReadBytes((int)fileStream.Length);
            }
        }

        // Replace <Subscription Key> with your valid subscription key.
        const string subscriptionKey = "1d95ae676a35431ebd21d1c70ca22eb4";

        // NOTE: You must use the same region in your REST call as you used to
        // obtain your subscription keys. For example, if you obtained your
        // subscription keys from westus, replace "westcentralus" in the URL
        // below with "westus".
        //
        // Free trial subscription keys are generated in the westcentralus region.
        // If you use a free trial subscription key, you shouldn't need to change
        // this region.
        const string uriBase =
            "https://westeurope.api.cognitive.microsoft.com/face/v1.0/detect";

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
            if (obj.ErrorState == true)
            {
                StackPanel stack = new StackPanel();
                void AddLabelTostack(string text)
                { 
                    Label label = new Label();
                    label.Content = text;
                    label.Foreground = Brushes.Yellow;
                    stack.Children.Add(label);
                }
                AddLabelTostack("Code: " + obj.ErrorResult.error.code);
                AddLabelTostack("Message: " + obj.ErrorResult.error.message);
                Canvas.SetRight(stack, 2);
                Canvas.SetTop(stack, 2);
                ObjectField.Children.Add(stack);
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

        async Task<string> GetJsonAsync(string image_name)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false" +
                "&returnFaceAttributes=age,gender";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = GetImageAsByteArray(image_name);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();
                return contentString;
            }
        }

        private  void DetectFacesAsync(object sender, ExecutedRoutedEventArgs e)
        {
            //List<Task> task_list = new List<Task>();
            var uis = TaskScheduler.FromCurrentSynchronizationContext();
            foreach (var person_control in persons_list)
            {
                person_control.ProgressBar.Visibility = Visibility.Visible;
                var task = Task.Factory.StartNew(() =>
                {
                    string contentString = GetJsonAsync(person_control.FileName).Result;
                    person_control.JsonFile = contentString;
                    // Display the JSON response.
                    // MessageBox.Show(JsonPrettyPrint(contentString));
                    object obj = ConvertToPersons(contentString);
                    if (obj is ErrorResult)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            person_control.ErrorState = true;
                        }, CancellationToken.None, TaskCreationOptions.None, uis);
                        
                        person_control.DetectedNum = 0;
                        person_control.ErrorResult = (ErrorResult)obj;
                    }
                    if (obj is List<Person>)
                    {
                        var PersonsList = (List<Person>)obj;
                        person_control.PersonsList = PersonsList;
                        person_control.DetectedNum = PersonsList.Count;
                        Task.Factory.StartNew(() =>
                        {
                            person_control.Result = true;
                        }, CancellationToken.None, TaskCreationOptions.None, uis);
                    }
                }).ContinueWith(t =>
                {
                    person_control.ProgressBar.Visibility = Visibility.Collapsed;
                    if ((persons_list.IndexOf(person_control) == PersonListBox.SelectedIndex) &&
                    (PersonListBox.SelectedIndex != -1))
                    {
                        DrawInfoOnObjectField((PersonControl)PersonListBox.SelectedItem);
                    }
                }, CancellationToken.None, TaskContinuationOptions.None, uis);
                
            }
            //await Task.WhenAll(task_list);
        }

        private void CanDetectFaces(object sender, CanExecuteRoutedEventArgs e)
        {
            if (persons_list.Count != 0)
            {
                e.CanExecute = true;
            }
        }
    }
}
