using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FB2Snitch.BLL
{

    public class FB2Person
    {
        public FB2Person()
        {

        }

        public String firstname;
        public String middlename;
        public String lastname;
        public String nickname;
        public String homepage;
        public String email;

        public override string ToString()
        {
            String strRet = "";

            if (firstname != null && firstname.Length > 0) strRet += firstname;
            if (middlename != null && middlename.Length > 0)
            {
                if (strRet.Length > 0) strRet = strRet + " " + middlename;
                else strRet = strRet + middlename;
            }

            if (lastname != null && lastname.Length > 0)
            {
                if (strRet.Length > 0) strRet = strRet + " " + lastname;
                else strRet = strRet + lastname;
            }



            return strRet;
        }

        public Boolean Parse(XmlNodeList nodelist)
        {
            Boolean retVal = true;

            foreach (XmlNode nPerson in nodelist)
            {
                switch (nPerson.Name)
                {
                    case "first-name": { firstname = nPerson.InnerText; break; }
                    case "middle-name": { middlename = nPerson.InnerText; break; }
                    case "last-name": { lastname = nPerson.InnerText; break; }
                    case "nickname": { nickname = nPerson.InnerText; break; }
                    case "home-page": { homepage = nPerson.InnerText; break; }
                    case "email": { email = nPerson.InnerText; break; }
                    default: { retVal = false; break; }
                }
            }


            return retVal;
        }

    }

    public class FB2TitleInfo
    {
        public FB2TitleInfo()
        {
            genre = new List<String>();
            author = new List<FB2Person>();
            translator = new FB2Person();
        }

        public List<String> genre; //Жанр произведения
        public List<FB2Person> author; //Автор произведения
        public String book_title; //Название книги
        public String annotation; //Аннотация. Краткое текстовое описание книги
        public String keywords; //Список ключевых слов, с помощью которых библиотечный софт может искать книгу
        public String date; //Дата написания книги
        public String coverpage; //Картинка обложки. Содержит внутри элемент image, в который непосредственно и находится ссылка на bin-объект. Элементов image может быть несколько
        public String lang; //Язык, на котором написана книга
        public String src_lang; //Язык, на котором написан оригинал (для переводных книг)
        public FB2Person translator; //Информация о переводчике  (для переводных книг) ФИО
        public String sequence; //Серия, в которую входит книга. Допускается неограниченное число вложенных серий.

        public Boolean Parse(XmlNodeList nodelist)
        {
            Boolean retVal = true;

            foreach (XmlNode nTitleInfo in nodelist)
            {
                switch (nTitleInfo.Name)
                {
                    case "genre": { genre.Add(nTitleInfo.InnerText); break; }
                    case "author":
                        {
                            FB2Person fb2person = new FB2Person();
                            fb2person.Parse(nTitleInfo.ChildNodes);
                            author.Add(fb2person);
                            break;
                        }
                    case "book-title": { book_title = nTitleInfo.InnerText; break; }
                    case "annotation": { annotation = nTitleInfo.InnerText; break; }
                    case "keywords": { keywords = nTitleInfo.InnerText; break; }
                    case "date": { date = nTitleInfo.InnerText; break; }
                    case "coverpage":
                        {
                            /*
                             * обложка книги. Внутри может содержать только тэг <image/>. От нуля до одного вхождения. 
                             * (значок # говорит, что эта ссылка локальная, то есть адресует в пределах документа).
                             */
                            XmlNodeList nLImage = nTitleInfo.ChildNodes;
                            foreach (XmlNode nImage in nLImage)
                            {
                                //XmlNodeList nLAttr = nImage.Attributes.Count;
                                if (nImage.Name == "image")
                                    for (int i = 0; i < nImage.Attributes.Count; i++)
                                    {
                                        if (nImage.Attributes[i].LocalName == "href")
                                        {
                                            coverpage = nImage.Attributes[i].Value;
                                            //удаляем значек # в начале
                                            if (coverpage.IndexOf("#") == 0) coverpage = coverpage.Substring(1);
                                        }
                                    }

                            }

                            break;
                        }
                    case "lang": { lang = nTitleInfo.InnerText; break; }
                    case "src-lang": { src_lang = nTitleInfo.InnerText; break; }
                    case "translator": { translator.Parse(nTitleInfo.ChildNodes); break; }
                    case "sequence": 
                        {
                            if (nTitleInfo.Attributes.Count != 0)
                            {
                                for (int i = 0; i < nTitleInfo.Attributes.Count; i++)
                                {
                                    switch (nTitleInfo.Attributes[i].Name)
                                    {
                                        case "name":
                                            {
                                                if (!String.IsNullOrEmpty(sequence))
                                                    sequence += ("; " + nTitleInfo.Attributes[i].Value);
                                                else
                                                    sequence = nTitleInfo.Attributes[i].Value;
                                                break;
                                            }
                                        case "number":
                                            {
                                                if (!String.IsNullOrEmpty(sequence))
                                                {
                                                    sequence += (", №" + nTitleInfo.Attributes[i].Value);
                                                }
                                                break;
                                            }

                                    }

                                }
                            }
                            //sequence = nTitleInfo.InnerText; 
                            break; 
                        }
                    default: { retVal = false; break; }
                }
            }

            return (retVal);
        }

    }

    public class FB2DocumentInfo
    {
        public FB2DocumentInfo()
        {
            history = new List<String>();
            author = new FB2Person();
        }

        public FB2Person author; //Cоздатель электронной книги
        public String program_used; //Программное обеспечение, использовавшееся при создании книги.
        public String date; //Дата создания файла
        public String src_url; //Ссылка на сайт, если исходный текст книги был скачан из Интернета
        public String src_ocr; //Информация о людях, которые сканировали (набирали) и вычитывали книгу
        public String id; //Уникальный идентификационный номер книги
        public String version; //Номер версии файла
        public List<String> history; //История изменений,  вносившихся в файл

        public Boolean Parse(XmlNodeList nodelist)
        {
            Boolean retVal = true;

            foreach (XmlNode nDocumentInfo in nodelist)
            {
                switch (nDocumentInfo.Name)
                {
                    case "author": { author.Parse(nDocumentInfo.ChildNodes); break; }
                    case "program-used": { program_used = nDocumentInfo.InnerText; break; }
                    case "date": { date = nDocumentInfo.InnerText; break; }
                    case "src-url": { src_url = nDocumentInfo.InnerText; break; }
                    case "src-ocr": { src_ocr = nDocumentInfo.InnerText; break; }
                    case "id": { id = nDocumentInfo.InnerText; break; }
                    case "version": { version = nDocumentInfo.InnerText; break; }
                    case "history": { foreach (XmlNode nHistory in nDocumentInfo.ChildNodes) history.Add(nHistory.InnerText); break; }
                    default: { retVal = false; break; }
                }
            }

            return retVal;
        }
    }

    public class FB2PublishInfo
    {
        public FB2PublishInfo()
        {
        }

        public String bookname; //Название бумажного оригинала
        public String publisher; //Название издательства, выпустившего бумажный оригинал.
        public String city; //Город, в котором был издан бумажный оригинал

        //Год выхода бумажного оригинала
        public String year; //Год выхода бумажного оригинала

        //ISBN-код бумажного оригинала
        public String isbn; //ISBN-код бумажного оригинала
        public String sequence; //Серия, в которую входит книга. Допускается неограниченное число вложенных серий.


        public Boolean Parse(XmlNodeList nodelist)
        {
            Boolean retVal = true;
            foreach (XmlNode nPublishInfo in nodelist)
            {
                switch (nPublishInfo.Name)
                {
                    case "book-name": //Название бумажного оригинала.
                        {
                            bookname = nPublishInfo.InnerText;
                            break;
                        }
                    case "publisher":
                        {
                            publisher = nPublishInfo.InnerText;
                            break;
                        }
                    case "city":
                        {
                            city = nPublishInfo.InnerText;
                            break;
                        }
                    case "year":
                        {
                            year = nPublishInfo.InnerText;
                            break;
                        }
                    case "isbn":
                        {
                            isbn = nPublishInfo.InnerText;
                            break;
                        }
                    case "sequence":
                        {
                            sequence = nPublishInfo.InnerText;
                            break;
                        }
                    default:
                        {
                            retVal = false;
                            break;
                        }
                }
            }

            return retVal;
        }


    }

    public class FB2Description
    {
        public FB2Description()
        {
            titleinfo = new FB2TitleInfo();
            srctitleinfo = new FB2TitleInfo();
            documentinfo = new FB2DocumentInfo();
            publishinfo = new FB2PublishInfo();
        }

        public FB2TitleInfo titleinfo;    // Содержит базовую информацию о книге (заголовок, информация об авторе и переводчике, аннотация, вхождение в серию и т.д.)
        public FB2TitleInfo srctitleinfo; //Cодержит базовую информацию о книге-оригинале (для переводных книг)
        public FB2DocumentInfo documentinfo; //Информация о самом файле FictionBook — кем, когда  и с помощью каких программных средств создана данная электронная книга
        public FB2PublishInfo publishinfo;  //Информация о бумажном оригинале книги, если таковой существовал в природе

        public Boolean Parse(XmlNodeList nodelist)
        {
            Boolean retVal = true;

            foreach (XmlNode nDescription in nodelist)
            {
                switch (nDescription.Name)
                {
                    case "title-info":
                        {
                            XmlNodeList nLTitleInfo = nDescription.ChildNodes;
                            titleinfo.Parse(nLTitleInfo);
                            break;
                        }
                    case "src-title-info":
                        {
                            XmlNodeList nLSrcTitleInfo = nDescription.ChildNodes;
                            srctitleinfo.Parse(nLSrcTitleInfo);
                            break;
                        }
                    case "document-info":
                        {
                            XmlNodeList nLDocumentInfo = nDescription.ChildNodes;
                            documentinfo.Parse(nLDocumentInfo);
                            break;
                        }
                    case "publish-info": //Информация о бумажном оригинале книги, если таковой существовал в природе
                        {
                            XmlNodeList nLPublishInfo = nDescription.ChildNodes;
                            publishinfo.Parse(nLPublishInfo);
                            break;
                        }
                    case "custom-info":
                        {
                            XmlNodeList nLCustomInfo = nDescription.ChildNodes;
                            foreach (XmlNode nCustomInfo in nLCustomInfo)
                            {
                                //Console.WriteLine("  <" + nCustomInfo.Name + ">" + " - " + nCustomInfo.InnerText);
                            }
                            break;
                        }
                }

            }

            return retVal;
        }
    }

    class FB2Image
    {
        public FB2Image(string id, string type, string data)
        {
            this.id = id;
            this.type = type;
            this.data = data;
        }

        public string id;   // идентификатор изображения
        public string type; // тип изображения - jpeg или png
        public string data; // само изображение в формате Base64
    }

    class FB2Binary
    {
        private List<FB2Image> images; //Жанр произведения

        public FB2Binary()
        {
            images = new List<FB2Image>();
        }

        public void Add(XmlNode node)
        {
            //Раздел "binary" должен содержать 2 обязательных аттрибута "id" и "content-type"
            //Из картинок поддерживаются форматы JPG (тип image/jpeg) и PNG (тип image/png) - поле "content-type"
            //Порядок аттрибутов может быть любой!!!!
            if (node.Attributes.Count >= 2)
            {
                if (node.Attributes[0].Name == "id" && node.Attributes[1].Name == "content-type" && (node.Attributes[1].Value == "image/jpeg" || node.Attributes[1].Value == "image/png"))
                {
                    if (node.ChildNodes.Count != 0)
                    {
                        FB2Image fb2image = new FB2Image(node.Attributes[0].Value, node.Attributes[1].Value, node.FirstChild.Value);
                        images.Add(fb2image);
                    }
                
                }
                else
                    if (node.Attributes[1].Name == "id" && node.Attributes[0].Name == "content-type" && (node.Attributes[0].Value == "image/jpeg" || node.Attributes[0].Value == "image/png"))
                    {
                        if (node.ChildNodes.Count != 0)
                        {
                            FB2Image fb2image = new FB2Image(node.Attributes[1].Value, node.Attributes[0].Value, node.FirstChild.Value);
                            images.Add(fb2image);
                        }

                    }

            }
        }

        public int Count
        {
            get
            {
                return images.Count;
            }
        }


        public FB2Image this[string id]
        {
            get
            {
                if (String.IsNullOrEmpty(id)) return null;
                FB2Image image = images.Find(x => x.id == id);
                if (image != null) return image; else return null;
            }
        }

        public FB2Image this[int num]
        {
            get
            {
                if (num > Count) return null;
                FB2Image image = images[num];
                if (image != null) return image; else return null;
            }
        }

    }

    class FB2Manager
    {
        static public FB2Description ReadDecription(string filename)
        {
            FB2Description FB2Desc = new FB2Description();
            XmlDocument doc = new XmlDocument();

            try
            {
                //using (FileStream fStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                //{
                TimeSpan ts_load;
                TimeSpan ts_parse;
                string elapsedTime;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                doc.LoadXml(System.IO.File.ReadAllText(filename));
                ts_load = stopWatch.Elapsed;
                stopWatch.Restart();
                //doc.Load(fStream);

                Encoding fileEncoding = null;
                    // The first child of a standard XML document is the XML declaration.
                    // The following code assumes and reads the first child as the XmlDeclaration.
                    if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    {
                        // Get the encoding declaration.
                        fileEncoding = Encoding.GetEncoding(((XmlDeclaration)doc.FirstChild).Encoding);
                    }
                    if (fileEncoding != null)
                    {
                        doc.LoadXml(System.IO.File.ReadAllText(filename, fileEncoding));
                        Console.WriteLine(fileEncoding.ToString());
                    }

                    XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("description");
                    if (nodes.Count >= 1)
                        FB2Desc.Parse(nodes[0].ChildNodes);
                    else
                        throw new FB2BaseException("Ошибка загрузки fb2. Элемент <description> не найден");

                ts_parse = stopWatch.Elapsed;
                stopWatch.Stop();

                Console.WriteLine("--- read fb2 description");
                Console.WriteLine(String.Format("--- load {0:00}.{1:00} - parse {2:00}.{3:00}",
                                 ts_load.Seconds, ts_load.Milliseconds, ts_parse.Seconds, ts_parse.Milliseconds));
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("ERROR! {0} - {1}", filename, ex.Message));
                throw new FB2BaseException("Ошибка загрузки fb2."); ;
            }

            return (FB2Desc);
        }

        static public FB2Binary ReadBinary(string filename)
        {
            FB2Binary FB2Bin = new FB2Binary();

            XmlDocument doc = new XmlDocument();

            doc.LoadXml(System.IO.File.ReadAllText(filename));

            Encoding fileEncoding = null;
            // The first child of a standard XML document is the XML declaration.
            // The following code assumes and reads the first child as the XmlDeclaration.
            if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                // Get the encoding declaration.
                fileEncoding = Encoding.GetEncoding(((XmlDeclaration)doc.FirstChild).Encoding);
            }
            if (fileEncoding != null)
            {
                doc.LoadXml(System.IO.File.ReadAllText(filename, fileEncoding));
            }

            XmlNodeList nodes = doc.DocumentElement.GetElementsByTagName("binary");

            foreach (XmlNode node in nodes)
            {
                FB2Bin.Add(node);
            }


            return (FB2Bin);       
        
        }
    }


}
