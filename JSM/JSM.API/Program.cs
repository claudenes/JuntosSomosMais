using JSM.Application.Services;
using JSM.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddSingleton<UserTransformerService>();
builder.Services.AddSingleton<DataLoaderService>();
builder.Services.AddSingleton<UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddServices();

var app = builder.Build();


// Carregar dados na inicialização
var dataLoader = app.Services.GetRequiredService<DataLoaderService>();
var userService = app.Services.GetRequiredService<UserService>();

var users = await dataLoader.LoadUsersAsync();
userService.Initialize(users);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
