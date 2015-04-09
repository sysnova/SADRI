using System.Collections.Generic;
using SADRI.Domain.Entities.Enums;

namespace SADRI.Infrastructure.Interfaces
{
    public interface IWorkflowWizardGeneric
    {
        States.UserWizard GetState { get; }
        System.Collections.IEnumerable PermittedTriggers { get; }
        bool TryFireTriggerParam(Triggers.UserWizard trigger, System.Collections.Hashtable arg);
    }
}
