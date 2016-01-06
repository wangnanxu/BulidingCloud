using System.Web;
using ML.BC.Services;

namespace ML.BC.Web.ViewModels
{
    public class LayoutViewModel
    {
        public string Title { get; set; }
        public string Error { get; set; }

        //public SessionUser User
        //{
        //    get { return HttpContext.Current.Session.GetMvcSolutionSession().User; }
        //}
    }
}