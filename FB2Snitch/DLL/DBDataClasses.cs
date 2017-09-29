using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.IO;

namespace FB2Snitch.DAL
{

    public class CDBookBaseRow
    {
        public int    Id;
        public string BookName;
        public string ArcFileName;
        public string UniquiFileName;
        public string OriginalFileName;
        public string MD5;

        public CDBookBaseRow()
        {
            this.BookName = string.Empty;
            this.ArcFileName = string.Empty;
            this.UniquiFileName = string.Empty;
            this.OriginalFileName = string.Empty;
            this.MD5 = string.Empty;
            this.Id = -1;
        }

        public CDBookBaseRow(string BookName, string ArcFileName, string UniquiFileName, string OriginalFileName, string MD5)
        {
            this.BookName = BookName;
            this.ArcFileName = ArcFileName;
            this.UniquiFileName = UniquiFileName;
            this.OriginalFileName = OriginalFileName;
            this.MD5 = MD5;
            this.Id = -1;
        }

        public CDBookBaseRow(int Id, string BookName, string ArcFileName, string UniquiFileName, string OriginalFileName, string MD5)
        {
            this.BookName = BookName;
            this.ArcFileName = ArcFileName;
            this.UniquiFileName = UniquiFileName;
            this.OriginalFileName = OriginalFileName;
            this.MD5 = MD5;
            this.Id = Id;
        }
    }

    public class CDAuthorRow
    {
        public int Id;
        public string FirstName;
        public string MiddleName;
        public string LastName;

        public CDAuthorRow()
        {
            this.FirstName = string.Empty;
            this.MiddleName = string.Empty;
            this.LastName = string.Empty;
            this.Id = -1;
        }

        public CDAuthorRow(string FirstName, string MiddleName, string LastName)
        {
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Id = -1;
        }

        public CDAuthorRow(int Id, string FirstName, string MiddleName, string LastName)
        {
            this.FirstName = FirstName;
            this.MiddleName = MiddleName;
            this.LastName = LastName;
            this.Id = Id;
        }

    }

    public class CDGenreRow
    {
        public int Id;
        public string Genre;

        public CDGenreRow()
        {
            this.Genre = string.Empty;
            this.Id = -1;
        }

        public CDGenreRow(string Genre)
        {
            this.Genre = Genre;
            this.Id = -1;
        }

        public CDGenreRow(int Id, string Genre)
        {
            this.Genre = Genre;
            this.Id = Id;
        }

    }

    public class CDBAbstractTbl
    {
        public string mConnectionString = string.Empty;
        public System.Data.OleDb.OleDbConnection mOleDbConnection = null;

        protected void ExecuteNonQuery(string SqlRequest)
        {
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = SqlRequest;
                    myOleDbCommand.Connection = mOleDbConnection;
                    myOleDbCommand.ExecuteNonQuery();
                }
            }
            catch
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                throw new Exception("ExecuteNonQuery exception");
            }
        
        }

        protected int ExecuteNonQueryAndReturnID(string SqlRequest)
        {
            int iRet = -1;
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = SqlRequest;
                    myOleDbCommand.Connection = mOleDbConnection;
                    myOleDbCommand.ExecuteNonQuery();

                    myOleDbCommand.CommandText = "SELECT @@IDENTITY";
                    iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
                }
            }
            catch
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                throw new Exception("ExecuteNonQueryAndReturnId exception");
            }

            return (iRet);
        }

    }

    public class CDBGenreTbl : CDBAbstractTbl
    {
        public CDBGenreTbl(string connectionstring) 
        {
            mConnectionString = connectionstring;        
        }

        // Ищет жанр в БД
        // Возвращает ID жанра
        public int Find(string genre)
        {
            int iRet = -1;

            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = "SELECT * FROM Genre WHERE Genre = '" + genre + "'";
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                            iRet = Convert.ToInt32(dr["ID"]);
                    }
                }
            }
            catch (Exception ex)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                iRet = -1;
            }

            return (iRet);
        }

        // добавляем только уникальные жанры
        // Возвращает ID жанра - либо найденного, либо добавленного
        public int Insert(string genre)
        {
            int iRet = -1;

            //Добавляем только уникальные жанры, так как не может быть двух одинаковых
            iRet = Find(genre);
            if (iRet > 0) return iRet;

            try
            {
                string sql_request = String.Format("INSERT INTO Genre(Genre) VALUES ('{0}')", genre);
                iRet = ExecuteNonQueryAndReturnID(sql_request);
            }
            catch
            {
                throw new Exception("CDBGenreTbl.Insert exception");  
            }

            //try
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
            //    {
            //        mOleDbConnection.Open();

            //        System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
            //        myOleDbCommand.CommandType = CommandType.Text;
            //        myOleDbCommand.CommandText = "INSERT INTO Genre(Genre) VALUES (@V_Genre)";
            //        myOleDbCommand.Connection = mOleDbConnection;
            //        myOleDbCommand.Parameters.AddWithValue("@V_Genre", genre);
            //        myOleDbCommand.ExecuteNonQuery();

            //        myOleDbCommand.CommandText = "SELECT @@IDENTITY";
            //        iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
            //    }
            //}
            //catch (Exception)
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    //Ошибку обрабатываем если что...
            //    iRet = -1;
            //}

            return (iRet);
        }

        public List<CDGenreRow> SelectAll()
        {

            List<CDGenreRow> genrelist = new List<CDGenreRow>();

            string sql_request = string.Format("SELECT * FROM Genre ORDER BY Genre");

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CDGenreRow auterrow = new CDGenreRow(Convert.ToInt32(dr["ID"]), dr["Genre"].ToString());
                            genrelist.Add(auterrow);
                        }
                    }

                }
            }
            catch (Exception)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }

            return (genrelist);
        }

        public void Delete(int IDGenre)
        {
            string sql_request = string.Format("DELETE * FROM Genre WHERE Genre.ID = {0}", IDGenre);
            try
            {                
                this.ExecuteNonQuery(sql_request);
            }
            catch
            {
                throw new Exception("CDBGenreTbl.Delete Exception");
            }
        }
    }

    public class CDBAuthorTbl : CDBAbstractTbl
    {

        public CDBAuthorTbl(string connectionstring)
        {
            mConnectionString = connectionstring;        
        }

        public int Find(string FirstName, string MiddleName, string LastName)
        {
            int iRet = -1;

            // Формируем строку запроса
            string strFields = string.Empty;

            if (!string.IsNullOrEmpty(FirstName)) strFields += ("FirstName = '" + FirstName + "'");
            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (!string.IsNullOrEmpty (strFields)) strFields += (" AND ");
                strFields += ("MiddleName = '" + MiddleName + "'");
            }

            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(strFields)) strFields += (" AND ");
                strFields += ("LastName = '" + LastName + "'");
            }

            string sql_request = "SELECT * FROM Author WHERE " + strFields;

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                            iRet = Convert.ToInt32(dr["ID"]);
                    }
                }
            }
            catch (Exception)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (iRet);
            }

            return (iRet);

        }

        public int Insert(string FirstName, string MiddleName, string LastName)
        {
            int iRet = -1;

            //Добавляем только уникальных авторов
            iRet = Find(FirstName, MiddleName, LastName);
            if (iRet > 0) return iRet;

            // Формируем строку запроса 
            string strFields1 = string.Empty;
            string strFields2 = string.Empty;

            if (!string.IsNullOrEmpty(FirstName))
            {
                strFields1 += "FirstName";
                strFields2 += "@V_FirstName";
            }
            if (!string.IsNullOrEmpty(MiddleName))
            {
                if (!string.IsNullOrEmpty(strFields1)) strFields1 += ", ";
                if (!string.IsNullOrEmpty(strFields2)) strFields2 += ", ";

                strFields1 += ("MiddleName");
                strFields2 += ("@V_MiddleName");
            }
            if (!string.IsNullOrEmpty(LastName))
            {
                if (!string.IsNullOrEmpty(strFields1)) strFields1 += ", ";
                if (!string.IsNullOrEmpty(strFields2)) strFields2 += ", ";

                strFields1 += ("LastName");
                strFields2 += ("@V_LastName");
            }

            string sql_request = string.Format("INSERT INTO Author({0}) VALUES ({1})", strFields1, strFields2);

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;
                    if (!string.IsNullOrEmpty(FirstName)) myOleDbCommand.Parameters.AddWithValue("@V_FirstName", FirstName);
                    if (!string.IsNullOrEmpty(MiddleName)) myOleDbCommand.Parameters.AddWithValue("@V_MiddleName", MiddleName);
                    if (!string.IsNullOrEmpty(LastName)) myOleDbCommand.Parameters.AddWithValue("@V_LastName", LastName);
                    myOleDbCommand.ExecuteNonQuery();

                    myOleDbCommand.CommandText = "SELECT @@IDENTITY";
                    iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (iRet);
            }

            return (iRet);
        }

        public List<CDAuthorRow> SelectAll()
        {

            List<CDAuthorRow> authorlist = new List<CDAuthorRow>();

            string sql_request = string.Format("SELECT * FROM Author ORDER BY LastName");

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CDAuthorRow auterrow = new CDAuthorRow(Convert.ToInt32(dr["ID"]), dr["FirstName"].ToString(), dr["MiddleName"].ToString(), dr["LastName"].ToString());
                            authorlist.Add(auterrow);
                        }
                    }

                }
            }
            catch (Exception)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }

            return (authorlist);
        }

        public void Delete(int IDAuthor)
        {
            string sql_request = string.Format("DELETE * FROM Author WHERE Author.ID = {0}", IDAuthor);
            try
            {
                this.ExecuteNonQuery(sql_request);
            }
            catch
            {
                throw new Exception("CDBAuthorTbl.Delete Exception");
            }
        }
    }

    public class CDBookBaseGenreTbl : CDBAbstractTbl
    {

        public CDBookBaseGenreTbl(string connectionstring)
        {
            mConnectionString = connectionstring;        
        }

        public int Insert(int IDBook, int IDGenre)
        {
            int iRet = -1;

            string sql_request = string.Format("INSERT INTO BookBase_Genre(IDBookBase, IDGenre) VALUES ({0}, {1})", IDBook, IDGenre);
            try
            {
                iRet = this.ExecuteNonQueryAndReturnID(sql_request);
            }
            catch 
            {
                throw new Exception("CDBookBaseGenreTbl.Insert Exception");
            }

            //try
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
            //    {
            //        mOleDbConnection.Open();

            //        System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
            //        myOleDbCommand.CommandType = CommandType.Text;
            //        myOleDbCommand.CommandText = "INSERT INTO BookBase_Genre(IDBookBase, IDGenre) VALUES (@V_IDBookBase, @V_IDGenre)";
            //        myOleDbCommand.Connection = mOleDbConnection;
            //        myOleDbCommand.Parameters.AddWithValue("@V_IDBookBase", IDBook);
            //        myOleDbCommand.Parameters.AddWithValue("@V_IDGenre", IDGenre);
            //        myOleDbCommand.ExecuteNonQuery();

            //        myOleDbCommand.CommandText = "SELECT @@IDENTITY";
            //        iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
            //    }
            //}
            //catch (Exception)
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    //Ошибку обрабатываем если что...
            //    return (iRet);
            //}

            return (iRet);
        }

        public void UpdateByGenreID(int IDPrimary, int IDSecondary)
        {
            string sql_request = string.Format("UPDATE BookBase_Genre SET BookBase_Genre.IDGenre = {0} WHERE BookBase_Genre.IDGenre = {1};", IDPrimary, IDSecondary);
            try
            {
                this.ExecuteNonQuery(sql_request);
            }
            catch
            {
                throw new Exception("CDBookBaseGenreTbl.UpdateByGenreID Exception");
            }
        }
    }

    public class CDBookBaseAuthorTbl : CDBAbstractTbl
    {

        public CDBookBaseAuthorTbl(string connectionstring)
        {
            mConnectionString = connectionstring;        
        }

        public int Insert(int IDBook, int IDAuthor)
        {
            int iRet = -1;

            string sql_request = string.Format("INSERT INTO BookBase_Author(IDBookBase, IDAuthor) VALUES ({0}, {1})", IDBook, IDAuthor);
            try
            {
                iRet = this.ExecuteNonQueryAndReturnID(sql_request);
            }
            catch
            {
                throw new Exception("CDBookBaseAuthorTbl.Insert Exception");
            }

            //try
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
            //    {
            //        mOleDbConnection.Open();

            //        System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
            //        myOleDbCommand.CommandType = CommandType.Text;
            //        myOleDbCommand.CommandText = "INSERT INTO BookBase_Author(IDBookBase, IDAuthor) VALUES (@V_IDBookBase, @V_IDAuthor)";
            //        myOleDbCommand.Connection = mOleDbConnection;
            //        myOleDbCommand.Parameters.AddWithValue("@V_IDBookBase", IDBook);
            //        myOleDbCommand.Parameters.AddWithValue("@V_IDAuthor", IDAuthor);
            //        myOleDbCommand.ExecuteNonQuery();

            //        myOleDbCommand.CommandText = "SELECT @@IDENTITY";
            //        iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
            //    }
            //}
            //catch (Exception)
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    //Ошибку обрабатываем если что...
            //    return (iRet);
            //}

            return (iRet);
        }

        public List<int> SelectBookIDbyAuthorID(int IDAuthor)
        {
            List<int> bidl = new List<int>();

            string sql_request = string.Format("SELECT * FROM BookBase_Author WHERE IDAuthor={0}", IDAuthor);

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            int idbook = Convert.ToInt32(dr["IDBookBase"]);
                            bidl.Add(idbook);
                        }
                    }

                }
            }
            catch (Exception)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }



            return (bidl);
        }

        public void UpdateByAutherID(int IDPrimary, int IDSecondary)
        {
            string sql_request = string.Format("UPDATE BookBase_Author SET BookBase_Author.IDAuthor = {0} WHERE BookBase_Author.IDAuthor = {1};", IDPrimary, IDSecondary);
            try
            {
                this.ExecuteNonQuery(sql_request);
            }
            catch
            {
                throw new Exception("CDBookBaseAuthorTbl.UpdateByAutherID Exception");
            }
        }
    }

    public class CDBookBaseTbl : CDBAbstractTbl
    {

        public CDBookBaseTbl(string connectionstring)
        {
            mConnectionString = connectionstring;        
        }

        public int Insert(string BookName, string ArcFileName, string UniquiFileName, string OriginalFileName, string MD5)
        {
            int iRet = -1;

            string sql_request = string.Format("INSERT INTO BookBase(BookName, ArcFileName, UniquiFileName, OriginalFileName, MD5) VALUES ('{0}','{1}','{2}','{3}','{4}')", BookName, ArcFileName, UniquiFileName, OriginalFileName, MD5);
            try
            {
                iRet = this.ExecuteNonQueryAndReturnID(sql_request);
            }
            catch
            {
                throw new Exception("CDBookBaseTbl.Insert Exception");
            }

            // Пробуем подключиться -----------------------------------------------------------------------
            //try
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
            //    {
            //        mOleDbConnection.Open();

            //        System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
            //        myOleDbCommand.CommandType = CommandType.Text;
            //        myOleDbCommand.CommandText = "INSERT INTO BookBase(BookName, ArcFileName, UniquiFileName, OriginalFileName, MD5) VALUES (@V_Book,@V_Arc,@V_UFN, @V_OFN, @V_MD5)";
            //        myOleDbCommand.Connection = mOleDbConnection;
            //        myOleDbCommand.Parameters.AddWithValue("@V_Book", BookName);
            //        myOleDbCommand.Parameters.AddWithValue("@V_Arc", ArcFileName);
            //        myOleDbCommand.Parameters.AddWithValue("@V_UFN", UniquiFileName);
            //        myOleDbCommand.Parameters.AddWithValue("@V_OFN", OriginalFileName);
            //        myOleDbCommand.Parameters.AddWithValue("@V_MD5", MD5);
            //        myOleDbCommand.ExecuteNonQuery();

            //        myOleDbCommand.CommandText = "SELECT @@IDENTITY";
            //        iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
            //    }
            //}
            //catch (Exception)
            //{
            //    if (mOleDbConnection != null) mOleDbConnection.Close();
            //    //Ошибку обрабатываем если что...
            //    return (iRet);
            //}

            return (iRet);
        }

        public CDBookBaseRow SelectIdByMD5( string MD5)
        {

            CDBookBaseRow BookBaseRow = new CDBookBaseRow();
            
            // Пробуем подключиться -----------------------------------------------------------------------
            if (mOleDbConnection != null) mOleDbConnection.Close();
            try
            {
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = "SELECT * FROM BookBase WHERE MD5 = '" + MD5 + "'";
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            BookBaseRow.ArcFileName = dr["ArcFileName"].ToString();
                            BookBaseRow.UniquiFileName = dr["UniquiFileName"].ToString();
                            BookBaseRow.BookName = dr["BookName"].ToString();
                            BookBaseRow.MD5 = dr["MD5"].ToString();
                            BookBaseRow.OriginalFileName = dr["OriginalFileName"].ToString();
                            BookBaseRow.Id = Convert.ToInt32(dr["ID"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }


            return (BookBaseRow);
        }

        public CDBookBaseRow SelectBookById(int IDBook)
        {
            CDBookBaseRow BookBaseRow = new CDBookBaseRow();

            string sql_request = string.Format("SELECT * FROM BookBase WHERE ID={0}", IDBook);
             
            // Пробуем подключиться -----------------------------------------------------------------------
            if (mOleDbConnection != null) mOleDbConnection.Close();
            try
            {
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            BookBaseRow.ArcFileName = dr["ArcFileName"].ToString();
                            BookBaseRow.UniquiFileName = dr["UniquiFileName"].ToString();
                            BookBaseRow.BookName = dr["BookName"].ToString();
                            BookBaseRow.MD5 = dr["MD5"].ToString();
                            BookBaseRow.OriginalFileName = dr["OriginalFileName"].ToString();
                            BookBaseRow.Id = Convert.ToInt32(dr["ID"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }


            return (BookBaseRow);
        }

        public List<CDBookBaseRow> SelectAll()
        {
            List<CDBookBaseRow> BookBaseRowList = new List<CDBookBaseRow>();

            string sql_request = string.Format("SELECT * FROM BookBase ORDER BY BookName");

            // Пробуем подключиться -----------------------------------------------------------------------
            if (mOleDbConnection != null) mOleDbConnection.Close();
            try
            {
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CDBookBaseRow BookBaseRow = new CDBookBaseRow(Convert.ToInt32(dr["ID"]), dr["BookName"].ToString(), dr["ArcFileName"].ToString(), dr["UniquiFileName"].ToString(), dr["OriginalFileName"].ToString(), dr["MD5"].ToString());
                            BookBaseRowList.Add(BookBaseRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }


            return (BookBaseRowList);
        }

    }

    public class DBManager 
    {
        public static int FindBookBy5Hash(String hash)
        {
            return -1;
        }

        public static int AddBook(BLL.FB2Description fb2desc, string arcshortfilename, string hash)
        {
            try {

                return 0;
            }
            catch
            {
                throw new FB2DBException("Не удалось сохранить книку в DB");
            }            
        }

    }

    public class CDBManager : CDBAbstractTbl
    {
        public CDBGenreTbl GenreTbl;
        public CDBAuthorTbl AuthorTbl;
        public CDBookBaseGenreTbl BookBaseGenreTbl;
        public CDBookBaseAuthorTbl BookBaseAuthorTbl;
        public CDBookBaseTbl BookBaseTbl;

        public CDBManager(string connectionstring) 
        {
            mConnectionString = connectionstring;
            GenreTbl         = new CDBGenreTbl(connectionstring);
            AuthorTbl        = new CDBAuthorTbl(connectionstring);
            BookBaseGenreTbl = new CDBookBaseGenreTbl(connectionstring);
            BookBaseAuthorTbl = new CDBookBaseAuthorTbl(connectionstring);
            BookBaseTbl      = new CDBookBaseTbl(connectionstring);
        }

        public List<CDAuthorRow> SelectDistinctAuthorListByGenreID(int IDGenre)
        {

            List<CDAuthorRow> authorlist = new List<CDAuthorRow>();
           
            string sql_request = string.Format("SELECT DISTINCT Author.* FROM Genre INNER JOIN ((BookBase INNER JOIN (Author INNER JOIN BookBase_Author ON Author.ID = BookBase_Author.IDAuthor) ON BookBase.ID = BookBase_Author.IDBookBase) INNER JOIN BookBase_Genre ON BookBase.ID = BookBase_Genre.IDBookBase) ON Genre.ID = BookBase_Genre.IDGenre WHERE (((BookBase_Genre.IDGenre)= {0}))", IDGenre);

            // Пробуем подключиться -----------------------------------------------------------------------
            try
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                using (mOleDbConnection = new System.Data.OleDb.OleDbConnection(mConnectionString))
                {
                    mOleDbConnection.Open();

                    System.Data.OleDb.OleDbCommand myOleDbCommand = mOleDbConnection.CreateCommand();
                    myOleDbCommand.CommandType = CommandType.Text;
                    myOleDbCommand.CommandText = sql_request;
                    myOleDbCommand.Connection = mOleDbConnection;

                    System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
                    
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            CDAuthorRow auterrow = new CDAuthorRow(Convert.ToInt32(dr["ID"]), dr["FirstName"].ToString(), dr["MiddleName"].ToString(), dr["LastName"].ToString());
                            authorlist.Add(auterrow);
                        }
                    }

                }
            }
            catch (Exception)
            {
                if (mOleDbConnection != null) mOleDbConnection.Close();
                //Ошибку обрабатываем если что...
                return (null);
            }

            return (authorlist);
        }


    }

    //public class CDbWorker
    //{



    //    // Ищет жанр в БД
    //    // Возвращает ID жанра
    //    public static int Genre_Find(string Genre)
    //    {
    //        int iRet = -1;
    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = "SELECT * FROM Genre WHERE Genre = '" + Genre + "'";
    //                myOleDbCommand.Connection = mSqlConnection;

    //                System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
    //                if (dr.HasRows)
    //                {
    //                    while (dr.Read())
    //                    {
    //                        iRet = Convert.ToInt32(dr["ID"]);
    //                    }
    //                }


    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
    //    }

    //    public static int Genre_Insert(string Genre)
    //    {
    //        int iRet = -1;

    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = "INSERT INTO Genre(Genre) VALUES (@V_Genre)";
    //                myOleDbCommand.Connection = mSqlConnection;
    //                myOleDbCommand.Parameters.AddWithValue("@V_Genre", Genre);
    //                myOleDbCommand.ExecuteNonQuery();

    //                myOleDbCommand.CommandText = "SELECT @@IDENTITY";
    //                iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
    //    }

    //    public static int Genre_InsertUnique(string Genre)
    //    {
    //        int iRet = -1;
            
    //        iRet = Genre_Find(Genre);
    //        if (iRet > 0) return iRet;

    //        return (Genre_Insert(Genre));        
    //    }

    //    public static int Author_Find(string FirstName, string MiddleName, string LastName)
    //    {
    //        int iRet = -1;
    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";


    //        string strFields = string.Empty;

    //        if (!string.IsNullOrEmpty(FirstName))  strFields += ("FirstName = '" + FirstName + "'");
    //        if (!string.IsNullOrEmpty(MiddleName)) strFields += (" AND MiddleName = '" + MiddleName + "'");
    //        if (!string.IsNullOrEmpty(LastName))   strFields += (" AND LastName = '" + LastName + "'");

    //        string sql_request = "SELECT * FROM Author WHERE " + strFields; 


    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = sql_request;
    //                myOleDbCommand.Connection = mSqlConnection;

    //                System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
    //                if (dr.HasRows)
    //                {
    //                    while (dr.Read())
    //                    {
    //                        iRet = Convert.ToInt32(dr["ID"]);
    //                    }
    //                }


    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
        
    //    }

    //    public static int Author_Insert(string FirstName, string MiddleName, string LastName)
    //    {
    //        int iRet = -1;

    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        string strFields1 = string.Empty;
    //        string strFields2 = string.Empty;

    //        if (!string.IsNullOrEmpty(FirstName))
    //        {
    //            strFields1 += "FirstName";
    //            strFields2 += "@V_FirstName";
    //        }
    //        if (!string.IsNullOrEmpty(MiddleName))
    //        {
    //            strFields1 += (", MiddleName");
    //            strFields2 += (", @V_MiddleName");
    //        }
    //        if (!string.IsNullOrEmpty(LastName))
    //        {
    //            strFields1 += (", LastName");
    //            strFields2 += (", @V_LastName");
    //        }

    //        string sql_request = string.Format("INSERT INTO Author({0}) VALUES ({1})", strFields1, strFields2); 


    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = sql_request;
    //                //myOleDbCommand.CommandText = "INSERT INTO Author(FirstName, LastName) VALUES (@V_FirstName, @V_LastName)";
    //                myOleDbCommand.Connection = mSqlConnection;
    //                if (!string.IsNullOrEmpty(FirstName)) myOleDbCommand.Parameters.AddWithValue("@V_FirstName", FirstName);
    //                if (!string.IsNullOrEmpty(MiddleName)) myOleDbCommand.Parameters.AddWithValue("@V_MiddleName", MiddleName);
    //                if (!string.IsNullOrEmpty(LastName)) myOleDbCommand.Parameters.AddWithValue("@V_LastName", LastName);
    //                myOleDbCommand.ExecuteNonQuery();

    //                myOleDbCommand.CommandText = "SELECT @@IDENTITY";
    //                iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
    //    }

    //    public static int Author_InsertUnique(string FirstName, string MiddleName, string LastName)
    //    {
    //        int iRet = -1;

    //        iRet = Author_Find(FirstName, MiddleName, LastName);
    //        if (iRet > 0) return iRet;

    //        return (Author_Insert(FirstName, MiddleName, LastName));
    //    }

    //    public static int BookBase_Genre_Insert(int IDBook, int IDGenre)
    //    {
    //        int iRet = -1;

    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = "INSERT INTO BookBase_Genre(IDBookBase, IDGenre) VALUES (@V_IDBookBase, @V_IDGenre)";
    //                myOleDbCommand.Connection = mSqlConnection;
    //                myOleDbCommand.Parameters.AddWithValue("@V_IDBookBase", IDBook);
    //                myOleDbCommand.Parameters.AddWithValue("@V_IDGenre", IDGenre);
    //                myOleDbCommand.ExecuteNonQuery();

    //                myOleDbCommand.CommandText = "SELECT @@IDENTITY";
    //                iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
    //    }

    //    public static int BookBase_Author_Insert(int IDBook, int IDAuthor)
    //    {
    //        int iRet = -1;

    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = "INSERT INTO BookBase_Author(IDBookBase, IDAuthor) VALUES (@V_IDBookBase, @V_IDAuthor)";
    //                myOleDbCommand.Connection = mSqlConnection;
    //                myOleDbCommand.Parameters.AddWithValue("@V_IDBookBase", IDBook);
    //                myOleDbCommand.Parameters.AddWithValue("@V_IDAuthor", IDAuthor);
    //                myOleDbCommand.ExecuteNonQuery();

    //                myOleDbCommand.CommandText = "SELECT @@IDENTITY";
    //                iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
    //    }

    //    public static int Insert(string BookName, string ArcFileName, string UniquiFileName, string OriginalFileName, string MD5)
    //    {
    //        int iRet = -1;

    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                // Вариант 1
    //                //OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                //myOleDbCommand.CommandText = "INSERT INTO BookBase(BookName, ArcFileName, UniquiFileName, OriginalFileName, MD5) VALUES ('Boor','Arc','UFN', 'OFN', 'MD5')";
    //                //myOleDbCommand.ExecuteNonQuery();

    //                // Вариант 2

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = "INSERT INTO BookBase(BookName, ArcFileName, UniquiFileName, OriginalFileName, MD5) VALUES (@V_Book,@V_Arc,@V_UFN, @V_OFN, @V_MD5)";
    //                myOleDbCommand.Connection = mSqlConnection;
    //                myOleDbCommand.Parameters.AddWithValue("@V_Book", BookName);
    //                myOleDbCommand.Parameters.AddWithValue("@V_Arc", ArcFileName);
    //                myOleDbCommand.Parameters.AddWithValue("@V_UFN", UniquiFileName);
    //                myOleDbCommand.Parameters.AddWithValue("@V_OFN", OriginalFileName);
    //                myOleDbCommand.Parameters.AddWithValue("@V_MD5", MD5);
    //                myOleDbCommand.ExecuteNonQuery();

    //                myOleDbCommand.CommandText = "SELECT @@IDENTITY";
    //                iRet = Convert.ToInt32(myOleDbCommand.ExecuteScalar());
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }

    //        return (iRet);
    //    }

    //    public static int SelectByMD5(string MD5, ref string ArcFileName, ref string UniquiFileName)
    //    {
    //        int iRet = -1;

    //        System.Data.OleDb.OleDbConnection mSqlConnection = null;

    //        // Создаем строку подключения ----------------------------------------------------------------
    //        string mSqlConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;User ID=Admin;Data Source=E:\testADB.mdb;Mode=ReadWrite;Persist Security Info=True";

    //        // Пробуем подключиться -----------------------------------------------------------------------
    //        if (mSqlConnection != null) mSqlConnection.Close();
    //        try
    //        {
    //            using (mSqlConnection = new System.Data.OleDb.OleDbConnection(mSqlConnectionString))
    //            {
    //                mSqlConnection.Open();

    //                System.Data.OleDb.OleDbCommand myOleDbCommand = mSqlConnection.CreateCommand();
    //                myOleDbCommand.CommandType = CommandType.Text;
    //                myOleDbCommand.CommandText = "SELECT * FROM BookBase WHERE MD5 = '" + MD5 + "'";
    //                myOleDbCommand.Connection = mSqlConnection;
                    
    //                System.Data.OleDb.OleDbDataReader dr = myOleDbCommand.ExecuteReader();
    //                if (dr.HasRows)
    //                {
    //                    while (dr.Read())
    //                    {
    //                        ArcFileName    = dr["ArcFileName"].ToString();
    //                        UniquiFileName = dr["UniquiFileName"].ToString();
    //                        iRet = Convert.ToInt32(dr["ID"]);
    //                    }
    //                }


    //            }
    //        }
    //        catch (Exception)
    //        {
    //            if (mSqlConnection != null) mSqlConnection.Close();
    //            //Ошибку обрабатываем если что...
    //            return (iRet);
    //        }


    //        return (iRet);
    //    }
        

    //}


    //public enum dbAuthType { atServer = 0, atWindow = 1, atUnknown = 2 };

    //#region Class CDbConnection - Содержит функции работы с базой
    //public class CDbConnection
    //{
    //    //------------------------------------------------------------------------------------------------------
    //    private SqlConnection mSqlConnection;
    //    private string        mSqlServerName;
    //    private dbAuthType    mSqlAuthenticType;
    //    private string        mSqlUserName;
    //    private string        mSqlPassword;
    //    private string        mSqlDBName;
    //    private string        mSqlConnectionString;
    //    //------------------------------------------------------------------------------------------------------
    //    public SqlConnection Connection       { get { return mSqlConnection;       } }
    //    //------------------------------------------------------------------------------------------------------
    //    public string        ConnectionString { get { return mSqlConnectionString; } }        
    //    public dbAuthType    AuthenticType    { get { return mSqlAuthenticType;    } }
    //    public string        ServerName       { get { return mSqlServerName;       } }
    //    public string        UserName         { get { return mSqlUserName;         } }
    //    public string        Password         { get { return mSqlPassword;         } }
    //    public string        DBName           { get { return mSqlDBName;           } }
    //    //------------------------------------------------------------------------------------------------------
        
    //    public CDbConnection()
    //    {
    //        mSqlAuthenticType    = dbAuthType.atUnknown;
    //        mSqlDBName           = "";
    //        mSqlServerName       = "";
    //        mSqlUserName         = "";
    //        mSqlPassword         = "";
    //        mSqlConnectionString = "";
    //        mSqlConnection       = null;
    //    }

    //    public CDbConnection(string dbName, string srvName) : this()
    //    {
    //        mSqlDBName        = dbName;
    //        mSqlServerName    = srvName;
    //        mSqlAuthenticType = dbAuthType.atWindow;
    //    }

    //    public CDbConnection(string dbName, string srvName, string userName, string password) : this()
    //    {
    //        mSqlDBName        = dbName;
    //        mSqlServerName    = srvName;
    //        mSqlUserName      = userName;
    //        mSqlPassword      = password;
    //        mSqlAuthenticType = dbAuthType.atServer;
    //    }

    //    public string CreateConnectionString(string dbName, string srvName)
    //    {
    //        try
    //        {
    //            return(CreateWndAuthConnectionString(dbName, srvName));
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;            
    //        }
    //    }

    //    public string CreateConnectionString(string dbName, string srvName, string userName, string password)
    //    {
    //        try
    //        {
    //            return (CreateSrvAuthConnectionString(dbName, srvName, userName, password));
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    public string CreateWndAuthConnectionString(string dbName, string srvName)
    //    {

    //        if (dbName.Length == 0 || srvName.Length == 0)
    //            throw new Exception("[CreateWndAuthConnectionString] Один или несколько параметров указаны не верно");

    //        mSqlAuthenticType = dbAuthType.atWindow;
    //        mSqlDBName     = dbName;
    //        mSqlServerName = srvName;
    //        mSqlUserName   = "";
    //        mSqlPassword   = "";

    //        mSqlConnectionString = "Integrated Security=SSPI;Persist Security Info=False;" +
    //                               "Initial Catalog=" + mSqlDBName + ";" +
    //                               "Data Source=" + mSqlServerName +
    //                               ";Connection Timeout=60";

    //        return (mSqlConnectionString);
    //    }

    //    public string CreateSrvAuthConnectionString(string dbName, string srvName, string userName, string password )
    //    {            
    //        if (dbName.Length == 0 || srvName.Length == 0 || userName.Length == 0)
    //            throw new Exception("[CreateSrvAuthConnectionString] Один или несколько параметров указаны не верно");

    //        mSqlAuthenticType = dbAuthType.atServer;
    //        mSqlDBName     = dbName;
    //        mSqlServerName = srvName;
    //        mSqlUserName   = userName;
    //        mSqlPassword   = password;

    //        mSqlConnectionString = "Password=" + mSqlPassword + ";" +
    //                               "Persist Security Info=True;" +
    //                               "User ID=" + mSqlUserName + ";" +
    //                               "Initial Catalog=" + mSqlDBName + ";" +
    //                               "Data Source=" + mSqlServerName +
    //                               ";Connection Timeout=60";

    //        return (mSqlConnectionString);
    //    }

    //    public bool Open (string dbName, string srvName)
    //    {
    //        try
    //        {
    //            return ((CreateConnectionString(dbName, srvName).Length == 0) ?(false) : Open());
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    public bool Open (string dbName, string srvName, string userName, string password)
    //    {
    //        try
    //        {
    //            return ((CreateConnectionString(dbName, srvName, userName, password).Length == 0) ? (false) : Open() );
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

    //    public bool Open()
    //    {
    //        if (mSqlConnection != null) Close();

    //        try
    //        {
    //            mSqlConnection = new SqlConnection(mSqlConnectionString);
    //            mSqlConnection.Open();
    //        }
    //        catch (Exception ex)
    //        {
    //            mSqlConnection = null;
    //            throw ex;
    //        }
    //        return (true);
    //    }

    //    public bool Close()
    //    {
    //        mSqlConnection.Close();
    //        mSqlConnection = null;
    //        return (true);
    //    }

    //}
    //#endregion

    //#region Class CTblFolderRow - Описывает строку в таблице tbl_Folder
    //public class CTblFolderRow
    //{
    //    public CTblFolderRow()
    //    {
    //        id       = Guid.Empty;
    //        idParent = Guid.Empty;
    //        Name     = "";
    //    }

    //    public CTblFolderRow(Guid idParent, String Name) : this()
    //    {
    //        this.idParent = idParent;
    //        this.Name = Name;
    //    }

    //    public Guid   id;
    //    public Guid   idParent;
    //    public String Name;

    //}
    //#endregion

    //#region Class CTblFileRow - Описывает строку в таблице tbl_File
    //public class CTblFileRow
    //{
    //    public CTblFileRow()
    //    {
    //        id          = Guid.Empty;
    //        idFolder    = Guid.Empty;	
    //        Name        = "";
    //        Extention   = "";
    //        Path        = "";
    //        Size        = 0;
    //        Description = "";
    //        Authors     = "";
    //        Pages       = 0;
    //        Year        = 0;
    //        Language    = "";
    //        Publisher   = "";
    //        idData      = Guid.Empty;
    //        ImageSize   = 0;
    //        idImageData = Guid.Empty;
    //        ImageName   = "";
    //        MD5        = new byte[16];
    //    }

    //    public CTblFileRow(Guid id, Guid idFolder, 
    //                       string Name, string Extention, string Path, 
    //                       int Size, 
    //                       string Description, string Authors, 
    //                       int Pages, int Year, 
    //                       string Language, string Publisher, 
    //                       Guid idData,
    //                       int ImageSize, Guid idImageData, string ImageName, byte[] MD5 ) : this()
    //    {
    //        this.id = id;
    //        this.idFolder = idFolder;
    //        this.Name = Name;
    //        this.Extention = Extention;
    //        this.Path = Path;
    //        this.Size = Size;
    //        this.Description = Description;
    //        this.Authors = Authors;
    //        this.Pages = Pages;
    //        this.Year = Year;
    //        this.Language = Language;
    //        this.Publisher = Publisher;
    //        this.idData = idData;
    //        this.ImageSize = ImageSize;
    //        this.idImageData = idImageData;
    //        this.ImageName = ImageName;
    //        this.MD5 = MD5;
    //    }


    //    public Guid   id;
    //    public Guid   idFolder;	
    //    public string Name;
    //    public string Extention;
    //    public string Path;
    //    public int    Size;
    //    public string Description;
    //    public string Authors;
    //    public int    Pages;
    //    public int    Year;
    //    public string Language;
    //    public string Publisher;
    //    public Guid   idData;
    //    public int    ImageSize;
    //    public Guid   idImageData;
    //    public string ImageName;
    //    public byte[] MD5;

    //}
    //#endregion

    //#region Class CTblLanguageRow - Описывает строку в таблице tbl_Language
    //public class CTblLanguageRow
    //{
    //    public CTblLanguageRow()
    //    {
    //        id = Guid.Empty;
    //        LangFullName  = "";
    //        LangShortName = "";
    //    }

    //    public CTblLanguageRow(String LangFullName, String LangShortName) : this()
    //    {
    //        this.LangFullName  = LangFullName;
    //        this.LangShortName = LangFullName;
    //    }

    //    public override string ToString() { return (LangShortName + " [" + LangFullName + "]"); }

    //    public Guid id;
    //    public String LangFullName;
    //    public String LangShortName;

    //}
    //#endregion

    //#region Class CTblExtRow - Описывает строку в таблице tbl_Extention
    //public class CTblExtRow
    //{
    //    public CTblExtRow()
    //    {
    //        id = Guid.Empty;
    //        Extention   = "";
    //        Description = "";
    //        ImageSize = 0;
    //        idImageData = Guid.Empty;
    //    }

    //    public CTblExtRow(String Extention, String Description, int ImageSize, Guid idImageData)
    //        : this()
    //    {
    //        this.Extention   = Extention;
    //        this.Description = Description;
    //        this.ImageSize   = ImageSize;
    //        this.idImageData = idImageData;
    //    }

    //    public override string ToString() { return (Extention + " [" + Description + "]"); }

    //    public Guid   id;
    //    public String Extention;
    //    public String Description;
    //    public int ImageSize;
    //    public Guid idImageData;


    //}
    //#endregion

    //#region Class CTblDataRow - Описывает строку в таблице tbl_Extention
    //public class CTblDataRow
    //{
    //    public CTblDataRow()
    //    {
    //        id   = Guid.Empty;
    //        Data = null;
    //    }

    //    public CTblDataRow(byte[] Data, string MD5) : this()
    //    {
    //        this.Data = Data;
    //    }

    //    public Guid id;
    //    public byte[] Data;

    //}
    //#endregion

    //#region Class CTblPublisherRow - Описывает строку в таблице tbl_Publisher
    //public class CTblPublisherRow
    //{
    //    public CTblPublisherRow()
    //    {
    //        id = Guid.Empty;
    //        Publisher = "";
    //    }

    //    public CTblPublisherRow(String Publisher) : this()
    //    {
    //        this.Publisher = Publisher;
    //    }

    //    public override string ToString() { return (Publisher); }

    //    public Guid id;
    //    public String Publisher;
    //}
    //#endregion

    //#region Class CTblFolder - Содержит функции по работе с таблицей Folder
    //public class CTblFolder
    //{
    //    CDbConnection DbConnection;

    //    public CTblFolder(CDbConnection conn) { DbConnection = conn; }

    //    // Возвращает список папок с определнным idParent
    //    public List<CTblFolderRow> GetFoldersByParentId(Guid idParent)
    //    {

    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[GetFoldersByParentID]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idparent", idParent);

    //        SqlDataReader sql_reader = null;

    //        try
    //        {
    //            sql_reader = cmd.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        List<CTblFolderRow> FoldersList = new List<CTblFolderRow>();
    //        while (sql_reader.Read())
    //        {
    //            CTblFolderRow folderRow = new CTblFolderRow();
    //            folderRow.id = (Guid)sql_reader[0];
    //            folderRow.idParent = (Guid)sql_reader[1];
    //            folderRow.Name = (string)sql_reader[2];
    //            FoldersList.Add(folderRow);
    //        }

    //        sql_reader.Close();

    //        return (FoldersList);
    //    }

    //    //Добавляет новую папок в базу
    //    public Guid AddNewFolder(Guid idParent, string Name)
    //    {

    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[AddFolder]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", null, ParameterDirection.Output);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idparent", idParent);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@name",     Name);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return ((Guid)cmd.Parameters["@id"].Value);
    //        //return ((Guid)paramId.Value);
    //    }

    //    //Добавляет новую папок в базу
    //    public bool AddNewFolder(ref CTblFolderRow folderRow)
    //    {
    //        try
    //        {
    //            folderRow.id = AddNewFolder(folderRow.idParent, folderRow.Name);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;            
    //        }
    //        return true;
    //    }

    //    //Удаляет строку с идентификатором id
    //    public bool DeleteFolderById(Guid id)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[DeleteFolderByID]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", id);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    public bool UpdateFolder(Guid id, Guid idParent, string Name)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[UpdateFolder]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id",       id);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idparent", idParent);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@name",     Name);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    public bool UpdateFolder(ref CTblFolderRow folderRow)
    //    {
    //        return (UpdateFolder(folderRow.id, folderRow.idParent, folderRow.Name));
    //    }

    //}


    //#endregion 

    //#region Class CTblFile - Содержит функции по работе с таблицей File
    //public class CTblFile
    //{
    //    CDbConnection DbConnection;

    //    public CTblFile(CDbConnection conn) { DbConnection = conn; }

    //    public List<CTblFileRow> GetFilesByFolderId(Guid idParent)
    //    {

    //        SqlCommand cmd = new SqlCommand();
    //        cmd.Connection = DbConnection.Connection;
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = @"[dbo].[GetFilesByFolderID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@idFolder";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = idParent;
    //        cmd.Parameters.Add(paramId);

    //        SqlDataReader sql_reader = null;

    //        try
    //        {
    //            sql_reader = cmd.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        List<CTblFileRow> FilesList = new List<CTblFileRow>();

    //        while (sql_reader.Read())
    //        {
    //            CTblFileRow fileRow = new CTblFileRow();

    //            fileRow.id          = (Guid)sql_reader["id"];
    //            fileRow.idFolder    = (Guid)sql_reader["idFolder"];
    //            fileRow.Name        = sql_reader["Name"].ToString();
    //            fileRow.Extention   = sql_reader["Extention"].ToString();
    //            fileRow.Path        = sql_reader["Path"].ToString();
    //            fileRow.Size        = (int)sql_reader["Size"];
    //            fileRow.Description = sql_reader["Description"].ToString();
    //            fileRow.Authors     = sql_reader["Authors"].ToString();
    //            fileRow.Pages       = (int)sql_reader["Pages"];
    //            fileRow.Year        = (int)sql_reader["Year"];
    //            fileRow.Language    = sql_reader["Language"].ToString();
    //            fileRow.Publisher   = sql_reader["Publisher"].ToString();
    //            fileRow.idData      = (Guid)sql_reader["idData"];
    //            fileRow.ImageSize   = (int)sql_reader["ImageSize"];
    //            fileRow.idImageData = (Guid)sql_reader["idImageData"];
    //            fileRow.ImageName   = sql_reader["ImageName"].ToString();
    //            fileRow.MD5         = (byte[])sql_reader["MD5"];

    //            FilesList.Add(fileRow);
    //        }

    //        sql_reader.Close();

    //        return (FilesList);
    //    }

    //    public CTblFileRow GetFileById(Guid idParent)
    //    {
    //        SqlDataReader sql_reader = null;

    //        CTblFileRow FileRow = null;

    //        SqlCommand cmd = new SqlCommand();
    //        cmd.Connection = DbConnection.Connection;
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = @"[dbo].[GetFileByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = idParent;
    //        cmd.Parameters.Add(paramId);

    //        try
    //        {
    //            sql_reader = cmd.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        if (sql_reader.Read())
    //        {
    //            FileRow = new CTblFileRow();

    //            FileRow.id          = (Guid)sql_reader["id"];
    //            FileRow.idFolder    = (Guid)sql_reader["idFolder"];
    //            FileRow.Name        = sql_reader["Name"].ToString();
    //            FileRow.Extention   = sql_reader["Extention"].ToString();
    //            FileRow.Path        = sql_reader["Path"].ToString();
    //            FileRow.Size        = (int)sql_reader["Size"];
    //            FileRow.Description = sql_reader["Description"].ToString();
    //            FileRow.Authors     = sql_reader["Authors"].ToString();
    //            FileRow.Pages       = (int)sql_reader["Pages"];
    //            FileRow.Year        = (int)sql_reader["Year"];
    //            FileRow.Language    = sql_reader["Language"].ToString();
    //            FileRow.Publisher   = sql_reader["Publisher"].ToString();
    //            FileRow.idData      = (Guid)sql_reader["idData"];
    //            FileRow.ImageSize   = (int)sql_reader["ImageSize"];
    //            FileRow.idImageData = (Guid)sql_reader["idImageData"];
    //            FileRow.ImageName   = sql_reader["ImageName"].ToString();
    //            FileRow.MD5 = (byte[])sql_reader["MD5"];
    //        }

    //        sql_reader.Close();

    //        return (FileRow);
    //    }

    //    //Добавляет новую папок в базу
    //    public Guid AddNewFile(Guid idFolder,
    //                           string Name, string Extention, string Path,
    //                           int Size,
    //                           string Description, string Authors,
    //                           int Pages, int Year,
    //                           string Language, string Publisher,
    //                           Guid idData,
    //                           int ImageSize, Guid idImageData, string ImageName, byte[] MD5)
    //    {

    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[AddFile]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id",          null, ParameterDirection.Output);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idFolder",    idFolder   );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Name",        Name       );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Extention",   Extention  );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Path",        Path       );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Size",        Size       );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Description", Description);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Authors",     Authors    );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.Int,              "@Pages",       Pages      );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.Int,              "@Year",        Year       );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Language",    Language   );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Publisher",   Publisher  );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idData",      idData     );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.Int,              "@ImageSize",   ImageSize  );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idImageData", idImageData);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@ImageName",   ImageName  );
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.VarBinary,        "@MD5",         MD5        );

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return ((Guid) cmd.Parameters["@id"].Value);
    //    }

    //    public bool AddNewFile(ref CTblFileRow fileRow)
    //    {

    //        try
    //        {

    //            fileRow.id = AddNewFile(fileRow.idFolder,
    //                                    fileRow.Name, fileRow.Extention, fileRow.Path,
    //                                    fileRow.Size,
    //                                    fileRow.Description, fileRow.Authors,
    //                                    fileRow.Pages, fileRow.Year,
    //                                    fileRow.Language, fileRow.Publisher,
    //                                    fileRow.idData,fileRow.ImageSize, fileRow.idImageData,fileRow.ImageName, fileRow.MD5);

    //        }
    //        catch(Exception ex)
    //        {
    //            throw ex;            
    //        }

    //        return true;
    //    }


    //    //Удаляет строку с идентификатором id
    //    public bool DeleteFileById(Guid id)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[DeleteFileByID]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", id);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    public CTblFileRow IsAlreadyExist(byte[] MD5)
    //    {

    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[IsFileExistByMD5]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.VarBinary, "@MD5", @MD5);

    //        SqlDataReader sql_reader = null;
    //        CTblFileRow FileRow = null;

    //        try
    //        {
    //            sql_reader = cmd.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        if (sql_reader.Read())
    //        {
    //            FileRow = new CTblFileRow();

    //            FileRow.id          = (Guid)sql_reader["id"];
    //            FileRow.idFolder    = (Guid)sql_reader["idFolder"];
    //            FileRow.Name        = sql_reader["Name"].ToString();
    //            FileRow.Extention   = sql_reader["Extention"].ToString();
    //            FileRow.Path        = sql_reader["Path"].ToString();
    //            FileRow.Size        = (int)sql_reader["Size"];
    //            FileRow.Description = sql_reader["Description"].ToString();
    //            FileRow.Authors     = sql_reader["Authors"].ToString();
    //            FileRow.Pages       = (int)sql_reader["Pages"];
    //            FileRow.Year        = (int)sql_reader["Year"];
    //            FileRow.Language    = sql_reader["Language"].ToString();
    //            FileRow.Publisher   = sql_reader["Publisher"].ToString();
    //            FileRow.idData      = (Guid)sql_reader["idData"];
    //            FileRow.ImageSize   = (int)sql_reader["ImageSize"];
    //            FileRow.idImageData = (Guid)sql_reader["idImageData"];
    //            FileRow.ImageName   = sql_reader["ImageName"].ToString();
    //        }

    //        sql_reader.Close();

    //        return (FileRow);
    //    }

    //    public bool UpdateFile(Guid id, Guid idFolder,
    //                   string Name, string Extention, string Path,
    //                   int Size,
    //                   string Description, string Authors,
    //                   int Pages, int Year,
    //                   string Language, string Publisher,
    //                   Guid idData,
    //                   int ImageSize, Guid idImageData, string ImageName, byte[] MD5)
    //    {

    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[UpdateFile]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id",          id);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idFolder",    idFolder);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Name",        Name);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Extention",   Extention);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Path",        Path);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Size",        Size);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Description", Description);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Authors",     Authors);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.Int,              "@Pages",       Pages);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.Int,              "@Year",        Year);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Language",    Language);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@Publisher",   Publisher);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idData",      idData);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.Int,              "@ImageSize",   ImageSize);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@idImageData", idImageData);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.NVarChar,         "@ImageName",   ImageName);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.VarBinary,        "@MD5",         MD5);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    public bool UpdateFile(ref CTblFileRow fileRow)
    //    {

    //        try
    //        {

    //            UpdateFile(fileRow.id,fileRow.idFolder,
    //                       fileRow.Name, fileRow.Extention, fileRow.Path,
    //                       fileRow.Size,
    //                       fileRow.Description, fileRow.Authors,
    //                       fileRow.Pages, fileRow.Year,
    //                       fileRow.Language, fileRow.Publisher,
    //                       fileRow.idData, fileRow.ImageSize, fileRow.idImageData, fileRow.ImageName, fileRow.MD5);

    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return true;
    //    }

    //}
    //#endregion

    //#region Class CTblLanguage - Содержит функции по работе с таблицей Language
    //public class CTblLanguage
    //{
    //    CDbConnection DbConnection;

    //    public CTblLanguage(CDbConnection conn) { DbConnection = conn; }

    //    //Добавляет новую папок в базу
    //    public Guid AddNewLanguage(string LangFullName, string LangShortName)
    //    {

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[AddLanguage]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Output;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        SqlParameter paramLangFullName = new SqlParameter();
    //        paramLangFullName.Direction = ParameterDirection.Input;
    //        paramLangFullName.ParameterName = "@LangFullName";
    //        paramLangFullName.SqlDbType = SqlDbType.NVarChar;

    //        SqlParameter paramLangShortName = new SqlParameter();
    //        paramLangShortName.Direction = ParameterDirection.Input;
    //        paramLangShortName.ParameterName = "@LangShortName";
    //        paramLangShortName.SqlDbType = SqlDbType.NVarChar;

    //        paramLangFullName.Value  = LangFullName;
    //        paramLangShortName.Value = LangShortName;

    //        sqlCommand.Parameters.Add(paramId);
    //        sqlCommand.Parameters.Add(paramLangFullName);
    //        sqlCommand.Parameters.Add(paramLangShortName);

    //        try
    //        {
    //            sqlCommand.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return ((Guid)paramId.Value);
    //    }

    //    //Добавляет новую папок в базу
    //    public bool AddNewLanguage(ref CTblLanguageRow langRow)
    //    {
    //        try
    //        {
    //            langRow.id = AddNewLanguage(langRow.LangFullName, langRow.LangShortName);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;            
    //        }
    //        return true;
    //    }

    //    //Удаляет строку с идентификатором id
    //    public bool DeleteLanguageById(Guid id)
    //    {
    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[DeleteLanguageByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = id;
    //        sqlCommand.Parameters.Add(paramId);

    //        try
    //        {
    //            sqlCommand.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    // Возвращает список папок с определнным id
    //    public List<CTblLanguageRow> GetLanguageById(Guid id)
    //    {
    //        SqlDataReader sql_reader = null;

    //        List<CTblLanguageRow> LanguageList = new List<CTblLanguageRow>();

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[GetLanguageByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = id;
    //        sqlCommand.Parameters.Add(paramId);

    //        try
    //        {
    //            sql_reader = sqlCommand.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        while (sql_reader.Read())
    //        {
    //            CTblLanguageRow langRow = new CTblLanguageRow();
    //            langRow.id = (Guid)sql_reader[0];
    //            langRow.LangFullName= sql_reader[1].ToString();
    //            langRow.LangShortName= sql_reader[2].ToString();
    //            LanguageList.Add(langRow);
    //        }

    //        sql_reader.Close();

    //        return (LanguageList);
    //    }

    //    // Возвращает список всех папок
    //    public List<CTblLanguageRow> GetAllLanguage()
    //    {
    //        SqlDataReader sql_reader = null;

    //        List<CTblLanguageRow> LanguageList = new List<CTblLanguageRow>();

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[GetAllLanguage]";

    //        try
    //        {
    //            sql_reader = sqlCommand.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        while (sql_reader.Read())
    //        {
    //            CTblLanguageRow langRow = new CTblLanguageRow();
    //            langRow.id = (Guid)sql_reader[0];
    //            langRow.LangFullName= sql_reader[1].ToString();
    //            langRow.LangShortName= sql_reader[2].ToString();
    //            LanguageList.Add(langRow);
    //        }

    //        sql_reader.Close();

    //        return (LanguageList);
    //    }


    //}
    //#endregion

    //#region Class CTblExt - Содержит функции по работе с таблицей Extention
    //public class CTblExt
    //{
    //    CDbConnection DbConnection;

    //    public CTblExt(CDbConnection conn) { DbConnection = conn; }

    //    //Добавляет новую папок в базу
    //    public Guid AddNewExt(string Extention, string Description, int ImageSize, Guid idImageData)
    //    {

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[AddExt]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Output;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        SqlParameter paramExtention = new SqlParameter();
    //        paramExtention.Direction = ParameterDirection.Input;
    //        paramExtention.ParameterName = "@Extention";
    //        paramExtention.SqlDbType = SqlDbType.NVarChar;

    //        SqlParameter paramDescription = new SqlParameter();
    //        paramDescription.Direction = ParameterDirection.Input;
    //        paramDescription.ParameterName = "@Description";
    //        paramDescription.SqlDbType = SqlDbType.NVarChar;

    //        SqlParameter paramImageSize = new SqlParameter();
    //        paramImageSize.Direction = ParameterDirection.Input;
    //        paramImageSize.ParameterName = "@ImageSize";
    //        paramImageSize.SqlDbType = SqlDbType.Int;

    //        SqlParameter paramImageData = new SqlParameter();
    //        paramImageData.Direction = ParameterDirection.Input;
    //        paramImageData.ParameterName = "@idImageData";
    //        paramImageData.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramExtention.Value   = Extention;
    //        paramDescription.Value = Description;
    //        paramImageSize.Value   = ImageSize;
    //        paramImageData.Value   = idImageData;

    //        sqlCommand.Parameters.Add(paramId);
    //        sqlCommand.Parameters.Add(paramExtention);
    //        sqlCommand.Parameters.Add(paramDescription);
    //        sqlCommand.Parameters.Add(paramImageSize);
    //        sqlCommand.Parameters.Add(paramImageData);

    //        try
    //        {
    //            sqlCommand.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return ((Guid)paramId.Value);
    //    }

    //    //Добавляет новую папок в базу
    //    public bool AddNewExt(ref CTblExtRow extRow)
    //    {
    //        try
    //        {
    //            extRow.id = AddNewExt(extRow.Extention, extRow.Description,extRow.ImageSize, extRow.idImageData);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return true;
    //    }

    //    //Удаляет строку с идентификатором id
    //    public bool DeleteExtById(Guid id)
    //    {
    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[DeleteExtByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = id;
    //        sqlCommand.Parameters.Add(paramId);

    //        try
    //        {
    //            sqlCommand.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    // Возвращает список папок с определнным id
    //    public List<CTblExtRow> GetExtById(Guid id)
    //    {
    //        SqlDataReader sql_reader = null;

    //        List<CTblExtRow> ExtList = new List<CTblExtRow>();

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[GetExtByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = id;
    //        sqlCommand.Parameters.Add(paramId);

    //        try
    //        {
    //            sql_reader = sqlCommand.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        while (sql_reader.Read())
    //        {
    //            CTblExtRow extRow = new CTblExtRow();
    //            extRow.id          = (Guid)sql_reader["id"];
    //            extRow.Extention   = sql_reader["Extention"].ToString();
    //            extRow.Description = sql_reader["Description"].ToString();
    //            extRow.ImageSize   = (int)sql_reader["ImageSize"];
    //            extRow.idImageData = (Guid)sql_reader["idImageData"];

    //            ExtList.Add(extRow);
    //        }

    //        sql_reader.Close();

    //        return (ExtList);
    //    }

    //    // Возвращает список всех папок
    //    public List<CTblExtRow> GetAllExt()
    //    {
    //        SqlDataReader sql_reader = null;

    //        List<CTblExtRow> ExtList = new List<CTblExtRow>();

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[GetAllExt]";

    //        try
    //        {
    //            sql_reader = sqlCommand.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        while (sql_reader.Read())
    //        {
    //            CTblExtRow extRow = new CTblExtRow();
    //            extRow.id          = (Guid)sql_reader["id"];
    //            extRow.Extention   = sql_reader["Extention"].ToString();
    //            extRow.Description = sql_reader["Description"].ToString();
    //            extRow.ImageSize   = (int)sql_reader["ImageSize"];
    //            extRow.idImageData = (Guid)sql_reader["idImageData"];
    //            ExtList.Add(extRow);
    //        }

    //        sql_reader.Close();

    //        return (ExtList);
    //    }


    //}
    //#endregion

    //#region Class CTblData - Содержит функции по работе с таблицей Extention
    //public class CTblData
    //{
    //    CDbConnection DbConnection;

    //    public CTblData(CDbConnection conn) { DbConnection = conn; }

    //    //Добавляет новую папок в базу
    //    public Guid AddNewData(byte[] Data)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[AddData]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", null, ParameterDirection.Output);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.VarBinary, "@Data", Data);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return ((Guid)cmd.Parameters["@id"].Value);
    //    }

    //    //Добавляет новую папок в базу
    //    public bool AddNewData(ref CTblDataRow dataRow)
    //    {
    //        try
    //        {
    //            dataRow.id = AddNewData(dataRow.Data);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return true;
    //    }

    //    //Удаляет строку с идентификатором id
    //    public bool DeleteDataById(Guid id)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[DeleteDataByID]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", id);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    // Возвращает список папок с определнным id
    //    public List<CTblDataRow> GetDataById(Guid id)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[GetDataByID]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", id);

    //        SqlDataReader sql_reader = null;
    //        try
    //        {
    //            sql_reader = cmd.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        List<CTblDataRow> DataList = new List<CTblDataRow>();
    //        while (sql_reader.Read())
    //        {
    //            CTblDataRow dataRow = new CTblDataRow();
    //            dataRow.id   = (Guid)sql_reader["id"];
    //            dataRow.Data = (byte[])sql_reader["Data"];

    //            DataList.Add(dataRow);
    //        }

    //        sql_reader.Close();

    //        return (DataList);
    //    }

    //    // Возвращает список всех папок
    //    public List<CTblDataRow> GetAllData()
    //    {

    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[GetAllData]");

    //        SqlDataReader sql_reader = null;
    //        try
    //        {
    //            sql_reader = cmd.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        List<CTblDataRow> DataList = new List<CTblDataRow>();
    //        while (sql_reader.Read())
    //        {
    //            CTblDataRow dataRow = new CTblDataRow();
    //            dataRow.id = (Guid)sql_reader["id"];
    //            dataRow.Data = (byte[])sql_reader["Data"];
    //            DataList.Add(dataRow);
    //        }

    //        sql_reader.Close();

    //        return (DataList);
    //    }

    //    public bool UpdateData(Guid id, byte[] Data)
    //    {
    //        SqlCommand cmd = DBManagementClass.CreateCmd(DbConnection.Connection, CommandType.StoredProcedure, @"[dbo].[UpdateData]");

    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.UniqueIdentifier, "@id", id);
    //        DBManagementClass.AddCmdParam(ref cmd, SqlDbType.VarBinary, "@Data", Data);

    //        try
    //        {
    //            cmd.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    public bool UpdateData(ref CTblDataRow dataRow)
    //    {
    //        try
    //        {
    //            UpdateData(dataRow.id, dataRow.Data);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }


    //}
    //#endregion

    //#region Class CTblPublisher - Содержит функции по работе с таблицей Publisher
    //public class CTblPublisher
    //{
    //    CDbConnection DbConnection;

    //    public CTblPublisher(CDbConnection conn) { DbConnection = conn; }

    //    //Добавляет новую папок в базу
    //    public Guid AddNewPublisher(string Publisher)
    //    {

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[AddPublisher]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Output;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        SqlParameter paramPublisher = new SqlParameter();
    //        paramPublisher.Direction = ParameterDirection.Input;
    //        paramPublisher.ParameterName = "@Publisher";
    //        paramPublisher.SqlDbType = SqlDbType.NVarChar;

    //        paramPublisher.Value = Publisher;

    //        sqlCommand.Parameters.Add(paramId);
    //        sqlCommand.Parameters.Add(paramPublisher);

    //        try
    //        {
    //            sqlCommand.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return ((Guid)paramId.Value);
    //    }

    //    //Добавляет новую папок в базу
    //    public bool AddNewPublisher(ref CTblPublisherRow publisherRow)
    //    {
    //        try
    //        {
    //            publisherRow.id = AddNewPublisher(publisherRow.Publisher);
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;            
    //        }
    //        return true;

    //    }

    //    //Удаляет строку с идентификатором id
    //    public bool DeletePublisherById(Guid id)
    //    {
    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[DeletePublisherByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = id;
    //        sqlCommand.Parameters.Add(paramId);

    //        try
    //        {
    //            sqlCommand.ExecuteNonQuery();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        return (true);
    //    }

    //    // Возвращает список папок с определнным id
    //    public List<CTblPublisherRow> GetPublisherById(Guid id)
    //    {
    //        SqlDataReader sql_reader = null;

    //        List<CTblPublisherRow> PublisherList = new List<CTblPublisherRow>();

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[GetPublisherByID]";

    //        SqlParameter paramId = new SqlParameter();
    //        paramId.Direction = ParameterDirection.Input;
    //        paramId.ParameterName = "@id";
    //        paramId.SqlDbType = SqlDbType.UniqueIdentifier;

    //        paramId.Value = id;
    //        sqlCommand.Parameters.Add(paramId);

    //        try
    //        {
    //            sql_reader = sqlCommand.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        while (sql_reader.Read())
    //        {
    //            CTblPublisherRow PublisherRow = new CTblPublisherRow();
    //            PublisherRow.id = (Guid)sql_reader[0];
    //            PublisherRow.Publisher = sql_reader[1].ToString();
    //            PublisherList.Add(PublisherRow);
    //        }

    //        sql_reader.Close();

    //        return (PublisherList);
    //    }

    //    // Возвращает список всех папок
    //    public List<CTblPublisherRow> GetAllPublisher()
    //    {
    //        SqlDataReader sql_reader = null;

    //        List<CTblPublisherRow> PublisherList = new List<CTblPublisherRow>();

    //        SqlCommand sqlCommand = new SqlCommand();
    //        sqlCommand.Connection = DbConnection.Connection;
    //        sqlCommand.CommandType = CommandType.StoredProcedure;
    //        sqlCommand.CommandText = @"[dbo].[GetAllPublisher]";

    //        try
    //        {
    //            sql_reader = sqlCommand.ExecuteReader();
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }

    //        while (sql_reader.Read())
    //        {
    //            CTblPublisherRow PublisherRow = new CTblPublisherRow();
    //            PublisherRow.id = (Guid)sql_reader[0];
    //            PublisherRow.Publisher = sql_reader[1].ToString();
    //            PublisherList.Add(PublisherRow);
    //        }

    //        sql_reader.Close();

    //        return (PublisherList);
    //    }


    //}
    //#endregion

    //#region Класс для работы с Базой Данных
    //public class DBManagementClass
    //{
    //    //-----------------------------------------------------------
    //    private CDbConnection _Connection   = null;
    //    //private CTblFolder    _TblFolder    = null;
    //    //private CTblFile      _TblFile      = null;
    //    //private CTblLanguage  _TblLanguage  = null;
    //    //private CTblExt       _TblExt       = null;
    //    //private CTblPublisher _TblPublisher = null;
    //    //private CTblData      _TblData      = null;
    //    //-----------------------------------------------------------
    //    public CDbConnection TblsConnection { get { return _Connection;  } }
    //    //public CTblFolder    TblFolder      { get { return _TblFolder;   } }
    //    //public CTblFile      TblFile        { get { return _TblFile;     } }
    //    //public CTblLanguage  TblLanguage    { get { return _TblLanguage; } }
    //    //public CTblExt       TblExt         { get { return _TblExt;      } }
    //    //public CTblPublisher TblPublisher   { get { return _TblPublisher;} }
    //    //public CTblData      TblData        { get { return _TblData;     } }
    //    //-----------------------------------------------------------

    //    public DBManagementClass(string dbName, string srvName)
    //    {
    //        _Connection   = new CDbConnection(dbName, srvName);
    //        //_TblFolder    = new CTblFolder   (_Connection);
    //        //_TblFile      = new CTblFile     (_Connection);
    //        //_TblLanguage  = new CTblLanguage (_Connection);
    //        //_TblExt       = new CTblExt      (_Connection);
    //        //_TblPublisher = new CTblPublisher(_Connection);
    //        //_TblData      = new CTblData     (_Connection);
    //    }

    //    public DBManagementClass(string dbName, string srvName, string userName, string password)
    //    {
    //        _Connection   = new CDbConnection(dbName, srvName, userName, password);
    //        //_TblFolder    = new CTblFolder   (_Connection);
    //        //_TblFile      = new CTblFile     (_Connection);
    //        //_TblLanguage  = new CTblLanguage (_Connection);
    //        //_TblExt       = new CTblExt      (_Connection);
    //        //_TblPublisher = new CTblPublisher(_Connection);
    //        //_TblData      = new CTblData     (_Connection);
    //    }

    //    public DBManagementClass()
    //    {
    //        _Connection   = new CDbConnection();
    //        //_TblFolder    = new CTblFolder   (_Connection);
    //        //_TblFile      = new CTblFile     (_Connection);
    //        //_TblLanguage  = new CTblLanguage (_Connection);
    //        //_TblExt       = new CTblExt      (_Connection);
    //        //_TblPublisher = new CTblPublisher(_Connection);
    //        //_TblData      = new CTblData     (_Connection);
    //    }

    //    internal static void AddCmdParam(ref SqlCommand command, SqlDbType dbType, string paramName, object value, ParameterDirection paramDirection)
    //    {
    //        SqlParameter retParam = new SqlParameter();
    //        retParam.Direction = paramDirection;
    //        retParam.ParameterName = paramName;
    //        retParam.SqlDbType = dbType;
    //        retParam.Value = value;
    //        command.Parameters.Add(retParam);
    //    }

    //    internal static void AddCmdParam(ref SqlCommand command, SqlDbType dbType, string paramName, object value)
    //    {
    //        SqlParameter retParam = new SqlParameter();
    //        retParam.Direction = ParameterDirection.Input;
    //        retParam.ParameterName = paramName;
    //        retParam.SqlDbType = dbType;
    //        retParam.Value = value;
    //        command.Parameters.Add(retParam);
    //    }

    //    internal static SqlCommand CreateCmd(SqlConnection connection, CommandType cmdType, string cmdText)
    //    {
    //        SqlCommand retCmd = new SqlCommand();
    //        retCmd.Connection = connection;
    //        retCmd.CommandType = cmdType;
    //        retCmd.CommandText = cmdText;
    //        return (retCmd);
    //    }

    //}

    //#endregion


    //#region Class CDataIntegrity - Целостность данных

    //public class CDataIntegrity
    //{

    //    internal static string GetMD5HashString(byte[] Data)
    //    {
    //        return (ConvertMD5BytesToStr(GetMD5HashBytes(Data)));
    //    }

    //    internal static string GetMD5HashString(Stream stream)
    //    {
    //        return (ConvertMD5BytesToStr(GetMD5HashBytes(stream)));
    //    }

    //    internal static byte[] GetMD5HashBytes(byte[] Data)
    //    {
    //        // Create a new instance of the MD5CryptoServiceProvider object.
    //        MD5 md5Hasher = MD5.Create();

    //        // Convert the input string to a byte array and compute the hash.

    //        byte[] data = md5Hasher.ComputeHash(Data);

    //        return (data);
    //    }

    //    internal static byte[] GetMD5HashBytes(Stream stream)
    //    {
    //        // Create a new instance of the MD5CryptoServiceProvider object.
    //        MD5 md5Hasher = MD5.Create();

    //        // Convert the input string to a byte array and compute the hash.

    //        byte[] data = md5Hasher.ComputeHash(stream);

    //        return (data);
    //    }

    //    internal static string ConvertMD5BytesToStr(byte[] bData)
    //    {
    //        // Create a new Stringbuilder to collect the bytes
    //        // and create a string.
    //        StringBuilder sBuilder = new StringBuilder();

    //        // Loop through each byte of the hashed data 
    //        // and format each one as a hexadecimal string.
    //        for (int i = 0; i < bData.Length; i++) sBuilder.Append(bData[i].ToString("X2"));

    //        // Return the hexadecimal string.
    //        return (sBuilder.ToString());
    //    }

    //    internal static byte[] ConvertMD5StrToBytes(string sData)
    //    {
    //        byte[] bBuffer = new byte[16];

    //        for (int i = 0; i < (sData.Length / 2); i++) 
    //        {
    //            string str = sData.Substring(i * 2, 2);
    //            bBuffer[i] = Convert.ToByte(str,16);
    //        }
    //        return (bBuffer);
    //    }



    //}
    //#endregion

    //public enum DbActionState
    //{   
    //    dbUnknown = 0,
    //    dbasEdit = 1,
    //    dbasAdd = 2
    //}
}
