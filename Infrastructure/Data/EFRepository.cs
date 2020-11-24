using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            var question = new Question();
            try
            {
                var answers = await _dbContext.Answers.Where(a => a.QuestionId == questionId && a.IsRemoved == false).OrderByDescending(a => a.DateAdded).ToListAsync();
                question = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();
                // increment view count for the extracted question
                question.Views += 1;

                _dbContext.Questions.Attach(question);
                _dbContext.Entry(question).Property(q => q.Views).IsModified = true;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                if (answers != null && answers.Count != 0)
                {
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

        public async Task VoteUpQuestionById(int questionId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                questionFromDb.Votes += 1;
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
                var questionFromDb = await _dbContext.Questions.Where(q => q.Id == questionId && q.IsRemoved == false).FirstOrDefaultAsync();

                questionFromDb.Votes -= 1;
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

        public async Task VoteUpAnswerById(int answerId)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();

                answerFromDb.Votes += 1;
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
                var answerFromDb = await _dbContext.Answers.Where(a => a.Id == answerId && a.IsRemoved == false).FirstOrDefaultAsync();

                answerFromDb.Votes -= 1;
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

    }
}
