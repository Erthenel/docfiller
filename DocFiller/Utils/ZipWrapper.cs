using DocFiller.Models;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DocFiller.Utils
{
    class ZipWrapper
    {
        public const string ProjectInfoFileName = "project_info.bin";
        public const string DictWordInfoFileName = "dictword_info.bin";
        public const string ProgramArhiveExtension = ".docfiller";

        public static ProjectModel ReadProjectModelFromArchive(string archiveFilePath)
        {
            using (MemoryStream tempStream = new MemoryStream())
            using (ZipFile zip = ZipFile.Read(archiveFilePath))
            {
                ZipEntry zipEntry = zip[ProjectInfoFileName];
                zipEntry.Extract(tempStream);
                tempStream.Seek(0, SeekOrigin.Begin);

                return (ProjectModel)BinaryConverter.ReadFromBinaryFile(tempStream);
            }
        }

        public static List<string> ExtractTemplateAndGetResultFilePaths(string archiveFilePath, string outputFilePath, string projectName)
        {
            List<string> result = new List<string>();

            using (ZipFile zip = ZipFile.Read(archiveFilePath))
            {
                var selection = from e in zip.Entries
                                where !Path.GetFileName(e.FileName).EndsWith(".bin")
                                select e;

                foreach (ZipEntry zipEntry in selection.ToList())
                {
                    zipEntry.FileName = projectName +"_" + DateTime.Now.ToString("HHmmssdd")+ "_"+ zipEntry.FileName;
                    zipEntry.Extract(outputFilePath);

                    result.Add(outputFilePath + "\\" + zipEntry.FileName);
                }

                return result;
            }
        }

        public static string CreateProjectFileArchiveAndGetResultFilePath(ProjectModel projectModel, string userProjectDirectory)
        {
            string projectFileName = userProjectDirectory + "\\" + ProjectInfoFileName;
            string projectArchiveName = userProjectDirectory + "\\" + projectModel.name + "_" + DateTime.Now.ToString("HHmmss_ddMMyyyy") + ProgramArhiveExtension;

            BinaryConverter.WriteToBinaryFile(projectFileName, projectModel);

            using (ZipFile zip = new ZipFile())
            {
                foreach (string templatePath in projectModel.templatePaths)
                {
                    zip.AddFile(templatePath, "");
                }

                zip.AddFile(projectFileName, "");
                zip.Save(projectArchiveName);
            }

            if (File.Exists(@projectFileName))
            {
                File.Delete(@projectFileName);
            }

            return projectArchiveName;
        }

        public static void UpdateDictWordInProjectFileArchive(string archiveFilePath, object dictword)
        {
            string dictWordInfoFilePath = archiveFilePath.Substring(0, archiveFilePath.LastIndexOf("\\") + 1) + DictWordInfoFileName;
            BinaryConverter.WriteToBinaryFile(dictWordInfoFilePath, dictword);

            using (ZipFile zip = ZipFile.Read(archiveFilePath))
            {
                if (zip.ContainsEntry(DictWordInfoFileName))
                {
                    zip.RemoveEntry(DictWordInfoFileName);
                    zip.Save();
                }

                zip.AddFile(dictWordInfoFilePath, "");
                zip.Save();
            }

            if (File.Exists(@dictWordInfoFilePath))
            {
                File.Delete(@dictWordInfoFilePath);
            }
        }

        public static object ReadDictWordFromArchive(string archiveFilePath)
        {
            using (MemoryStream tempStream = new MemoryStream())
            using (ZipFile zip = ZipFile.Read(archiveFilePath))
            {
                if (!zip.ContainsEntry(DictWordInfoFileName))
                {
                    return null;
                }

                ZipEntry zipEntry = zip[DictWordInfoFileName];
                zipEntry.Extract(tempStream);
                tempStream.Seek(0, SeekOrigin.Begin);

                return BinaryConverter.ReadFromBinaryFile(tempStream);
            }
        }
    }
}
