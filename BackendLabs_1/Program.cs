using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{    
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.MapGet("/healthcheck", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        time = DateTime.Now.ToString(" yyyy MMMM dd HH:mm:ss K", CultureInfo.InvariantCulture)
    });

});

app.Run();
