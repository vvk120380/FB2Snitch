using System;
using System.Collections.Generic;
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
            string shortarcfilename = string.Empty;
            string hash = string.Empty;

            try
            {
                //1. Проверяем что это именно fb2 файл и сваливаем если это не так
                FB2Description fb2desc = FB2Manager.ReadDecription(fb2fullfilename);
                //2. Подсчитали MD5-хешь сумму
                hash = MD5Hash.GetFileHash(fb2fullfilename);
                //3. Проверяем в DB, что такой файл еще не добавлен
                int bookid = dbManager.IsBookHasBeenAlreadyAddedInDB(hash);
                if (bookid > -1) return new RetStatus(eRetError.ErrAlreadyAdd, bookid);
                //4. Добавили его в архив (нашли архив в который его добавлять, сгенерировали уникальное имя, заархивировали)
                shortarcfilename = ZipBLL.AddFile(fb2fullfilename, hash + ".fb2");
                //5. Добавили его в DB 
                return new RetStatus(eRetError.NoErr, dbManager.AddBook(fb2desc, shortarcfilename, hash));  
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

        public DAL.BookRow GetBookById(int id)
        {
            return dbManager.GetBookById(id);
        }

        public int GetBookCount()
        {
            return dbManager.GetBookCount();
        }
        public int GetAuthorCount()
        {
            return dbManager.GetAuthorCount();
        }
    }
}
