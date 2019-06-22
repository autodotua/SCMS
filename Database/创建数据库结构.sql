--create database SCMS
--go

use SCMS

create table Person
(
	Id bigint primary key not null,
	Name nvarchar(30) not null,
	Password char(32) not null default('670B14728AD9902AECBA32E22FA4F6BD'),
	Role nvarchar(5) check (role in ('学生','教师','管理员')),
	StartYear smallint,
	Major nvarchar(20),
	Gender tinyint check (gender in (0,1,2)),
	Born date
)

create table Course
(
	Id int not null primary key,
	Name nvarchar(30) not null,
	Year smallint not null default(0),
	Term tinyint not null default(0),
	TeacherId bigint,
	Credit tinyint not null default(1),
	constraint fk_class_teacher foreign key(TeacherId) references person(id)
)

--create table CourseDetail
--(
--	Id int not null primary key,
--	Course int not null,
--	Time nvarchar(30) not null,
--	Location nvarchar(30) not null,
--	constraint fk_classDetail_class foreign key(course) references Course(id)
--)


create table StudentCourse
(
	Id int not null primary key identity(1,1),
	StudentId bigint not null,
	CourseId int not null,
	Score tinyint,
	constraint fk_studentClass_student foreign key(StudentId) references person(id),
	constraint fk_studentClass_class foreign key(CourseId) references Course(id)
)