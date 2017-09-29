using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    public enum ResponseStatus { Error = 0, Success };

    public struct Response {
        public ResponseStatus status;
        public String         description;

        public Response(ResponseStatus status, String description)
        {
            this.description = description;
            this.status = status;
        }
    }

    public struct AddBookResopse
    {

    }

    class FB2SnitchManager
    {
        private String getAvailableArchive()
        {

            return "";
        }

        public Response AddBook(String fb2filename)
        {
            string filefullname = fb2filename;
            string fileshortname = FileUtils.GetShotFileName(filefullname);
            string uniquifilename = FileUtils.GenerateUniqueFileName("fb2");
            string arcfilename = getAvailableArchive();
            string arcshortfilename = string.Empty;

            //1. Проверяем что это именно fb2 файл и сваливаем если это не так
            FB2Description fb2desc = FB2Manager.ReadDecription(fb2filename);
            if (fb2desc == null)
                return (new Response(ResponseStatus.Error, "Не верный формат файла FB2") { });

            //2. Подсчитали MD5-хешь сумму
            string filemd5hash = MD5Hash.GetFileHash(fb2filename);
            if (string.IsNullOrEmpty(filemd5hash))
                return (new Response(ResponseStatus.Error, "Не удалось подсчитать MD5hash для файла") { });

            //3. Проверяем в DB, что такой файл еще не добавлен
            if (!DAL.DBManager.FindBookBy5Hash(filemd5hash))
                return (new Response(ResponseStatus.Error, "Ошибка при вызове метода FindBookBy5Hash") { });

            //4. Добавили его в архив (нашли архив в который его добавлять, сгенерировали уникальное имя, заархивировали)
            arcshortfilename = ZipBLL.AddFile(fb2filename, uniquifilename);
            if (string.IsNullOrEmpty(arcshortfilename))
                return (new Response(ResponseStatus.Error, "Не удалось добавить файл в архив") { });

            //5. Добавили его в DB 
            if (!DAL.DBManager.AddBook(fb2desc))
            {
                //Удаляем файл из архива, если при добавлении информации в DB произошла ошибка
                ZipBLL.DeleteFile(arcshortfilename, uniquifilename);
                return (new Response(ResponseStatus.Error, "Не удалось добавить информацию о файле в DB") { });
            }

            return (new Response(ResponseStatus.Success, "Файл успешно добавлен") { });
        }
    }
}
