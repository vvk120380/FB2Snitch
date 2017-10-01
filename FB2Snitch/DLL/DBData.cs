using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace FB2Snitch.DLL
{

    public class BookRow
    {
        public int Id;
        public string BookName;
        public string ArcFileName;
        public string MD5;

        public BookRow()
        {
            this.BookName = string.Empty;
            this.ArcFileName = string.Empty;
            this.MD5 = string.Empty;
            this.Id = -1;
        }

        public BookRow(string BookName, string ArcFileName, string MD5)
        {
            this.BookName = BookName;
            this.ArcFileName = ArcFileName;
            this.MD5 = MD5;
            this.Id = -1;
        }

        public BookRow(int Id, string BookName, string ArcFileName, string MD5)
        {
            this.BookName = BookName;
            this.ArcFileName = ArcFileName;
            this.MD5 = MD5;
            this.Id = Id;
        }
    }

    public class AuthorRow
    {
        public int Id;
        public string FirstName;
        public string MiddleName;
        public string LastName;

        public AuthorRow()
        {
            this.FirstName = string.Empty;
            this.MiddleName = string.Empty;
            this.LastName = string.Empty;
            this.Id = -1;
        }

        public AuthorRow(string FirstName, string MiddleName, string LastName)
        {
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Id = -1;
        }

        public AuthorRow(int Id, string FirstName, string MiddleName, string LastName)
        {
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Id = Id;
        }

    }

    public class GenreRow
    {
        public int Id;
        public string Genre;
        public string Genre_ru;
        public int root;

        public GenreRow()
        {
            this.Genre = string.Empty;
            this.Genre_ru = string.Empty;
            this.root = -1;
            this.Id = -1;
        }

        public GenreRow(string Genre)
        {
            this.Genre = Genre;
            this.Genre_ru = string.Empty;
            this.root = -1;
            this.Id = -1;
        }

        public GenreRow(int Id, string Genre)
        {
            this.Genre = Genre;
            this.Genre_ru = string.Empty;
            this.root = -1;
            this.Id = Id;
        }

        public GenreRow(int Id, string Genre, string Genre_ru, int root)
        {
            this.Genre = Genre;
            this.Genre_ru = Genre_ru;
            this.root = root;
            this.Id = Id;
        }

    }


    public class AbstractTbl
    {
        private static SQLiteConnection m_dbConn = null;
        private static SQLiteCommand m_sqlCmd = null;
        protected String DBTableName = String.Empty;

        public AbstractTbl()
        {
            m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString);
            m_sqlCmd = new SQLiteCommand();
            m_dbConn.Open();
        }

        protected void ExecuteNonQuery(string SqlRequest)
        {
            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString);
                    m_dbConn.Open();
                }

                if (m_sqlCmd == null) m_sqlCmd = new SQLiteCommand();

                m_sqlCmd.Connection = m_dbConn;
                m_sqlCmd.CommandText = SqlRequest;
                m_sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n%s", ex.Message));
            }
        }

        protected int ExecuteNonQueryAndReturnID(string SqlRequest)
        {
            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString);
                    m_dbConn.Open();
                }

                if (m_sqlCmd == null) m_sqlCmd = new SQLiteCommand();

                m_sqlCmd.Connection = m_dbConn;
                m_sqlCmd.CommandText = SqlRequest;
                m_sqlCmd.ExecuteNonQuery();

                m_sqlCmd.CommandText = "SELECT id FROM " + DBTableName + " WHERE rowid = last_insert_rowid()";
                return (Convert.ToInt32(m_sqlCmd.ExecuteScalar()));
            }
            catch (SQLiteException ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n{0}", ex.Message));
            }
        }


        protected SQLiteDataReader ExecuteReader(string SqlRequest)
        {
            try
            {
                if (m_dbConn.State != ConnectionState.Open)
                {
                    m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString);
                    m_dbConn.Open();
                }

                if (m_sqlCmd == null) m_sqlCmd = new SQLiteCommand();

                m_sqlCmd.Connection = m_dbConn;
                m_sqlCmd.CommandText = SqlRequest;
                m_sqlCmd.CommandType = CommandType.Text;

                return (m_sqlCmd.ExecuteReader());
            }
            catch (SQLiteException ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n{0}", ex.Message));
            }
        }
    }

    public class BookTbl : AbstractTbl
    {
        public BookTbl() : base()
        {
            this.DBTableName = "Book";
        }

        public BookRow Select(string MD5)
        {

            string sql_request = String.Format("SELECT * FROM {0} WHERE MD5 = '{1}'", DBTableName, MD5);

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                SQLiteDataReader dr = this.ExecuteReader(sql_request);
                if (dr.HasRows)
                {
                    if (dr.Read())
                        return new BookRow(Convert.ToInt32(dr["id"]),
                                           Convert.ToString(dr["BookName"]),
                                           Convert.ToString(dr["ArcFileName"]),
                                           Convert.ToString(dr["MD5"]));
                }
                return (null);
            }
            catch
            {
                throw;
            }
        }

        public int Insert(string BookName, string ArcFileName, string MD5)
        {
            string sql_request = string.Format("INSERT INTO " + this.DBTableName + "(BookName, ArcFileName, MD5) VALUES ('{0}','{1}','{2}')", BookName, ArcFileName, MD5);

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request));
            }
            catch
            {
                throw;
            }

        }
    }

    public class AuthorTbl : AbstractTbl
    {
        public AuthorTbl() : base() {
            this.DBTableName = "Author";
        }

        public int Select(string FirstName, string MiddleName, string LastName)
        {
            
            // Формируем строку запроса
            string strFields = string.Empty;

            if (!string.IsNullOrEmpty(FirstName)) strFields += ("FirstName = '" + FirstName + "'");
            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (!string.IsNullOrEmpty(strFields)) strFields += (" AND ");
                strFields += ("MiddleName = '" + MiddleName + "'");
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(strFields)) strFields += (" AND ");
                strFields += ("LastName = '" + LastName + "'");
            }

            string sql_request = "SELECT * FROM " + this.DBTableName + " WHERE " + strFields;

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                SQLiteDataReader dr = this.ExecuteReader(sql_request);
                if (dr.HasRows)
                {
                    if (dr.Read())
                        return Convert.ToInt32(dr["id"]);
                }
                return (-1);
            }
            catch 
            {
                throw;
            }

            
        }

        public int Insert(string FirstName, string MiddleName, string LastName)
        {
            int iRet = -1;

            //Добавляем только уникальных авторов
            iRet = Select(FirstName, MiddleName, LastName);
            if (iRet > 0) return iRet;

            // Формируем строку запроса 
            string strFields1 = string.Empty;
            string strFields2 = string.Empty;

            if (!string.IsNullOrEmpty(FirstName))
            {
                strFields1 += "FirstName";
                strFields2 += "'" + FirstName + "'";
            }
            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (!string.IsNullOrEmpty(strFields1)) strFields1 += ", ";
                if (!string.IsNullOrEmpty(strFields2)) strFields2 += ", ";

                strFields1 += "MiddleName";
                strFields2 += "'" + MiddleName + "'";
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(strFields1)) strFields1 += ", ";
                if (!string.IsNullOrEmpty(strFields2)) strFields2 += ", ";

                strFields1 += "LastName";
                strFields2 += "'" + LastName + "'";
            }

            string sql_request = string.Format("INSERT INTO " + DBTableName + "({0}) VALUES ({1})", strFields1, strFields2);

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request));
            }
            catch
            {
                throw;
            }

        }
    }


    public class BookAuthorTbl : AbstractTbl
    {
        public BookAuthorTbl() : base()
        {
            this.DBTableName = "BookAuthor";
        }

        public int Select(int bookId, int authorId)
        {

            string sql_request = String.Format("SELECT * FROM {0} WHERE BookId = {1} AND AuthorId = {2}", DBTableName, bookId, authorId);

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                SQLiteDataReader dr = this.ExecuteReader(sql_request);
                if (dr.HasRows)
                {
                    if (dr.Read())
                        return Convert.ToInt32(dr["id"]);
                }
                return (-1);
            }
            catch
            {
                throw;
            }
        }

        public int Insert(int bookId, int authorId)
        {
            int iRet = -1;

            //Добавляем только уникальных авторов
            iRet = Select(bookId, authorId);
            if (iRet > 0) return iRet;

            string sql_request = string.Format("INSERT INTO BookBase(BookId, AuthorId) VALUES ({0}, {1})", bookId, authorId);

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request));
            }
            catch
            {
                throw;
            }

        }
    }

    public class GenreTbl : AbstractTbl
    {
        public GenreTbl() : base()
        {
            DBTableName = "Genre";
        }

        public int Select(string genre)
        {
            string sql_request = String.Format ("SELECT * FROM {0} WHERE gener = '{1}'", DBTableName, genre);
            try
            {
                SQLiteDataReader dr = this.ExecuteReader(sql_request);
                if (dr.HasRows)
                {
                    if (dr.Read())
                        return Convert.ToInt32(dr["id"]);
                }
                return (-1);
            }
            catch
            {
                throw;
            }


        }

        public int Insert(string FirstName, string MiddleName, string LastName)
        {
            int iRet = -1;

            //Добавляем только уникальных авторов
            iRet = Select(FirstName, MiddleName, LastName);
            if (iRet > 0) return iRet;

            // Формируем строку запроса 
            string strFields1 = string.Empty;
            string strFields2 = string.Empty;

            if (!string.IsNullOrEmpty(FirstName))
            {
                strFields1 += "FirstName";
                strFields2 += "'" + FirstName + "'";
            }
            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (!string.IsNullOrEmpty(strFields1)) strFields1 += ", ";
                if (!string.IsNullOrEmpty(strFields2)) strFields2 += ", ";

                strFields1 += "MiddleName";
                strFields2 += "'" + MiddleName + "'";
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(strFields1)) strFields1 += ", ";
                if (!string.IsNullOrEmpty(strFields2)) strFields2 += ", ";

                strFields1 += "LastName";
                strFields2 += "'" + LastName + "'";
            }

            string sql_request = string.Format("INSERT INTO " + DBTableName + "({0}) VALUES ({1})", strFields1, strFields2);

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request));
            }
            catch
            {
                throw;
            }

        }
    }


    public class DBManager

    {
        BookTbl tblBookBase = null;
        AuthorTbl tblAuthor = null;
        BookAuthorTbl tblBookAuthor = null;

        public DBManager()
        {
            tblBookBase = new BookTbl();
            tblAuthor = new AuthorTbl();
        }

        public int FindBookBy5Hash(String hash)
        {
            return -1;
        }

        public int AddBook(BLL.FB2Description fb2desc, string arcshortfilename, string hash)
        {
            try
            {
                BookRow rowBook = null;
                int bookId = -1;
                int authorId = -1;

                rowBook = tblBookBase.Select(hash);
                if (rowBook != null) return rowBook.Id;

                bookId = tblBookBase.Insert(fb2desc.titleinfo.book_title, arcshortfilename, hash);

                foreach (BLL.FB2Person author in fb2desc.titleinfo.author)
                {
                    authorId = tblAuthor.Insert(author.firstname, author.middlename, author.lastname);
                    tblBookAuthor.Insert(bookId, authorId);
                }


                return 0;
            }
            catch
            {
                throw new FB2DBException("Не удалось сохранить книку в DB");
            }
        }
    }

    class DBData
    {
    }
}
