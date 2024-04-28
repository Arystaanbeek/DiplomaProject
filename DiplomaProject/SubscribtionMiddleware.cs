using DiplomaProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class SubscriptionMiddleware
{
    private readonly RequestDelegate _next;

    public SubscriptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, ApplicationDbContext dbContext)
    {
        var user = context.User;

        if (user.Identity.IsAuthenticated)
        {
            var userId = userManager.GetUserId(user);
            var appUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser != null && (!appUser.IsSubscribed || appUser.SubscriptionEndDate <= DateTime.UtcNow))
            {
                context.Response.Redirect("/subscription");
                return;
            }
        }

        await _next(context);
    }
}
