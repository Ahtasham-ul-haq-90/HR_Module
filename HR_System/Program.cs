using HR_System.Extension.Filters;
using HR_System.Extension.MiddleWare;
using infrastructure.DepandancyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
   //options =>  options.Filters.Add<ApiResponseFilter>()
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseMiddleware<ExceptionMiddleWare>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ApiResponseMiddleware>();
app.MapControllers();

app.Run();
