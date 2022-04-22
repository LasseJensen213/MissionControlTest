CREATE TABLE `Time`
(
    `id`     guid PRIMARY KEY AUTOINCREMENT,
    `source` text check ( source in ('telemetry', 'groundProtocol') ),
    `time`   timestamp,
    FOREIGN KEY (id) REFERENCES Time(id)
);

CREATE TABLE `Accelerometer`
(
    `id` guid PRIMARY KEY,
    `x`  double COMMENT 'Acceleration in x-axis',
    `y`  double COMMENT 'Acceleration in y-axis',
    `z`  double COMMENT 'Acceleration in z-axis',
    FOREIGN KEY (id) REFERENCES Time(id)
);

CREATE TABLE `Gyroscope`
(
    `id` guid PRIMARY KEY,
    `x`  double COMMENT 'Rotation in x-axis',
    `y`  double COMMENT 'Rotation in y-axis',
    `z`  double COMMENT 'Rotation in z-axis',
    FOREIGN KEY (id) REFERENCES Time(id)
);

CREATE TABLE `Temperature`
(
    `id`            guid PRIMARY KEY,
    `oxidizer_feed` double COMMENT 'Temperature in oxidizer feed hose',
    `fuel_feed`     double COMMENT 'Temperature in oxidizer feed hose',
    `oxidizer_tank` double COMMENT 'Temperature in oxidizer tank',
    FOREIGN KEY (id) REFERENCES Time(id)
);

CREATE TABLE `Barometer`
(
    `id`          guid PRIMARY KEY,
    `pressure`    double COMMENT 'Pressure in Pa',
    `temperature` double COMMENT 'Barometer temperature',
    FOREIGN KEY (id) REFERENCES Time(id)

);

CREATE TABLE `Power`
(
    `id`        guid PRIMARY KEY,
    `bat1_amp`  double COMMENT 'Battery 1 amp usage',
    `bat1_volt` double COMMENT 'Battery 1 Volt',
    `bat2_amp`  double COMMENT 'p1 in Ampere',
    `bat2_volt` double COMMENT 'p1 in Volt',
    `bat3_amp`  double COMMENT 'p1 in Ampere',
    `bat3_volt` double COMMENT 'p1 in Volt',
    FOREIGN KEY (id) REFERENCES Time(id)

);

CREATE TABLE `Health`
(
    `id`        guid PRIMARY KEY,
    `main`      boolean COMMENT 'True if alive, false if not',
    `sensor`    boolean COMMENT 'True if alive, false if not',
    `inertia`   boolean COMMENT 'True if alive, false if not',
    `power`     boolean COMMENT 'True if alive, false if not',
    `telemetry` boolean COMMENT 'True if alive, false if not',
    FOREIGN KEY (id) REFERENCES Time(id)

);

CREATE TABLE `Gps`
(
    `id`         guid PRIMARY KEY,
    `latitude`   double,
    `longitude`  double,
    `altitude`   double,
    `satellites` int COMMENT 'Number of satellites',
    FOREIGN KEY (id) REFERENCES Time(id)
);


