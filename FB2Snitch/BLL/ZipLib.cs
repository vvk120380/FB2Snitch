using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FB2Snitch.BLL
{
    class ZipLib
    {
        /// <summary>
        /// Функция создания ZIP архива
        /// </summary>
        /// <param name="arc_name">Имя архивного файла, включая путь</param>
        /// <param name="file_list">Список файлов для добавления</param>
        /// <param name="overright_flag">Флаг указывает создавать ли новый архив или добавлять файлы в существующий</param>
        /// <remarks>ВНИМАНИЕ!!! Обязательно указывать кодировку "cp866" для имен на кириллице</remarks>
        /// <returns></returns>
        /// 
        #region [CreateArchive] Функция создания ZIP архива
        private bool CreateArchive(string arc_name, string[] file_list, bool overright_flag = false)
        {
            ZipArchiveMode zip_mode = ZipArchiveMode.Create;
            int i_count = 0;
            // Проверить наличие файла архива и если он существует проверить значение флага overright_flag 
            // и в зависимости от него создать архив или добавить файлы в существующий
            try
            {
                if (File.Exists(arc_name))
                {
                    if (overright_flag)
                        File.Delete(arc_name);
                    else
                        zip_mode = ZipArchiveMode.Update;
                }
            }
            catch
            {
                throw;
            }

            //Создаем новый архив или открываем существующий
            //Проходим по списку файлов, получем имя файла без пути, и добавляем файл в архив
            //ВНИМАНИЕ!!! Обязательно указывать кодировку "cp866" для имен на кириллице
            try
            {
                using (ZipArchive archive = ZipFile.Open(arc_name, zip_mode, Encoding.GetEncoding("cp866")))
                {
                    foreach (string in_file in file_list)
                    {
                        i_count++;
                        FileInfo fi = new FileInfo(in_file);
                        archive.CreateEntryFromFile(in_file, fi.Name, CompressionLevel.Optimal);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return (true);
        }
        #endregion

        #region [GetItemCount] Возвращает количество файлов в архиве
        static public int GetItemCount(string arc_name)
        {
            if (!File.Exists(arc_name)) return (-1);
            try
            {
                using (ZipArchive archive = ZipFile.Open(arc_name, ZipArchiveMode.Update, Encoding.GetEncoding("cp866"))) return archive.Entries.Count;                
            }
            catch
            {
                return (-1);
            }
        }
        #endregion

        #region [CreateEmpty] Создает пустой архив
        static public bool CreateEmpty(string arc_name)
        {
            if (File.Exists(arc_name)) return (false);
            try
            {
                using (ZipArchive archive = ZipFile.Open(arc_name, ZipArchiveMode.Create, Encoding.GetEncoding("cp866"))) return (true); 
            }
            catch
            {
                return (false);
            }            
        }
        #endregion

        #region [AddFile] Добавляет 1 файл в архив
        static public bool AddFile(string arc_name, string filename, string entryname)
        {
            ZipArchiveMode zip_mode = ZipArchiveMode.Update;

            if (!File.Exists(arc_name))
                zip_mode = ZipArchiveMode.Create;

            try
            {
                using (ZipArchive archive = ZipFile.Open(arc_name, zip_mode, Encoding.GetEncoding("cp866")))
                {
                    if (string.IsNullOrEmpty(entryname))
                    {
                        FileInfo fi = new FileInfo(filename);
                        archive.CreateEntryFromFile(filename, fi.Name, CompressionLevel.Fastest);
                    }
                    else
                    {
                        archive.CreateEntryFromFile(filename, entryname, CompressionLevel.Fastest);
                    }

                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine("Exception! Func: [AddFile]\n Message:" + ex.Message);
#endif

                return (false);
            }

            return (true);
        }
        #endregion

        #region [AddFile] Добавляет 1 файл в архив
        static public bool AddFile(string arc_name, string filename)
        {
            return (AddFile(arc_name, filename, string.Empty));
        }
        #endregion

        #region [AddFiles] Добавляет несколько файл в архив
        static public bool AddFiles(string arc_name, string[] filelist)
        {
            ZipArchiveMode zip_mode = ZipArchiveMode.Update;

            if (!File.Exists(arc_name))
                zip_mode = ZipArchiveMode.Create;
            try
            {
                using (ZipArchive archive = ZipFile.Open(arc_name, zip_mode, Encoding.GetEncoding("cp866")))
                {
                    foreach (string fn in filelist)
                    {
                        FileInfo fi = new FileInfo(fn);
                        archive.CreateEntryFromFile(fn, fi.Name, CompressionLevel.Optimal);
                    }
                }
            }
            catch
            {
                return (false);
            }

            return (true);
        }
        #endregion

        #region [DeleteFile] Удаляет файл с заданным именем из архива
        static public bool DeleteFile(string arc_name, string filename)
        {
            ZipArchiveMode zip_mode = ZipArchiveMode.Update;

            if (!File.Exists(arc_name)) return false;
                
            try
            {
                using (ZipArchive archive = ZipFile.Open(arc_name, zip_mode, Encoding.GetEncoding("cp866")))
                {
                    FileInfo fi = new FileInfo(filename);

                    IEnumerable<ZipArchiveEntry> query = archive.Entries.Where(q => q.Name == filename); 
                    List<ZipArchiveEntry> list = query.ToList<ZipArchiveEntry>();
                    for (int i = 0; i < list.Count; i++) list[i].Delete();
                }
            }
            catch 
            {
                return (false);
            }

            return (true);
        }
        #endregion
    }
}
