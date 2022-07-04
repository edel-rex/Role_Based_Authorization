using Microsoft.EntityFrameworkCore;

using jwt_employee.Data;
using jwt_employee.Interface;
using jwt_employee.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using jwt_employee.Constants;
using jwt_employee.Middlewares;
using AutoSend_Email.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddTransient<IEmployees, EmployeeRepository>();
builder.Services.AddTransient<IUserInfos, UserInfoRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();

// Json Cycle Issue

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString(DbConstants.DbContext) ?? throw new InvalidOperationException($"Connection string {DbConstants.DbContext} not found.")));

builder.Services.AddSwaggerGen(
    options =>
    {
        options.AddSecurityDefinition(DbConstants.Bearer, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = DbConstants.Authorization,
            Type = SecuritySchemeType.Http,
            Scheme = DbConstants.Bearer,
            BearerFormat = DbConstants.JWT,
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        });
        options.OperationFilter<SecurityRequirementsOperationFilter>();
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = DbConstants.Bearer
                    }
                },
                new string[] {}
        }
    });
    }
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>

{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration[DbConstants.ValidAudience],
        ValidIssuer = builder.Configuration[DbConstants.ValidIssuer],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration[DbConstants.IssuerSigningKey]))
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.GetClaims();

app.MapControllers();

app.Run();
