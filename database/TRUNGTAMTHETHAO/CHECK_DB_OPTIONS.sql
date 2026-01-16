-- Check database options and lock behavior

SELECT name, is_read_committed_snapshot_on, snapshot_isolation_state_desc
FROM sys.databases
WHERE name IN ('TRUNGTAMTHETHAO', 'TRUNGTAMTHETHAO_FIXED');
GO
