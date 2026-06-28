use EduManageDB

INSERT INTO Teachers
(FullName, Subject, AssignedClass, AssignedSection, Contact, Username, Password, Role)
VALUES
('Ayesha Malik','Chemistry','Class 9','A','555-2001','ayesham','Pwd@123','Teacher'),
('Hassan Raza','Biology','Class 9','B','555-2002','hassanr','Pwd@123','Teacher'),
('Fatima Noor','Computer Science','Class 11','A','555-2003','fatiman','Pwd@123','Teacher'),
('Usman Tariq','Urdu','Class 11','B','555-2004','usmant','Pwd@123','Teacher'),
('Bilal Ahmed','Pakistan Studies','Class 10','B','555-2005','bilala','Pwd@123','Teacher'),
('Zainab Khan','Islamiat','Class 11','A','555-2006','zainabk','Pwd@123','Teacher');


INSERT INTO Students
(FullName,FatherName,DateOfBirth,Gender,Class,Section,RollNumber,Contact,Address,Username,Password,Attendance,OverallGrade)
VALUES
('Ali Khan','Imran Khan','2010-01-12','Male','Class 9','A','STU004','03001111111','Karachi','alikhan','pwd004',91,'A'),
('Sara Ahmed','Naveed Ahmed','2010-02-14','Female','Class 9','A','STU005','03001111112','Karachi','saraahmed','pwd005',95,'A+'),
('Hamza Sheikh','Rashid Sheikh','2010-03-01','Male','Class 9','B','STU006','03001111113','Karachi','hamzas','pwd006',84,'B+'),
('Areeba Malik','Javed Malik','2010-04-10','Female','Class 9','A','STU007','03001111114','Karachi','areeba','pwd007',89,'A'),
('Talha Noor','Akram Noor','2010-05-15','Male','Class 9','B','STU008','03001111115','Karachi','talhan','pwd008',80,'B'),
('Maham Ali','Asif Ali','2010-06-11','Female','Class 9','A','STU009','03001111116','Karachi','mahama','pwd009',93,'A'),
('Huzaifa Qureshi','Bilal Qureshi','2010-07-19','Male','Class 9','B','STU010','03001111117','Karachi','huzaifa','pwd010',88,'B+'),
('Iqra Khan','Waseem Khan','2010-08-09','Female','Class 9','A','STU011','03001111118','Karachi','iqrak','pwd011',97,'A+'),
('Abdullah Raza','Raza Ali','2010-09-02','Male','Class 9','B','STU012','03001111119','Karachi','abdullahr','pwd012',82,'B'),
('Hira Saleem','Saleem Ahmed','2010-10-28','Female','Class 9','A','STU013','03001111120','Karachi','hiras','pwd013',94,'A');


INSERT INTO Students
(FullName,FatherName,DateOfBirth,Gender,Class,Section,RollNumber,Contact,Address,Username,Password,Attendance,OverallGrade)
VALUES
('Ahmed Farooq','Farooq Ahmed','2009-01-15','Male','Class 10','A','STU014','03002222111','Karachi','ahmedf','pwd014',92,'A'),
('Laiba Khan','Sajid Khan','2009-03-05','Female','Class 10','A','STU015','03002222112','Karachi','laibak','pwd015',96,'A+'),
('Daniyal Malik','Tariq Malik','2009-04-21','Male','Class 10','B','STU016','03002222113','Karachi','daniyalm','pwd016',85,'B+'),
('Minal Noor','Noor Ahmed','2009-05-08','Female','Class 10','A','STU017','03002222114','Karachi','minaln','pwd017',90,'A'),
('Saad Khan','Junaid Khan','2009-06-11','Male','Class 10','B','STU018','03002222115','Karachi','saadk','pwd018',81,'B'),
('Eman Ali','Noman Ali','2009-07-17','Female','Class 10','A','STU019','03002222116','Karachi','emana','pwd019',95,'A+'),
('Yasir Sheikh','Shahid Sheikh','2009-08-01','Male','Class 10','B','STU020','03002222117','Karachi','yasirs','pwd020',87,'B+'),
('Anaya Raza','Raza Ahmed','2009-09-09','Female','Class 10','A','STU021','03002222118','Karachi','anayar','pwd021',93,'A'),
('Haris Malik','Imran Malik','2009-10-10','Male','Class 10','B','STU022','03002222119','Karachi','harism','pwd022',84,'B'),
('Zoya Khan','Waqas Khan','2009-11-20','Female','Class 10','A','STU023','03002222120','Karachi','zoyak','pwd023',97,'A+');

INSERT INTO Students
(FullName,FatherName,DateOfBirth,Gender,Class,Section,RollNumber,Contact,Address,Username,Password,Attendance,OverallGrade)
VALUES
('Taha Ahmed','Ahmed Raza','2008-01-02','Male','Class 11','A','STU024','03003333111','Karachi','tahaa','pwd024',94,'A'),
('Muneeba Khan','Sohail Khan','2008-02-10','Female','Class 11','A','STU025','03003333112','Karachi','muneebak','pwd025',96,'A+'),
('Owais Malik','Khalid Malik','2008-03-08','Male','Class 11','B','STU026','03003333113','Karachi','owaism','pwd026',86,'B+'),
('Aiman Noor','Noor Alam','2008-04-04','Female','Class 11','A','STU027','03003333114','Karachi','aimann','pwd027',92,'A'),
('Shayan Ali','Rizwan Ali','2008-05-05','Male','Class 11','B','STU028','03003333115','Karachi','shayana','pwd028',83,'B'),
('Kiran Ahmed','Ahmed Saleem','2008-06-06','Female','Class 11','A','STU029','03003333116','Karachi','kirana','pwd029',95,'A+'),
('Fahad Khan','Kashif Khan','2008-07-07','Male','Class 11','B','STU030','03003333117','Karachi','fahadk','pwd030',88,'B+'),
('Noor Fatima','Fatima Zahra','2008-08-08','Female','Class 11','A','STU031','03003333118','Karachi','noorf','pwd031',97,'A+'),
('Ahsan Raza','Raza Karim','2008-09-09','Male','Class 11','B','STU032','03003333119','Karachi','ahsanr','pwd032',85,'B'),
('Rabia Khan','Younis Khan','2008-10-10','Female','Class 11','A','STU033','03003333120','Karachi','rabiak','pwd033',93,'A');



INSERT INTO Assignments
(Title, Description, DueDate, TeacherId, Class, FilePath, CreatedAt)
VALUES
('Linear Equations Worksheet','Solve all questions from Chapter 4',DATEADD(day,7,GETDATE()),1,'Class 10',NULL,GETDATE()),
('Physics Motion Report','Prepare a report on Laws of Motion',DATEADD(day,10,GETDATE()),2,'Class 10',NULL,GETDATE()),
('Essay Writing Task','Write a 500-word essay on Technology',DATEADD(day,6,GETDATE()),3,'Class 11',NULL,GETDATE()),
('Chemistry Lab Activity','Complete balancing chemical equations',DATEADD(day,8,GETDATE()),4,'Class 9',NULL,GETDATE()),
('Computer Programming Basics','Write a C# console application',DATEADD(day,12,GETDATE()),6,'Class 11',NULL,GETDATE()),
('Pakistan Studies Project','Prepare presentation on Quaid-e-Azam',DATEADD(day,14,GETDATE()),8,'Class 10',NULL,GETDATE());

INSERT INTO AssignmentSubmissions
(AssignmentId,StudentId,SubmissionDate,FilePath,Grade,Feedback,Status)
VALUES
(1,4,GETDATE(),'submissions/stu004.pdf','A','Excellent work','Graded'),
(1,5,GETDATE(),'submissions/stu005.pdf','A+','Outstanding','Graded'),
(2,14,GETDATE(),'submissions/stu014.pdf',NULL,NULL,'Submitted'),
(3,24,GETDATE(),'submissions/stu024.pdf','B+','Good effort','Graded'),
(4,6,GETDATE(),'submissions/stu006.pdf',NULL,NULL,'Submitted'),
(5,25,GETDATE(),'submissions/stu025.pdf','A','Well structured code','Graded'),
(6,15,GETDATE(),'submissions/stu015.pdf',NULL,NULL,'Submitted');





INSERT INTO Attendances
(StudentId,Date,Status,Remarks,Subject)
VALUES
(4,GETDATE(),'Present','On Time','Chemistry'),
(5,GETDATE(),'Present','On Time','Chemistry'),
(6,GETDATE(),'Absent','Medical Leave','Biology'),
(7,GETDATE(),'Present','On Time','Biology'),
(8,GETDATE(),'Present','On Time','Chemistry'),
(14,GETDATE(),'Present','On Time','Mathematics'),
(15,GETDATE(),'Present','On Time','Mathematics'),
(16,GETDATE(),'Absent','Family Emergency','Physics'),
(17,GETDATE(),'Present','On Time','Physics'),
(18,GETDATE(),'Present','On Time','Mathematics'),
(24,GETDATE(),'Present','On Time','English'),
(25,GETDATE(),'Present','On Time','Computer Science'),
(26,GETDATE(),'Present','On Time','Urdu'),
(27,GETDATE(),'Absent','Medical Leave','Computer Science'),
(28,GETDATE(),'Present','On Time','Urdu');


INSERT INTO Exams
(Name,Date,Class,Subject,TotalMarks)
VALUES
('Class 9 Midterm Chemistry',DATEADD(day,-20,GETDATE()),'Class 9','Chemistry',100),
('Class 9 Biology Quiz',DATEADD(day,-15,GETDATE()),'Class 9','Biology',50),
('Class 10 Mathematics Midterm',DATEADD(day,-18,GETDATE()),'Class 10','Mathematics',100),
('Class 10 Physics Quiz',DATEADD(day,-12,GETDATE()),'Class 10','Physics',50),
('Class 11 English Midterm',DATEADD(day,-14,GETDATE()),'Class 11','English',100),
('Class 11 Computer Science Quiz',DATEADD(day,-10,GETDATE()),'Class 11','Computer Science',50);



INSERT INTO Results
(ExamId,StudentId,ObtainedMarks,Grade,Remarks,IsPublished)
VALUES
(3,14,91,'A+','Excellent',1),
(3,15,95,'A+','Outstanding',1),
(3,16,87,'B','Good',1),
(3,17,88,'A','Very Good',1),
(3,18,71,'B','Satisfactory',1),

(1,4,45,'A','Good Performance',1),
(1,5,30,'B-','Excellent',1),
(1,6,36,'B','Needs Improvement',1),
(1,7,44,'A','Very Good',1),
(1,8,48,'A+','Good Effort',1),

(5,24,90,'A+','Excellent Writing',1),
(5,25,94,'A+','Outstanding',1),
(5,26,81,'A','Very Good',1),
(5,27,87,'A','Good Work',1),
(5,28,75,'B','Needs Improvement',1);





INSERT INTO Announcements
(Title,Content,TargetClass,AuthorName,AuthorRole,CreatedAt,ExpiryDate)
VALUES
('Sports Week','Annual sports week begins next month.','All','Principal','Admin',GETDATE(),DATEADD(day,30,GETDATE())),
('Class 9 Test Schedule','Monthly test schedule uploaded.','Class 9','Ayesha Malik','Teacher',GETDATE(),DATEADD(day,15,GETDATE())),
('Class 10 Mathematics Quiz','Quiz will be held on Friday.','Class 10','Sarah Connor','Teacher',GETDATE(),DATEADD(day,7,GETDATE())),
('Class 11 Project Submission','Submit projects before deadline.','Class 11','Fatima Noor','Teacher',GETDATE(),DATEADD(day,20,GETDATE())),
('Library Week','Students are encouraged to borrow books.','All','Admin Office','Admin',GETDATE(),DATEADD(day,25,GETDATE()));

INSERT INTO Events
(Title,Description,StartDate,EndDate,Type)
VALUES
('Debate Competition','Inter-class debate contest',DATEADD(day,10,GETDATE()),DATEADD(day,10,GETDATE()),'Competition'),
('Science Exhibition','Display innovative projects',DATEADD(day,20,GETDATE()),DATEADD(day,21,GETDATE()),'Event'),
('Sports Gala','Annual sports activities',DATEADD(day,30,GETDATE()),DATEADD(day,32,GETDATE()),'Sports'),
('Independence Day','National celebration event',DATEADD(day,45,GETDATE()),DATEADD(day,45,GETDATE()),'Holiday'),
('Career Counseling Session','Guidance for senior students',DATEADD(day,15,GETDATE()),DATEADD(day,15,GETDATE()),'Seminar');


INSERT INTO Messages
(SenderId,SenderRole,SenderName,ReceiverId,ReceiverRole,ReceiverName,Subject,Body,SentAt,IsRead)
VALUES
(14,'Student','Ahmed Farooq',1,'Teacher','Sarah Connor','Quiz Query','Need clarification about chapter 5.',GETDATE(),0),
(1,'Teacher','Sarah Connor',14,'Student','Ahmed Farooq','RE Quiz Query','Please review examples 3 and 4.',GETDATE(),1),
(24,'Student','Taha Ahmed',3,'Teacher','Clark Kent','Essay Topic','Can I choose a custom topic?',GETDATE(),0),
(3,'Teacher','Clark Kent',24,'Student','Taha Ahmed','RE Essay Topic','Yes, with approval.',GETDATE(),1),
(5,'Student','Sara Ahmed',4,'Teacher','Ayesha Malik','Lab Work','Need help with experiment.',GETDATE(),0);

INSERT INTO LeaveRequests
(RequesterId,RequesterRole,RequesterName,StartDate,EndDate,Reason,Status,RequestedAt)
VALUES
(6,'Student','Hamza Sheikh',DATEADD(day,2,GETDATE()),DATEADD(day,3,GETDATE()),'Medical Appointment','Approved',GETDATE()),
(16,'Student','Daniyal Malik',DATEADD(day,5,GETDATE()),DATEADD(day,6,GETDATE()),'Family Function','Pending',GETDATE()),
(25,'Student','Muneeba Khan',DATEADD(day,1,GETDATE()),DATEADD(day,2,GETDATE()),'Health Issue','Approved',GETDATE()),
(4,'Teacher','Ayesha Malik',DATEADD(day,7,GETDATE()),DATEADD(day,8,GETDATE()),'Personal Work','Pending',GETDATE()),
(6,'Teacher','Fatima Noor',DATEADD(day,10,GETDATE()),DATEADD(day,12,GETDATE()),'Conference Attendance','Approved',GETDATE());




INSERT INTO Remarks
(StudentId,TeacherName,RemarkType,RemarkText,CreatedAt)
VALUES
(4,'Ayesha Malik','Academic','Excellent chemistry performance.',GETDATE()),
(5,'Ayesha Malik','Academic','Outstanding participation.',GETDATE()),
(14,'Sarah Connor','Academic','Strong mathematical concepts.',GETDATE()),
(16,'Bruce Wayne','Behavioral','Needs more focus in class.',GETDATE()),
(24,'Clark Kent','Academic','Excellent writing skills.',GETDATE()),
(27,'Fatima Noor','Academic','Shows strong programming aptitude.',GETDATE());

INSERT INTO StudyMaterials
(Title,Description,FilePath,Class,TeacherId,UploadedAt)
VALUES
('Chemistry Notes Chapter 1','Introduction to Chemistry','files/chem_ch1.pdf','Class 9',4,GETDATE()),
('Biology Notes','Cell Structure','files/bio_cells.pdf','Class 9',5,GETDATE()),
('Mathematics Formula Sheet','Important formulas','files/math_formula.pdf','Class 10',1,GETDATE()),
('Physics Notes','Motion and Force','files/physics_motion.pdf','Class 10',2,GETDATE()),
('English Essay Guide','Essay writing techniques','files/essay_guide.pdf','Class 11',3,GETDATE()),
('C# Programming Basics','Introduction to C#','files/csharp_basics.pdf','Class 11',6,GETDATE());



INSERT INTO UpdateRequests
(StudentId,FieldName,CurrentValue,NewValue,Reason,RequestedAt)
VALUES
(4,'Contact','03001111111','03005555555','New phone number',GETDATE()),
(15,'Address','Karachi','Lahore','Family relocation',GETDATE()),
(24,'Contact','03003333111','03006666666','Updated mobile number',GETDATE()),
(27,'Address','Karachi','Islamabad','Residence changed',GETDATE()),
(31,'Contact','03003333118','03007777777','New SIM number',GETDATE());



