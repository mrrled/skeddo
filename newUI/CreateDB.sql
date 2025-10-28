CREATE TABLE IF NOT EXISTS ScheduleGroups
(
    Id INTEGER PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS Schedules
(
    Id      INTEGER PRIMARY KEY,
    GroupId INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Teachers
(
    Id          INTEGER PRIMARY KEY,
    FullName    TEXT    NOT NULL,
    Description TEXT,
    GroupId     INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Lessons
(
    Id            INTEGER PRIMARY KEY,
    SchoolSubject TEXT,
    StudyGroup    TEXT,
    LessonNumber  INTEGER,
    Classroom     TEXT,
    TeacherId     INTEGER,
    ScheduleId    INTEGER NOT NULL,

    FOREIGN KEY (ScheduleId) REFERENCES Schedules (Id) ON DELETE CASCADE,
    FOREIGN KEY (TeacherId) REFERENCES Teachers (Id)
);

CREATE TABLE IF NOT EXISTS Classrooms
(
    Name    TEXT    NOT NULL,
    GroupId INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS SchoolSubjects
(
    Name    TEXT    NOT NULL,
    GroupId INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS StudyGroups
(
    Name    TEXT    NOT NULL,
    GroupId INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS TimeSlots
(
    Time         TEXT    NOT NULL,
    LessonNumber INTEGER NOT NULL,
    GroupId      INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS SubjectTeacher
(
    SchoolSubject TEXT    NOT NULL,
    TeacherId     INTEGER NOT NULL,
    GroupId       INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE,
    FOREIGN KEY (TeacherId) REFERENCES Teachers (Id) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS StudyGroupTeacher
(
    TeacherId  INTEGER NOT NULL,
    StudyGroup TEXT    NOT NULL,
    GroupId    INTEGER NOT NULL,

    FOREIGN KEY (GroupId) REFERENCES ScheduleGroups (Id) ON DELETE CASCADE,
    FOREIGN KEY (TeacherId) REFERENCES Teachers (Id) ON DELETE CASCADE
);