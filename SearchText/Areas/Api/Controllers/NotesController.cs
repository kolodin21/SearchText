using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Notes.Areas.Api.Contracts;
using Notes.DataAccess;
using SearchText.Models;

namespace Notes.Areas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController(NotesDbContext dbContext) : ControllerBase
    {
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddNoteRequest request)
        {
            var notes = new Note(request.Title);

            await dbContext.Notes.AddAsync(notes);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get([FromQuery] GetNoteRequest request)
        {
            if (string.IsNullOrEmpty(request.Search))
                return Ok(Array.Empty<string>());

            //Этот метод предпочтительнее, но SQlite не поддерживает поиск с учетом регистра 
            //var results = await dbContext.Notes
            //    .Where(p => EF.Functions.Like(p.Title.ToLower(), $"{request.Search.ToLower()}%")) // Поиск по первым буквам
            //    .Select(p => p.Title) // Выбираем поле Title
            //    .ToListAsync(); // Получаем данные из базы данных

            var results = await dbContext.Notes
                .Select(p => p.Title) // Сначала получаем все данные
                .ToListAsync();

            // Затем применяем фильтрацию по поисковому запросу на стороне клиента
            var filteredResults = results
                .Where(title => title.ToLower().StartsWith(request.Search.ToLower())) // Поиск по первым буквам
                .ToList();

            return Ok(filteredResults);
        }
    }
}
