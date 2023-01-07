﻿using ZapMe.Data.Models;

namespace ZapMe.Services.Interfaces;

/// <summary>
/// Represents a store for sign in entities.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="sessionName"></param>
    /// <param name="ipAddress"></param>
    /// <param name="countryCode"></param>
    /// <param name="userAgent"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity> CreateAsync(Guid userId, string sessionName, string ipAddress, string countryCode, UserAgentEntity userAgent, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<SessionEntity[]> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="expiresAt"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> SetExipresAtAsync(Guid sessionId, DateTime expiresAt, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteSessionAsync(Guid sessionId, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> DeleteUserSessionsAsync(Guid userId, CancellationToken cancellationToken);
}