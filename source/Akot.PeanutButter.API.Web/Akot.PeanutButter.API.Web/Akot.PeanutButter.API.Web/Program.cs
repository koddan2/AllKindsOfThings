using Akot.PeanutButter.API.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Insert(0, new AvroInputFormatter());
    options.OutputFormatters.Insert(0, new AvroOutputFormatter(codec: SolTechnology.Avro.CodecType.Null));
    ////options.OutputFormatters.Insert(0, new AvroWithSchemaOutputFormatter(codec: SolTechnology.Avro.CodecType.Null));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(a =>
{
    a.AllowAnyHeader();
    a.AllowAnyMethod();
    a.AllowAnyOrigin();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
