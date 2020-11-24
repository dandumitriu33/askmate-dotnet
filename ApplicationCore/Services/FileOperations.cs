using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Services
{
    public class FileOperations : IFileOperations
    {
        public bool ValidateImageType(string fileName)
        {
            if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png"))
            {
                return true;
            }
            return false;
        }

        public string AssembleQuestionUploadedFileName(string questionTitle, string systemFileName)
        {
            return "QTitle_"
                   + questionTitle + "_"
                   + DateTime.Now.Year.ToString() + "_"
                   + DateTime.Now.Month.ToString() + "_"
                   + DateTime.Now.Day.ToString() + "_"
                   + DateTime.Now.Hour.ToString() + "_"
                   + DateTime.Now.Minute.ToString() + "_"
                   + DateTime.Now.Second.ToString() + "_"
                   + Guid.NewGuid().ToString() + "_" + systemFileName;
        }

        public string AssembleAnswerUploadedFileName(int questionId, string systemFileName)
        {
            return "AQID_"
                   + questionId + "_"
                   + DateTime.Now.Year.ToString() + "_"
                   + DateTime.Now.Month.ToString() + "_"
                   + DateTime.Now.Day.ToString() + "_"
                   + DateTime.Now.Hour.ToString() + "_"
                   + DateTime.Now.Minute.ToString() + "_"
                   + DateTime.Now.Second.ToString() + "_"
                   + Guid.NewGuid().ToString() + "_" + systemFileName;
        }
    }
}
