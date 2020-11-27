using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Interfaces
{
    public interface IFileOperations
    {
        bool ValidateImageType(string fileName);
        string AssembleQuestionUploadedFileName(int questionId, string systemFileName);
        string AssembleAnswerUploadedFileName(int questionId, string systemFileName);
    }
}
