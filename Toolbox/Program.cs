using Microsoft.AspNetCore.Mvc;
using Toolbox;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPut("signature/{id:long}", async Task<IResult> ([FromRoute] long id, [FromBody] SignInvoiceDto dto) =>
    {
        SignInvoice signInvoice = new SignInvoice();
        var result = await signInvoice.Sign(id, dto);
        if (result != String.Empty) return Results.BadRequest(result);
        return Results.Ok($"Invoice {id} signed successfully.");
    })
    .WithName("Invoice")
    .WithOpenApi();

app.Run();