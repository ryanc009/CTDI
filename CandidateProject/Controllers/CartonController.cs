using CandidateProject.EntityModels;
using CandidateProject.ViewModels;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Configuration;
using System;
using System.Collections.Generic;
using log4net;

namespace CandidateProject.Controllers
{
    public class CartonController : Controller
    {

        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CartonContext db = new CartonContext();

        // GET: Carton
        public ActionResult Index()
        {
            try
            {
                var cartons = db.Cartons
                    .Select(c => new CartonViewModel()
                    {
                        Id = c.Id,
                        CartonNumber = c.CartonNumber
                    })
                    .ToList();

                //per customer enhancement #1 display # of pieces of equipment in carton
                int itemsInCarton = 0;
                foreach (var carton in cartons)
                {
                    int.TryParse(db.CartonDetails.Where(cd => cd.CartonId == carton.Id).ToList().Count.ToString(), out itemsInCarton);
                    carton.ItemCount = itemsInCarton;
                }

                //set viewbag for max count here so if there's an error the exception handler will trap it and 
                //  takes away logic from view to read config file
                if (ConfigurationManager.AppSettings["MaxItemsPerCarton"] != null)
                    ViewBag.MaxEquipCount = ConfigurationManager.AppSettings["MaxItemsPerCarton"].ToString();
                else
                    ViewBag.MaxEquipCount = 10;

                return View(cartons);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // GET: Carton/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("BadRequest", "Error");
                }
                var carton = db.Cartons
                    .Where(c => c.Id == id)
                    .Select(c => new CartonViewModel()
                    {
                        Id = c.Id,
                        CartonNumber = c.CartonNumber
                    })
                    .SingleOrDefault();
                if (carton == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                //per customer enhancement #1 display # of pieces of equipment in carton displaying here in addition to index for consistency
                int itemsInCarton = 0;
                int.TryParse(db.CartonDetails.Where(cd => cd.CartonId == carton.Id).ToList().Count.ToString(), out itemsInCarton);
                carton.ItemCount = itemsInCarton;

                //set viewbag for max count here so if there's an error the exception handler will trap it and 
                //  takes away logic from view to read config file
                if (ConfigurationManager.AppSettings["MaxItemsPerCarton"] != null)
                    ViewBag.MaxEquipCount = ConfigurationManager.AppSettings["MaxItemsPerCarton"].ToString();
                else
                    ViewBag.MaxEquipCount = 10;

                return View(carton);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // GET: Carton/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Carton/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CartonNumber")] Carton carton)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //existing issue allowed cartons to be named the same, assuming each carton name/number should be unique
                    //  added check for existing carton with same carton number
                    var existingCarton = db.Cartons.Where(c => c.CartonNumber == carton.CartonNumber).Select(c => c.Id).FirstOrDefault();

                    if (existingCarton <= 0)
                    {
                        db.Cartons.Add(carton);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CartonNumber", "A carton with that name exists, please enter another name");
                        return View();
                    }
                }

                return View(carton);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // GET: Carton/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("BadRequest", "Error");
                }
                var carton = db.Cartons
                    .Where(c => c.Id == id)
                    .Select(c => new CartonViewModel()
                    {
                        Id = c.Id,
                        CartonNumber = c.CartonNumber
                    })
                    .SingleOrDefault();
                if (carton == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }
                return View(carton);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // POST: Carton/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CartonNumber")] CartonViewModel cartonViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //existing issue allowed cartons to be named the same, assuming each carton name/number should be unique
                    //  added check for existing carton with same carton number
                    var existingCarton = db.Cartons.Where(c => c.CartonNumber == cartonViewModel.CartonNumber).Select(c => c.Id).FirstOrDefault();
                    if (existingCarton <= 0)
                    {
                        var carton = db.Cartons.Find(cartonViewModel.Id);
                        carton.CartonNumber = cartonViewModel.CartonNumber;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CartonNumber", "A carton with that name exists, please enter another name");
                        return View();
                    }
                }

                return View(cartonViewModel);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // GET: Carton/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("BadRequest", "Error");
                }

                Carton carton = db.Cartons.Where(c => c.Id == id).FirstOrDefault();

                if (carton == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                //including details to display on confirmation screen
                carton.CartonDetails = db.CartonDetails.Include("Equipment.ModelType").Where(cd => cd.CartonId == carton.Id).ToList();

                return View(carton);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        // POST: Carton/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Carton carton = db.Cartons.Find(id);

                if (carton == null)
                    return RedirectToAction("BadRequest", "Error");

                //Bug #1: without code-first ef migrations on, setting the cascade delete on model building won't update actual database constraint
                //  so removing details if they exist and then the carton itself.
                var cartonDetails = db.CartonDetails.Where(cd => cd.CartonId == id).ToList();
                foreach (var item in cartonDetails)
                {
                    db.CartonDetails.Remove(item);
                }

                if (cartonDetails != null && cartonDetails.Count > 0)
                    db.SaveChanges();

                db.Cartons.Remove(carton);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This overloaded constructor is built with basically optional parameters, and is called so that the equipment list is
        /// re-loaded after user clicks Add from the Add Equipment view
        /// </summary>
        /// <param name="id">id of carton equipment was added too</param>
        /// <param name="equipmentDetail">Contains Model - SN for confirmation message</param>
        /// <param name="errorMessage">Contains message explaining business rule preventing item from being added</param>
        /// <returns></returns>
        public ActionResult AddEquipment(int? id, string equipmentDetail, string errorMessage)
        {
            if (id == null)
            {
                return RedirectToAction("BadRequest", "Error");
            }

            try
            {
                var carton = db.Cartons
                    .Where(c => c.Id == id)
                    .Select(c => new CartonDetailsViewModel()
                    {
                        CartonNumber = c.CartonNumber,
                        CartonId = c.Id
                    })
                    .SingleOrDefault();

                if (carton == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                carton.CartonEquipmentCount = db.CartonDetails.Where(cd => cd.CartonId == carton.CartonId).ToList().Count.ToString();

                // equipment list with every item except those already in the carton
                var equipment = db.Equipments
                    .Where(e => !db.CartonDetails.Where(cd => cd.CartonId == id).Select(cd => cd.EquipmentId).Contains(e.Id))
                    .Select(e => new EquipmentViewModel()
                    {
                        Id = e.Id,
                        ModelType = e.ModelType.TypeName,
                        SerialNumber = e.SerialNumber
                    })
                    .ToList();

                carton.Equipment = equipment;

                //equipmentDetail will be empty if coming from index view, only has value after being add has been clicked and 
                //no business rules were violated
                if (equipmentDetail != null)
                {
                    ViewBag.Message = string.Format("Item: " + Environment.NewLine + " {0} " + Environment.NewLine + " has been added to carton {1}.", equipmentDetail, carton.CartonNumber);
                    ViewBag.MessageTitle = "Succes";
                }
                //when errormessage has a value, a business rule was broken
                else if (errorMessage != null)
                {
                    ViewBag.Message = errorMessage;
                    ViewBag.MessageTitle = "Error";
                }

                //set viewbag for max count here so if there's an error the exception handler will trap it and 
                //  takes away logic from view to read config file
                if (ConfigurationManager.AppSettings["MaxItemsPerCarton"] != null)
                    ViewBag.MaxEquipCount = ConfigurationManager.AppSettings["MaxItemsPerCarton"].ToString();
                else
                    ViewBag.MaxEquipCount = 10;

                return View(carton);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }


        }

        public ActionResult AddEquipmentToCarton([Bind(Include = "CartonId,EquipmentId")] AddEquipmentViewModel addEquipmentViewModel)
        {
            try
            {
                string dialogText = string.Empty;

                //Bug #3: in case max carton volume changes, max count is a key in the web.config
                int maxItemCount = 0;
                //if the key is missing default to 10
                if (!int.TryParse(ConfigurationManager.AppSettings["MaxItemsPerCarton"].ToString(), out maxItemCount))
                    maxItemCount = 10;

                //Bug #2: currently a piece of equipment can only be in one carton, there is a key to toggle that option in web.config
                bool multipleCartons = false;
                //if key is missing or an invalid type default is false
                if (!bool.TryParse(ConfigurationManager.AppSettings["AllowItemInMultipleCartons"].ToString(), out multipleCartons))
                    multipleCartons = false;

                if (ModelState.IsValid)
                {
                    var carton = db.Cartons
                        .Include(c => c.CartonDetails)
                        .Where(c => c.Id == addEquipmentViewModel.CartonId)
                        .SingleOrDefault();
                    if (carton == null)
                    {
                        return RedirectToAction("NotFound", "Error");
                    }

                    //Bug #2: check all cartons to see if this equipment has already been added to another one               
                    var currentEquipment = db.CartonDetails
                        .Where(c => c.EquipmentId == addEquipmentViewModel.EquipmentId)
                        .SingleOrDefault();

                    if (currentEquipment != null && !multipleCartons)
                    {
                        dialogText = "This item has already been added to another carton.";
                        return RedirectToAction("AddEquipment", new { id = addEquipmentViewModel.CartonId, serialNumber = string.Empty, errorMessage = dialogText });
                    }

                    //Bug #3: make sure there aren't more than 10 (value stored in maxItemCount) items in this carton already
                    var cartonDetails = db.CartonDetails.Where(e => e.CartonId == addEquipmentViewModel.CartonId).ToList();

                    if (cartonDetails == null || cartonDetails.Count < maxItemCount)
                    {
                        var equipment = db.Equipments
                            .Include("ModelType")
                            .Where(e => e.Id == addEquipmentViewModel.EquipmentId)
                            .SingleOrDefault();
                        if (equipment == null)
                        {
                            return RedirectToAction("NotFound", "Error");
                        }
                        var detail = new CartonDetail()
                        {
                            Carton = carton,
                            Equipment = equipment
                        };
                        carton.CartonDetails.Add(detail);
                        db.SaveChanges();
                        return RedirectToAction("AddEquipment", new { id = addEquipmentViewModel.CartonId, equipmentDetail = string.Format("{0} {1}", equipment.ModelType.TypeName, equipment.SerialNumber) });
                    }
                    else
                    {
                        dialogText = string.Format("This carton is full and contains {0} items. Please select another carton.", maxItemCount.ToString());
                    }

                }
                // will only make it here if the carton is full, outside of main if so all paths return an action
                return RedirectToAction("AddEquipment", new { id = addEquipmentViewModel.CartonId, serialNumber = string.Empty, errorMessage = dialogText });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        public ActionResult ViewCartonEquipment(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("BadRequest", "Error");
                }
                var carton = db.Cartons
                    .Where(c => c.Id == id)
                    .Select(c => new CartonDetailsViewModel()
                    {
                        CartonNumber = c.CartonNumber,
                        CartonId = c.Id,
                        Equipment = c.CartonDetails
                            .Select(cd => new EquipmentViewModel()
                            {
                                Id = cd.EquipmentId,
                                ModelType = cd.Equipment.ModelType.TypeName,
                                SerialNumber = cd.Equipment.SerialNumber
                            })
                    })
                    .SingleOrDefault();
                if (carton == null)
                {
                    return RedirectToAction("NotFound", "Error");
                }

                //set viewbag for max count here so if there's an error the exception handler will trap it and 
                //  takes away logic from view to read config file
                if (ConfigurationManager.AppSettings["MaxItemsPerCarton"] != null)
                    ViewBag.MaxEquipCount = ConfigurationManager.AppSettings["MaxItemsPerCarton"].ToString();
                else
                    ViewBag.MaxEquipCount = 10;

                return View(carton);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Implementation of item removal
        /// Development item #1 and customer request #2 for current iteration
        /// </summary>
        /// <param name="removeEquipmentViewModel">When the EquipmentId is 0, the user is requesting to remove all items </param>
        /// <returns></returns>
        public ActionResult RemoveEquipmentOnCarton([Bind(Include = "CartonId,EquipmentId")] RemoveEquipmentViewModel removeEquipmentViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //customer request #2: When the EquipmentId is 0, the user is requesting to remove all items
                    if (removeEquipmentViewModel.EquipmentId == 0)
                    {
                        //remove all items from carton
                        List<CartonDetail> items = db.CartonDetails
                            .Where(cd => cd.CartonId == removeEquipmentViewModel.CartonId).ToList();

                        db.CartonDetails.RemoveRange(items);
                    }
                    else
                    {
                        //Development item #1: using DBContext remove item user selected by getting it from the DBContext, which in turn sets entity state to delete
                        db.CartonDetails.Remove(db.CartonDetails.Single(cd => cd.CartonId == removeEquipmentViewModel.CartonId && cd.EquipmentId == removeEquipmentViewModel.EquipmentId));
                    }

                    db.SaveChanges();
                }
                return RedirectToAction("ViewCartonEquipment", new { id = removeEquipmentViewModel.CartonId });
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }
    }
}
