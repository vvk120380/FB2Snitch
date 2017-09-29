﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch.BLL
{
    class ZipBLL
    {

        #region [AddFb2ToZipArchive] Добавляет файл в архив и возвращает имя архива в который он добавлен или String.Empty в случае ошибки
        /// <summary>
        /// Добавляет файл в архив и возвращает имя архива в который он добавлен или String.Empty в случае ошибки
        /// </summary>
        /// <param name="filefullname">Полное имя добвляемого в архив файла</param>
        /// <param name="ziparchivefolder">Директория в которой размещены файлы архивов</param>
        /// <returns></returns>
        public static string AddFile(string filefullname, string uniquefilename)
        {
            String dirpath = Properties.Settings.Default.BaseArcDir;

            string arcshortname = string.Empty;
            string arcfullname = string.Empty;

            // Проверить - существует ли директория
            //!!!

            //Находим zip файл с самым большим номером
            int arcnum = FindLastArcNumToAdd(dirpath);
            if (arcnum != -1)
            {
                arcshortname = string.Format("{0:X8}.zip", arcnum);
                arcfullname = dirpath + "\\" + arcshortname;

                //Получаем сколько файлов уже есть в найденном zip архиве
                int arcitemcount = ZipLib.GetItemCount(arcfullname);

                // Возникла ошибка, сваливаем
                if (arcitemcount == -1) return (String.Empty);

                if (arcitemcount >= Properties.Settings.Default.MaxFileCountInArchive)
                {
                    arcshortname = string.Format("{0:X8}.zip", arcnum + 1);
                    arcfullname = dirpath + "\\" + arcshortname;
                }
            }
            else
            {
                arcshortname = "00000000.zip";
                arcfullname = dirpath + "\\" + arcshortname;
            }

            if (!ZipLib.AddFile(arcfullname, filefullname, uniquefilename)) return (String.Empty);

            return (arcshortname);
        }
        #endregion

        #region
        public static bool DeleteFile(String arcshortname, String uniquefilename)
        {
            String arcfullname = Properties.Settings.Default.BaseArcDir + "\\" + arcshortname;
            return ZipLib.DeleteFile(arcfullname, uniquefilename);
        }
        #endregion

        #region [FindLastArcNumToAdd] Просматриваем все архивы в заданной дирректори и возвращаем максимальный номер архива
        private static int FindLastArcNumToAdd(string arcpath)
        {
            int maxacrnum = -1;

            try
            {
                string[] files = System.IO.Directory.GetFiles(arcpath, "*.zip", System.IO.SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    //Получаем имя файла с расширение
                    string filename = (file.Substring(file.LastIndexOf("\\") + 1));
                    //отрезаем расширение
                    string shotfilename = filename.Substring(0, filename.LastIndexOf(".zip"));

                    int currarcnum = -1;
                    if(Int32.TryParse(shotfilename,System.Globalization.NumberStyles.HexNumber,new System.Globalization.CultureInfo("en-US"), out currarcnum))
                    {
                        if (currarcnum > maxacrnum)
                            maxacrnum = currarcnum;
                    }

                }
            }
            catch 
            {
                return (-1);
            }

            return (maxacrnum);
        }
        #endregion
        //---------------------------------------------------------------------------------------------------------------------------------------------------------------


    }
}