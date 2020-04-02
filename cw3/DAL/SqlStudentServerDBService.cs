using cw3.DTOs.Requests;
using cw3.Models;
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
        }

        


        public Enrollment PromoteStudents(PromoteRequest request)
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
        }



        public IEnumerable<Student> GetStudents()
        {
            throw new NotImplementedException();
        }
    }
}
