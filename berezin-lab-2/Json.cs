using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace berezin_lab_2
{
    public struct FaceRectangle
    {
        public double top;
        public double left;
        public double width;
        public double height;
        public override string ToString()
        {
            return "faceRectangle\n " +
                " top: [" + top.ToString() + "]\n"
                + " left: [" + left.ToString() + "]\n"
                + " width: [" + width.ToString() + "]\n"
                + " height: [" + height.ToString() + "]\n";
        }
    }

    public struct FaceAttributes
    {
        public string gender;
        public double age;
        public override string ToString()
        {
            return "faceAttributes\n"
                + "gender: " + gender + "\n"
                + " age: [" + age.ToString() + "]\n";
        }
    }
    public struct Error
    {
        public string code;
        public string message;
    }

    public struct ErrorResult
    {
        public Error error;
        public override string ToString()
        {
            return "Error\n"
                + "code: " + error.code + "\n"
                + "message: " + error.message + "\n";
        }
    }

    public struct Person
    {
        public string faceId;
        public FaceRectangle faceRectangle;
        public FaceAttributes faceAttributes;
        public override string ToString()
        {
            return "Person: \n"
                + "faceId: "+faceId+"\n"
                + faceRectangle.ToString()+ faceAttributes.ToString();
        }
    }

    public class Json
    {
        /// <summary>
        /// Formats the given JSON string by adding line breaks and indents.
        /// </summary>
        /// <param name="json">The raw JSON string to format.</param>
        /// <returns>The formatted JSON string.</returns>
        public static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;
            
            //json = json.Replace(Environment.NewLine, "").Replace("\t", "");

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

        static public object ConvertToPersons(string json)
        {
            try
            {
                ErrorResult error = JsonConvert.DeserializeObject<ErrorResult>(json);
                return error;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                try
                {
                    List<Person> personsList = JsonConvert.DeserializeObject<List<Person>>(json);
                    return personsList;

                }
                catch
                {
                    return "Can't deserialize json file";
                }
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

        static public async Task<string> GetJsonAsync(BitmapImage image)
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
            byte[] byteData = BitmapImageConverter.ImageToByteArray(image);

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
    }
}

