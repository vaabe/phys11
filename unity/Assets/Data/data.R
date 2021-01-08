options(warn = -1)

setwd(".")

## read data
state_cartesian = read.csv("simdata.csv", header = F) 
time = read.csv("timedata.csv", header = F) 
meta = read.delim("metadata.txt", header = F)

## extract position and velocity data
position_cartesian = state_cartesian[seq(1, nrow(state_cartesian), 2), ]
velocity_cartesian = state_cartesian[seq(2, nrow(state_cartesian), 2), ]

## extract simulation parameters
system_name = as.character(meta[1, 1]) 
time_step = as.numeric(as.character(meta[2, 1]))
num_time_steps = dim(time)[1] - 1
run_time = time_step * num_time_steps

## extract system parameters
num_particles = as.integer(as.character(meta[3, 1]))
mass_values = as.numeric(as.character(meta[4:(3 + num_particles), 1]))

## build dataset:

## create time indicator
time_indicator = NULL 
for (i in 1:num_time_steps) {
	time_indicator = c(time_indicator, 
			   rep(time$V1[i], num_particles)) 
}

vdata = data.frame(time = time_indicator)

## create particle indicator 
mass_labels = NULL
for (i in 1:num_particles) {
	mass_labels[i] = paste("m", i, sep = "")
}

particle_indicator = rep(mass_labels, num_time_steps)

vdata$particle = particle_indicator

## add mass values
vdata$mass = rep(mass_values, num_time_steps)

## merge cartesian state data
vdata[ , 4:6] = position_cartesian
vdata[ , 7:9] = velocity_cartesian

## rename columns
names(vdata)[4:9] = c("x", "y", "z", "vx", "vy", "vz")

## compute momentum data
vdata$p_x = vdata$mass * vdata$vx
vdata$p_y = vdata$mass * vdata$vy
vdata$p_z = vdata$mass * vdata$vz

## make horizontal dataset [reshape]
hdata = reshape(vdata, 
		direction = "wide", 
		idvar = "time", 
		timevar = "particle", 
		v.names = c("mass", "x", "y", "z", 
			    "vx", "vy", "vz", 
			    "p_x", "p_y", "p_z")) 

## save data
save(time_step, run_time, num_time_steps, num_particles, 
     mass_values, vdata, hdata,
     file = "sim.Rdata")

write.csv(vdata, "cleaned_data.csv")
