using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ML.BC.EnterpriseData.Model.Extend;

namespace ML.BC.EnterpriseData.Model
{
    #region organization
    public partial class Department : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            if (!NeedNotified(entry)) return null;

            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "Department",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>() {
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="DepartmentID",
                         Value=this.DepartmentID.ToString()
                    },
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="EnterpriseID",
                         Value=this.EnterpriseID
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.Department,
                ChangeTime = this.UpdateTime
            };
            return result;
        }

        private bool NeedNotified(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            if (entry.State == System.Data.Entity.EntityState.Added) return true;

            var checkFields = new List<string> { "DepartmentID", "Name", "EnterpriseID", "ParentID", "Description", "Available", "Deleted" };
            var result = false;
            foreach (var field in checkFields)
            {
                if (entry.CurrentValues[field] == null && entry.OriginalValues[field] == null) continue;
                if (entry.CurrentValues[field] != null && entry.OriginalValues[field] != null && entry.CurrentValues[field].Equals(entry.OriginalValues[field]))
                    continue;

                result = true;
                break;
            }
            return result;
        }
    }
    public partial class FrontUser : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var checkFields = new List<string> { "UserID", "Name", "DepartmentID", "EnterpiseID", "Closed" };
            if (!EntityHelper.WhetherNeedNotified(entry, checkFields)) return null;

            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "FrontUser",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>() {
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="UserID",
                         Value=this.UserID
                    },
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="EnterpriseID",
                         Value=this.EnterpiseID
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.User,
                ChangeTime = this.UpdateTime
            };
            return result;
        }

        private bool NeedNotified(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var checkFields = new List<string> { "UserID", "Name", "DepartmentID", "EnterpiseID", "Closed" };
            var result = false;
            foreach (var field in checkFields)
            {
                if (entry.CurrentValues[field] == null && entry.OriginalValues[field] == null) continue;
                if (entry.CurrentValues[field] != null && entry.OriginalValues[field] != null && entry.CurrentValues[field].Equals(entry.OriginalValues[field]))
                    continue;

                result = true;
                break;
            }
            if (!result)
            {
                this.UpdateTime = entry.OriginalValues.GetValue<DateTime>("UpdateTime");
            }
            return result;
        }
    }
    public partial class RFARole : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "RFARole",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>() {
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="RoleID",
                         Value=this.RoleID.ToString()
                    },
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="EnterpriseID",
                         Value=this.OwnerID
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.Role,
                ChangeTime = this.UpdateTime
            };
            return result;
        }
    }
    public partial class UserRole : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "UserRole",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>() {
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="User_RoleID",
                         Value=this.UserID+"_"+this.RoleID
                    },
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="UserID",
                         Value=this.UserID
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.Role,
                ChangeTime = this.UpdateTime
            };
            return result;
        }
    }

    #endregion

    #region project and scene
    public partial class Project : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "Project",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>() {
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="ProjectID",
                         Value=this.ProjectID
                    },
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="DepartMentIDs",
                         Value=this.Departments
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.Project,
                ChangeTime = DBTimeHelper.DBNowTime()
            };
            return result;
        }
    }
    public partial class Scene : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "Scene",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>() {
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="SceneID",
                         Value=this.SceneID
                    },
                    new  ML.BC.Infrastructure.MsmqHelper.CustomKeyValue{
                         Key="ProjectID",
                         Value=this.ProjectID
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.Scene,
                ChangeTime = DBTimeHelper.DBNowTime()
            };
            return result;
        }
    }

    public partial class SceneType : IEntityHooks
    {
        public Infrastructure.MsmqHelper.MessageItem GetChangedEntryData(
            System.Data.Entity.Infrastructure.DbEntityEntry entry)
        {
            var result = new Infrastructure.MsmqHelper.MessageItem
            {
                EntityName = "SceneType",
                Data = new List<Infrastructure.MsmqHelper.CustomKeyValue>()
                {
                    new ML.BC.Infrastructure.MsmqHelper.CustomKeyValue
                    {
                        Key = "SceneTypeID",
                        Value = this.ID.ToString()
                    },
                    new ML.BC.Infrastructure.MsmqHelper.CustomKeyValue
                    {
                        Key = "EnterpriseID",
                        Value = this.EnterpriseID
                    }
                },
                Operation = (ML.BC.Infrastructure.MsmqHelper.OperationEnum)entry.State,
                Type = Infrastructure.MsmqHelper.TypeEnum.SceneType,
                ChangeTime = DBTimeHelper.DBNowTime()
            };
            return result;
        }
    }

    #endregion

    internal static class EntityHelper
    {
        public static bool WhetherNeedNotified(System.Data.Entity.Infrastructure.DbEntityEntry entry, List<string> checkFields)
        {
            if (entry.State == System.Data.Entity.EntityState.Added) return true;
            if (entry.State == System.Data.Entity.EntityState.Deleted) return true;

            if (checkFields == null) return false;
            var result = false;
            foreach (var field in checkFields)
            {
                if (entry.CurrentValues[field] == null && entry.OriginalValues[field] == null) continue;
                if (entry.CurrentValues[field] != null && entry.OriginalValues[field] != null && entry.CurrentValues[field].Equals(entry.OriginalValues[field]))
                    continue;

                result = true;
                break;
            }
            if (!result)
            {
                //this.UpdateTime = entry.OriginalValues.GetValue<DateTime>("UpdateTime");
            }
            return result;
        }
    }
}
