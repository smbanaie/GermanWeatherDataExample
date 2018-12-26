# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

library(RODBC)
library(dplyr)
library(infuser)
library(readr)
library(magrittr)
library(ggplot2)

# Connection String for the SQL Server Instance:
connectionString <- "Driver=SQL Server;Server=.;Database=LocalWeatherDatabase;Trusted_Connection=Yes"

# Connect to the Database:
connection <- odbcDriverConnect(connectionString)

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\WeatherDataColumnStore\\WeatherDataColumnStore\\R\\anomaly\\query.sql") %>% infuse(wban = "12957", start_date="2015-09-01 00:00:00.000", end_date="2015-09-30  23:59:59.997", simple_character = TRUE) 

# Query the Database: 
ts_temp <- sqlQuery(connection, query)

# Close ODBC Connection:
odbcClose(connection)

# Create the Plot with the September data:
temperature_september <- ggplot(ts_temp, aes(timestamp, temperature)) + 
    geom_line(na.rm=TRUE) +  
	geom_smooth(size = 1, se=FALSE) +
    ggtitle("Temperature September 2015") +
    xlab("Timestamp") + 
	ylab("Temperature (C)") + 
    theme_bw() +
    theme(plot.title = element_text(hjust = 0.5))

# And display it:
temperature_september

