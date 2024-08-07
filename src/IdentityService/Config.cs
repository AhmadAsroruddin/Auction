﻿using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("auctionApp", "Auction App full access"),
        ];

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
           new Client
           {
                ClientId = "postman",
                ClientName = "postman",
                AllowedScopes = {"openid", "profile", "auctionApp"},
                RedirectUris = {"https://www.getpostman.com/oauth2/callback"},
                ClientSecrets = new []{new Secret("THISISSECRET".Sha256())},
                AllowedGrantTypes = {GrantType.ResourceOwnerPassword}
           },
           new Client
           {
                ClientId = "nextApp",
                ClientName = "nextApp",
                ClientSecrets = new []{new Secret("NOTSECRET".Sha256())},
                AllowedScopes = {"openid", "profile", "auctionApp"},
                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                RequirePkce = false,
                RedirectUris = {"http://localhost:3000/api/auth/callback/id-server"},
                AllowOfflineAccess = true,
                AccessTokenLifetime = 3600 * 24 * 30
           }
        };
}
