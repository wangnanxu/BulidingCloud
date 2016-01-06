using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ML.BC.Services
{
    public class ScenesDto
    {
        public string SceneID { get; set; }

        public string Address { get; set; }
        public string LatitudeAndLongitude { get; set; }
        public string Name { get; set; }
        public string EnterpriseID { get; set; }
        public string EnterpriseName { get; set; }
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string ParentSceneID { get; set; }
        public List<GroupedUser> Wokers { get; set; }//workerIDS 
        public string SceneType { get; set; }
        public string tempUsers { get; set; }
        public System.DateTime RegistDate { get; set; }
        public Nullable<System.DateTime> BeginDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public ML.BC.EnterpriseData.Common.Status Status { get; set; }
        public bool Deleted { get; set; }

        public bool HasData { get; set; }
    }

    public class GroupedUser
    {
        public GroupedUser()
        {

        }
        public GroupedUser(int id, List<string> users)
        {
            roleId = id;
            UserID = users;
        }
        public int roleId { get; set; }
        public List<string> UserID { get; set; }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;

            var temp = obj as GroupedUser;
            if (temp == null) return false;
            if (roleId != temp.roleId) return false;
            if (UserID == null && temp.UserID != null) return false;
            if (UserID != null && temp.UserID == null) return false;
            if (UserID == null && temp.UserID == null) return true;
            if (UserID.Except(temp.UserID).Count() > 0 || temp.UserID.Except(UserID).Count() > 0) return false;
            return true;
        }
        public override int GetHashCode()
        {
            var str = string.Format("{0}_{1}", roleId, UserID == null ? "" : string.Join(",", UserID));
            return str.GetHashCode();
        }
    }
    
}
