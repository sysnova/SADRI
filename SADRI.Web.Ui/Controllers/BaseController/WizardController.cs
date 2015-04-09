using System;
using System.Collections;
using System.Web;
using System.Web.Mvc;
//
using SADRI.Domain.Entities.Enums;
using SADRI.Infrastructure.Interfaces;
//using Workflows;
using System.Reflection;
using System.Threading;
//
using Ninject.Parameters;

namespace SADRI.Web.Ui.Controllers.BaseController
{
    public abstract class WizardController<T, W> : Controller
    //T: Modelo de Datos del controller padre.
    //W: Tipo de StateMachine instanciada en el controller padre.
    // Este controller es Genérico, instancia y maneja una StateMachine. Los estados deben coincidir con las Vistas
    {

        //private W _sm = default(W);
        private T _viewModel = default(T);
        static readonly object padlock = new object();
        private W _sm;

        // Inyecto la StateMachine en el Constructor del Controller.
        // Lo comento porque si no me estaría Reseteando el Estado en cada Request.

        //public WizardController(W _smInject)
        //{
        //    _sm = _smInject;
        //}
        [AllowAnonymous]
        [HttpGet]
        // GET: Base
        public ActionResult Wizard()
        {
            return View(StepInit);
        }
        //public abstract ActionResult GetIndex();

        [HttpPost]
        public abstract ActionResult Wizard(T model, string submitNext, string submitPrev);

        private string StepInit
        {
            get
            {
                //Ejemplo por Reflection para cuando no tenemos IoC
                //IParameter parameter = new ConstructorArgument("_step", States.UserWizard.Step2);
                //_sm = (W)Activator.CreateInstance(typeof(W), new object[] { States.UserWizard.Init });
                
                //Levanta la implementacion de una interface(W) por Ninject, que me crea la Instancia definida en el module.
                W _sm = (W)DependencyResolver.Current.GetService(typeof(W));// (parameter);

                //Guardo la SM instanciada por Reflection en TempData para proximas invocaciones
                StateMachineManager = _sm;
                
                //Borro el modelo de datos ya que estamos haciendo una nueva instancia.
                TempData["Model"] = null;
                ViewBag.PermittedTriggers = StateMachineManager_PermittedTriggers;
                
                return StateMachineManager_GetState;
            }
        }

        public T StateMachineManager_ViewModel
        {
            get
            {
                return _viewModel;
            }
            set
            {
                _viewModel = value;
            }
        }

        private T TransferModeltoDataTemp(T model)
        //Persiste el Model actual al Temp Model para mantener el modelo durante la navegacion del WF
        {
            if (TempData["Model"] == null)
            {
                TempData["Model"] = model;
                return model;
            }
            else
            {
                T BakModel = (T)TempData["Model"];

                // Bindeo genérico por reflection -> Model to TempData
                // Creo el BakModel porque el TempData una vez leido, se borran los datos.
                foreach (var propertyInfo in model.GetType()
                .GetProperties(
                        BindingFlags.Public
                        | BindingFlags.Instance))
                {
                    //Hago el Set de un Objeto a Otro por Reflection. Actualizo la Info en TempData si el Modelo tiene nuevo datos del Step
                    if (propertyInfo.GetValue(model) != null)
                    {
                        PropertyInfo BakpropertyInfo = BakModel.GetType().GetProperty(propertyInfo.Name);
                        BakpropertyInfo.SetValue(BakModel, Convert.ChangeType(propertyInfo.GetValue(model), propertyInfo.PropertyType), null);
                    }
                }

                // Guardo el Modelo Actualizado en TempData.
                TempData["Model"] = BakModel;
                StateMachineManager_ViewModel = BakModel;

                return BakModel;
            }

        }

        private W StateMachineManager
        {
            get
            {
                lock (padlock)
                {
                    if (_sm == null)
                    {
                        _sm = (W)TempData["SM"];
                    }
                    return _sm;
                }
            }
            set
            {
                _sm = value;
                TempData["SM"] = value;
            }
        }

        public string StateMachineManager_GetState
        {
            get
            {
                //Por Reflection. Toma una Interfaz y busca el Metodo GetState.
                //return StateMachineManager.GetType().GetProperty("GetState").GetValue(StateMachineManager).ToString();
                
                //Por Interfaz Genérica
                IWorkflowWizardGeneric _smGet = (IWorkflowWizardGeneric)StateMachineManager;
                return _smGet.GetState.ToString() ;
            }
        }

        public IEnumerable StateMachineManager_PermittedTriggers
        {
            get
            {
                //Por Reflection
                //TempData["PermittedTriggers"] = StateMachineManager.GetType().GetProperty("PermittedTriggers").GetValue(StateMachineManager);

                IWorkflowWizardGeneric _smPermittedTriggers = (IWorkflowWizardGeneric)StateMachineManager;
                TempData["PermittedTriggers"] = _smPermittedTriggers.PermittedTriggers;

                return (IEnumerable)_smPermittedTriggers.PermittedTriggers;
            }
        }

        public void StateMachineManager_Execute(Triggers.UserWizard Action)
        {

            #region Documentacion Reflection. Crear instancias
            //Documentacion Reflection. Crear instancias

            //MethodInfo method = typeof(W).GetMethod("TryFireTriggerParam");
            //Type typeEnum = typeof(WizardUser.Trigger);
            //Type typeHash = typeof(System.Collections.Hashtable);
            //MethodInfo generic = method.MakeGenericMethod(typeEnum, typeHash);
            //generic.Invoke(sm, new object[] { WizardUser.Trigger.Next, arg });

            //Type unboundGenericType = typeof(W);
            //Type typeEnum = typeof(WizardUser.Trigger);
            //Type typeHash = typeof(System.Collections.Hashtable);

            //Type boundGenericType = unboundGenericType.MakeGenericType(typeEnum, typeHash);
            //object instance = Activator.CreateInstance(boundGenericType);

            #endregion
            //Preparo HashMap para invocar el Trigger con Parametros
            //Transfiero el Modelo al DataTemp["Model"] para actualizar los datos en el Temp y asi mantener los datos del flujo
            //W smTemp = SM;
            //Se tiene que hacer por Reflection porque la StateMachine puede ser de cualquier tipo.
            try
            {
                Hashtable arg = StateMachineManager_SetArgument(TransferModeltoDataTemp(StateMachineManager_ViewModel));
                //Ejecucion por Reflection
                //MethodInfo TryFireTriggerParamMethod = StateMachineManager.GetType().GetMethod("TryFireTriggerParam");
                //TryFireTriggerParamMethod.Invoke(StateMachineManager, new object[] { Action, arg }); //Action viene por Parametro, y Arg(Argumentos del Trigger) se genera StateMachineManager_SetArgument
               
                //Ejecucion por Ninject
                IWorkflowWizardGeneric _sm = (IWorkflowWizardGeneric) StateMachineManager;
                _sm.TryFireTriggerParam(Action, arg);
                
            }
            catch (Exception e)
            {
                    throw (e.InnerException != null) ? e.InnerException : e;
            }
            finally
            {
                //Como fue invocado por Reflection no invoca al Set del StateMachineManager, por lo tanto se pierde el TempData["SM"]
                ClearModelErrors(StateMachineManager_ViewModel);
                
                StateMachineManager = _sm;
            }
            //return sm;
        }
        public Hashtable StateMachineManager_SetArgument(T model) //Toma del modelo las prop publicas para armar el Hashmap con los argumentos.
        {
            //Preparo HashMap para invocar Transicion con Parametros
            Hashtable arg = new Hashtable();
            //Recorro objecto generico. Reflection
            foreach (var propertyInfo in model.GetType()
                        .GetProperties(
                                BindingFlags.Public
                                | BindingFlags.Instance))
            {
                arg[propertyInfo.Name] = propertyInfo.GetValue(model);
            }
            return arg;
        }

        private void ClearModelErrors(T model)
        {

            foreach (var propertyInfo in model.GetType()
            .GetProperties(
                    BindingFlags.Public
                    | BindingFlags.Instance))
            {
                if (ModelState.ContainsKey(propertyInfo.Name))
                    ModelState[propertyInfo.Name].Errors.Clear();
            }

        }

    }
}