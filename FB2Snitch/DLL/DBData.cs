using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;

namespace FB2Snitch.DAL
{

    public class BookRow
    {
        public int    Id;
        public string BookName;
        public string ArcFileName;
        public string MD5;
        public string Lang;

        public BookRow()
        {
            this.BookName = string.Empty;
            this.ArcFileName = string.Empty;
            this.MD5 = string.Empty;
            this.Id = -1;
            this.Lang = String.Empty;
        }

        public BookRow(string BookName, string ArcFileName, string MD5, string Lang) : this()
        {
            this.BookName = BookName;
            this.ArcFileName = ArcFileName;
            this.MD5 = MD5;
            this.Lang = Lang;
        }

        public BookRow(int Id, string BookName, string ArcFileName, string MD5,  string Lang) : this(BookName, ArcFileName, MD5, Lang)
        {
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

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", LastName, FirstName, MiddleName ).Replace("  ", " ").Replace("  ", " "); 
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

        public override String ToString()
        {
            return String.Format("{0} ({1})", Genre_ru, Genre);
        }
    }


    public class AbstractTbl
    {
        protected String DBTableName = String.Empty;

        public AbstractTbl()
        {
        }

        protected int ExecuteNonQuery(string SqlRequest)
        {
            try
            {
                using (SQLiteConnection m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString))
                {
                    m_dbConn.Open();
                    using (SQLiteCommand m_sqlCmd = new SQLiteCommand())
                    {
                        m_sqlCmd.Connection = m_dbConn;
                        m_sqlCmd.CommandText = SqlRequest;
                        return (m_sqlCmd.ExecuteNonQuery());
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n{0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new FB2DBException(ex.Message);
            }
        }


        protected int ExecuteScalar(string SqlRequest)
        {
            try
            {
                using (SQLiteConnection m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString))
                {
                    m_dbConn.Open();
                    using (SQLiteCommand m_sqlCmd = new SQLiteCommand())
                    {
                        m_sqlCmd.Connection = m_dbConn;
                        m_sqlCmd.CommandText = SqlRequest;
                        return (Convert.ToInt32(m_sqlCmd.ExecuteScalar()));
                    }

                }
            }
            catch (SQLiteException ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n{0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new FB2DBException(ex.Message);
            }
        }



        protected int ExecuteNonQueryAndReturnID(string SqlRequest, List<SQLiteParameter> parameters)
        {
            try
            {
                using (SQLiteConnection m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString))
                {
                    m_dbConn.Open();
                    using (SQLiteCommand m_sqlCmd = new SQLiteCommand())
                    {
                        m_sqlCmd.Connection = m_dbConn;
                        m_sqlCmd.CommandText = SqlRequest;
                        if (parameters != null)
                            m_sqlCmd.Parameters.AddRange(parameters.ToArray());
                        m_sqlCmd.ExecuteNonQuery();
                        m_sqlCmd.CommandText = "SELECT id FROM " + DBTableName + " WHERE rowid = last_insert_rowid()";
                        return (Convert.ToInt32(m_sqlCmd.ExecuteScalar()));
                    }

                }
            }
            catch (SQLiteException ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n{0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new FB2DBException(ex.Message);
            }
        }

        protected DataTable ExecuteReader(string SqlRequest)
        {
            try
            {
                DataTable dt = new DataTable();

                using (SQLiteConnection m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString))
                {
                    m_dbConn.Open();
                    using (SQLiteCommand m_sqlCmd = new SQLiteCommand())
                    {
                        m_sqlCmd.Connection = m_dbConn;
                        m_sqlCmd.CommandText = SqlRequest;
                        m_sqlCmd.CommandType = CommandType.Text;

                        SQLiteDataReader reader = m_sqlCmd.ExecuteReader();
                        dt.Load(reader);
                        return (dt);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                throw new FB2DBException(String.Format("Запрос выполнился с ошибкой /n{0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw new FB2DBException(ex.Message);
            }
        }

        public int toInt(object obj) { return Convert.ToInt32(obj); }

        public String toText(object obj) { return Convert.ToString(obj); }

    }

    public class BookTbl : AbstractTbl
    {
        public BookTbl() : base()
        {
            DBTableName = "Book";
        }

        public List<BookRow> Select()
        {
            List<BookRow> books = new List<BookRow>();

            string sql_request = String.Format("SELECT * FROM Book");

            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            books.Add(new BookRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["BookName"]), toText(dt.Rows[i]["ArcFileName"]), toText(dt.Rows[i]["MD5"]), toText(dt.Rows[i]["Lang"])));
                    return (books);
                }
            }
            catch { throw; }
        }

        public BookRow Select(int id)
        {
            string sql_request = String.Format("SELECT * FROM Book WHERE id = {0}", id);

            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                    return (dt.Rows.Count > 0) ? new BookRow(toInt(dt.Rows[0]["id"]), toText(dt.Rows[0]["BookName"]), toText(dt.Rows[0]["ArcFileName"]), toText(dt.Rows[0]["MD5"]), toText(dt.Rows[0]["Lang"])) : null;
            }
            catch { throw; }

        }

        public BookRow Select(string MD5)
        {
            string sql_request = String.Format("SELECT * FROM {0} WHERE MD5 = '{1}'", DBTableName, MD5);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        return new BookRow(toInt(dt.Rows[0]["id"]), toText(dt.Rows[0]["BookName"]), toText(dt.Rows[0]["ArcFileName"]), toText(dt.Rows[0]["MD5"]), toText(dt.Rows[0]["Lang"]));
                    return (null);
                }
            }
            catch { throw; }
        }

        public int Insert(string BookName, string ArcFileName, string MD5, string Lang)
        {
            string sql_request = string.Format("INSERT INTO {0} (BookName, ArcFileName, MD5, Lang) VALUES (@BookName, @ArcFileName, @MD5, @Lang)", DBTableName);
            List<SQLiteParameter> parameters = new List<SQLiteParameter>()
            {
                new SQLiteParameter("@BookName", BookName),
                new SQLiteParameter("@ArcFileName", ArcFileName),
                new SQLiteParameter("@MD5", MD5),
                new SQLiteParameter("@Lang", Lang)
            };
            
            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request, parameters));
            }
            catch { throw; }
        }

        public bool Delete(int id)
        {
            string sql_request = string.Format("DELETE FROM {0} WHERE id = {1}", DBTableName, id);
            try
            {
                return ExecuteNonQuery(sql_request) > 0 ? true : false;
            }
            catch { throw; }
        }


    }

    public class AuthorTbl : AbstractTbl
    {
        public AuthorTbl() : base() {
            DBTableName = "Author";
        }

        public AuthorRow Select(string FirstName, string MiddleName, string LastName)
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

            string sql_request = String.Format("SELECT * FROM {0} WHERE {1}", DBTableName, strFields);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if(dt.Rows.Count > 0)
                        return new AuthorRow(toInt(dt.Rows[0]["id"]), toText(dt.Rows[0]["FirstName"]), toText(dt.Rows[0]["MiddleName"]), toText(dt.Rows[0]["LastName"]));
                    return (null);
                }
            }
            catch  { throw; }            
        }

        public int Insert(string FirstName, string MiddleName, string LastName)
        {
            //Добавляем только уникальных авторов
            AuthorRow row = Select(FirstName, MiddleName, LastName);
            if (row != null) return row.Id;

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
                return (this.ExecuteNonQueryAndReturnID(sql_request, null));
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
            DBTableName = "BookAuthor";
        }

        public int Select(int bookId, int authorId)
        {
            string sql_request = String.Format("SELECT * FROM {0} WHERE BookId = {1} AND AuthorId = {2}", DBTableName, bookId, authorId);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        return toInt(dt.Rows[0]["id"]);
                    return (-1);
                }
            }
            catch { throw; }
        }

        public int Insert(int bookId, int authorId)
        {
            int iRet = Select(bookId, authorId);
            if (iRet > 0) return iRet;

            string sql_request = string.Format("INSERT INTO {0}(BookId, AuthorId) VALUES (@BookId, @AuthorId)", DBTableName);
            List<SQLiteParameter> parameters = new List<SQLiteParameter>()
            {
                new SQLiteParameter("@bookId", bookId),
                new SQLiteParameter("@authorId", authorId)
            };

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request, parameters));
            }
            catch { throw; }

        }
    }

    public class GenreTbl : AbstractTbl
    {
        public GenreTbl() : base()
        {
            DBTableName = "Genre";
        }

        public List<GenreRow> Select(string genre)
        {
            List<GenreRow> rowList = new List<GenreRow>();
            string sql_request = String.Format ("SELECT * FROM {0} WHERE genre = '{1}' ORDER BY genre_ru", DBTableName, genre);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            rowList.Add(new GenreRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["Genre"]), toText(dt.Rows[i]["Genre_ru"]), toInt(dt.Rows[i]["root"])));
                    return (rowList);
                }
            }
            catch { throw; }
        }

        public List<GenreRow> Select(int id, bool isRoot)
        {
            List<GenreRow> rowList = new List<GenreRow>(); 
            string sql_request = isRoot ? 
                                 String.Format("SELECT * FROM {0} WHERE root = {1} ORDER BY genre_ru", DBTableName, id) : 
                                 String.Format("SELECT * FROM {0} WHERE id = {1} ORDER BY genre_ru", DBTableName, id);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0 )
                        for (int i = 0; i < dt.Rows.Count; i++)
                            rowList.Add(new GenreRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["Genre"]), toText(dt.Rows[i]["Genre_ru"]), toInt(dt.Rows[i]["root"])));
                    return (rowList);
                }
            }
            catch { throw; }
        }

        public int Insert(String genre, string genre_ru, int root)
        {
            List<GenreRow> rows = Select(genre);
            if (rows.Count > 0) return rows[0].Id; // Это косячек, не должно быть больше одного (1 или менее элементов)

            string sql_request = string.Format("INSERT INTO {0} (genre, genre_ru, root) VALUES (@genre, @genre_ru, @root)", DBTableName);
            List<SQLiteParameter> parameters = new List<SQLiteParameter>()
            {
                new SQLiteParameter("@genre", genre),
                new SQLiteParameter("@genre_ru", genre_ru),
                new SQLiteParameter("@root", root)
            };

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request, parameters));
            }
            catch { throw; }
        }

        public int Insert(String genre)
        {
            List<GenreRow> rows = Select(genre);
            if (rows.Count > 0) return rows[0].Id; // Это косячек, не должно быть больше одного (1 или менее элементов)

            string sql_request = string.Format("INSERT INTO {0} (genre, genre_ru, root) VALUES (@genre, @genre_ru, @root)", DBTableName);
            List<SQLiteParameter> parameters = new List<SQLiteParameter>()
            {
                new SQLiteParameter("@genre", genre),
                new SQLiteParameter("@genre_ru", ""),
                new SQLiteParameter("@root", 15)
            };

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request, parameters));
            }
            catch { throw; }
        }
    }

    public class BookGenreTbl : AbstractTbl
    {
        public BookGenreTbl() : base()
        {
            DBTableName = "BookGenre";
        }

        public int Select(int bookId, int genreId)
        {
            string sql_request = String.Format("SELECT * FROM {0} WHERE BookId = {1} AND GenreId = {2}", DBTableName, bookId, genreId);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        return toInt(dt.Rows[0]["id"]);
                    return (-1);
                }
            }
            catch { throw; }
        }

        public int Insert(int bookId, int genreId)
        {
            int iRet = Select(bookId, genreId);
            if (iRet > 0) return iRet;

            string sql_request = string.Format("INSERT INTO {0}(BookId, GenreId) VALUES (@bookId, @genreId)", DBTableName);
            List<SQLiteParameter> pasrameters = new List<SQLiteParameter>()
            {
                new SQLiteParameter("@bookId", bookId),
                new SQLiteParameter("@genreId", genreId)
            };

            try
            {
                return (this.ExecuteNonQueryAndReturnID(sql_request, pasrameters));
            }
            catch { throw; }

        }
    }


    public class DBManager : AbstractTbl
    {
        BookTbl tblBookBase;
        AuthorTbl tblAuthor;
        BookAuthorTbl tblBookAuthor;
        GenreTbl tblGenre;
        BookGenreTbl tblBookGenre;

        public DBManager()
        {
            tblBookBase = new BookTbl();
            tblAuthor = new AuthorTbl();
            tblBookAuthor = new BookAuthorTbl();
            tblGenre = new GenreTbl();
            tblBookGenre = new BookGenreTbl();
        }

        public int IsBookHasBeenAlreadyAddedInDB(String hash)
        {
            BookRow rowBook = tblBookBase.Select(hash);
            return (rowBook != null ? rowBook.Id : -1);
        }

        public int AddBook(BLL.FB2Description fb2desc, string arcshortfilename, string hash)
        {
            try
            {
                BookRow rowBook = tblBookBase.Select(hash);
                if (rowBook != null) return rowBook.Id;

                int bookId = tblBookBase.Insert(fb2desc.titleinfo.book_title, arcshortfilename, hash, fb2desc.titleinfo.lang);

                foreach (BLL.FB2Person author in fb2desc.titleinfo.author)
                {
                    int authorId = tblAuthor.Insert(author.firstname, author.middlename, author.lastname);
                    tblBookAuthor.Insert(bookId, authorId);
                }

                foreach (string genre in fb2desc.titleinfo.genre)
                {
                    int genreId = tblGenre.Insert(genre);
                    tblBookGenre.Insert(bookId, genreId);
                }

                return bookId;
            }
            catch (Exception ex)
            {
                throw new FB2DBException(String.Format("Не удалось сохранить книку в DB\n{0}", ex.Message));
            }
        }

        public List<GenreRow> GetGenresInRoot()
        {
            return GetGenresByRootId(-1);
        }

        public List<GenreRow> GetGenresByRootId(int rootId)
        {
            return tblGenre.Select(rootId, true);
        }

        public List<GenreRow> GetGenresByName(String genre)
        {
            return tblGenre.Select(genre);
        }

        public List<GenreRow> GetGenresById(int id)
        {
            return tblGenre.Select(id, false);
        }

        public List<AuthorRow> GetAuthorByGenreId(int id)
        {
            List<AuthorRow> authors = new List<AuthorRow>();
            string sql_request = String.Format("SELECT DISTINCT a.id, a.FirstName, a.MiddleName, a.LastName " +
                                                "FROM Genre AS g " +
                                                "JOIN BookGenre AS bg ON g.id = bg.GenreId " +
                                                "JOIN Book AS b ON b.id = bg.BookId " +
                                                "JOIN BookAuthor AS ba ON ba.BookId = b.id " +
                                                "JOIN Author AS a ON ba.AuthorId = a.id " +
                                                "WHERE g.id = {0} ORDER BY a.LastName", id);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            authors.Add(new AuthorRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["FirstName"]), toText(dt.Rows[i]["MiddleName"]), toText(dt.Rows[i]["LastName"])));
                    return (authors);
                }
            }
            catch { throw; }
        }

        public List<AuthorRow> GetAuthorByGenreId(int id, string lang)
        {
            List<AuthorRow> authors = new List<AuthorRow>();
            string sql_request = String.Format("SELECT DISTINCT a.id, a.FirstName, a.MiddleName, a.LastName " +
                                                "FROM Genre AS g " +
                                                "JOIN BookGenre AS bg ON g.id = bg.GenreId " +
                                                "JOIN Book AS b ON b.id = bg.BookId " +
                                                "JOIN BookAuthor AS ba ON ba.BookId = b.id " +
                                                "JOIN Author AS a ON ba.AuthorId = a.id " +
                                                "WHERE g.id = {0} and b.lang = '{1}' ORDER BY a.LastName", id, lang);
            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            authors.Add(new AuthorRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["FirstName"]), toText(dt.Rows[i]["MiddleName"]), toText(dt.Rows[i]["LastName"])));
                    return (authors);
                }
            }
            catch { throw; }
        }

        public int GetAuthorCountByGenreId(int id, string lang)
        {
            string sql_request = String.Format("SELECT DISTINCT a.id, a.FirstName, a.MiddleName, a.LastName " +
                                                "FROM Genre AS g " +
                                                "JOIN BookGenre AS bg ON g.id = bg.GenreId " +
                                                "JOIN Book AS b ON b.id = bg.BookId " +
                                                "JOIN BookAuthor AS ba ON ba.BookId = b.id " +
                                                "JOIN Author AS a ON ba.AuthorId = a.id " +
                                                "WHERE g.id = {0} and b.lang = '{1}' ORDER BY a.LastName", id, lang); try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                        return (dt.Rows.Count);
                }
            }
            catch { throw; }
        }


        public List<BookRow> GetBookByAuthorId(int id)
        {
            List<BookRow> books = new List<BookRow>();

            string sql_request = String.Format("SELECT b.id, b.BookName, b.ArcFileName, b.MD5, b.Lang " +
                                                "FROM Author AS a " +
                                                "JOIN BookAuthor AS ba ON ba.AuthorId = a.id " +
                                                "JOIN Book AS b ON b.id = ba.BookId " +
                                                "WHERE a.id = {0} ORDER BY b.BookName", id);

            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            books.Add(new BookRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["BookName"]), toText(dt.Rows[i]["ArcFileName"]), toText(dt.Rows[i]["MD5"]), toText(dt.Rows[i]["Lang"])));
                    return (books);
                }
            }
            catch { throw; }
        }

        public List<BookRow> GetBookByAuthorId(int id, string lang)
        {
            List<BookRow> books = new List<BookRow>();

            string sql_request = String.Format("SELECT b.id, b.BookName, b.ArcFileName, b.MD5, b.Lang " +
                                                "FROM Author AS a " +
                                                "JOIN BookAuthor AS ba ON ba.AuthorId = a.id " +
                                                "JOIN Book AS b ON b.id = ba.BookId " +
                                                "WHERE a.id = {0} AND b.Lang = '{1}' ORDER BY b.BookName", id, lang);

            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            books.Add(new BookRow(toInt(dt.Rows[i]["id"]), toText(dt.Rows[i]["BookName"]), toText(dt.Rows[i]["ArcFileName"]), toText(dt.Rows[i]["MD5"]), toText(dt.Rows[i]["Lang"])));
                    return (books);
                }
            }
            catch { throw; }
        }

        public BookRow GetBookById(int id)
        {
            return tblBookBase.Select(id);
        }

        public List<BookRow> GetAllBooks()
        {
            return tblBookBase.Select();
        }

        public bool CheckConnection()
        {
            try
            {
                using (SQLiteConnection m_dbConn = new SQLiteConnection(Properties.Settings.Default.MSSQLConnectionString))
                {
                    m_dbConn.Open();
                    return m_dbConn.State == ConnectionState.Open ? true : false;
                }
            }
            catch (SQLiteException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetBookCount()
        {
            try
            {
                string sql_request = String.Format("SELECT DISTINCT COUNT() as BookCount FROM Book ");
                return ExecuteScalar(sql_request);
            }
            catch { throw; }
        }

        public int GetAuthorCount()
        {
            try
            {
                string sql_request = String.Format("SELECT DISTINCT COUNT() as AuthorCount FROM Author");
                return ExecuteScalar(sql_request);
            }
            catch { throw; }
        }

        public List<Tuple<int, string>> GetLanguages()
        {
            List<Tuple<int, string>> langs = new List<Tuple<int, string>>();
            string sql_request = String.Format("SELECT Lang, count(id) as Count FROM Book GROUP BY Lang");

            try
            {
                using (DataTable dt = this.ExecuteReader(sql_request))
                {
                    if (dt.Rows.Count > 0)
                        for (int i = 0; i < dt.Rows.Count; i++)
                            langs.Add(Tuple.Create(toInt(dt.Rows[i]["Count"]), toText(dt.Rows[i]["Lang"])));
                    return (langs);
                }
            }
            catch { throw; }
        }

        public bool DeleteBookById(int id)
        {
            return tblBookBase.Delete(id);
        }
    }

}
