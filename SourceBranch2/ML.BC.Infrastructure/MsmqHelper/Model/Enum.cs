using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.MsmqHelper
{
    public enum TypeEnum
    {
        /// <summary>
        /// 部门
        /// </summary>
        Department = 0,
        /// <summary>
        /// 用户
        /// </summary>
        User = 1,
        /// <summary>
        /// 消息
        /// </summary>
        Message = 2,
        /// <summary>
        /// 角色
        /// </summary>
        Role = 3,
        /// <summary>
        /// 项目
        /// </summary>
        Project = 4,
        /// <summary>
        /// 现场
        /// </summary>
        Scene = 5,
        /// <summary>
        /// 现场数据
        /// </summary>
        SceneData = 6,
        /// <summary>
        /// 现场类型
        /// </summary>
        SceneType = 7
    }

    public enum OperationEnum
    {
        /// <summary>
        /// 增加
        /// </summary>
        Added = 4,
        /// <summary>
        /// 删除
        /// </summary>
        Deleted = 8,
        /// <summary>
        /// 修改
        /// </summary>
        Modified = 16,
    }
}
