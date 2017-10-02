using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    public enum ResponseStatus { Error = 0, Success, FileAlreadyWasAdd };

    //public struct Response {
    //    public ResponseStatus status;
    //    public String         description;

    //    public Response(ResponseStatus status, String description)
    //    {
    //        this.description = description;
    //        this.status = status;
    //    }
    //}

    class FB2SnitchManager
    {
        DAL.DBManager dbManager = null;

        public FB2SnitchManager()
        {
            dbManager = new DAL.DBManager();
        }

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
    }
}
