﻿using FlyIt.DataAccess.Entities.Identity;
using System;

namespace FlyIt.DataAccess.Repositories
{
    public interface IUserTokenRepository
    {
        public void RemoveUserToken(User user, string loginProvider);

        public UserToken AddUserToken(User user, string accessToken, string refreshToken, DateTime accessTokenExpiration, DateTime refreshTokenExpiration, string loginProvider);

        public UserToken UpdateUserToken(User user, string accessToken, DateTime accessTokenExpiration);

        public bool ValidateAuthenticationToken(User user, string refreshToken, string accessToken);
    }
}
