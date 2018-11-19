using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            catch (Exception ex)
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
    }
}

