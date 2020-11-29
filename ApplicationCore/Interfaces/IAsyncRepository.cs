using ApplicationCore.Entities;
using Microsoft.AspNetCore.Identity;
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
        Task RemoveQuestionImageByQuestionId(int questionId);
        Task VoteUpQuestionById(int questionId);
        Task VoteDownQuestionById(int questionId);
        Task<Answer> AddAnswerAsync(Answer answer);
        Task<Answer> GetAnswerByIdWithoutDetailsAsync(int answerId);
        Task EditAnswerAsync(Answer answer);
        Task RemoveAnswerById(int answerId);
        Task RemoveAnswerImageByAnswerId(int answerId);
        Task VoteUpAnswerById(int answerId);
        Task VoteDownAnswerById(int answerId);
        Task<List<Question>> GetSearchResults(string searchPhrase);
        Task<QuestionComment> AddQuestionCommentAsync(QuestionComment comment);
        Task<AnswerComment> AddAnswerCommentAsync(AnswerComment answerComment);
        Task<AnswerComment> GetAnswerCommentById(int answerCommentId);
        Task EditAnswerCommentAsync(AnswerComment answerComment);
        Task<QuestionComment> GetQuestionCommentById(int questionCommentId);
        Task EditQuestionCommentAsync(QuestionComment questionComment);
        Task RemoveAnswerCommentById(int answerCommentId);
        Task RemoveQuestionCommentById(int questionCommentId);
        Task<List<Tag>> GetAllTags();
        Task<List<Tag>> GetAllTagsNoDuplicates(int questionId);
        Task AddQuestionTagAsync(QuestionTag questionTag);
        Task<Tag> AddTagAsync(Tag tag);
        Task<List<int>> GetTagIdsForQuestionId(int questionId);
        Task<List<Tag>> GetTagsFromListFromDb(List<int> tagIds);
        Task DetachTag(QuestionTag questionTag);
        Task<Dictionary<int, int>> GetTagInfo();
        Task<List<ApplicationUser>> GetAllUsers();
        Task<List<Question>> GetUserQuestions(string userId);
        Task<List<Answer>> GetUserAnswers(string userId);
        Task<List<QuestionComment>> GetUserQuestionComments(string userId);
        Task<List<AnswerComment>> GetUserAnswerComments(string userId);
        Task EditAnswerAccepted(int answerId);
        Task ModifyUserReputation(int value, string userId);
        Task<List<ApplicationClaim>> GetAllUserClaims();
        Task<ApplicationClaim> GetApplicationClaimById(int applicationClaimId);
    }
}
