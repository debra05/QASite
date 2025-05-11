using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QASite.Data
{
    public class QARepository
    {
        private string _connectionString;
        public QARepository(string connectionString)
        {
            {
                _connectionString = connectionString;
            }
        }

        public List<Question> GetQuestions()
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Questions
           .Include(q => q.Answers)
           .Include(q => q.QuestionsTags)
            .ThenInclude(qt => qt.Tag)
        .OrderByDescending(q => q.DatePosted).ToList();
        }
        public Question GetQuestionById(int id)
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Questions
                .Include(q => q.User)
                .Include(q => q.Answers)
                .ThenInclude(i => i.User)
                .Include(q => q.QuestionsTags)
                    .ThenInclude(qt => qt.Tag)
                .FirstOrDefault(q => q.Id == id);

        }
        public void AddQuestion(Question question, List<string> tagNames)
        {
            using var ctx = new QADataContext(_connectionString);

            ctx.Questions.Add(question);
            ctx.SaveChanges();

            foreach (var tagName in tagNames)
            {
                var tag = ctx.Tags.FirstOrDefault(t => t.Name == tagName);
                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    ctx.Tags.Add(tag);
                    ctx.SaveChanges();
                }

                ctx.QuestionsTags.Add(new QuestionsTags
                {
                    QuestionId = question.Id,
                    TagId = tag.Id
                });
            }

            ctx.SaveChanges();
        }
        public void AddAnswer(Answer answer)
        {
            using var ctx = new QADataContext(_connectionString);
            ctx.Answers.Add(answer);
            ctx.SaveChanges();
        }

    }
}
