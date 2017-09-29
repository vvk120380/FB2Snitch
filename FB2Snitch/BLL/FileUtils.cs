using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    class FileUtils
    {
        //Метод генерирует уникальное имя файла с расширением fileextention ---------------------------------------------------------------------------------------------
        public static string GenerateUniqueFileName(string fileextention)
        {
            string fileName = String.Format("{0}_{1}.{2}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Guid.NewGuid(), fileextention);
            return (fileName);
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

        //Получает короткое имя файла из полного имени файла ---------------------------------------------------------------------------------------------
        public static string GetShotFileName(string filefullname)
        {
            return (filefullname.Substring(filefullname.LastIndexOf("\\") + 1));
        }
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------

    }
}
