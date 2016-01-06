using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Enterprise.Dtos
{
    public class EnterpriseDto
    {
        public string EnterpriseID { get; set; }
        public string Name { get; set; }
        public string ProfessionID { get; set; }
        public string ProfessionName { get; set; }
        public string PropertyID { get; set; }
        public string PropertyName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public byte Status { get; set; }
        public System.DateTime RegistDate { get; set; }
        public System.DateTime UpdateTime { get; set; }
        public bool Deleted { get; set; }

        static public implicit operator ML.BC.EnterpriseData.Model.Enterprise(EnterpriseDto obj)
        {
            return new ML.BC.EnterpriseData.Model.Enterprise
            {
                EnterpriseID = obj.EnterpriseID,
                Name = obj.Name,
                ProfessionID = obj.ProfessionID,
                Province = obj.Province,
                City = obj.City,
                Address = obj.Address,
                Telephone = obj.Telephone,
                Fax = obj.Fax,
                Status = obj.Status,
                RegistDate = obj.RegistDate,
                UpdateTime = obj.UpdateTime,
                Deleted = obj.Deleted,
                PropertyID = obj.PropertyID

            };
        }

        static public implicit operator EnterpriseDto(ML.BC.EnterpriseData.Model.Enterprise obj)
        {
            return new EnterpriseDto
            {
                EnterpriseID = obj.EnterpriseID,
                Name = obj.Name,
                ProfessionID = obj.ProfessionID,
                Province = obj.Province,
                City = obj.City,
                Address = obj.Address,
                Telephone = obj.Telephone,
                Fax = obj.Fax,
                Status = obj.Status,
                RegistDate = obj.RegistDate,
                UpdateTime = obj.UpdateTime,
                Deleted = obj.Deleted,
                PropertyID = obj.PropertyID

            };
        } 
    }
}
