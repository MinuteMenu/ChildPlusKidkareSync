-- ============================================
-- SQL Script to create SyncLogTable
-- ============================================

/*
    File: CreateSyncLogTable.sql
    Purpose: Create the SyncLogTable for logging synchronization results
    Usage: Run this script on each KidKare Sync database (e.g., CXADMIN)
*/

USE [CXADMIN];
GO

-- ================================================
-- STEP 1: Create or Update SyncLogTable
-- ================================================

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SyncLogTable')
BEGIN
    -- Create new table with composite timestamp support
    CREATE TABLE SyncLogTable (
        LogId                   INT IDENTITY(1,1) PRIMARY KEY,         -- Auto-incremented log ID
        EntityType              NVARCHAR(50) NOT NULL,                 -- Entity type (e.g., Child, Staff, Center)
        SourceId                NVARCHAR(100) NOT NULL,                -- ID from ChildPlus (e.g., ChildId, StaffId)
        TargetId                NVARCHAR(100) NULL,                    -- ID returned from KidKare (if available)
        SyncAction              NVARCHAR(20) NOT NULL,                 -- Action taken: Insert, Update, Skip, Error
        SyncStatus              NVARCHAR(20) NOT NULL,                 -- Result: Success or Failed
        Message                 NVARCHAR(MAX) NULL,                    -- Optional message or error details
        
        -- Timestamp columns (support both DATETIME and VARBINARY for ROWVERSION)
        RowVersionChildPlus     VARBINARY(8) NULL,                     -- Main table timestamp (ROWVERSION from ChildPlus)
        RowVersionComposite     VARBINARY(8) NULL,                     -- Composite timestamp (MAX of all related tables)
        RelatedTablesVersion    NVARCHAR(MAX) NULL,                    -- JSON detail of all related table timestamps
        
        TimestampSynced         DATETIME NOT NULL DEFAULT GETDATE(),   -- Timestamp when log was recorded
        CenterId                NVARCHAR(100) NULL,                    -- Optional: used for grouping by center
        RequestId               UNIQUEIDENTIFIER NOT NULL,             -- Unique ID for each sync request
        CreatedBy               NVARCHAR(100) NOT NULL                 -- System or user that performed the sync
    );
    
    PRINT 'SyncLogTable created successfully with composite timestamp support.';
END
ELSE
BEGIN
    -- Table exists - add new columns if they don't exist
    IF NOT EXISTS (SELECT * FROM sys.columns 
                   WHERE object_id = OBJECT_ID('SyncLogTable') 
                   AND name = 'RowVersionComposite')
    BEGIN
        ALTER TABLE SyncLogTable 
        ADD RowVersionComposite VARBINARY(8) NULL;
        
        PRINT 'Added column: RowVersionComposite';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns 
                   WHERE object_id = OBJECT_ID('SyncLogTable') 
                   AND name = 'RelatedTablesVersion')
    BEGIN
        ALTER TABLE SyncLogTable 
        ADD RelatedTablesVersion NVARCHAR(MAX) NULL;
        
        PRINT 'Added column: RelatedTablesVersion';
    END
    
    -- Convert RowVersionChildPlus from DATETIME to VARBINARY if needed
    IF EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID('SyncLogTable') 
               AND name = 'RowVersionChildPlus' 
               AND system_type_id = TYPE_ID('DATETIME'))
    BEGIN
        PRINT 'Warning: RowVersionChildPlus is DATETIME type. Consider data migration.';
        -- Note: Manual intervention may be needed to convert existing DATETIME data
    END
    
    PRINT 'SyncLogTable updated with composite timestamp columns.';
END
GO

-- ================================================
-- STEP 2: Create/Update Indexes for Performance
-- ================================================

-- Drop old indexes if they exist
IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name = 'IX_SyncLogTable_EntityType_SourceId' 
           AND object_id = OBJECT_ID('SyncLogTable'))
BEGIN
    DROP INDEX IX_SyncLogTable_EntityType_SourceId ON SyncLogTable;
    PRINT 'Dropped old index: IX_SyncLogTable_EntityType_SourceId';
END

IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name = 'IX_SyncLogTable_RequestId' 
           AND object_id = OBJECT_ID('SyncLogTable'))
BEGIN
    DROP INDEX IX_SyncLogTable_RequestId ON SyncLogTable;
    PRINT 'Dropped old index: IX_SyncLogTable_RequestId';
END

IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name = 'IX_SyncLogTable_CenterId' 
           AND object_id = OBJECT_ID('SyncLogTable'))
BEGIN
    DROP INDEX IX_SyncLogTable_CenterId ON SyncLogTable;
    PRINT 'Dropped old index: IX_SyncLogTable_CenterId';
END

IF EXISTS (SELECT * FROM sys.indexes 
           WHERE name = 'IX_SyncLogTable_TimestampSynced' 
           AND object_id = OBJECT_ID('SyncLogTable'))
BEGIN
    DROP INDEX IX_SyncLogTable_TimestampSynced ON SyncLogTable;
    PRINT 'Dropped old index: IX_SyncLogTable_TimestampSynced';
END

-- Create optimized indexes for composite timestamp queries
CREATE NONCLUSTERED INDEX IX_SyncLog_Composite_EntitySourceStatus
ON SyncLogTable(EntityType, SourceId, SyncStatus)
INCLUDE (RowVersionComposite, RowVersionChildPlus, TimestampSynced)
WITH (ONLINE = ON, FILLFACTOR = 90);

CREATE NONCLUSTERED INDEX IX_SyncLog_CompositeTimestamp
ON SyncLogTable(EntityType, SourceId)
INCLUDE (RowVersionComposite, SyncStatus, TimestampSynced)
WHERE RowVersionComposite IS NOT NULL
WITH (ONLINE = ON, FILLFACTOR = 90);

CREATE NONCLUSTERED INDEX IX_SyncLog_RequestId
ON SyncLogTable(RequestId)
INCLUDE (EntityType, SourceId, SyncStatus, TimestampSynced)
WITH (ONLINE = ON, FILLFACTOR = 90);

CREATE NONCLUSTERED INDEX IX_SyncLog_CenterId_Time
ON SyncLogTable(CenterId, TimestampSynced DESC)
WHERE CenterId IS NOT NULL
WITH (ONLINE = ON, FILLFACTOR = 90);

CREATE NONCLUSTERED INDEX IX_SyncLog_TimestampSynced
ON SyncLogTable(TimestampSynced DESC)
INCLUDE (EntityType, SourceId, SyncStatus)
WITH (ONLINE = ON, FILLFACTOR = 90);

PRINT 'Indexes created successfully.';
GO

-- ================================================
-- STEP 3: Backfill Existing Data (Optional)
-- ================================================

-- For existing records without composite timestamp, copy from main timestamp
UPDATE SyncLogTable
SET RowVersionComposite = RowVersionChildPlus,
    RelatedTablesVersion = 
        CASE 
            WHEN RowVersionChildPlus IS NOT NULL 
            THEN JSON_OBJECT('MainTable': CONVERT(VARCHAR(MAX), RowVersionChildPlus, 1))
            ELSE NULL
        END
WHERE RowVersionComposite IS NULL
    AND RowVersionChildPlus IS NOT NULL;

DECLARE @UpdatedRows INT = @@ROWCOUNT;
PRINT 'Backfilled ' + CAST(@UpdatedRows AS VARCHAR(10)) + ' existing records with composite timestamps.';
GO

-- ================================================
-- STEP 4: Update Statistics
-- ================================================

UPDATE STATISTICS SyncLogTable WITH FULLSCAN;
PRINT 'Statistics updated.';
GO

-- ================================================
-- STEP 5: Verification
-- ================================================

-- Check table structure
SELECT 
    'Table Structure' AS CheckType,
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('SyncLogTable')
ORDER BY c.column_id;

-- Check indexes
SELECT 
    'Indexes' AS CheckType,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique,
    STUFF((
        SELECT ', ' + c.name
        FROM sys.index_columns ic
        INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
        WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id
        ORDER BY ic.key_ordinal
        FOR XML PATH('')
    ), 1, 2, '') AS IndexColumns
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID('SyncLogTable')
    AND i.name IS NOT NULL
ORDER BY i.name;

-- Check data statistics
SELECT 
    'Data Statistics' AS CheckType,
    COUNT(*) AS TotalRecords,
    COUNT(CASE WHEN RowVersionComposite IS NOT NULL THEN 1 END) AS RecordsWithComposite,
    COUNT(CASE WHEN RelatedTablesVersion IS NOT NULL THEN 1 END) AS RecordsWithJsonDetail,
    CAST(COUNT(CASE WHEN RowVersionComposite IS NOT NULL THEN 1 END) * 100.0 / NULLIF(COUNT(*), 0) AS DECIMAL(5,2)) AS PercentageWithComposite
FROM SyncLogTable;

-- Check by entity type
SELECT 
    'Data by Entity Type' AS CheckType,
    EntityType,
    COUNT(*) AS TotalRecords,
    COUNT(CASE WHEN RowVersionComposite IS NOT NULL THEN 1 END) AS WithComposite,
    COUNT(CASE WHEN SyncStatus = 'Success' THEN 1 END) AS Successful,
    COUNT(CASE WHEN SyncStatus = 'Failed' THEN 1 END) AS Failed
FROM SyncLogTable
GROUP BY EntityType
ORDER BY TotalRecords DESC;

-- Sample records
SELECT TOP 5
    'Sample Records' AS CheckType,
    LogId,
    EntityType,
    SourceId,
    SyncStatus,
    CONVERT(BIGINT, RowVersionChildPlus) AS MainTimestamp,
    CONVERT(BIGINT, RowVersionComposite) AS CompositeTimestamp,
    CASE 
        WHEN RowVersionComposite > RowVersionChildPlus THEN 'Composite Greater'
        WHEN RowVersionComposite = RowVersionChildPlus THEN 'Same'
        ELSE 'Different'
    END AS ComparisonResult,
    LEFT(RelatedTablesVersion, 100) + '...' AS JsonDetailPreview,
    TimestampSynced
FROM SyncLogTable
WHERE RowVersionComposite IS NOT NULL
ORDER BY TimestampSynced DESC;

GO

PRINT '';
PRINT '================================================';
PRINT 'Database setup completed successfully!';
PRINT '================================================';
PRINT 'Next steps:';
PRINT '1. Verify the index creation';
PRINT '2. Test sync decision queries';
PRINT '3. Deploy updated application code';
PRINT '4. Monitor performance improvements';
PRINT '================================================';
GO