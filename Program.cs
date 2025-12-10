using Microsoft.EntityFrameworkCore;
using PuckDrop.Configuration;
using PuckDrop.Data;
using PuckDrop.Components;
using PuckDrop.Services;
using PuckDrop.State;
using PuckDrop.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure SQLite database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=nhl.db";
builder.Services.AddDbContextFactory<NhlDbContext>(options =>
    options.UseSqlite(connectionString));

// Get NHL API options for HTTP client configuration
var nhlApiOptions = builder.Configuration.GetSection(NhlApiOptions.SectionName).Get<NhlApiOptions>() ?? new NhlApiOptions();

// Add HTTP client for NHL API with retry handling
builder.Services.AddHttpClient("NhlApi", client =>
{
    client.BaseAddress = new Uri(nhlApiOptions.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.DefaultRequestHeaders.Add("User-Agent", "PuckDrop/1.0");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
})
.AddStandardResilienceHandler();

// Add memory cache for API responses
builder.Services.AddMemoryCache();

// Register application services
builder.Services.AddScoped<INhlApiClient, NhlApiClient>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IWebhookRuleService, WebhookRuleService>();
builder.Services.AddScoped<IWebhookDispatchService, WebhookDispatchService>();
builder.Services.AddScoped<IEventProcessingService, EventProcessingService>();
builder.Services.AddSingleton<IMockDataService, MockDataService>();
builder.Services.AddSingleton<IWebhookNotificationService, WebhookNotificationService>();

// Register singleton state container
builder.Services.AddSingleton<GameStateContainer>();

// Register background services
builder.Services.AddHostedService<GamePollingBackgroundService>();

var app = builder.Build();

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NhlDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
