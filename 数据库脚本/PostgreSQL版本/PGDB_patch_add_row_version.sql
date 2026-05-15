DO
$$
DECLARE
    r record;
BEGIN
    FOR r IN
        SELECT c.table_name
        FROM information_schema.columns c
        WHERE c.table_schema = current_schema()
          AND c.column_name = 'delete_mk'
          AND NOT EXISTS (
                SELECT 1
                FROM information_schema.columns x
                WHERE x.table_schema = c.table_schema
                  AND x.table_name = c.table_name
                  AND x.column_name = 'row_version'
            )
    LOOP
        EXECUTE format('ALTER TABLE %I ADD COLUMN row_version BIGINT NOT NULL DEFAULT 1;', r.table_name);
        EXECUTE format('COMMENT ON COLUMN %I.row_version IS %L;', r.table_name, 'Row version (optimistic lock)');
    END LOOP;
END
$$;
