using LogosAPI.Data;
using LogosAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LogosAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LogosController : ControllerBase
    {
        private readonly ApiContext _context;

        public LogosController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost, AllowAnonymous]
        public JsonResult CreateEditissues(issues issue)
        {
            if (issue.id == 0)
            {
                _context.issues.Add(issue);
            }
            else
            {
                var issuesInDb = _context.issues.Find(issue.id);
                if (issuesInDb == null)
                    return new JsonResult(NotFound());
                issuesInDb = issue;
            }

            _context.SaveChanges();

            return new JsonResult(Ok(issue));
        }

        //get
        [HttpGet("/Getissues"), AllowAnonymous]
        public JsonResult Getissues(int id)
        {
            var issues = _context.issues.Find(id);

            if (issues == null)
                return new JsonResult(NotFound());

            return new JsonResult(Ok(issues));
        }


        //delete
        [HttpDelete("/Deleteissues")]
        public JsonResult Deleteissues(int id)
        {
            var issue = _context.issues.Find(id);

            if (issue == null)
                return new JsonResult(NotFound(id));

            _context.issues.Remove(issue);

            _context.SaveChanges();

            return new JsonResult(Ok(issue));
        }

        //GetAll
        [HttpGet("/GetAllissues")]
        public JsonResult GetAllissues()
        {
            var issues = _context.issues.ToList();

            return new JsonResult(issues);
        }

        //planned Maintenance
        [HttpPost, AllowAnonymous]
        public JsonResult CreateEditMaintenance(plannedmaintenance pm)
        {
            if (pm.id == 0)
            {
                _context.plannedmaintenance.Add(pm);
            }
            else
            {
                var pmInDb = _context.plannedmaintenance.Find(pm.id);
                if (pmInDb == null)
                    return new JsonResult(NotFound());
                pmInDb = pm;
            }

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }

        [HttpGet("/GetPm"), AllowAnonymous]
        public JsonResult GetPm(int id)
        {
            var pm = _context.plannedmaintenance.Find(id);

            if (pm == null)
            {
                return new JsonResult(NotFound());
            }

            return new JsonResult(Ok(pm));
        }

        [HttpDelete]
        public JsonResult DeletePm(int id)
        {
            var pm = _context.plannedmaintenance.Find(id);

            if (pm == null)
            {
                return new JsonResult(NotFound());
            }

            _context.plannedmaintenance.Remove(pm);

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }

        [HttpGet("/GetAllPm")]
        public JsonResult GetAllPm()
        {
            var pms = _context.plannedmaintenance.ToList();

            return new JsonResult(pms);
        }

        //specific function

        //get plannedMaintennace ScheduledDate > currentDate
        [HttpGet("/GetPmScheduledDate"), AllowAnonymous]
        public JsonResult GetPmScheduledDate()
        {
            List<plannedmaintenance> pms = new List<plannedmaintenance>();

            foreach (var pm in _context.plannedmaintenance)
            {
                int Results = DateTime.Compare(pm.scheduled, DateTime.Now.AddHours(2));

                if (Results >= 0)
                    pms.Add(pm);
            }

            var orderByPm = from pm in pms
                            orderby pm.dateofCreation
                            select pm;

            pms = orderByPm.ToList();

            return new JsonResult(pms);
        }


        //get issues for current issues page
        [HttpGet("/GetCurrentissues"), AllowAnonymous]
        public JsonResult GetCurrentissues()
        {
            List<issues> issues = new List<issues>();

            foreach (var issue in _context.issues)
            {
                if (issue.Closing == null)
                    issues.Add(issue);

            }

            var orderByTimeCreation = from issue in issues
                                      orderby issue.Date descending
                                      select issue;

            //issues.OrderByDescending(i => i.Date)

            issues = orderByTimeCreation.ToList();

            return new JsonResult(issues);
        }

        //get issues for historical issues
        [HttpGet("/GetHistoricalissues"), AllowAnonymous]
        public JsonResult GetHistoricalissues()
        {
            List<issues> issues = new List<issues>();

            foreach (var issue in _context.issues)
            {
                if (issue.Closing != null)
                    issues.Add(issue);

            }

            var orderByTimeCreation = from issue in issues
                                      orderby issue.Date descending
                                      select issue;


            issues = orderByTimeCreation.ToList();

            return new JsonResult(issues);
        }


        //admin part

        [HttpPost("/SolvedIssue")]
        public JsonResult SolvedIssue(int id, DateTime date)
        {
            var issue = _context.issues.Find(id);

            if (issue == null)
            {
                return new JsonResult(NotFound());
            }

            _context.issues.Find(id).Closing = DateTime.UtcNow.AddHours(2);
            _context.issues.Find(id).Solving = date;


            _context.SaveChanges();

            return new JsonResult(issue);

        }

        [HttpPost("/CreateIssue")]
        public JsonResult CreateIssue(string AffectedSolutions, string Text, DateTime ETA)
        {
            issues issue = new issues(AffectedSolutions, Text, ETA);
            CreateEditissues(issue);

            return new JsonResult(issue);
        }


        [HttpDelete("/DeletePmAdmin")]
        public JsonResult DeletePmAdmin(int id)
        {
            var pm = _context.plannedmaintenance.Find(id);

            if (pm == null)
            {
                return new JsonResult(NotFound());
            }

            _context.plannedmaintenance.Remove(pm);

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }

        [HttpPost("/CreatePm")]
        public JsonResult CreatePm(string AffectedSolutions, string Text, DateTime s, DateTime d)
        {
            plannedmaintenance pm = new(AffectedSolutions, Text, s, d);
            CreateEditMaintenance(pm);

            return new JsonResult(Ok(pm));
        }


        //view historical maintenance
        [HttpGet("/GetHistoricalPmAdmin")]
        public JsonResult GetHistoricalPmAdmin()
        {
            List<plannedmaintenance> pms = new List<plannedmaintenance>();

            foreach (var pm in _context.plannedmaintenance)
            {
                int Results = DateTime.Compare(pm.scheduled, DateTime.Now.AddHours(2));

                if (Results < 0)
                    pms.Add(pm);
            }

            var orderByPm = from pm in pms
                            orderby pm.dateofCreation descending
                            select pm;

            pms = orderByPm.ToList();

            return new JsonResult(pms);
        }


        //modif issue text
        [HttpPost("/editTextIssue")]
        public JsonResult editTextIssue(int id, string t)
        {
            var issue = _context.issues.Find(id);

            if (issue == null)
            {
                return new JsonResult(NotFound());
            }

            _context.issues.Find(id).Text = t;
            issue = _context.issues.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(issue));
        }

        //edit issue solutions
        [HttpPost("/editSolutionIssue")]
        public JsonResult editSolutionIssue(int id, string s)
        {
            var issue = _context.issues.Find(id);

            if (issue == null)
            {
                return new JsonResult(NotFound());
            }

            _context.issues.Find(id).AffectedSolutions = s;
            issue = _context.issues.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(issue));
        }

        //edit eta issue
        [HttpPost("/editEtaIssue")]
        public JsonResult editEtaIssue(int id, DateTime date)
        {
            var issue = _context.issues.Find(id);

            if (issue == null)
            {
                return new JsonResult(NotFound());
            }

            _context.issues.Find(id).ETA = date;
            issue = _context.issues.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(issue));
        }

        //modif pm text
        [HttpPost("/editTextPm")]
        public JsonResult editTextPm(int id, string t)
        {
            var pm = _context.plannedmaintenance.Find(id);

            if (pm == null)
            {
                return new JsonResult(NotFound());
            }

            _context.plannedmaintenance.Find(id).text = t;
            pm = _context.plannedmaintenance.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }

        //modif pm solutions
        [HttpPost("/editSolutionPm")]
        public JsonResult editSolutionPm(int id, string t)
        {
            var pm = _context.plannedmaintenance.Find(id);

            if (pm == null)
            {
                return new JsonResult(NotFound());
            }

            _context.plannedmaintenance.Find(id).affectedSolutions = t;
            pm = _context.plannedmaintenance.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }

        //modif pm solutions
        [HttpPost("/editScheduledPm")]
        public JsonResult editScheduledPm(int id, DateTime date)
        {
            var pm = _context.plannedmaintenance.Find(id);

            if (pm == null)
            {
                return new JsonResult(NotFound());
            }

            _context.plannedmaintenance.Find(id).scheduled = date;
            pm = _context.plannedmaintenance.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }
                //modif duration pm
        [HttpPost("/ModifDuration")]
        public JsonResult ModifDuration(int id, DateTime date){

            var pm = _context.plannedmaintenance.Find(id);

            if(pm == null){
                return new JsonResult(NotFound());
            }

            _context.plannedmaintenance.Find(id).duration = date.ToString("t");
            pm = _context.plannedmaintenance.Find(id);

            _context.SaveChanges();

            return new JsonResult(Ok(pm));
        }

    }
}
