using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IGradeService
    {
        List<int> GetGrades(string studentId);
        int GetGradeById(string studentId, int index);
        void AssignGrade(string studentId, int grade);
        void DeleteGrade(string studentId, int index);
        void UpdateGrade(string studentId, int index, int newGrade);
    }
}
