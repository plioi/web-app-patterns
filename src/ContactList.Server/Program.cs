using ContactList.Server.Infrastructure;
using ContactList.Server.Model;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllersWithViews(options => options.Filters.Add<UnitOfWork>())
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Program>());

builder.Services.AddDbContext<Database>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
