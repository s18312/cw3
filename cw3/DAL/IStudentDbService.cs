using cw3.DTOs.Requests;
using cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DAL
{
    public interface IStudentDbService
    {

        public Enrollment EnrollStudent(EnrollRequest request);
        public Enrollment PromoteStudents(PromoteRequest request);

        public bool exists(string index);



    }
}
