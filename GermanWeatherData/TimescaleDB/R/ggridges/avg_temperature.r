# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

#install.packages("DBI")
#instal.packages("RPostgres")
#install.packages("ggplot2")
#install.packages("viridis")
#install.packages("readr")
#install.packages("ggridges")

library(DBI)
library(ggplot2)
library(viridis)
library(readr)
library(ggridges)

# Connect to the database:
connection <- dbConnect(RPostgres::Postgres(),
                 dbname = 'sampledb', 
                 host = 'localhost', # i.e. 'ec2-54-83-201-96.compute-1.amazonaws.com'
                 port = 5432, # or any other port specified by your DBA
                 user = 'philipp',
                 password = 'test_pwd')

# Read the SQL Query from an external file. Keeps the Script clean:
query <- read_file("D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\ggridges\\query.sql")

# Query the Database and bind the positional query parameters: 
temperatures <- dbGetQuery(connection, query, param = list('01766', '2017-01-01', '2018-01-01'))

# As a good citizen close RPostgres connection:
dbDisconnect(connection)

# First we trim off any whitespaces added by RPostgres, and then turn the characters 
# into Factors. The Factors will be unordered, so we order the factors by month. I am 
# adjusting it to be reverse, so it looks good in ggridges:
temperatures$month <- trimws(temperatures$month)
temperatures$month <- as.factor(temperatures$month)
temperatures$month <- factor(temperatures$month, levels = c("December", "November", "October", "September", "August", "July", "June", "May", "April","March", "February", "January"))

# Create the ggridges plot. This uses the same approach like described in the ggridges 
# documentation at: https://cran.r-project.org/web/packages/ggridges/vignettes/introduction.html.
# 
# I have used Station 01766, which is Münster/Osnabrück, which I am hardcoding here:
ggplot(temperatures, aes(x = `avg_temp`, y = `month`, fill = ..x..)) +
  geom_density_ridges_gradient(scale = 3, rel_min_height = 0.01) +
  scale_fill_viridis(name = "avg_temp", option = "C") +
  labs(title = 'Temperatures Münster/Osnabrück in 2017')
