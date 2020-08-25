using System;

namespace CustomerService.Services
{
    public class AssignmentService : IAssignmentService
    {
        private Random _rand = new Random();

        public int GetAppropriateUser(int questionId)
        {
            return _rand.Next(1,5);
        }
    }
}
