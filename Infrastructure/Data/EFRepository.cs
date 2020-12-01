using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class EFRepository : IAsyncRepository
    {
        private readonly AskMateContext _dbContext;

        public EFRepository(AskMateContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Question>> ListAllAsync(string orderBy, string direction)
        {
            if (String.Equals("Descending", direction))
            {
                switch (orderBy)
                {
                    case "Title":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderByDescending(q => q.Title).ToListAsync();
                    case "Body":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderByDescending(q => q.Body).ToListAsync();
                    case "Votes":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderByDescending(q => q.Votes).ToListAsync();
                    case "Views":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderByDescending(q => q.Views).ToListAsync();
                    default:
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderByDescending(q => q.DateAdded).ToListAsync();
                }
            }
            else
            {
                switch (orderBy)
                {
                    case "Title":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderBy(q => q.Title).ToListAsync();
                    case "Body":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderBy(q => q.Body).ToListAsync();
                    case "Votes":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderBy(q => q.Votes).ToListAsync();
                    case "Views":
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderBy(q => q.Views).ToListAsync();
                    default:
                        return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderBy(q => q.DateAdded).ToListAsync();
                }
            }
        }

        public async Task<List<Question>> GetLatestQuestions(int numberOfQuestions)
        {
            return await _dbContext.Questions.Where(q => q.IsRemoved == false).OrderByDescending(q => q.DateAdded).Take(numberOfQuestions).ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var question = new Question();
            try
            {
                var questionComments = await _dbContext.QuestionComments
                                                .Where(c => c.QuestionId == questionId && c.IsRemoved == false)
                                                .OrderByDescending(c => c.DateAdded)
                                                .ToListAsync();

                var answers = await _dbContext.Answers
                                        .Where(a => a.QuestionId == questionId && a.IsRemoved == false)
                                        .OrderByDescending(a => a.IsAccepted)
                                        .ThenByDescending(a => a.Votes)
                                        .ToListAsync();

                var allAnswerCommentsOfQuestion = await _dbContext.AnswerComments
                                                            .Where(c => c.QuestionId == questionId && c.IsRemoved == false)
                                                            .ToListAsync();

                question = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                // increment view count for the extracted question
                question.Views += 1;
                _dbContext.Questions.Attach(question);
                _dbContext.Entry(question).Property(q => q.Views).IsModified = true;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // attach answers, questionComments and answerComments
                if (questionComments != null && questionComments.Count != 0)
                {
                    question.QuestionComments = questionComments;
                }

                if (answers != null && answers.Count != 0)
                {
                    foreach (var answer in answers)
                    {
                        if (answer.AnswerComments != null && answer.AnswerComments.Count != 0)
                        {
                            answer.AnswerComments = allAnswerCommentsOfQuestion
                                                        .Where(c => c.AnswerId == answer.Id && c.IsRemoved == false)
                                                        .OrderByDescending(c => c.DateAdded)
                                                        .ToList();
                        }
                    }
                    question.Answers = answers;
                }
                
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
            return question;
        }

        public async Task<Question> GetQuestionByIdWithoutDetailsAsync(int questionId)
        {
            return await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();
        }

        private Task<bool> TryUpdateModelAsync<T>(T question, string v, Func<object, object> p1, Func<object, object> p2, Func<object, object> p3)
        {
            throw new NotImplementedException();
        }

        public async Task<Question> AddQuestionAsync(Question question)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                question.DateAdded = DateTime.Now;
                await _dbContext.Questions.AddAsync(question);
                await _dbContext.SaveChangesAsync();

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
            return question;
        }

        public async Task EditQuestionAsync(Question question)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == question.Id && q.IsRemoved == false).FirstOrDefaultAsync();
                if (question.ImageNamePath == null && questionFromDb.ImageNamePath != null)
                {
                    question.ImageNamePath = questionFromDb.ImageNamePath;
                }
                questionFromDb.Title = question.Title;
                questionFromDb.Body = question.Body;
                questionFromDb.ImageNamePath = question.ImageNamePath;
                _dbContext.Questions.Attach(questionFromDb);
                _dbContext.Entry(questionFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task RemoveQuestionById(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                questionFromDb.IsRemoved = true;
                _dbContext.Questions.Attach(questionFromDb);
                _dbContext.Entry(questionFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task RemoveQuestionImageByQuestionId(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                questionFromDb.ImageNamePath = null;
                _dbContext.Questions.Attach(questionFromDb);
                _dbContext.Entry(questionFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task VoteUpQuestionById(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                int reputationModificationValue = 1;
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                questionFromDb.Votes += 1;
                await ModifyUserReputation(reputationModificationValue, questionFromDb.UserId);
                _dbContext.Questions.Attach(questionFromDb);
                _dbContext.Entry(questionFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task VoteDownQuestionById(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                int reputationModificationValue = -1;
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                questionFromDb.Votes -= 1;
                await ModifyUserReputation(reputationModificationValue, questionFromDb.UserId);
                _dbContext.Questions.Attach(questionFromDb);
                _dbContext.Entry(questionFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task<Answer> AddAnswerAsync(Answer answer)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                answer.DateAdded = DateTime.Now;
                await _dbContext.Answers.AddAsync(answer);
                await _dbContext.SaveChangesAsync();

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
            return answer;
        }

        public async Task<Answer> GetAnswerByIdWithoutDetailsAsync(int answerId)
        {
            return await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();
        }

        public async Task EditAnswerAsync(Answer answer)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answer.Id && a.IsRemoved == false).FirstOrDefaultAsync();
                if (answer.ImageNamePath == null && answerFromDb.ImageNamePath != null)
                {
                    answer.ImageNamePath = answerFromDb.ImageNamePath;
                }
                answerFromDb.Body = answer.Body;
                answerFromDb.ImageNamePath = answer.ImageNamePath;
                _dbContext.Answers.Attach(answerFromDb);
                _dbContext.Entry(answerFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task RemoveAnswerById(int answerId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();

                answerFromDb.IsRemoved = true;
                _dbContext.Answers.Attach(answerFromDb);
                _dbContext.Entry(answerFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task RemoveAnswerImageByAnswerId(int answerId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();

                answerFromDb.ImageNamePath = null;
                _dbContext.Answers.Attach(answerFromDb);
                _dbContext.Entry(answerFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task VoteUpAnswerById(int answerId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                int reputationModificationValue = 1;
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();

                answerFromDb.Votes += 1;
                await ModifyUserReputation(reputationModificationValue, answerFromDb.UserId);
                _dbContext.Answers.Attach(answerFromDb);
                _dbContext.Entry(answerFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task VoteDownAnswerById(int answerId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                int reputationModificationValue = -1;
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();

                answerFromDb.Votes -= 1;
                await ModifyUserReputation(reputationModificationValue, answerFromDb.UserId);
                _dbContext.Answers.Attach(answerFromDb);
                _dbContext.Entry(answerFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task<List<Question>> GetSearchResults(string searchPhrase)
        {
            HashSet<int> allQuestionIds = new HashSet<int>();
            // get questions with searchPhrase in title or body
            var titleAndBodyQuestions = _dbContext.Questions.Where(q => q.IsRemoved == false && (q.Title.Contains(searchPhrase) || q.Body.Contains(searchPhrase))).ToList();
            foreach (var question in titleAndBodyQuestions)
            {
                allQuestionIds.Add(question.Id);
            }
            // get answers with searchPhrase in body and grab question IDs
            var answersWithSearchPhrase = _dbContext.Answers.Where(a => a.IsRemoved == false && a.Body.Contains(searchPhrase)).ToList();
            foreach (var answer in answersWithSearchPhrase)
            {
                allQuestionIds.Add(answer.QuestionId);
            }
            // return all questions with the Id in the allQuestionIds set
            return await _dbContext.Questions.Where(q => q.IsRemoved == false && allQuestionIds.Contains(q.Id)).OrderByDescending(q => q.Votes).ToListAsync();
        }

        public async Task<QuestionComment> AddQuestionCommentAsync(QuestionComment questionComment)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                questionComment.DateAdded = DateTime.Now;
                await _dbContext.QuestionComments.AddAsync(questionComment);
                await _dbContext.SaveChangesAsync();
                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
            return questionComment;
        }

        public async Task<AnswerComment> AddAnswerCommentAsync(AnswerComment answerComment)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                answerComment.DateAdded = DateTime.Now;
                await _dbContext.AnswerComments.AddAsync(answerComment);
                await _dbContext.SaveChangesAsync();
                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
            return answerComment;
        }

        public async Task<AnswerComment> GetAnswerCommentById(int answerCommentId)
        {
            return await _dbContext.AnswerComments.Where(c => c.IsRemoved == false && c.Id == answerCommentId).FirstOrDefaultAsync();
        }

        public async Task<QuestionComment> GetQuestionCommentById(int questionCommentId)
        {
            return await _dbContext.QuestionComments.Where(c => c.IsRemoved == false && c.Id == questionCommentId).FirstOrDefaultAsync();
        }

        public async Task EditAnswerCommentAsync(AnswerComment answerComment)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answerCommentFromDb = await _dbContext.AnswerComments.Where(c => c.Id == answerComment.Id && c.IsRemoved == false).FirstOrDefaultAsync();

                answerCommentFromDb.Body = answerComment.Body;
                answerCommentFromDb.IsEdited = answerComment.IsEdited;
                answerCommentFromDb.DateAdded = answerComment.DateAdded;
                _dbContext.AnswerComments.Attach(answerCommentFromDb);
                _dbContext.Entry(answerCommentFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task EditQuestionCommentAsync(QuestionComment questionComment)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var questionCommentFromDb = await _dbContext.QuestionComments.Where(c => c.Id == questionComment.Id && c.IsRemoved == false).FirstOrDefaultAsync();

                questionCommentFromDb.Body = questionComment.Body;
                questionCommentFromDb.IsEdited = questionComment.IsEdited;
                questionCommentFromDb.DateAdded = questionComment.DateAdded;
                _dbContext.QuestionComments.Attach(questionCommentFromDb);
                _dbContext.Entry(questionCommentFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task RemoveAnswerCommentById(int answerCommentId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answerCommentFromDb = await _dbContext.AnswerComments.Where(c => c.Id == answerCommentId && c.IsRemoved == false).FirstOrDefaultAsync();

                answerCommentFromDb.IsRemoved = true;
                _dbContext.AnswerComments.Attach(answerCommentFromDb);
                _dbContext.Entry(answerCommentFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task RemoveQuestionCommentById(int questionCommentId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var questionCommentFromDb = await _dbContext.QuestionComments.Where(c => c.Id == questionCommentId && c.IsRemoved == false).FirstOrDefaultAsync();

                questionCommentFromDb.IsRemoved = true;
                _dbContext.QuestionComments.Attach(questionCommentFromDb);
                _dbContext.Entry(questionCommentFromDb).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task<List<Tag>> GetAllTags()
        {
            return await _dbContext.Tags.Where(t => t.IsRemoved == false).OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int tagId)
        {
            return await _dbContext.Tags.Where(t => t.Id == tagId).FirstOrDefaultAsync();
        }

        public async Task<List<Tag>> GetAllTagsNoDuplicates(int questionId)
        {
            List<int> currentQuestionTags = await _dbContext.QuestionTags.Where(qt => qt.QuestionId == questionId).Select(qt => qt.TagId).ToListAsync();
            return await _dbContext.Tags.Where(t => t.IsRemoved == false && currentQuestionTags.Contains(t.Id) == false).OrderBy(t => t.Name).ToListAsync();
        }

        public async Task AddQuestionTagAsync(QuestionTag questionTag)
        {
            await _dbContext.QuestionTags.AddAsync(questionTag);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Tag> AddTagAsync(Tag tag)
        {
            await _dbContext.Tags.AddAsync(tag);
            await _dbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<List<int>> GetTagIdsForQuestionId(int questionId)
        {
            return await _dbContext.QuestionTags.Where(qt => qt.QuestionId == questionId).Select(qt => qt.TagId).ToListAsync();
        }

        public async Task<List<Tag>> GetTagsFromListFromDb(List<int> tagIds)
        {
            return await _dbContext.Tags.Where(t => tagIds.Contains(t.Id) && t.IsRemoved == false).OrderBy(t => t.Name).ToListAsync();
        }

        public async Task DetachTag(QuestionTag questionTag)
        {
            var questionTagToDelete = await _dbContext.QuestionTags.Where(t => t.TagId == questionTag.TagId && t.QuestionId == questionTag.QuestionId).FirstOrDefaultAsync();
            _dbContext.QuestionTags.Remove(questionTagToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Dictionary<int, int>> GetTagInfo()
        {
            return await _dbContext.QuestionTags
                            .GroupBy(qt => qt.TagId)
                            .Select(g => new { TagId = g.Key, count = g.Count() })
                            .ToDictionaryAsync(k => k.TagId, i => i.count);
        }

        public async Task<List<ApplicationUser>> GetAllUsers()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<List<Question>> GetUserQuestions(string userId)
        {
            return await _dbContext.Questions.Where(q => q.IsRemoved == false && q.UserId == userId).OrderByDescending(q => q.DateAdded).ToListAsync();
        }

        public async Task<List<Answer>> GetUserAnswers(string userId)
        {
            return await _dbContext.Answers.Where(a => a.IsRemoved == false && a.UserId == userId).OrderByDescending(a => a.DateAdded).ToListAsync();
        }

        public async Task<List<QuestionComment>> GetUserQuestionComments(string userId)
        {
            return await _dbContext.QuestionComments.Where(qc => qc.IsRemoved == false && qc.UserId == userId).OrderByDescending(qc => qc.DateAdded).ToListAsync();
        }

        public async Task<List<AnswerComment>> GetUserAnswerComments(string userId)
        {
            return await _dbContext.AnswerComments.Where(ac => ac.IsRemoved == false && ac.UserId == userId).OrderByDescending(ac => ac.DateAdded).ToListAsync();
        }

        public async Task EditAnswerAccepted(int answerId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answer = await _dbContext.Answers.Where(a => a.IsRemoved == false && a.Id == answerId).FirstOrDefaultAsync();

                answer.IsAccepted = true;
                _dbContext.Answers.Attach(answer);
                _dbContext.Entry(answer).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // TODO: Handle failure - UX message
                transaction.Rollback();
            }
        }

        public async Task ModifyUserReputation(int value, string userId)
        {
            var user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            user.Reputation += value;
            _dbContext.Users.Attach(user);
            _dbContext.Entry(user).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ApplicationClaim>> GetAllUserClaims()
        {
            return await _dbContext.ApplicationClaims.ToListAsync();
        }

        public async Task<ApplicationClaim> GetApplicationClaimById(int applicationClaimId)
        {
            return await _dbContext.ApplicationClaims.Where(c => c.Id == applicationClaimId).FirstOrDefaultAsync();
        }
    }
}
