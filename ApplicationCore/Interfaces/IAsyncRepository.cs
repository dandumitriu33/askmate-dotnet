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
        Task<Question> GetQuestionByIdWithoutDetailsAsync(int questionId);
        Task EditQuestionAsync(Question question);
        Task RemoveQuestionById(int questionId);
        Task VoteUpQuestionById(int questionId);
        Task VoteDownQuestionById(int questionId);
        Task<Answer> AddAnswerAsync(Answer answer);
        Task RemoveAnswerById(int answerId);
        Task VoteUpAnswerById(int answerId);
        Task VoteDownAnswerById(int answerId);
    }
}
