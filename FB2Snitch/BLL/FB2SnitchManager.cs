using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{

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
        /// <summary>
        /// Добавлеяет книгу в Zip-архив и в DB
        /// param name="fb2fullfilename" - полный путь к FB2 файлу
        /// returns id-книги или -1 в случае если книгу уже была добавлена ранее
        /// </summary>
        public int AddBook(String fb2fullfilename)
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
                if (bookid > -1) return bookid;
                //4. Добавили его в архив (нашли архив в который его добавлять, сгенерировали уникальное имя, заархивировали)
                shortarcfilename = ZipBLL.AddFile(fb2fullfilename, hash + ".fb2");
                //5. Добавили его в DB 
                return dbManager.AddBook(fb2desc, shortarcfilename, hash);
            }
            catch (FB2BaseException ex)
            {
                throw new FB2BLLException(ex.Message);
            }
            catch (FB2MD5HashException ex)
            {
                throw new FB2BLLException(ex.Message);
            }
            catch (FB2ZipException ex)
            {
                throw new FB2BLLException(ex.Message);
            }
            catch (FB2DBException ex)
            {
                try {
                    ZipBLL.DeleteFile(shortarcfilename, hash + ".fb2");
                    throw;
                }
                catch (FB2ZipException e)
                {
                    throw new FB2BLLException(ex.Message + "\n" + e.Message);
                }                
            }
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

        public List<DAL.BookRow> GetBookByAuthorId(int id)
        {
            return dbManager.GetBookByAuthorId(id);
        }

    }
}
