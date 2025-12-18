using ChildPlusKidkareSync.Core.Constants;
using ChildPlusKidkareSync.Core.Models.ChildPlus;
using ChildPlusKikareSync.Core.Models.ChildPlus;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace ChildPlusKidkareSync.Infrastructure.Data;

// ==================== CHILDPLUS REPOSITORY ====================
public interface IChildPlusRepository
{
    Task<List<ChildPlusSite>> GetSitesAsync(string tenantId, string connectionString);
    Task<List<ChildPlusStaff>> GetStaffsBySiteIdAsync(string connectionString, string siteId);
    Task<List<ChildPlusChild>> GetChildrenBySiteIdAsync(string connectionString, string siteId);
    Task<List<ChildPlusGuardian>> GetGuardiansByChildIdAsync(string connectionString, string childId);
    Task<List<ChildPlusEnrollment>> GetEnrollmentsByChildIdAsync(string connectionString, string childId);
    Task<List<ChildPlusAttendance>> GetAttendanceByChildIdAsync(string connectionString, string childId);
}

public class ChildPlusRepository : IChildPlusRepository
{
    private readonly ILogger<ChildPlusRepository> _logger;

    public ChildPlusRepository(ILogger<ChildPlusRepository> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get sites with composite timestamp support
    /// </summary>
    public async Task<List<ChildPlusSite>> GetSitesAsync(string tenantId, string connectionString)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            var sites = await connection.QueryAsync<ChildPlusSite>(SyncConstants.SqlQueries.GetSites, new { AgencyId = tenantId });
            return sites.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sites from ChildPlus");
            throw;
        }
    }

    public async Task<List<ChildPlusStaff>> GetStaffsBySiteIdAsync(string connectionString, string siteId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            var staff = await connection.QueryAsync<ChildPlusStaff>(SyncConstants.SqlQueries.GetStaffBySiteId, new { SiteId = siteId });
            return staff.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching staff for site {SiteId}", siteId);
            throw;
        }
    }

    public async Task<List<ChildPlusChild>> GetChildrenBySiteIdAsync(string connectionString, string siteId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            var children = await connection.QueryAsync<ChildPlusChild>(
                SyncConstants.SqlQueries.GetChildrenBySiteId,
                new { SiteId = siteId });
            return children.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching children for site {SiteId}", siteId);
            throw;
        }
    }

    public async Task<List<ChildPlusGuardian>> GetGuardiansByChildIdAsync(string connectionString, string childId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            var guardians = await connection.QueryAsync<ChildPlusGuardian>(
                SyncConstants.SqlQueries.GetGuardiansByChildId,
                new { ChildId = childId });
            return guardians.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching guardians for child {ChildId}", childId);
            throw;
        }
    }

    public async Task<List<ChildPlusEnrollment>> GetEnrollmentsByChildIdAsync(string connectionString, string childId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            var enrollments = await connection.QueryAsync<ChildPlusEnrollment>(
                SyncConstants.SqlQueries.GetEnrollmentsByChildId,
                new { ChildId = childId });
            return enrollments.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching enrollments for child {ChildId}", childId);
            throw;
        }
    }

    public async Task<List<ChildPlusAttendance>> GetAttendanceByChildIdAsync(string connectionString, string childId)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            var attendance = await connection.QueryAsync<ChildPlusAttendance>(
                SyncConstants.SqlQueries.GetAttendanceByChildId,
                new { ChildId = childId });
            return attendance.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching attendance for child {ChildId}", childId);
            throw;
        }
    }
}