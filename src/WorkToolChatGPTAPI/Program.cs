using Entities;
using ReprintTech.Universe.ChatApi.Extensions;
using Services;
using Services.IReplyServices;
using Services.ReplyServices;
using Utility.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Êý¾Ý¿â
builder.Services.AddStorage<SqlDbContext>(builder.Configuration.GetConnectionString("MySQL"))
    .AddServices(typeof(Program), typeof(BaseService));

builder.Configuration
    .AddIniFile("ChatIniConfig.ini", optional: true, reloadOnChange: true)
    .AddIniFile($"ChatIniConfig.{builder.Environment.EnvironmentName}.ini",
                optional: true, reloadOnChange: true);

builder.Services.AddTransient<Utility.Helper.ChatGPTTool>();
builder.Services.AddScoped<IMessageService, MessageService>();

builder.Services.Configure<ChatGPTSettings>(builder.Configuration.GetSection("ChatGPT"));

//»º´æ
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
