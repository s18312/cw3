using cw3.DTOs.Requests;
using cw3.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DAL
{
    public class SqlStudentServerDBService : IStudentDbService
    {




        public Enrollment EnrollStudent(EnrollRequest request)
        {
            using (var db = new StudentDbContext())
            {
                var enrollment = new Enrollment();
                
                //sprawdzenie czy studia istnieją
                if (!db.Studies.Any(studies => studies.Name == request.Studies))
                {
                    throw new Exception("Studia nie istnieją");
                }

                var studies = db.Studies.Where(studies => studies.Name == request.Studies).First();
                enrollment.Semester = 1;
                enrollment.StartDate = DateTime.Parse(DateTime.Now.ToShortTimeString());
                enrollment.IdStudy = studies.IdStudy;

                //Sprawdzenie czy istnieje enrollment
                if (db.Enrollment.Any(e => e.IdStudy == studies.IdStudy && e.Semester == 1 && e.StartDate == db.Enrollment.Max(en => en.StartDate)))
                {
                    var enroll = db.Enrollment.Where(e => e.IdStudy == studies.IdStudy && e.Semester == 1 && e.StartDate == db.Enrollment.Max(en => en.StartDate)).Single();
                    db.Student.Add(new Student { IndexNumber = request.IndexNumber, FirstName = request.FirstName, LastName = request.LastName, Birthdate = request.Birthdate, IdEnrollment = enroll.IdEnrollment });
                    enrollment.IdEnrollment = enroll.IdEnrollment;
                    db.SaveChanges();
                }
                else
                {
                    //sprawdzam czy istnieje student
                    if (!db.Student.Any(st => st.IndexNumber == request.IndexNumber))
                    {
                        throw new Exception("Student już istnieje");
                    }
                    else
                    {
                        var enrollments = db.Enrollment.Max(emp => emp.IdEnrollment);
                        Enrollment nowy = new Enrollment { IdEnrollment = enrollments + 1, Semester = 1, IdStudy = studies.IdStudy, StartDate = DateTime.Now };
                        enrollment.IdEnrollment = nowy.IdEnrollment;

                        db.Enrollment.Add(nowy);
                        db.Student.Add(new Student { IndexNumber = request.IndexNumber, FirstName = request.FirstName, LastName = request.LastName, Birthdate = request.Birthdate, IdEnrollment = nowy.IdEnrollment });
                        db.SaveChanges();
                    }
                }
                return enrollment;
            }
        }

        /*public Enrollment EnrollStudent(EnrollRequest request)
        {


            var st = new Student();
            st.IndexNumber = request.IndexNumber;
            st.FirstName = request.FirstName;
            st.LastName = request.LastName;
            st.Birthdate = request.Birthdate;
            st.Studies = request.Studies;
            Enrollment en = new Enrollment();



            using (SqlConnection con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18312;Integrated Security=True"))
            {


                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    con.Open();
                    var tran = con.BeginTransaction();
                    com.Transaction = tran;

                    try
                    {


                        //Sprawdzenie czy istnieją studia
                        com.CommandText = "SELECT IdStudy from Studies WHERE name=@name";
                        com.Parameters.AddWithValue("name", request.Studies);
                        var dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            tran.Rollback();
                            return null;
                        }


                        //Sprawdzenie czy istnieje enrollment
                        int idStudies = (int)dr["IdStudy"];

                        dr.Close();
                        com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudies AND Semester = 1 AND StartDate = (SELECT MAX(StartDate) FROM Enrollment)";
                        com.Parameters.AddWithValue("IdStudies", idStudies);
                        dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            com.CommandText = "SELECT MAX(IdEnrollment) FROM Enrollment";
                            dr = com.ExecuteReader();
                            Enrollment en1 = new Enrollment();
                            en1.IdEnrollment = (int)dr["IdEnrollment"] + 1;
                            dr.Close();
                            en1.IdStudy = idStudies;
                            en1.Semester = 1;
                            en1.StartDate = DateTime.Now;
                            st.IdEnrollment = en1.IdEnrollment;
                            en = en1;
                            dr.Close();
                        }
                        st.IdEnrollment = (int)dr["IdEnrollment"];
                        en.IdEnrollment = st.IdEnrollment;
                        en.IdStudy = idStudies;
                        en.Semester = 1;
                        en.StartDate = DateTime.Now;
                        dr.Close();

                        //Dodanie Studenta
                        com.CommandText = "SELECT IndexNumber from Student WHERE IndexNumber=@IndexNumber";
                        com.Parameters.AddWithValue("IndexNumber", request.IndexNumber);
                        dr = com.ExecuteReader();
                        if (!dr.Read())
                        {
                            dr.Close();
                            com.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) VALUES (@Index, @Fname, @Lname, @Bdate, @IdE)";
                            com.Parameters.AddWithValue("Index", st.IndexNumber);
                            com.Parameters.AddWithValue("Fname", st.FirstName);
                            com.Parameters.AddWithValue("Lname", st.LastName);
                            com.Parameters.AddWithValue("Bdate", st.Birthdate);
                            com.Parameters.AddWithValue("IdE", st.IdEnrollment);
                            com.ExecuteNonQuery();
                            tran.Commit();
                        }
                        else
                        {
                            dr.Close();
                            tran.Rollback();
                            return null;
                        }
                        dr.Close();
                        return  en;
                    }
                    catch (SqlException e)
                    {
                        Console.WriteLine(e.Message);
                        tran.Rollback();
                    }
                }
            }
            return null;
        }*/



        public Enrollment PromoteStudents(PromoteRequest request)
        {
            using (var db = new StudentDbContext())
            {
                if (!db.Studies.Any(studies => studies.Name == request.Studies))
                {
                    throw new Exception("Studia nie istnieją");
                }
                else
                {
                    db.Database.ExecuteSqlRaw("exec Procedura @Studies, @Semester", request.Studies, request.Semester);
                    

                    var studia = db.Studies.Where(studies => studies.Name == request.Studies).FirstOrDefault();
                    var enrollment = db.Enrollment.Where(enroll => enroll.IdStudy == studia.IdStudy && enroll.Semester == request.Semester + 1).FirstOrDefault();

                    return enrollment;
                }
            }
        }


            /*public Enrollment PromoteStudents(PromoteRequest request)
            {
                Enrollment en = new Enrollment();
                using (SqlConnection con = new SqlConnection("Data Source = db - mssql; Initial Catalog = s18312; Integrated Security = True"))
                {

                    using (SqlCommand com = new SqlCommand())
                    {
                        com.Connection = con;
                        con.Open();
                        var tran = con.BeginTransaction();
                        com.Transaction = tran;

                        try
                        {
                            com.CommandText = "SELECT IdStudy from Studies WHERE name=@name";
                            com.Parameters.AddWithValue("name", request.Studies);
                            var dr = com.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                tran.Rollback();
                                return null;
                            }
                            int idStudies = (int)dr["IdStudy"];
                            en.IdStudy = idStudies;
                            dr.Close();
                            com.CommandText = "SELECT Semester FROM Enrollment WHERE IdStudy = @IdStudies ";
                            com.Parameters.AddWithValue("IdStudies", idStudies);
                            dr = com.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                tran.Rollback();
                                return null;
                            }
                            dr.Close();
                            com.CommandText = "exec Procedura @Studies, @Semester";
                            com.Parameters.AddWithValue("Studies", "Informatyka");
                            com.Parameters.AddWithValue("Semester", request.Semester);
                            en.Semester = request.Semester + 1;
                            dr = com.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                tran.Rollback();
                                return null;
                            }
                            else
                            {
                                dr.Close();
                                com.BeginExecuteNonQuery();
                                tran.Commit();
                            }

                            com.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE IdStudy = @IdStudies AND Semester = 2";
                            com.Parameters.AddWithValue("IdStudies", idStudies);
                            dr = com.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                return null;
                            }
                            en.IdEnrollment = (int)dr["IdEnrollment"];
                            dr.Close();

                            com.CommandText = "SELECT StartDate FROM Enrollment WHERE IdStudy = @IdStudies AND Semester = 2";
                            com.Parameters.AddWithValue("IdStudies", idStudies);
                            dr = com.ExecuteReader();
                            if (!dr.Read())
                            {
                                dr.Close();
                                return null;
                            }
                            en.StartDate = (DateTime)dr["StartDate"];

                        }
                        catch (SqlException e)
                        {
                            Console.WriteLine(e.Message);
                            tran.Rollback();
                        }


                    }
                }


                return  en;
            }*/



            public bool exists(string Index)
        {
            using (SqlConnection con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s18312;Integrated Security=True"))
            {
                using (SqlCommand com = new SqlCommand())
                {
                    com.Connection = con;
                    com.CommandText = "select IndexNumber from Student WHERE IndexNumber=@index";
                    com.Parameters.AddWithValue("index", Index);

                    con.Open();
                    SqlDataReader dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        return false;
                    }

                    return true;
                }
            }
        }
    }
}
