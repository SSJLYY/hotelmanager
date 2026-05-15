SET @schema_name = DATABASE();
SET SESSION group_concat_max_len = 1024000;

SELECT GROUP_CONCAT(
         CONCAT(
           'ALTER TABLE `', c.table_name, '` ',
           'ADD COLUMN `row_version` BIGINT NOT NULL DEFAULT 1 COMMENT ''Row version (optimistic lock)'';'
         )
         SEPARATOR ' '
       )
INTO @ddl_sql
FROM information_schema.columns c
WHERE c.table_schema = @schema_name
  AND c.column_name = 'delete_mk'
  AND NOT EXISTS (
        SELECT 1
        FROM information_schema.columns x
        WHERE x.table_schema = c.table_schema
          AND x.table_name = c.table_name
          AND x.column_name = 'row_version'
      );

SET @ddl_sql = IFNULL(@ddl_sql, 'SELECT 1;');
PREPARE stmt FROM @ddl_sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SELECT GROUP_CONCAT(
         CONCAT(
           'ALTER TABLE `', c.table_name, '` ',
           'ADD UNIQUE INDEX `uk_id_row_version` (`id`, `row_version`);'
         )
         SEPARATOR ' '
       )
INTO @idx_sql
FROM information_schema.columns c
WHERE c.table_schema = @schema_name
  AND c.column_name = 'delete_mk'
  AND EXISTS (
        SELECT 1
        FROM information_schema.columns x
        WHERE x.table_schema = c.table_schema
          AND x.table_name = c.table_name
          AND x.column_name = 'id'
      )
  AND EXISTS (
        SELECT 1
        FROM information_schema.columns x
        WHERE x.table_schema = c.table_schema
          AND x.table_name = c.table_name
          AND x.column_name = 'row_version'
      )
  AND NOT EXISTS (
        SELECT 1
        FROM (
            SELECT s.index_name,
                   SUM(CASE WHEN s.column_name = 'id' THEN 1 ELSE 0 END) AS has_id,
                   SUM(CASE WHEN s.column_name = 'row_version' THEN 1 ELSE 0 END) AS has_row_version,
                   COUNT(*) AS column_count
            FROM information_schema.statistics s
            WHERE s.table_schema = c.table_schema
              AND s.table_name = c.table_name
              AND s.non_unique = 0
            GROUP BY s.index_name
        ) idx
        WHERE idx.has_id = 1
          AND idx.has_row_version = 1
          AND idx.column_count = 2
      );

SET @idx_sql = IFNULL(@idx_sql, 'SELECT 1;');
PREPARE stmt FROM @idx_sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;
