# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

#install.packages("DBI")
#instal.packages("RPostgres")
#install.packages("dplyr")
#install.packages("infuser")
#install.packages("magrittr")
#install.packages("viridis")
#install.packages("ggthemes")
#install.packages("ggplot2")
#install.packages("readr")
#install.packages("sp")
#install.packages("sf")

library(DBI)
library(dplyr)
library(infuser)
library(magrittr)
library(viridis)
library(ggthemes)
library(ggplot2)
library(readr)
library(sf)

# Connect to the Database:
connection <- dbConnect(RPostgres::Postgres(),
                 dbname = 'sampledb', 
                 host = 'localhost', # i.e. 'ec2-54-83-201-96.compute-1.amazonaws.com'
                 port = 5432, # or any other port specified by your DBA
                 user = 'philipp',
                 password = 'test_pwd')

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\maps\\query.sql")

# Query the Database: 
temperatures <- dbGetQuery(connection, query, param = list('2017-01-01', '2018-01-01'))

# Close Postgres Connection:
dbDisconnect(connection)

# Load the Germany Shapefile:
germany_shp <- st_read('D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\shapes\\Zensus_2011_Bundeslaender.shp')

# Fortify and Join with Temperatures:
germany_shp.df <- merge(fortify(germany_shp), as.data.frame(temperatures), by.x="NAME", by.y="state")

ggplot(germany_shp.df) +
    geom_sf(aes(fill=avg_temp)) +
    scale_fill_viridis()

