using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
