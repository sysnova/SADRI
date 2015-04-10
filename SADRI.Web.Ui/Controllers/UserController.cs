using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SADRI.Web.Ui.ViewModels;
using System.Reflection;
using SADRI.Web.Ui.Controllers.BaseController;
using SADRI.Infrastructure.Interfaces;
using SADRI.Domain.Entities.Enums;

namespace SADRI.Web.Ui.Controllers
{
    public class UserController : WizardController<RegisterViewModel, IWorkflowWizardUser, IWorkflowWizardGenericFactory>
    {
        // Inyecto la StateMachine en el Constructor del Controller.
        // Lo comento porque si no me estaría Reseteando el Estado en cada Request.

        //public UserController(IWorkflowWizardGenericFactory factory) : base (factory)
        //{

        //}

        [AllowAnonymous]
        [HttpPost]
        public override ActionResult Wizard(RegisterViewModel model, string submitNext, string submitPrev)
        {
            try
            {
                if (submitNext == "Next")
                {
                    //Le paso el Modelo a la StateMachine. 
                    //Ademas en el proceso se actualiza el TempData
                    StateMachineManager_ViewModel = model;
                    StateMachineManager_Execute(Triggers.UserWizard.Next);
                }

                if (submitPrev == "Previous")
                {
                    StateMachineManager_ViewModel = model;
                    StateMachineManager_Execute(Triggers.UserWizard.Previous);
                }

                if (submitNext == "New")
                {
                    StateMachineManager_ViewModel = model;
                    StateMachineManager_Execute(Triggers.UserWizard.New);
                    return RedirectToAction("Wizard", "User");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("CustomErrorService", e.Message.ToString());
            }
            ViewBag.PermittedTriggers = StateMachineManager_PermittedTriggers;
            return View(StateMachineManager_GetState, StateMachineManager_ViewModel);
        }
    }
}