using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Services.Common
{
    public interface IUniqeIdGenerator : IServiceBase
    {
        //string ProjectID { get; set; }
        //string SceneID { get; set; }
        //string BackUserID { get; set; }
        //string FrontUserID { get; set; }


        /// <summary>
        /// 生成企业ID
        /// </summary>
        /// <param name="professionIdentify">行业标识</param>
        /// <param name="propertyIdentify">属性标识</param>
        /// <param name="length">字符串默认指定长度为7</param>
        /// <returns></returns>
        string GeneratorEnterpriseID(string professionIdentify, string propertyIdentify, uint length = 7);

        /// <summary>
        /// 生成项目ID
        /// </summary>
        /// <param name="length">字符串长度默认5</param>
        /// <returns></returns>
        string GeneratorProjectID(uint length = 5);

        /// <summary>
        /// 生成现场ID
        /// </summary>
        /// <param name="length">字符串长度默认5</param>
        /// <returns></returns>
        string GeneratorScenceID(uint length = 5);

        /// <summary>
        /// 生成后台用户ID
        /// </summary>
        /// <param name="length">字符串长度默认6</param>
        /// <returns></returns>
        string GeneratorBackUserID(uint length = 6);

        /// <summary>
        /// 生成前台用户ID
        /// </summary>
        /// <param name="length">字符串长度默认6</param>
        /// <returns></returns>
        string GeneratorFrontUserID(uint length = 6);
    }
}
