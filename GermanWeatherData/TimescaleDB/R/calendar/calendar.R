# Copyright (c) Philipp Wagner. All rights reserved.
# Licensed under the MIT license. See LICENSE file in the project root for full license information.

library(RODBC)
library(dplyr)
library(infuser)
library(readr)
library(magrittr)
library(lubridate)
library(viridis)
library(ggthemes)
library(ggplot2)

# Connection String for the SQL Server Instance:
connectionString <- "Driver=SQL Server;Server=.;Database=LocalWeatherDatabase;Trusted_Connection=Yes"

# Connect to the Database:
connection <- odbcDriverConnect(connectionString)

# Read the SQL Query from an external file and infuse the variables. Keeps the Script clean:
query <- read_file("D:\\github\\WeatherDataColumnStore\\WeatherDataColumnStore\\R\\calendar\\query.sql") %>% infuse(wban = "00150", simple_character = TRUE) 

# Query the Database: 
results <- sqlQuery(connection, query)

# Close ODBC Connection:
odbcClose(connection)

# Add a date column to the SQL Result:
results <- results %>% mutate(date = lubridate::make_date(year, month, day))

# Build a dense timeseries with all expected timestamps:
dense <- data.frame(date=seq(as.Date("2015-01-01"), as.Date("2015-12-31"), by="1 day"))

# Build the Dense series by left joining both series:
merged_series <- left_join(dense, results, by = c("date" = "date"))

# The entire following code is taken from the amazing blog post: 
#
#	https://towardsdatascience.com/visualising-temperatures-in-amsterdam-as-a-heatmap-in-r-part-ii
#
merged_series <- merged_series %>% mutate(
	month = month(date, label=T, abbr=T),
    week = strftime(date,"%W"), 
    weekday = substring(wday(date, label=T, abbr=T),1,2),
    day = day(date))

# Set Priorities:
merged_series$weekday <- factor(merged_series$weekday, levels=c("Mo", "Tu", "We", "Th", "Fr", "Sa", "Su"))
merged_series$week <- factor(merged_series$week, levels=rev(sort(unique(merged_series$week))))

# Create the Plot:
ggplot(data = merged_series, aes(x = weekday, y = week)) + 
geom_tile(aes(fill = averageTemperature)) + 
coord_equal(ratio = 1) + 
scale_fill_viridis(option = "magma", na.value='#FFFFFF00') + 
theme_tufte(base_family = "Helvetica") +
facet_wrap(~month, nrow = 3, scales="free") + 
geom_text(aes(label = day), color = "gray", size = 3) + 
# Hide y-axis ticks and labels:
theme(axis.ticks.y = element_blank()) +
theme(axis.text.y = element_blank()) +
# Hide main x and y-axis titles:
theme(axis.title.x = element_blank()) + 
theme(axis.title.y = element_blank()) +
# Move x-axis labels (week names) to top, hide ticks:
scale_x_discrete(position = "top") +
theme(axis.ticks.x = element_blank()) +
# Move panel title (month names) outside (above week names):
theme(strip.placement = "outside") +
theme(strip.text.x = element_text(size = "14", hjust = 0)) +
# Center-aligned plot title:
ggtitle("Heatmap of average temperatures") + 
theme(plot.title = element_text(size = "16", hjust = 0.5))