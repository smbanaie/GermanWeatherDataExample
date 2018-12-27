# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

#install.packages("devtools")
#devtools::install_github("twitter/AnomalyDetection")

library(DBI)
library(dplyr)
library(infuser)
library(readr)
library(magrittr)
library(ggplot2)
library(zoo)
library(AnomalyDetection)

# Connect to the Database:
connection <- dbConnect(RPostgres::Postgres(),
                 dbname = 'sampledb', 
                 host = 'localhost', # i.e. 'ec2-54-83-201-96.compute-1.amazonaws.com'
                 port = 5432, # or any other port specified by your DBA
                 user = 'philipp',
                 password = 'pwd')

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\anomaly\\query.sql") %>% 
    infuse(start_date='2016-01-01', end_date='2018-01-01', station='02497', simple_character = TRUE) 

# Query the Database: 
temperatures <- dbGetQuery(connection, query)

# Close Postgres Connection:
dbDisconnect(connection)

# Build a dense timeseries with all expected timestamps:
timeseries <- data.frame(timestamp=seq(as.POSIXct("2016-01-01"), as.POSIXct("2017-12-31"), by="hour"))

# Build the Dense series by left joining both series:
timeseries <- left_join(timeseries, temperatures, by = c("timestamp" = "timestamp"))

# Drop all NA and extrapolate Borders with rule=2:
timeseries$temperature <- na.approx(timeseries$temperature, rule=2)

# Detect Anomalies:
res = AnomalyDetectionVec(timeseries$temperature, direction='both', period=8760, plot=TRUE)

# Plot the Anomalies:
res$plot

# Display the Anomalies:
res$anoms
