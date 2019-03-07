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
library(tibble)
library(ggridges)

# Connect to the Database:
connection <- dbConnect(RPostgres::Postgres(),
                 dbname = 'sampledb', 
                 host = 'localhost', # i.e. 'ec2-54-83-201-96.compute-1.amazonaws.com'
                 port = 5432, # or any other port specified by your DBA
                 user = 'philipp',
                 password = 'test_pwd')

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\ggridges\\query.sql")

# Query the Database: 
temperatures <- dbGetQuery(connection, query, param = list('01766', '2017-01-01', '2018-01-01'))
temperatures$month <- trimws(temperatures$month)
temperatures$month <- as.factor(temperatures$month)
temperatures$month <- factor(temperatures$month, levels = c("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"))


# Close Postgres Connection:
dbDisconnect(connection)

ggplot(temperatures, aes(x = `avg_temp`, y = `month`, fill = ..x..)) +
  geom_density_ridges_gradient(scale = 3, rel_min_height = 0.01) +
  scale_fill_viridis(name = "avg_temp", option = "C") +
  labs(title = 'Temperatures Münster/Osnabrück in 2017')
