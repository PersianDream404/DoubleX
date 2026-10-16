using Framwork.Bus.Query;
using Framwork.Bus.Command;
using Framwork.PagedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;

namespace Modules.Personal.Presentation.Controllers;

public class PersonalController : Controller
{
    private readonly IQueryBus _queryBus;
    private readonly ICommandBus _commandBus;
    private readonly ILogger<PersonalController> _logger;

    public PersonalController(IQueryBus queryBus, ICommandBus commandBus, ILogger<PersonalController> logger)
    {
        _queryBus = queryBus;
        _commandBus = commandBus;
        _logger = logger;
    }

    public async Task<IActionResult> Index(GetAllPersonnelRequestDto request)
    {
        try
        {
            var query = new GetAllPersonnelQuery(request);
            var result = await _queryBus.Send<GetAllPersonnelQuery, PagedList<GetAllPersonalResponseDto>>(query);


            return View(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت لیست بنرها");
            return RedirectToAction("Error");
        }
    }

    [HttpGet()]
    public IActionResult Create()
    {
        var model = new CreatePersonnelRequestDto(
            new PersonalInfoRequestDto(),
            new JobInfoRequestDto(),
            new FinancialInfoRequestDto(),
            new HealthInfoRequestDto(),
            new GuarantorInfoRequestDto(),
            new List<DocumentRequestDto>
            {
                new(),
                new()
            }
        );

        return View(model);
    }

    //[HttpPost("create")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(CreatePersonalViewModel viewModel)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return View(viewModel);
    //    }

    //    try
    //    {
    //        var dto = new CreatePersonalRequestDto
    //        {
    //            Title = viewModel.Title,
    //            Description = viewModel.Description,
    //            ImageUrl = viewModel.ImageUrl,
    //            Url = viewModel.Link,
    //            Order = 0,
    //            StartDate = DateTime.Now,
    //            EndDate = null
    //        };

    //        var command = new CreatePersonalCommand(dto);
    //        var result = await _commandBus.Send<CreatePersonalCommand, bool>(command);

    //        if (result)
    //        {
    //            TempData["Success"] = "بنر با موفقیت ایجاد شد";
    //            return RedirectToAction("Index");
    //        }

    //        ModelState.AddModelError("", "خطا در ایجاد بنر");
    //        return View(viewModel);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "خطا در ایجاد بنر جدید");
    //        ModelState.AddModelError("", "خطای سرور رخ داده است");
    //        return View(viewModel);
    //    }
    //}

    //[HttpGet("edit/{id}")]
    //public async Task<IActionResult> Edit(int id)
    //{
    //    try
    //    {
    //        var query = new GetByIdPersonalQuery(id);
    //        var result = await _queryBus.Send<GetByIdPersonalQuery, GetByIdPersonalResponseDto>(query);

    //        if (result == null)
    //        {
    //            return NotFound();
    //        }

    //        var viewModel = new EditPersonalViewModel
    //        {
    //            Id = (int)result.Value.Id,
    //            Title = result.Value.Title,
    //            Description = result.Value.Description,
    //            ImageUrl = result.Value.ImageUrl,
    //            Link = result.Value.Url,
    //            IsActive = result.Value.Status
    //        };

    //        return View(viewModel);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "خطا در دریافت بنر برای ویرایش: {PersonalId}", id);
    //        return RedirectToAction("Index");
    //    }
    //}

    //[HttpPost("edit/{id}")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int id, EditPersonalViewModel viewModel)
    //{
    //    if (id != viewModel.Id)
    //    {
    //        return BadRequest();
    //    }

    //    if (!ModelState.IsValid)
    //    {
    //        return View(viewModel);
    //    }

    //    try
    //    {
    //        var dto = new UpdatePersonalRequestDto
    //        {
    //            Id = viewModel.Id,
    //            Title = viewModel.Title,
    //            Description = viewModel.Description,
    //            ImageUrl = viewModel.ImageUrl,
    //            Url = viewModel.Link
    //        };

    //        var command = new UpdatePersonalCommand(dto);
    //        var result = await _commandBus.Send<UpdatePersonalCommand, bool>(command);

    //        if (result)
    //        {
    //            TempData["Success"] = "بنر با موفقیت به‌روزرسانی شد";
    //            return RedirectToAction("Index");
    //        }

    //        ModelState.AddModelError("", "خطا در به‌روزرسانی بنر");
    //        return View(viewModel);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "خطا در ویرایش بنر: {PersonalId}", id);
    //        ModelState.AddModelError("", "خطای سرور رخ داده است");
    //        return View(viewModel);
    //    }
    //}

    //[HttpPost("delete/{id}")]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Delete(int id)
    //{
    //    try
    //    {
    //        var command = new DeletePersonalCommand(id);
    //        var result = await _commandBus.Send<DeletePersonalCommand, bool>(command);

    //        if (result)
    //        {
    //            TempData["Success"] = "بنر با موفقیت حذف شد";
    //            return RedirectToAction("Index");
    //        }

    //        TempData["Error"] = "خطا در حذف بنر";
    //        return BadRequest();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "خطا در حذف بنر: {PersonalId}", id);
    //        TempData["Error"] = "خطای سرور رخ داده است";
    //        return BadRequest();
    //    }
    //}
}