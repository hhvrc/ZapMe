﻿using System.Threading.RateLimiting;

namespace ZapMe.App;

partial class App
{
    private void AddRateLimiting()
    {
        Services.AddRateLimiter(opt =>
        {
            opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            opt.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                ZapMe.Data.Models.SignInEntity? signIn = ctx.GetSignIn();
                if (signIn == null)
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(ctx.GetRemoteIP(), key => new SlidingWindowRateLimiterOptions()
                    {
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        PermitLimit = 120,
                        SegmentsPerWindow = 6,
                        AutoReplenishment = true,
                    });
                }

                ZapMe.Data.Models.AccountEntity user = signIn.User;
                if (user?.UserRoles?.Select(r => r.RoleName).Contains("admin") ?? false)
                {
                    return RateLimitPartition.GetNoLimiter("admin");
                }

                return RateLimitPartition.GetTokenBucketLimiter(signIn.UserId.ToString(), key => new TokenBucketRateLimiterOptions()
                {
                    TokenLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 20,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    TokensPerPeriod = 5,
                    AutoReplenishment = true
                });
            });
        });
    }
}