# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

#install.packages("ggplot2")
#install.packages("sf")

library(ggplot2)
library(sf)

# Load the Germany Shapefile:
germany_shp <- st_read('D:\\github\\GermanWeatherDataExample\\GermanWeatherData\\TimescaleDB\\R\\shapes\\Zensus_2011_Bundeslaender.shp')

ggplot(germany_shp) +
    geom_sf()
