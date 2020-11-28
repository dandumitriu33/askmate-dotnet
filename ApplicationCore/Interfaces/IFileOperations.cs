using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Interfaces
{
    public interface IFileOperations
    {
        bool ValidateImageType(string fileName);
        string AssembleQuestionUploadedFileName(string userId, string systemFileName);
        string AssembleAnswerUploadedFileName(string userId, string systemFileName);
    }
}
