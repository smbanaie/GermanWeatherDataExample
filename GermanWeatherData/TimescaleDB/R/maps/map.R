# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

library(RODBC)
library(dplyr)
library(infuser)
library(readr)
library(magrittr)
library(albersusa)
library(viridis)
library(ggthemes)
library(ggplot2)
library(readr)
library(sp)

# Connection String for the SQL Server Instance:
connectionString <- "Driver=SQL Server;Server=.;Database=LocalWeatherDatabase;Trusted_Connection=Yes"

# Connect to the Database:
connection <- odbcDriverConnect(connectionString)

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\WeatherDataColumnStore\\WeatherDataColumnStore\\R\\maps\\query.sql") %>% 
	infuse(start_date="2015-07-01 00:00:00.000", end_date="2015-07-31 23:59:59.997", simple_character = TRUE) 

# Query the Database: 
station_temperatures <- sqlQuery(connection, query, as.is=c(TRUE, FALSE, FALSE, FALSE))

# Close ODBC Connection:
odbcClose(connection)

# Load the US Shapefile:
usa <- counties_composite()

# Exclude Hawaii and Alaska:
usa <- usa[!usa$state_fips %in% c("02", "15"),]

# Define the Coordinates of the Station:
pts <- as.data.frame(station_temperatures[,c("lon", "lat")])

coordinates(pts) <- ~lon+lat

# Assign the WGS84 Coordinate System to the Points:
proj4string(pts) <- proj4string(usa)

# Project each Station Position into the US Map:
merged_temperatures <- bind_cols(station_temperatures, over(pts, usa))

# Plot the Station Temperatures:
state_temperature <- merged_temperatures %>% 
    group_by(state) %>%		
    summarize(meanTemperature = mean(temperature, na.rm = FALSE))

us_state_map <- fortify(usa, region="state")

gg_state_temperatures <- ggplot() + 
	ggtitle("Average Temperature for the US States from July 2015") +
	geom_map(aes(x = long, y = lat, map_id = id), data = us_state_map, map = us_state_map, fill = "#ffffff", color = "#000000", size = 0.15) +
	geom_map(data=state_temperature, map=us_state_map, aes(fill=meanTemperature, map_id=state), size=0.15) +
	scale_fill_viridis(na.value='#404040') + 
	coord_fixed(x=us_state_map$long,y=us_state_map$lat) +
	theme(legend.position="right")

gg_state_temperatures

# Plot the County Temperatures:	
counties_temperature <- merged_temperatures %>% 
    group_by(fips) %>%		
    summarize(meanTemperature = mean(temperature, na.rm = FALSE))

us_counties_map <- fortify(usa, region="fips")

# Plot the Map with Stations:
gg_stations <- ggplot() + 
	ggtitle("Positions of US Weather Stations in 2015") +
	geom_map(data=us_counties_map, map=us_counties_map, aes(x=long, y=lat, map_id=id), color="#2b2b2b", size=0.1, fill=NA) +
	geom_point(data=station_temperatures, aes(x=lon, y=lat), color="red") +
	coord_fixed(x=us_counties_map$long,y=us_counties_map$lat) +
	theme_map()

gg_stations

gg_county_temperatures <- ggplot() + 
	ggtitle("Average Temperature for the US Counties from July 2015") +
	geom_map(aes(x = long, y = lat, map_id = id), data = us_counties_map, map = us_counties_map, fill = "#ffffff", color = "#000000", size = 0.15) +
	geom_map(data=counties_temperature, map=us_counties_map, aes(fill=meanTemperature, map_id=fips), size=0.15) +
	scale_fill_viridis(na.value='#404040') + 
	coord_fixed(x=us_counties_map$long,y=us_counties_map$lat) +
	theme(legend.position="right")

gg_county_temperatures