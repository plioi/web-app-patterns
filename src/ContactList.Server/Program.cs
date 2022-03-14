using ContactList.Contracts;
using ContactList.Server.Infrastructure;
using ContactList.Server.Model;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews(options =>
    {
        options.Filters.Add<UnitOfWork>();
    })
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());
builder.Services.AddRazorPages();

builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.MapGet("/api/contacts",
    async (IMediator mediator)
        => await mediator.Send(new GetContactsQuery()));

app.MapPost("/api/contacts/add",
    async (AddContactCommand command, Database database, IValidator<AddContactCommand> validator, IMediator mediator) =>
    {
        try
        {
            await database.BeginTransactionAsync();

            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    ));

            var response = await mediator.Send(command);
            var result = Results.Ok(response);

            await database.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    });

app.MapGet("/api/contacts/edit",
    async (Guid id, IMediator mediator)
        => await mediator.Send(new EditContactQuery {Id = id}));

app.MapPost("/api/contacts/edit",
    async (EditContactCommand command, Database database, IValidator<EditContactCommand> validator, IMediator mediator) =>
    {
        try
        {
            await database.BeginTransactionAsync();

            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    ));

            var response = await mediator.Send(command);
            var result = Results.Ok(response);
            await database.CommitTransactionAsync();
            return result;
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    });

app.MapPost("/api/contacts/delete",
    async (DeleteContactCommand command, Database database, IMediator mediator) =>
    {
        try
        {
            await database.BeginTransactionAsync();

            var response = await mediator.Send(command);
            var result = Results.Ok(response);
            await database.CommitTransactionAsync();

            return result;
        }
        catch (Exception)
        {
            await database.RollbackTransactionAsync();
            throw;
        }
    });

app.Run();
