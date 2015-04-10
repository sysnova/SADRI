using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SADRI.Domain.Entities.Enums;

namespace SADRI.Infrastructure.Interfaces
{
    public interface IWorkflowWizardGenericFactory
    {
        IWorkflowWizardGeneric CreateWorkflow(string _metaState);
    }
}
