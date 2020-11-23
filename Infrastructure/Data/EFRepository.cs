﻿using ApplicationCore.Entities;
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
                        return await _dbContext.Questions.OrderByDescending(q => q.Title).ToListAsync();
                    case "Body":
                        return await _dbContext.Questions.OrderByDescending(q => q.Body).ToListAsync();
                    case "Votes":
                        return await _dbContext.Questions.OrderByDescending(q => q.Votes).ToListAsync();
                    case "Views":
                        return await _dbContext.Questions.OrderByDescending(q => q.Views).ToListAsync();
                    default:
                        return await _dbContext.Questions.OrderByDescending(q => q.DateAdded).ToListAsync();
                }
            }
            else
            {
                switch (orderBy)
                {
                    case "Title":
                        return await _dbContext.Questions.OrderBy(q => q.Title).ToListAsync();
                    case "Body":
                        return await _dbContext.Questions.OrderBy(q => q.Body).ToListAsync();
                    case "Votes":
                        return await _dbContext.Questions.OrderBy(q => q.Votes).ToListAsync();
                    case "Views":
                        return await _dbContext.Questions.OrderBy(q => q.Views).ToListAsync();
                    default:
                        return await _dbContext.Questions.OrderBy(q => q.DateAdded).ToListAsync();
                }
            }
            
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            var answers = await _dbContext.Answers.Where(a => a.QuestionId == questionId).OrderByDescending(a => a.DateAdded).ToListAsync();
            var question = await _dbContext.Questions.Where(q => q.Id == questionId).FirstOrDefaultAsync();
            if (answers != null && answers.Count != 0)
            {
                question.Answers = answers;
            }
            return question;
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


    }
}
