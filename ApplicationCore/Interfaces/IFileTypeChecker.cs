using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Interfaces
{
    public interface IFileTypeChecker
    {
        bool ValidateImageType(string fileName);
    }
}
