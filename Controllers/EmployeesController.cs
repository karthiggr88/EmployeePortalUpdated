using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EmployeePortal.Models;
using log4net;

namespace EmployeePortal.Controllers
{
    public class EmployeesController : Controller
    {
        private EmployeeDBEntities db = new EmployeeDBEntities();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Employees
        public ActionResult Employee(string Name, string gender)
        {
            log.Info("Get Employee Details");
            if(!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(gender))
            {
                return View(db.Employees.Where(x => x.gender == gender && x.first_name == Name).ToList());
            }
            else if (!string.IsNullOrEmpty(Name))
            {
                return View(db.Employees.Where(x => x.first_name == Name).ToList());
            }
            else if (!string.IsNullOrEmpty(gender))
            {
                return View(db.Employees.Where(x => x.gender == gender).ToList());
            }
            else {
                return View(db.Employees.ToList());
            }           
        }

        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            log.Info("Fetch Employee Details");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<Employment_history> employee = db.Employment_history.Where(x=>x.emp_id == id).Select(n=>n).ToList();
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.emp_id = new SelectList(db.Employment_history, "id", "organization_name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "emp_id,first_name,last_name,gender,dob,pan_num,aadhaar_num,mobile_num,email_id,office_mail,permanent_address,present_address,blood_group,profile_pict,doj,emp_level,post_name,basic_pay,house_allowance")] Employee employee)
        {
            log.Info("create Employee Details");
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Employee");
            }

            ViewBag.emp_id = new SelectList(db.Employees, "id", "organization_name", employee.emp_id);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            log.Info("Edit Employee Details");
            if (id == null)
            {
                log.Info("Entered employee id is not valid");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                log.Info("Employee not found");
                return HttpNotFound();
            }
            ViewBag.emp_id = new SelectList(db.Employees, "id", "organization_name", employee.emp_id);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "emp_id,first_name,last_name,gender,dob,pan_num,aadhaar_num,mobile_num,email_id,office_mail,permanent_address,present_address,blood_group,profile_pict,doj,emp_level,post_name,basic_pay,house_allowance")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Employee");
            }
            ViewBag.emp_id = new SelectList(db.Employees, "id", "organization_name", employee.emp_id);
            return View(employee);
        }

        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            log.Info("Delete Employee Details");
            IEnumerable<Employment_history> employeehisttory = db.Employment_history.Where(x=>x.emp_id == id);
            foreach(var empl in employeehisttory)
            {
                db.Employment_history.Remove(empl);
            }            
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            ViewBag.ResponseMessage = "successfully Deleted";
            return RedirectToAction("Employee");
        }
         
         
        public ActionResult ExportCSV()
        {
            log.Info("Export Employee Details");
            var sb = new  StringBuilder();
            // You can write sql query according your need  
            var employees = db.Employees;
            var list = employees.ToList();
            sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", "Emp Id", "Name","Doj", "Post", "Level", "Mobile", "Personal mail", "office mail", "Dob", "Blood Group", "Pan No", "Aadhaar No"
, Environment.NewLine);
            sb.AppendLine();
            sb.AppendLine();
            foreach (var item in list)
            {
                sb.AppendLine();
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}", item.emp_id, item.first_name, item.doj, item.post_name, item.emp_level, item.mobile_num, item.email_id,item.office_mail,
                    item.dob,item.blood_group,item.pan_num,item.aadhaar_num,Environment.NewLine);
            }
            //Get Current Response  
            var response = System.Web.HttpContext.Current.Response;
            response.BufferOutput = true;
            response.Clear();
            response.ClearHeaders();
            response.ContentEncoding = Encoding.Unicode;
            response.AddHeader("content-disposition", "attachment;filename=Employee.CSV ");
            response.ContentType = "text/plain";
            response.Write(sb.ToString());
            response.End();
            return RedirectToAction("Employee");
        }

         
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
