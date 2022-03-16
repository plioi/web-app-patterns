using ContactList.Contracts;
using ContactList.Server.Infrastructure;
using ContactList.Server.Model;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddServerValidators();

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
app.UseMiddleware<UnitOfWork>();

app.MapRazorPages();
app.MapFallbackToFile("index.html");

app.MapGet("/api/contacts",
    async (IMediator mediator)
        => await mediator.Send(new GetContactsQuery()));

app.MapPost("/api/contacts/add",
    async (AddContactCommand command, IValidator<AddContactCommand> validator, IMediator mediator) =>
    {
        var validationResult = validator.Validate(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                ));

        var response = await mediator.Send(command);
        return Results.Ok(response);
    });

app.MapGet("/api/contacts/edit",
    async (Guid id, IMediator mediator)
        => await mediator.Send(new EditContactQuery {Id = id}));

app.MapPost("/api/contacts/edit",
    async (EditContactCommand command, IValidator<EditContactCommand> validator, IMediator mediator) =>
    {
        var validationResult = validator.Validate(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                ));

        var response = await mediator.Send(command);
        return Results.Ok(response);
    });

app.MapPost("/api/contacts/delete",
    async (DeleteContactCommand command, IMediator mediator) =>
    {
        var response = await mediator.Send(command);
        return Results.Ok(response);
    });

app.Run();
