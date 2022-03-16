using ContactList.Contracts;
using ContactList.Server.Features;
using ContactList.Server.Infrastructure;
using ContactList.Server.Model;
using FluentValidation;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddServerValidators();
builder.Services.AddTransient<GetContactsQueryHandler>();
builder.Services.AddTransient<AddContactCommandHandler>();
builder.Services.AddTransient<EditContactQueryHandler>();
builder.Services.AddTransient<EditContactCommandHandler>();
builder.Services.AddTransient<DeleteContactCommandHandler>();

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
    (GetContactsQueryHandler handler)
        => handler.Handle(new GetContactsQuery()));

app.MapPost("/api/contacts/add",
    async (AddContactCommand command, IValidator<AddContactCommand> validator, AddContactCommandHandler handler) =>
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                ));

        var response = handler.Handle(command);
        return Results.Ok(response);
    });

app.MapGet("/api/contacts/edit",
    (Guid id, EditContactQueryHandler handler)
        => handler.Handle(new EditContactQuery {Id = id}));

app.MapPost("/api/contacts/edit",
    async (EditContactCommand command, IValidator<EditContactCommand> validator, EditContactCommandHandler handler) =>
    {
        var validationResult = await validator.ValidateAsync(command);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                ));

        handler.Handle(command);
        return Results.Ok();
    });

app.MapPost("/api/contacts/delete",
    (DeleteContactCommand command, DeleteContactCommandHandler handler) =>
    {
        handler.Execute(command);
        return Results.Ok();
    });

app.Run();
