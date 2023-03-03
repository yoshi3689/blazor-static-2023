using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Functions101.Models.Toons;
using System.Linq;

namespace Snoopy.Function
{
    public class HttpWebApi
    {
        private readonly SchoolContext _context;

        public HttpWebApi(SchoolContext context)
        {
            _context = context;
        }

        [FunctionName("HttpWebApi")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "hello")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("GetStudents")]
        public IActionResult GetStudents(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "students")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP GET/posts trigger function processed a request in GetStudents().");

            var students = _context.Students.ToArray();

            return new OkObjectResult(students);
        }

        [FunctionName("GetStudent")]
        public IActionResult GetStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "students/{id}")] HttpRequest req,
        ILogger log, int id)
        {
            log.LogInformation("C# HTTP GET/posts trigger function processed a request.");
            if (id < 1)
            {
                return new NotFoundResult();
            }
            var students = _context.Students.FindAsync(id).Result;
            if (students == null)
            {
                return new NotFoundResult();
            }
            log.LogInformation(students.StudentId.ToString());
            return new OkObjectResult(students);
        }

        [FunctionName("CreateStudent")]
        public async Task<IActionResult> CreateStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "students")] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP POST/posts trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<Student>(requestBody);
            var student = new Student()
            {
                StudentId = input.StudentId,
                FirstName = input.FirstName,
                LastName = input.LastName,
                School = input.School,
            };
            _context.Add(student);
            await _context.SaveChangesAsync();
            log.LogInformation(requestBody);
            return new OkObjectResult(student);
        }

        [FunctionName("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "students/{id}")] HttpRequest req,
        ILogger log, int id) {
            log.LogInformation("C# HTTP PUT/posts trigger function processed a request.");
            if (id < 1)
            {
                return new NotFoundResult();
            }
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return new NotFoundResult();
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<Student>(requestBody);
            student.FirstName = input.FirstName;
            student.LastName = input.LastName;
            student.School = input.School;

            _context.Update(student);
            await _context.SaveChangesAsync();
            log.LogInformation(requestBody);
            return new OkObjectResult(student);
        }

        [FunctionName("DeleteStudent")]
        public IActionResult DeleteStudent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "students/{id}")] HttpRequest req,
        ILogger log, int id) {
            log.LogInformation("C# HTTP DELETE/posts trigger function processed a request.");
            if (id < 1)
            {
                return new NotFoundResult();
            }
            var student = _context.Students.FindAsync(id).Result;
            if (student == null)
            {
                return new NotFoundResult();
            }
            _context.Remove(student);
            _context.SaveChangesAsync();
            return new OkResult();
        }

    }
}
