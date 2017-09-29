using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    public struct BookInfo
    {
        private int     _id;
        private string _bookname;
        private string _arcfilename;
        private string _uniquifilename;
        private string _originalfilename;
        private string _md5;

        public int id { get { return (_id); } }
        public string bookname { get { return (_bookname); } }
        public string arcfilename { get { return (_arcfilename); } }
        public string uniquifilename { get { return (_uniquifilename); } }
        public string originalfilename { get { return (_originalfilename); } }
        public string md5 { get { return (_md5); } }


        public BookInfo(string bookname, string arcfilename, string uniquifilename, string originalfilename, string md5)
        {
            _id = -1;
            _bookname = bookname;
            _arcfilename = arcfilename;
            _uniquifilename = uniquifilename;
            _originalfilename = originalfilename;
            _md5 = md5;
        }

        public BookInfo(int id, string bookname, string arcfilename, string uniquifilename, string originalfilename, string md5)
        {
            _id = id;
            _bookname = bookname;
            _arcfilename = arcfilename;
            _uniquifilename = uniquifilename;
            _originalfilename = originalfilename;
            _md5 = md5;
        }

        public BookInfo(int id)
        {
            _id = id;
            _bookname = string.Empty;
            _arcfilename = string.Empty;
            _uniquifilename = string.Empty;
            _originalfilename = string.Empty;
            _md5 = string.Empty;
        }

    }

    public struct AuthorInfo
    {
        private int _id;
        private string _firstname;
        private string _middlename;
        private string _lastname;

        public int id { get { return (_id); } }
        public string firstname { get { return (_firstname); } }
        public string middlename { get { return (_middlename); } }
        public string lastname { get { return (_lastname); } }


        public AuthorInfo(string firstname, string middlename, string lastname)
        {
            _id = -1;
            _firstname = firstname;
            _middlename = middlename;
            _lastname = lastname;
        }

        public AuthorInfo(int id, string firstname, string middlename, string lastname)
        {
            _id = id;
            _firstname = firstname;
            _middlename = middlename;
            _lastname = lastname;
        }

        public AuthorInfo(int id)
        {
            _id = id;
            _firstname = string.Empty;
            _middlename = string.Empty;
            _lastname = string.Empty;
        }

        public override string ToString()
        {
            string retstring = lastname;

            if (!string.IsNullOrEmpty(retstring)) { retstring += " "; }
            retstring += firstname;

            if (!string.IsNullOrEmpty(retstring)) { retstring += " "; }
            retstring += middlename;

            return (retstring);
        }
    }

    public struct GenreInfo
    {
        private int _id;
        private string _genre;

        public int id { get { return (_id); } }
        public string genre { get { return (_genre); } }


        public GenreInfo(string genre)
        {
            _id = -1;
            _genre = genre;
        }

        public GenreInfo(int id, string genre)
        {
            _id = id;
            _genre = genre;
        }

        public GenreInfo(int id)
        {
            _id = id;
            _genre = string.Empty;
        }

        public override string ToString()
        {
            return (_genre);
        }
   }

    public class CatalogManager
    {

        //Добавление книги в каталог
        public BookInfo AddBookToCatalog(string fb2filename, string arcstoragefolder)
        {
            string filefullname   = fb2filename;
            string fileshortname = FileUtils.GetShotFileName(filefullname);
            string uniquifilename = FileUtils.GenerateUniqueFileName("fb2");
            string arcshortfilename = string.Empty;
            string filemd5hash = MD5Hash.GetFileHash(filefullname);

            //1. Проверяем что это именно fb2 файл и сваливаем если это не так
            FB2Description fb2desc = FB2Manager.ReadDecription(filefullname);
            if (fb2desc == null) { return (new BookInfo(-1)); }

            //2. Подсчитали MD5-хешь сумму
            // Проверили, что файл не содержится с Каталоге (или вернули его ID в случае если он уже добавлен)
            BookInfo bi = GetBookInfoByHash(filemd5hash);            
            if (bi.id >= 0) return (bi);

            //3. Добавили его в архив (нашли архив в который его добавлять, сгенерировали уникальное имя, заархивировали)
            arcshortfilename = ZipBLL.AddFb2ToZipArchive(filefullname, uniquifilename, arcstoragefolder);

           
            //4. Сохранили информацию о файле в БД
            //4.1 Сохраняем инфу о книче с основную таблицу
            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);

            int bookbaseid = dbmng.BookBaseTbl.Insert(fb2desc.titleinfo.book_title, arcshortfilename, uniquifilename, fileshortname, filemd5hash);
            if (bookbaseid == -1) 
            {
                return (new BookInfo(-1));
            }

            //4.2 Сохраняем информацию о жанре
            //Получаем IDки всех жанров и если такого жанра нет в БД, добавляем его
            for (int i = 0; i < fb2desc.titleinfo.genre.Count; i++)
            {
                int genreid = dbmng.GenreTbl.Insert(fb2desc.titleinfo.genre[i]);
                if (genreid >= 0)
                    dbmng.BookBaseGenreTbl.Insert(bookbaseid, genreid);
                else { /*Обработать удаление*/}
            }

            //4.3 Сохраняем информацию об авторе
            // Получаем IDки всех авторов и если такого автора нет в БД, добавляем его
            for (int i = 0; i < fb2desc.titleinfo.author.Count; i++)
            {
                int authorid = dbmng.AuthorTbl.Insert(fb2desc.titleinfo.author[i].firstname, fb2desc.titleinfo.author[i].middlename, fb2desc.titleinfo.author[i].lastname);
                //Все же не удалось добавить жанр !!!! Обработать удаление
                if (authorid >= 0)
                    dbmng.BookBaseAuthorTbl.Insert(bookbaseid, authorid);
                else { /*Обработать удаление*/}
            }

            return (new BookInfo(bookbaseid, fb2desc.titleinfo.book_title, arcshortfilename, uniquifilename, fileshortname, filemd5hash));
        }

        //Удалить книгу из каталога
        public bool DeleteBookFromCatalogByID(int ID)
        {
            bool bRet = false;


            return (bRet);
        }

        public BookInfo GetBookInfoByID(int ID)
        {
            BookInfo bookinfo = new BookInfo(-1);


            return (bookinfo);
        }

        public BookInfo GetBookInfoByHash(string hash)
        {

            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);
            DAL.CDBookBaseRow bbr = dbmng.BookBaseTbl.SelectIdByMD5(hash);

            return (new BookInfo(bbr.Id, bbr.BookName, bbr.ArcFileName, bbr.UniquiFileName, bbr.OriginalFileName, bbr.MD5));
        }

        public List<BookInfo> GetBookInfoByAuthorID(int ID)
        {

            List<BookInfo> bil = new List<BookInfo>();

            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);

            List<int> bidl = dbmng.BookBaseAuthorTbl.SelectBookIDbyAuthorID(ID);
            foreach (int id in bidl)
            {
                DAL.CDBookBaseRow bbr = dbmng.BookBaseTbl.SelectBookById(id);
                BookInfo bi = new BookInfo(bbr.Id, bbr.BookName, bbr.ArcFileName, bbr.UniquiFileName, bbr.OriginalFileName, bbr.MD5);
                bil.Add(bi);
            }


            return (bil);
        }

        public List<AuthorInfo> GetAuthorList()
        {
            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);
            List<DAL.CDAuthorRow> auterlist = dbmng.AuthorTbl.SelectAll();

            List<AuthorInfo> auterinfolist = new List<AuthorInfo>();
            foreach (DAL.CDAuthorRow ar in auterlist)
                auterinfolist.Add(new AuthorInfo(ar.Id, ar.FirstName, ar.MiddleName, ar.LastName));

            return (auterinfolist);
        }

        public List<GenreInfo> GetGenreList()
        {
            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);
            List<DAL.CDGenreRow> genrelist = dbmng.GenreTbl.SelectAll();

            List<GenreInfo> genreinfolist = new List<GenreInfo>();

            foreach (DAL.CDGenreRow ar in genrelist)
                genreinfolist.Add(new GenreInfo(ar.Id, ar.Genre));

            return (genreinfolist);
        }

        public List<AuthorInfo> GetAuthorListByGenreID(int IDGenre)
        {
            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);
            List<DAL.CDAuthorRow> auterlist = dbmng.SelectDistinctAuthorListByGenreID(IDGenre);

            List<AuthorInfo> auterinfolist = new List<AuthorInfo>();
            foreach (DAL.CDAuthorRow ar in auterlist)
                auterinfolist.Add(new AuthorInfo(ar.Id, ar.FirstName, ar.MiddleName, ar.LastName));

            return (auterinfolist);
        }

        public List<BookInfo> GetBookInfoList()
        {
            List<BookInfo> bil = new List<BookInfo>();

            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);

            List<DAL.CDBookBaseRow> bbrl = dbmng.BookBaseTbl.SelectAll();

            foreach (DAL.CDBookBaseRow bbr in bbrl)
            {
                BookInfo bi = new BookInfo(bbr.Id, bbr.BookName, bbr.ArcFileName, bbr.UniquiFileName, bbr.OriginalFileName, bbr.MD5);
                bil.Add(bi);
            }

            return (bil);                
        }

        public bool UnionAuther(int IDPrimary, int IDSecondary)
        {

            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);
            dbmng.BookBaseAuthorTbl.UpdateByAutherID(IDPrimary, IDSecondary);
            dbmng.AuthorTbl.Delete(IDSecondary);

            return (true);
        }


        public bool UnionGenre(int IDPrimary, int IDSecondary)
        {
            DAL.CDBManager dbmng = new DAL.CDBManager(Properties.Settings.Default.MSSQLConnectionString);
            dbmng.BookBaseGenreTbl.UpdateByGenreID(IDPrimary, IDSecondary);
            dbmng.GenreTbl.Delete(IDSecondary);

            return (true);
        }
    }
}
