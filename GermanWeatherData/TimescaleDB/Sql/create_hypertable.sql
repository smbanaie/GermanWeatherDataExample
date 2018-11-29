DO $$

BEGIN

--
-- The user needs to be SUPERUSER to create extensions. So before executing the Script run 
-- something along the lines:
--
--      postgres=# ALTER USER philipp WITH SUPERUSER;
-- 
-- And after executing, you can revoke the SUPERUSER Role again:
--
--      postgres=# ALTER USER philipp WITH NOSUPERUSER;
--
CREATE EXTENSION IF NOT EXISTS timescaledb CASCADE;

--
-- Make sure to create the timescaledb Extension in the Schema:
--
PERFORM create_hypertable('sample.weather_data', 'timestamp');

END

$$