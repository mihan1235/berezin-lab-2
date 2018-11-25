namespace berezin_lab_2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class DataBase : DbContext
    {
        // Контекст настроен для использования строки подключения "Data" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "berezin_lab_2.Data" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "Data" 
        // в файле конфигурации приложения.
        public DataBase()
            : base("name=Data")
        {
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<PersonControlData> PersonControlDatas { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class PersonControlData
    {
        public int Id
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public string FileNameShort
        {
            get;
            set;
        }

        public virtual byte[] ImageByteArray
        {
            get;
            set;
        }

        public string JsonFile
        {
            get;
            set;
        }

        public double DetectedNum
        {
            get;
            set;
        }

        public virtual ICollection<Person> PersonsList
        {
            get;
            set;
        }

        public ErrorResult ErrorResult
        {
            get;
            set;
        }

        //public bool ErrorState
        //{
        //    get;
        //    set;
        //}

        public bool Result
        {
            get;
            set;
        }
    }

    public class FaceRectangle
    {
        public int FaceRectangleId
        {
            get;
            set;
        }
        public double top { get; set; }
        public double left { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public override string ToString()
        {
            return "faceRectangle\n " +
                " top: [" + top.ToString() + "]\n"
                + " left: [" + left.ToString() + "]\n"
                + " width: [" + width.ToString() + "]\n"
                + " height: [" + height.ToString() + "]\n";
        }
    }

    public class FaceAttributes
    {
        public int FaceAttributesId
        {
            get;
            set;
        }
        public string gender { get; set; }
        public double age { get; set; }
        public override string ToString()
        {
            return "faceAttributes\n"
                + "gender: " + gender + "\n"
                + " age: [" + age.ToString() + "]\n";
        }
    }
    public class Error
    {
        public int ErrorId
        {
            get;
            set;
        }
        public string code { get; set; }
        public string message { get; set; }
    }

    public class ErrorResult
    {
        public int ErrorResultId
        {
            get;
            set;
        }
        public Error error { get; set; }
        public override string ToString()
        {
            return "Error\n"
                + "code: " + error.code + "\n"
                + "message: " + error.message + "\n";
        }
    }

    public class Person
    {
        [Key]
        public int PersonId
        {
            get;
            set;
        }
        public string faceId { get; set; }
        public virtual FaceRectangle faceRectangle { get; set; }
        public virtual FaceAttributes faceAttributes { get; set; }
        public override string ToString()
        {
            return "Person: \n"
                + "faceId: " + faceId + "\n"
                + faceRectangle.ToString() + faceAttributes.ToString();
        }
    }


}