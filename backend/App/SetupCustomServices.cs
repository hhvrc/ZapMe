﻿using System.Net.Http.Headers;
using ZapMe.Services;
using ZapMe.Services.Interfaces;

namespace ZapMe.App;

partial class App
{
    private void AddCustomServices()
    {
        Action<HttpClient> SetupHttpClient = static cli =>
        {
            cli.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.AppName, Constants.AppVersion.String));
        };

        Services.AddHttpClient<IDebounceService, DebounceService>(SetupHttpClient);
        Services.AddHttpClient<IGoogleReCaptchaService, GoogleReCaptchaService>(SetupHttpClient);
        Services.AddHttpClient<IMailGunService, MailGunService>(SetupHttpClient);

        Services.AddTransient<IHybridCache, HybridCache>();
        Services.AddTransient<IPasswordHasher, PasswordHasher>();
        Services.AddTransient<IUserStore, UserStore>();
        Services.AddTransient<IUserManager, UserManager>();
        Services.AddTransient<ISignInStore, SignInStore>();
        Services.AddTransient<ILockOutStore, LockOutStore>();
        Services.AddTransient<ILockOutManager, LockOutManager>();
        Services.AddTransient<ISignInManager, SignInManager>();
        Services.AddTransient<IUserRelationStore, UserRelationStore>();
        Services.AddTransient<IFriendRequestStore, FriendRequestStore>();
        Services.AddTransient<IUserRelationManager, UserRelationManager>();
        Services.AddTransient<IWebSocketInstanceManager, WebSocketInstanceManager>();
    }
}