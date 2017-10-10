using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    class FileUtils
    {
        #region [GenerateUniqueFileName] Метод генерирует уникальное имя файла с расширением fileextention
        public static string GenerateUniqueFileName(string fileextention)
        {
            string fileName = String.Format("{0}_{1}.{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), fileextention);
            return (fileName);
        }
        #endregion

        #region [GetShotFileName] Получает короткое имя файла из полного имени файла
        public static string GetShotFileName(string filefullname)
        {
            return (filefullname.Substring(filefullname.LastIndexOf("\\") + 1));
        }
        #endregion

        #region [GetFileList] Получаем список файлов по заданному пути
        public static string[] GetFileList(string path)
        {
            return (Directory.GetFiles(path, "*.fb2"));
        }
        #endregion

        #region [isFileExist] Проверяем что указанный файл существует
        public static bool isFileExist(string path)
        {
            return (File.Exists(path));
        }
        #endregion

        #region [isFolderExists] Проверяем что указанная директория существует
        public static bool isFolderExists(string dir)
        {
            return (Directory.Exists(dir));
        }
        #endregion
    }
}
