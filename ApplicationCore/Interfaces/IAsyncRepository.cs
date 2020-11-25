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
        Task<List<Question>> GetLatestQuestions(int numberOfQuestions);
        Task<Question> GetQuestionByIdAsync(int questionId);
        Task<Question> AddQuestionAsync(Question question);
        Task<Question> GetQuestionByIdWithoutDetailsAsync(int questionId);
        Task EditQuestionAsync(Question question);
        Task RemoveQuestionById(int questionId);
        Task VoteUpQuestionById(int questionId);
        Task VoteDownQuestionById(int questionId);
        Task<Answer> AddAnswerAsync(Answer answer);
        Task<Answer> GetAnswerByIdWithoutDetailsAsync(int answerId);
        Task EditAnswerAsync(Answer answer);
        Task RemoveAnswerById(int answerId);
        Task VoteUpAnswerById(int answerId);
        Task VoteDownAnswerById(int answerId);
        Task<List<Question>> GetSearchResults(string searchPhrase);
        Task<QuestionComment> AddQuestionCommentAsync(QuestionComment comment);
        Task<AnswerComment> AddAnswerCommentAsync(AnswerComment answerComment);
    }
}
