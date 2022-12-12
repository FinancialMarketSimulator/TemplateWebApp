﻿using CleanMicroserviceSystem.Themis.Domain.Identity;

namespace CleanMicroserviceSystem.Themis.Application.Repository;

public interface IOceanusUserRepository
{
    Task<IEnumerable<OceanusUser>> Search(
        int? id,
        string? userName,
        string? email,
        string? phoneNumber,
        int start,
        int count);
}
