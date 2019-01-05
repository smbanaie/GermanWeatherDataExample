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
-- The data I am going to import contains 27 years of data. The default chunk_time_interval of TimescaleDB defaults to 1 week, so 
-- for 27 years of data we would end up with ~1400 chunks each with 280,000 rows. This will be too execessive especially on the 
-- reading side, so for now the chunk time interval is set to 1 year.
--
-- I want to maximize the the insert rate, so I am deactivating the index creation during load and will defer the index creation,  
-- which will give TimescaleDB an additional speedup. Please note, that the official TimescaleDB benchmarks do include the index 
-- creation during inserts.
--
PERFORM create_hypertable('sample.weather_data', 'timestamp', chunk_time_interval => interval '1 year', create_default_indexes  => FALSE);

END

$$