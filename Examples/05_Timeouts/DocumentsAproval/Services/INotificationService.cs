using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsAproval.Services
{
    public interface INotificationService
    {
        Task NotifyReviewEscalate(int taskId);
    }
}
