using ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Services
{
    public class FileTypeChecker : IFileTypeChecker
    {
        public bool ValidateImageType(string fileName)
        {
            if (fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") || fileName.EndsWith(".png"))
            {
                return true;
            }
            return false;
        }
    }
}
