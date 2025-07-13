//-
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace SocialNetwork2.Tools;

public class ConnectionTools
{
    /// <summary>
    /// Получаем значение строки подключения к серверу базы данных
    ///  из приватной области репозитория
    /// </summary>
    public static string GetConnectionString()
    {
        // получаем значение строки подключения из файла
        string connectionString = string.Empty;
        string privateDataFolder = DirectoryTools.GetRootForFolderName("private-data");
        if (!string.IsNullOrWhiteSpace(privateDataFolder))
        {
            string connectionStringFilePath = Path.Combine(privateDataFolder, "private-data", "SQL_SERVER_CONNECTION_STRING");
            if (File.Exists(connectionStringFilePath))
            {
                connectionString = File.ReadLines(connectionStringFilePath).First();
            }
        }

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new Exception("connection string is not found");

        return connectionString;
    }


    /// <summary>
    /// Обновляем имеющуюся строку подключения к серверу базы данных
    ///  значениями из приватной области репозитория
    /// </summary>
    public static string GetConnectionString(string? connectionString)
    {
        string privateConnectionString = GetConnectionString();

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return privateConnectionString;
        }

        if (string.IsNullOrWhiteSpace(privateConnectionString))
        {
            return connectionString;
        }

        Dictionary<string, string> privateConnectionDict;
        privateConnectionDict = privateConnectionString.Split(";")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Split("="))
            .Where(x => x.Length > 1)
            .ToDictionary(x => x[0], x => x[1]);

        string connection = string.Empty;
        foreach (string keyValue in connectionString.Split(";"))
        {
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                continue;
            }

            string[] keyValuePair = keyValue.Split("=");

            string key = keyValuePair[0];
            string value = string.Empty;

            if (keyValuePair.Length > 1)
            {
                value = keyValuePair[1];

                // заменяем только те значения, которые отмечены треугольными скобками
                //  остальные значения принимаем из исходной строки подключения
                if (value.StartsWith('<') && value.EndsWith('>'))
                {
                    if (privateConnectionDict.ContainsKey(key))
                        value = privateConnectionDict[key];
                }

                value = "=" + value;
            }

            connection += key + value + ";";
        }

        return connection;
    }
}
