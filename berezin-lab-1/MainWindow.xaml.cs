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
            e.CanExecute = true;
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

        string FileName;
        BitmapImage img;

        private void OpenImage(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog open_diag = new OpenFileDialog();
            if (open_diag.ShowDialog() == true)
            {
                try
                {
                    img = new BitmapImage(new Uri(open_diag.FileName, UriKind.RelativeOrAbsolute));
                    ImageObject.Source = img;
                    FileName = open_diag.FileName;
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


        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            StringBuilder sb = new StringBuilder();
            bool quote = false;
            bool ignore = false;
            int offset = 0;
            int indentLength = 3;

            foreach (char ch in json)
            {
                switch (ch)
                {
                    case '"':
                        if (!ignore) quote = !quote;
                        break;
                    case '\'':
                        if (quote) ignore = !ignore;
                        break;
                }

                if (quote)
                    sb.Append(ch);
                else
                {
                    switch (ch)
                    {
                        case '{':
                        case '[':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', ++offset * indentLength));
                            break;
                        case '}':
                        case ']':
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', --offset * indentLength));
                            sb.Append(ch);
                            break;
                        case ',':
                            sb.Append(ch);
                            sb.Append(Environment.NewLine);
                            sb.Append(new string(' ', offset * indentLength));
                            break;
                        case ':':
                            sb.Append(ch);
                            sb.Append(' ');
                            break;
                        default:
                            if (ch != ' ') sb.Append(ch);
                            break;
                    }
                }
            }

            return sb.ToString().Trim();
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

        private void DetectFace(object sender, RoutedEventArgs e)
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
            byte[] byteData = GetImageAsByteArray(FileName);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = client.PostAsync(uri, content).Result;

                // Get the JSON response.
                string contentString = response.Content.ReadAsStringAsync().Result;

                // Display the JSON response.
                MessageBox.Show(JsonPrettyPrint(contentString));
            }
            
            double dpi = img.DpiX;
            double resizeFactor = (dpi > 0) ? 96 / dpi : 1;

            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Yellow);
            rect.Fill = new SolidColorBrush(Colors.Transparent);
            rect.Width = 97 * resizeFactor;
            rect.Height = 97 * resizeFactor;
            Canvas.SetLeft(rect, 954*resizeFactor);
            Canvas.SetTop(rect, 85*resizeFactor);
            ObjectField.Children.Add(rect);

            rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Yellow);
            rect.Fill = new SolidColorBrush(Colors.Transparent);
            rect.Width = 78 * resizeFactor;
            rect.Height = 78 * resizeFactor;
            Canvas.SetLeft(rect, 811 * resizeFactor);
            Canvas.SetTop(rect, 158 * resizeFactor);
            ObjectField.Children.Add(rect);

            rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Yellow);
            rect.Fill = new SolidColorBrush(Colors.Transparent);
            rect.Width = 74 * resizeFactor;
            rect.Height = 74 * resizeFactor;
            Canvas.SetLeft(rect, 491 * resizeFactor);
            Canvas.SetTop(rect, 176 * resizeFactor);
            ObjectField.Children.Add(rect);
        }
    }
}
