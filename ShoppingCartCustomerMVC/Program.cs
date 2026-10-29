using Microsoft.AspNetCore.Authentication.Google;
using Hangfire;
using Hangfire.SqlServer;
using ShoppingCartCustomerMVC.Services.BackgroundService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCartCustomerMVC.Data;
using ShoppingCartCustomerMVC.Services.API_Service.AuthService;
using ShoppingCartCustomerMVC.Services.API_Service.CartService;
using ShoppingCartCustomerMVC.Services.API_Service.CategoryService;
using ShoppingCartCustomerMVC.Services.API_Service.CustomerService;
using ShoppingCartCustomerMVC.Services.API_Service.OrderService;
using ShoppingCartCustomerMVC.Services.API_Service.PaymentService;
using ShoppingCartCustomerMVC.Services.API_Service.ProductService;
using ShoppingCartCustomerMVC.Services.API_Service.SubCategoryService;
using ShoppingCartCustomerMVC.Services.API_Service.WishlistService;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHangfire(configuration =>
{
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
            builder.Configuration.GetConnectionString(
                "ShoppingCartConnection"));
});

builder.Services.AddHangfireServer();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwtToken"];

            return Task.CompletedTask;
        }
    };
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
});

builder.Services.AddScoped<DbConnectionFactory>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<SubCategoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddDbContext<ShoppingCartDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ShoppingCartConnection")));
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<HangfireEmailJob>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();