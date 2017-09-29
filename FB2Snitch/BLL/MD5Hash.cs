using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    class MD5Hash
    {
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region [ConverterHashToString] Переводит массив byte в строку hash функции (в шестнадцетиричный вид, заглавные буквы)
        private static String ConverterHashToString(byte[] hashValue)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
                sBuilder.Append(hashValue[i].ToString("X2"));
            return sBuilder.ToString().Replace("-","");
        }
        #endregion
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region [GetMD5HashFromFile] Получить значение Хеша из файла
        public static string GetFileHash(string FileName)
        {
            try
            {
                string strHash = "";
                using (System.IO.FileStream fileStream = System.IO.File.Open(FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    fileStream.Position = 0;
                    using (System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create())
                    {
                        byte[] hashValue = md5Hash.ComputeHash(fileStream);
                        strHash = ConverterHashToString(hashValue);
                    }
                    fileStream.Close();
                }
                return strHash;
            }
            catch
            {
                throw new FB2MD5HashException("Ошибка при расчете MD5 хэш-суммы");
            }
        }
        #endregion
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
