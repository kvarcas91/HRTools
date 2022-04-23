using Domain.DataManager;
using Domain.Models;
using Domain.Repository;
using Domain.Storage;
using Domain.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Domain.IO
{
    public static class FileHelper
    {
        private const string NAME = nameof(FileHelper);

        public static string FileFullName(string path)
        {
            return Path.GetFullPath(path);
        }
        public static string VerifyCSV(string data)
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;

            if (data.Contains(","))
            {
                data = string.Format("\"{0}\"", data);
            }

            if (data.Contains(Environment.NewLine))
            {
                data = string.Format("\"{0}\"", data);
            }

            if (data.Equals("Default") || data.Equals("NoOutcome")) data = string.Empty;

            if (data.Equals(DateTime.MinValue.ToString(DataStorage.LongPreviewDateFormat)) || data.Equals(DateTime.MinValue.ToString(DataStorage.ShortPreviewDateFormat))) data = string.Empty;

            return data;
        }

        public static void RunProcess(string path)
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }

        public static Task<bool> DirectoryExistsAsync(string path)
        {
            return Task.Run(() =>
            {
                return Directory.Exists(path);
            });
        }

        public static Task<bool> FileExistsAsync(string path)
        {
            return Task.Run(() =>
            {
                return File.Exists(path);
            });
        }

        public static Task<bool> CheckDbHealthAsync(string path)
        {
            return Task.Run(() =>
            {
                var repository = new RepositoryHelper();
                if (File.Exists(path))
                {
                    return repository.CheckDbHealth();
                }

                return repository.CreateDatabase(path);
                
            });
        }

        public static bool CreateDirectoryIfNotExists(string path)
        {
            
            return Directory.CreateDirectory(path).Exists;
        }

        public static Task<bool> CreateDirectoryIfNotExistsAsync(string path)
        {    
            return Task.Run(() =>
            {
                var dbPath = Directory.GetParent(path).FullName;
                if (Directory.Exists(dbPath))
                    return true;
                else
                    return Directory.CreateDirectory(path).Exists;
            });
           
        }

        public static Task<bool> CopyFileAsync(string from, string to)
        {
            return Task.Run(() =>
            {
                try
                {
                    File.Copy(from, to);
                    return true;
                }
                catch (Exception e) 
                {
                    LoggerManager.Log($"{NAME}.CopyFileAsync", e.Message);
                    return false; 
                }
                
            });
        }

        private static long GetFileSize(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                return new FileInfo(FilePath).Length;
            }
            return 0;
        }

        public static Task<Response> CopyFileToMeetingIDAsync(string from, string meetingID)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (GetFileSize(from) / 1024 > 25) return new Response { Success = false, Message = "File is too big. Maximum file size - 25mb" };
                    var to = Environment.UserName == "eslut" ? $"{DataStorage.AppSettings.MeetingContentTestPath}{meetingID}" : $"{DataStorage.AppSettings.MeetingContentProductionPath}{meetingID}";
                    File.Copy(from, to);
                    return new Response { Success = true};
                }
                catch (Exception e)
                {
                    LoggerManager.Log($"{NAME}.CopyFileAsync", e.Message);
                    return new Response { Success = false, Message = e.Message };
                }

            });
        }

        public static Task<bool> CopyAndReplaceFileAsync(string from, string to)
        {
            return Task.Run(() =>
            {
                try
                {
                    var temp = $@"{Path.GetDirectoryName(to)}\db_temp.db";
                    if (File.Exists(temp)) File.Delete(temp);
                    File.Copy(from, temp);
                    if (File.Exists(to)) File.Delete(to);
                    File.Move(temp, to);
                    
                    return true;
                }
                catch (Exception e) 
                {
                    LoggerManager.Log($"{NAME}.CopyAndReplaceFileAsync", e.Message);
                    return false; 
                }

            });
        }

        public static DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public static Task<List<CaseFile>> GetFolderContentFromMeetingID(string id)
        {
            return Task.Run(() =>
            {
                var path = Environment.UserName == "eslut" ? $"{DataStorage.AppSettings.MeetingContentTestPath}\\{id}" : $"{DataStorage.AppSettings.MeetingContentProductionPath}\\{id}";
                var output = new List<CaseFile>();

                if (!Directory.Exists(path)) return output;
                string[] contentFiles = Directory.GetFiles(path);
                foreach (var item in contentFiles)
                {
                    output.Add(new CaseFile(item, Path.GetFileNameWithoutExtension(item), Path.GetExtension(item)));
                }

                return output;

            });
        }
    }
}
