using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IAsyncRepository
    {
        Task<List<Question>> ListAllAsync(string orderBy, string direction);
        Task<Question> GetQuestionByIdAsync(int questionId);
        Task<Question> AddQuestionAsync(Question question);
    }
}
