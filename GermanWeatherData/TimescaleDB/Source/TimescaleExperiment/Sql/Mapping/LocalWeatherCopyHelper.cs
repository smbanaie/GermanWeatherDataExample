using PostgreSQLCopyHelper;
using NpgsqlTypes;
using TimescaleExperiment.Sql.Model;

namespace TimescaleExperiment.Sql.Mapping
{
    public class LocalWeatherCopyHelper : PostgreSQLCopyHelper<LocalWeatherData>
    {
        public LocalWeatherCopyHelper()
            : base("sample", "weather_data")
        {
            Map("station_identifier", x => x.StationIdentifier, NpgsqlDbType.Varchar);
            Map("timestamp", x => x.TimeStamp, NpgsqlDbType.Timestamp);
            Map("quality_code", x => x.QualityCode, NpgsqlDbType.Smallint);
            MapNullable("station_pressure", x => x.StationPressure, NpgsqlDbType.Real);
            MapNullable("air_temperature_at_2m", x => x.AirTemperatureAt2m, NpgsqlDbType.Real);
            MapNullable("air_temperature_at_5cm", x => x.AirTemperatureAt5cm, NpgsqlDbType.Real);
            MapNullable("relative_humidity", x => x.RelativeHumidity, NpgsqlDbType.Real);
            MapNullable("dew_point_temperature_at_2m", x => x.RelativeHumidity, NpgsqlDbType.Real);
        }
    }
}
