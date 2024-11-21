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

            try
            {
                var lowerSearch = request.Search.ToLower();
                IEnumerable<string> query = await dbContext.Notes
                    .Select(p => p.Title)
                    .ToListAsync(); // Получаем все заголовки (минимизация нагрузки на запрос)

                //Лучше этот вариант, но SQlite не поддерживает такой запрос
                //query = request.SortItem.ToLower() switch
                //{
                //    "startswith" => query.Where(title => EF.Functions.Like(title, $"{request.Search}%")),
                //    "substring" => query.Where(title => EF.Functions.Like(title, $"%{request.Search}%")),
                //    _ => throw new ArgumentException("Некорректный тип поиска. Укажите 'startswith' или 'substring'.")
                //};

                var filteredResults = request.SortItem.ToLower() switch
                {
                    "startswith" => query.Where(title => title.ToLower().StartsWith(lowerSearch)),
                    "substring" => query.Where(title => title.ToLower().Contains(lowerSearch)),
                    _ => throw new ArgumentException("Некорректный тип поиска. Укажите 'startswith' или 'substring'.")
                };

                return Ok(filteredResults.ToList());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Произошла ошибка сервера.");
            }
        }
    }
}
