using System;
using System.Diagnostics;
using System.Net.Mime;
using DbBackupAndRestorer.library;
using Microsoft.SqlServer.Management.Smo;

namespace DbBackupAndRestorer.Console
{
    class Program
    {
        
        static void Main(string[] args)
        {

            System.Console.WriteLine($"\t\t\t******Welcome to Db backup and restorer*******\n");
            System.Console.WriteLine($"\tMenu List \t\n");
            System.Console.WriteLine($"\t1: View Databases \t\t\t\t\t 2: Back Up Database \t\t\t\t\t 3: Restore Database\t\t\t\t 4: Exit Application");

            start:
            int menu = int.Parse(System.Console.ReadLine() ?? throw new InvalidOperationException());
            var sqlConnector = new DbBackUpAndRestorer(".", "sa", "password1");
            switch (menu)
            {

                case 1:
                    var databases = sqlConnector.DatabaseList();
                    System.Console.WriteLine($"Total Number Of Databases: {databases.Count}");
                    foreach (var database in databases)
                    {
                        System.Console.WriteLine(database.Name);
                    }
                    goto start;
                    break;
                case 2:
                    System.Console.WriteLine($"Enter  the database you want to backup from the list above");
                    var databaseToBackUp = System.Console.ReadLine();
                    sqlConnector.BackupDatabase(databaseToBackUp, @"C:\\BackUp");
                    goto start;
                    break;
                case 3:
                    System.Console.WriteLine($"Enter  the database you want to restore");
                    var databaseToRestore = System.Console.ReadLine();
                    sqlConnector.RestoreDatabase(@"C:\BackUp\"+databaseToRestore, databaseToRestore);
                    break;
                case 4:
                    return;
                    break;
                default:
                    throw new Exception($"Out of range of menu");
            }
       

           

            System.Console.ReadLine();

        }
    }
}
