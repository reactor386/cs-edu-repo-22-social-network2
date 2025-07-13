//-
using System;
using System.IO;
using System.Reflection;


namespace SocialNetwork2.Tools;

public class DirectoryTools
{
    /// <summary>
    /// Получаем путь до каталога, в который вложен указанный каталог,
    ///  выполняя поиск от текущего исполняемого файла
    /// </summary>
    public static string GetRootForFolderName(string folderName)
    {
        string rootLocation = Path.GetDirectoryName(
            Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        while (!string.IsNullOrWhiteSpace(rootLocation))
        {
            if (Directory.GetDirectories(rootLocation, folderName,
                SearchOption.TopDirectoryOnly).Length > 0)
            {
                break;
            }
            rootLocation = Path.GetDirectoryName(rootLocation) ?? string.Empty;
        }

        return rootLocation;
    }
}
