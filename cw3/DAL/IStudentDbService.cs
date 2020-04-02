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

        void EnrollStudent(EnrollRequest request);
        void PromoteStudents(int semester, string studies);

        public IEnumerable<Student> GetStudents();

    }
}
