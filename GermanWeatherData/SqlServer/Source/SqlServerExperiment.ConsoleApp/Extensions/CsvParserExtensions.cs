using System;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace SqlServerExperiment.ConsoleApp.Extensions
{
    public static class CsvParserExtensions
    {
        public static ParallelQuery<CsvMappingResult<TEntity>> ReadFromFile<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding, int skip)
            where TEntity : class, new()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            var lines = File
                // Read in all lines:
                .ReadLines(fileName, encoding)
                // And index them all:
                .Select((line, index) => new Row(index, line))
                // But skip some of them:
                .Skip(skip);

            return csvParser.Parse(lines);
        }
    }
}
