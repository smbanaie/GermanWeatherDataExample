# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

library(RODBC)
library(dplyr)
library(infuser)
library(readr)
library(magrittr)
library(ggplot2)
library(zoo)
library(AnomalyDetection)

# Connection String for the SQL Server Instance:
connectionString <- "Driver=SQL Server;Server=.;Database=LocalWeatherDatabase;Trusted_Connection=Yes"

# Connect to the Database:
connection <- odbcDriverConnect(connectionString)

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\WeatherDataColumnStore\\WeatherDataColumnStore\\R\\anomaly\\query.sql") %>% infuse(wban = "12957", start_date="2014-01-01 00:00:00.000", end_date="2016-07-01 23:59:59.997", simple_character = TRUE) 

# Query the Database: 
ts_temp <- sqlQuery(connection, query)

# Close ODBC Connection:
odbcClose(connection)

# Build a dense timeseries with all expected timestamps:
ts_dense <- data.frame(timestamp=seq(as.POSIXct("2014-01-01"), as.POSIXct("2015-12-31"), by="hour"))

# Build the Dense series by left joining both series:
ts_merged <- left_join(ts_dense, ts_temp, by = c("timestamp" = "timestamp"))

# Use zoo to interpolate missing values:
ts_merged$interpolated_temperature <- na.approx(ts_merged$temperature)

# Detect Anomalies:
res = AnomalyDetectionVec(ts_merged$interpolated_temperature, direction='both', period=8760, plot=TRUE)

# Plot the Anomalies:
res$plot

# Display the Anomalies:
res$anoms
