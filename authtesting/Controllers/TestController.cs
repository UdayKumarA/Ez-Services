using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using authtesting.filters;
using authtesting.Models;

namespace authtesting.Controllers
{

    public class TestController : ApiController
    {
        Registration objregistration = new Registration();
        // GET api/<controller>
        [Authorize]
        [HttpGet]
        public List<Userssss> Get()
        {
            List<Userssss> UserList = new List<Userssss>
            {
                new Userssss{Name = "GoodEvening"},
              
                //new User{Id = 2, UserName = "Anita Negi"},
                //new User{Id = 3, UserName = "Asha Singh"},
                //new User{Id = 4, UserName = "Ashish Gupta" }
            };
            //using (evolutyzalexaEntities entities = new evolutyzalexaEntities())
            //{
            //    AuthDatetime auth = new AuthDatetime();
            //    int result = 0;
            //    auth.GetCurrentDate = DateTime.Now;
            //    entities.AuthDatetimes.Add(auth);
            //    result = entities.SaveChanges();

            //}


            return UserList;
        }
        [HttpPost]
        public HttpResponseMessage uploadImage()
        {
            var request = HttpContext.Current.Request;

            if (Request.Content.IsMimeMultipartContent())
            {
                if (request.Files.Count > 0)
                {
                    var postedFile = request.Files.Get("file");
                    var title = request.Params["title"];
                    //string root = "http://192.168.75.19:5001/Content/images/CompanyLogos";

                    string root = HttpContext.Current.Server.MapPath("~/192.168.75.19:5009/Content/images/CompanyLogos");
                    root = root + "/" + postedFile.FileName;
                    postedFile.SaveAs(root);
                    //Save post to DB
                    return Request.CreateResponse(HttpStatusCode.Found, new
                    {
                        error = false,
                        status = "created",
                        path = root
                    });

                }
            }

            return null;
        }
        [HttpPost]
        public HttpResponseMessage uploadImage1()
        {
            string name = "download.png";
            string filename = System.IO.Path.GetFileName(name);

            try
            {
                //webClient.UploadFileCompleted += new AsyncCompletedEventArgs(client_UploadFileCompleted);
                //webClient.UploadFileAsync(new Uri(@"http://192.168.75.99:5003/upload/TeacherResource/home.png" ), @"C:\home.png");

                string strServerurl = @"http://192.168.75.19:5001/Content/images/CompanyLogos/" + filename;
                Uri urls = new Uri(strServerurl);
                var storeurl = urls;
                //string fileName = Path.GetFileName(name);
                //System.IO.File.Move(name, @"http://192.168.75.19:5001/Content/images/CompanyLogos/" + fileName);
                //WebClient webClient = new WebClient();
                //var returnResponse = webClient.UploadFile("http://192.168.75.99:5003/upload/TeacherResource/home.png", txtbox.Text);
                //if(returnResponse == null)
                //{

                //}
                //else
                //{

                //}
                // File.Move(filename, strServerurl);


                HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(urls);

                return Request.CreateResponse(HttpStatusCode.Found, new
                {
                    error = false,
                    status = "created",
                    path = storeurl
                });


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public HttpResponseMessage Post()

        {

            HttpResponseMessage result = null;

            var httpRequest = HttpContext.Current.Request;

            if (httpRequest.Files.Count > 0)

            {

                var docfiles = new List<string>();

                foreach (string file in httpRequest.Files)

                {

                    var postedFile = httpRequest.Files[file];

                    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);

                    postedFile.SaveAs(filePath);



                    docfiles.Add(filePath);

                }

                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);

            }

            else

            {

                result = Request.CreateResponse(HttpStatusCode.BadRequest);

            }

            return result;

        }

        private static readonly string ServerUploadFolder = "C:\\Temp"; //Path.GetTempPath();

        //[Route("files")]
        [HttpPost]
        [ValidateMimeMultipartContentFilter]
        public async Task<FileResult> UploadSingleFile()
        {
            var streamProvider = new MultipartFormDataStreamProvider(ServerUploadFolder);
            await Request.Content.ReadAsMultipartAsync(streamProvider);

            return new FileResult
            {
                FileNames = streamProvider.FileData.Select(entry => entry.LocalFileName),
                Names = streamProvider.FileData.Select(entry => entry.Headers.ContentDisposition.FileName),
                ContentTypes = streamProvider.FileData.Select(entry => entry.Headers.ContentType.MediaType),
                Description = streamProvider.FormData["description"],
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
                DownloadLink = "TODO, will implement when file is persisited"
            };
        }
        [HttpGet]
        public List<Userssss> GetName()
        {
            List<Userssss> listuser = new List<Controllers.Userssss>
            {
                new Userssss{Name="Hello Evolutyz"},
            };
            return listuser;
        }
        //[HttpGet]
        //public HttpResponseMessage GetAllEmployees()
        //{
        //    using (EvolutyzCornerDBTestingEntities objEntities = new EvolutyzCornerDBTestingEntities())
        //    {
        //        List<User> lstEmp = new List<User>();
        //        var travelCompany = objEntities.Users.Include("Account").OrderBy(a => a.Usr_UserID).ToList();
        //        lstEmp = (from e in objEntities.Users select e).ToList();
        //        try
        //        {
        //            if (lstEmp != null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, lstEmp);
        //            }
        //            else
        //            {
        //                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no data");
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.InnerException.ToString());
        //        }
        //    }
        //}

        //[HttpGet]
        //public HttpResponseMessage GetStudents()
        //{
        //    using (MedicalDBEntities objEntities = new MedicalDBEntities())
        //    {
        //        List<StudentMaster> lstEmp = new List<StudentMaster>();
        //        lstEmp = (from e in objEntities.StudentMasters select e).ToList();
        //        try
        //        {
        //            if (lstEmp != null)
        //            {
        //                return Request.CreateResponse(HttpStatusCode.OK, lstEmp);
        //            }
        //            else
        //            {
        //                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "There is no data");
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.InnerException.Message);
        //        }
        //    }
        //}


        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<controller>/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

        [HttpPost]
        public HttpResponseMessage RegisterUser([FromBody] UserProperties objuser)
        {
            string result = objregistration.UserRegister(objuser);
            try
            {
                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Not Successful");
                }
            }
            catch(Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.Message.ToString());
            }
        }
        [HttpPost]
        public HttpResponseMessage LoginUser(UserLogin objuser)
        {
            string result = objregistration.UserLogin(objuser);
            try
            {
                if (result!="")
                {
                    return Request.CreateResponse(HttpStatusCode.OK,"Success");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Failed");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.InnerException.Message.ToString());
            }
        }

    }
    public class Userssss
    {

        public string Name { get; set; }
    }
    public class uploadimage
    {
        public string filename { get; set; }
    }
    public class FileResult
    {
        public IEnumerable<string> FileNames { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public DateTime UpdatedTimestamp { get; set; }
        public string DownloadLink { get; set; }
        public IEnumerable<string> ContentTypes { get; set; }
        public IEnumerable<string> Names { get; set; }
    }

}