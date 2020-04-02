CREATE PROCEDURE PromoteStudents @Studies NVarchar(100), @Semester INT
AS
BEGIN
DECLARE @NextSms int = @Semester+1;
DECLARE @IdStudy int = (SELECT IdStudy FROM  Studies WHERE Studies.Name = @Studies);
IF Exists (SELECT IdEnrollment From Enrollment WHERE IdStudy= @IdStudy AND Semester = @NextSms)
UPDATE Student SET IdEnrollment = (SELECT IdEnrollment From Enrollment WHERE IdStudy= @IdStudy AND Semester = @NextSms) WHERE IdEnrollment = (SELECT IdEnrollment FROM Enrollment JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE Studies.Name = @Studies);
ELSE
INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES ((SELECT MAX(IdEnrollment) FROM Enrollment)+1, @NextSms, @IdStudy, GETDATE())
DECLARE @IdEnrollment int = (SELECT MAX(IdEnrollment) FROM Enrollment)
UPDATE Student SET IdEnrollment = @IdEnrollment WHERE IdEnrollment =  (SELECT IdEnrollment FROM Enrollment JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy WHERE Studies.Name = @Studies);
END
