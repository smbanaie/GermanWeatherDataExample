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
    infuse(start_date='2017-01-01', end_date='2018-01-01', station='02497', simple_character = TRUE) 

# Query the Database: 
temperatures <- dbGetQuery(connection, query)

# Close Postgres Connection:
dbDisconnect(connection)


# Create the Plot with the September data:
gg_plot <- ggplot(temperatures, aes(timestamp, temperature)) + 
    geom_line(na.rm=TRUE) +  
	geom_smooth(size = 1, se=FALSE) +
    ggtitle("Hourly Average Temperature") +
    xlab("Timestamp") + 
	ylab("Temperature (C)") + 
    theme_bw() +
    theme(plot.title = element_text(hjust = 0.5))

# And display it:
gg_plot
