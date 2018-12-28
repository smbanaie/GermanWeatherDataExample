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
library(viridis)
library(ggthemes)
library(ggplot2)
library(lubridate)

# Connect to the Database:
connection <- dbConnect(RPostgres::Postgres(),
                 dbname = 'sampledb', 
                 host = 'localhost', # i.e. 'ec2-54-83-201-96.compute-1.amazonaws.com'
                 port = 5432, # or any other port specified by your DBA
                 user = 'philipp',
                 password = 'pwd')

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\calendar\\query.sql") %>% 
    infuse(start_date='2016-01-01', end_date='2017-01-01', station='02497', simple_character = TRUE) 

# Query the Database: 
temperatures <- dbGetQuery(connection, query)

# Close Postgres Connection:
dbDisconnect(connection)

# Make sure we operate on R Dates:
temperatures <- temperatures %>%
    mutate(date=as.Date(day))

# Build a dense timeseries with all expected timestamps:
timeseries <- data.frame(date=seq(as.Date("2016-01-01"), as.Date("2016-12-31"), by="days"))

# Build the Dense series by left joining both series:
timeseries <- left_join(timeseries, temperatures, by = c("date" = "date")) %>% 
    mutate(dayOfMonth=day(timeseries$date), monthOfYear=month(timeseries$date)) 

# Create the Plot:
ggplot(timeseries, aes(dayOfMonth, monthOfYear, fill = temperature)) + 
    geom_raster(hjust = 0, vjust = 0)
