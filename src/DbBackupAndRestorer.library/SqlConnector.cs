using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace DbBackupAndRestorer.library
{
    public class SqlConnector
    {
        private readonly Server _server;

        public SqlConnector(string host, string user, string password )
        {
            var serverConnection = new ServerConnection(host, user, password);
            _server = new Server(serverConnection);
        }

        public List<DatabaseProperty> DatabaseList()
        {
            var databaseList = new List<DatabaseProperty>();
            var databases = _server.Databases;
            foreach (Database database in databases)
            {
                databaseList.Add(new DatabaseProperty{Name = database.Name});
            }

            return databaseList;
        }

        
        public void DbBackup(string databaseName, string destinationPath)
        {
            try
            {
                var rand = new Random();
                var backupFileFormat = rand.Next(0, 1000000000).ToString();
                ValidateParams(databaseName);
                Database database = _server.Databases[databaseName];
                if (database is null)  throw new Exception("Database does not exist");
                Backup backup = new Backup
                {
                    Action = BackupActionType.Database,
                    BackupSetDescription = databaseName + "-full backup",
                    BackupSetName = databaseName + "backup",
                    Database = databaseName
                }; 
                var dbname = $"{backupFileFormat}_{databaseName}"+".bak"; 
                    if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);
                    _server.BackupDirectory = destinationPath;
                    var backupFileName = Path.Combine(destinationPath, dbname);
                BackupDeviceItem deviceItem = new BackupDeviceItem(backupFileName, DeviceType.File);  
                backup.Devices.Add(deviceItem);
                backup.Initialize = true;
                backup.Checksum = true;
                backup.ContinueAfterError = true;
                backup.Incremental = false;

                backup.LogTruncation = BackupTruncateLogType.Truncate; 
                backup.SqlBackup(_server);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            } 
            
        }

        
        public void RestoreDatabase(string backupFilePath, 
            string destinationDatabaseName)
        {
            try
            {
                var restoreObj = new Restore
                {
                    Action = RestoreActionType.Database,
                    Database = destinationDatabaseName,
                    NoRecovery = true
                };
                Database currentDb = _server.Databases[destinationDatabaseName];
                if(currentDb != null)
                    _server.KillAllProcesses(destinationDatabaseName);
                restoreObj.ReplaceDatabase = true;  
                restoreObj.Devices.AddDevice(backupFilePath, DeviceType.File);
                restoreObj.SqlRestore(_server);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }

        public void ValidateParams(string databaseName)
        {
            if (string.IsNullOrEmpty(databaseName)) throw new Exception("Database name is required");
        }

        public void Dispose()
        {
            Dispose();
        }

    }
}
