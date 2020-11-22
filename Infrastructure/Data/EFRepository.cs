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
        public async Task<List<Question>> ListAllAsync()
        {
            return await _dbContext.Questions.OrderByDescending(q => q.DateAdded).ToListAsync();
        }

        public async Task<Question> GetQuestionByIdAsync(int questionId)
        {
            var answers = await _dbContext.Answers.Where(a => a.QuestionId == questionId).OrderByDescending(a => a.DateAdded).ToListAsync();
            var question = await _dbContext.Questions.Where(q => q.Id == questionId).FirstOrDefaultAsync();
            question.Answers = answers;
            return question;
        }

        
    }
}
