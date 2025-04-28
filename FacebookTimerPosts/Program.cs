using FacebookTimerPosts.AppDbContext;
using FacebookTimerPosts.Models;
using FacebookTimerPosts.Services;
using FacebookTimerPosts.Services.IRepository;
using FacebookTimerPosts.Services.IRepository.Base;
using FacebookTimerPosts.Services.Repository;
using FacebookTimerPosts.Services.Repository.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IFacebookPageRepository, FacebookPageRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICountdownTimerRepository, CountdownTimerRepository>();
builder.Services.AddScoped<IPaymentResultRepository, PaymentResultRepository>();
builder.Services.AddScoped<IFacebookService, FacebookService>();

// Register HttpClient
builder.Services.AddHttpClient();

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration.GetSection("AppSettings:Token").Value)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration.GetSection("AppSettings:Issuer").Value,
        ValidateAudience = true,
        ValidAudience = builder.Configuration.GetSection("AppSettings:Audience").Value,
        ValidateLifetime = true,
    };
});
// Add this after your existing authentication configuration
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        options.CallbackPath = "/signin-facebook";
    });

// Register HttpClient for Facebook API
builder.Services.AddHttpClient("FacebookGraph", client =>
{
    client.BaseAddress = new Uri("https://graph.facebook.com/v22.0/");
});
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register background services
builder.Services.AddHostedService<ScheduledTaskService>();
builder.Services.AddHostedService<PostRefreshService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Create database and apply migrations if not exists
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // Apply migrations
    dbContext.Database.Migrate();

    // Seed default admin user
    if (!userManager.Users.Any())
    {
        var admin = new User
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            FirstName = "Admin",
            LastName = "User",
            RegistrationDate = DateTime.UtcNow,
            IsActive = true
        };

        var result = await userManager.CreateAsync(admin, "Admin123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

app.Run();