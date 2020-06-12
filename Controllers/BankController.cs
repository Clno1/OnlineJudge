using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Semicolon.Auth.Models;
using Semicolon.OnlineJudge.Data;
using Semicolon.OnlineJudge.Models.Problemset;
using Semicolon.OnlineJudge.Models.ViewModels.Problemset;

namespace Semicolon.OnlineJudge.Controllers
{
    public class BankController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<SemicolonUser> _userManager;

        public BankController(ApplicationDbContext context, UserManager<SemicolonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexModel
            {
                ProblemModels = new List<ProblemModel>()
            };
            foreach (var problem in _context.Problems.ToList())
            {
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();

                var html = Markdown.ToHtml(problem.Description, pipeline);
                var raw = Markdown.ToPlainText(problem.Description);

                var author = await _userManager.FindByIdAsync(problem.AuthorId);

                model.ProblemModels.Add(new ProblemModel
                {
                    Id = problem.Id,
                    Title = problem.Title,
                    Description = problem.Description,
                    ContentRaw = raw,
                    ContentHtml = html,
                    AuthorId = problem.AuthorId,
                    Author = author.UserName,
                    ExampleData = problem.ExampleData,
                    JudgeProfile = problem.JudgeProfile,
                    PassRate = problem.PassRate,
                    PublishTime = problem.PublishTime
                });
            }

            return View(model);
        }
    }
}