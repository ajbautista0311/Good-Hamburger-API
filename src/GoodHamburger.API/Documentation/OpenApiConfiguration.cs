namespace GoodHamburger.API.Documentation;

using Microsoft.OpenApi;

public static class OpenApiConfiguration
{
    public static void ConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = GetApiInfo();
                document.Servers = GetServers();
                return Task.CompletedTask;
            });
        });
    }

    private static OpenApiInfo GetApiInfo()
    {
        return new OpenApiInfo
        {
            Title = "Good Hamburger API",
            Version = "v1",
            Description = GetApiDescription(),
            Contact = new OpenApiContact
            {
                Name = "Good Hamburger Team",
                Email = "support@goodhamburger.com"
            }
        };
    }

    private static string GetApiDescription()
    {
        return @"
# 🍔 REST API for Order System

## Available Menu

### Sandwiches
- **Sandwich** - $5.00
- **Egg** - $4.50
- **Bacon** - $7.00

### Extras
- **Fries** - $2.00
- **Soft drink** - $2.50

## Discount System
- 🎉 **20% OFF**: Sandwich + Fries + Soft drink
- 🎊 **15% OFF**: Sandwich + Soft drink
- 🎁 **10% OFF**: Sandwich + Fries

## Rules
- Every order must include at least one sandwich
- Only one item of each type per order
- No duplicates allowed
";
    }

    private static List<OpenApiServer> GetServers()
    {
        return new List<OpenApiServer>
        {
            new() { Url = "http://localhost:5000", Description = "Development" },
            new() { Url = "https://api.goodhamburger.com", Description = "Production" }
        };
    }
}