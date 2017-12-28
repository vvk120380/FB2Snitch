using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    public enum eRetError { NoErr = 0, ErrReadDesc, ErrAlreadyAdd, ErrAddToDB, ErrAddToArc, ErrMD5, ErrDelFile} 
    public struct RetStatus
    {
        public eRetError error;
        public int id;

        public RetStatus(eRetError err, int id)
        {
            error = err;
            this.id = id;
        }
    }
    public class FB2SnitchManager
    {
        DAL.DBManager dbManager = null;

        public FB2SnitchManager()
        {
            dbManager = new DAL.DBManager();
        }

        public bool CheckConnection()
        {
            return dbManager.CheckConnection();
        }

        public RetStatus AddBook(String fb2fullfilename)
        {

            TimeSpan ts_read_description;
            TimeSpan ts_get_hash;
            TimeSpan ts_is_book_added;
            TimeSpan ts_add_book_to_zip;
            TimeSpan ts_add_book_to_db;
            string elapsedTime;
            Stopwatch stopWatch = new Stopwatch();

            string shortarcfilename = string.Empty;
            string hash = string.Empty;

            try
            {
                stopWatch.Start();

                Console.WriteLine($"-- start {fb2fullfilename}");

                //1. Проверяем что это именно fb2 файл и сваливаем если это не так
                //FB2Description fb2desc = FB2Manager.ReadDecription(fb2fullfilename);
                FB2Description fb2desc = FB2Manager.ReadDecriptionFast(fb2fullfilename);                
                ts_read_description = stopWatch.Elapsed;
                stopWatch.Restart();
                //2. Подсчитали MD5-хешь сумму
                hash = MD5Hash.GetFileHash(fb2fullfilename);
                ts_get_hash = stopWatch.Elapsed;
                stopWatch.Restart();
                //3. Проверяем в DB, что такой файл еще не добавлен
                int bookid = dbManager.IsBookHasBeenAlreadyAddedInDB(hash);
                if (bookid > -1) return new RetStatus(eRetError.ErrAlreadyAdd, bookid);
                ts_is_book_added = stopWatch.Elapsed;
                stopWatch.Restart();
                //4. Добавили его в архив (нашли архив в который его добавлять, сгенерировали уникальное имя, заархивировали)
                shortarcfilename = ZipBLL.AddFile(fb2fullfilename, hash + ".fb2");
                ts_add_book_to_zip = stopWatch.Elapsed;
                stopWatch.Restart();
                //5. Добавили его в DB 
                RetStatus retStaus = new RetStatus(eRetError.NoErr, dbManager.AddBook(fb2desc, shortarcfilename, hash));
                ts_add_book_to_db = stopWatch.Elapsed;
                stopWatch.Restart();

                Console.WriteLine (String.Format("{0:00}.{1:00} - {2:00}.{3:00} - {4:00}.{5:00} - {6:00}.{7:00}", 
                                                ts_read_description.Seconds, ts_read_description.Milliseconds, 
                                                ts_get_hash.Seconds, ts_get_hash.Milliseconds, 
                                                ts_add_book_to_zip.Seconds, ts_add_book_to_zip.Milliseconds, 
                                                ts_add_book_to_db.Seconds, ts_add_book_to_db.Milliseconds));

                Console.WriteLine($"-- stop {fb2fullfilename}");
                Console.WriteLine();


                return retStaus;  
            }
            catch (FB2BaseException)
            {
                return new RetStatus(eRetError.ErrReadDesc, -1);
            }
            catch (FB2MD5HashException)
            {
                return new RetStatus(eRetError.ErrMD5, -1);
            }
            catch (FB2ZipException)
            {
                return new RetStatus(eRetError.ErrAddToArc, -1);
            }
            catch (FB2DBException)
            {
                try {
                    ZipBLL.DeleteFile(shortarcfilename, hash + ".fb2");
                    return new RetStatus(eRetError.ErrAddToDB, -1);
                }
                catch (FB2ZipException)
                {
                    return new RetStatus(eRetError.ErrAddToArc, -1);
                }
            }
        }

        public List<Tuple<int, string>> GetLanguages()
        {
            return dbManager.GetLanguages();
        }

        public List<DAL.GenreRow> GetGenresInRoot()
        {
            return dbManager.GetGenresInRoot();
        }

        public List<DAL.GenreRow> GetGenresByRootId(int rootId)
        {
            return dbManager.GetGenresByRootId(rootId);
        }

        public List<DAL.GenreRow> GetGenresByName(String genre)
        {
            return dbManager.GetGenresByName(genre);
        }

        public List<DAL.GenreRow> GetGenresById(int id)
        {
            return dbManager.GetGenresById(id);
        }

        public List<DAL.AuthorRow> GetAuthorByGenreId(int id)
        {
            return dbManager.GetAuthorByGenreId(id);
        }

        public List<DAL.AuthorRow> GetAuthorByGenreId(int id, string lang)
        {
            return dbManager.GetAuthorByGenreId(id, lang);
        }

        public int GetAuthorCountByGenreId(int id, string lang)
        {
            return dbManager.GetAuthorCountByGenreId(id, lang);
        }

        public List<DAL.BookRow> GetBookByAuthorId(int id)
        {
            return dbManager.GetBookByAuthorId(id);
        }

        public List<DAL.BookRow> GetBookByAuthorId(int id, int genre_id, string lang)
        {
            return dbManager.GetBookByAuthorId(id, genre_id, lang);
        }

        public DAL.BookRow GetBookById(int id)
        {
            return dbManager.GetBookById(id);
        }

        public List<DAL.BookRow> GetAllBooks()
        {
            return dbManager.GetAllBooks();
        }

        public int GetBookCount()
        {
            return dbManager.GetBookCount();
        }
        public int GetAuthorCount()
        {
            return dbManager.GetAuthorCount();
        }

        public bool DeleteBookById(int id)
        {
            return dbManager.DeleteBookById(id);
        }
    }
}
