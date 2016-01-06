using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ML.BC.BCBackData.Model;
using ML.BC.EnterpriseData.Model;
using ML.BC.Infrastructure.Exceptions;

namespace ML.BC.Services.Common
{
    public class UniqeIdGenerator : IUniqeIdGenerator
    {
        private static string _projectID;
        private static string _sceneID;
        private static string _backUserID;
        private static string _frontUserID;

        #region 注释暂时不用代码

        //private string ProjectID
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_projectID))
        //        {
        //            //  从数据库返回最大ID                  
        //            using (var db = new BCEnterpriseContext())
        //            {
        //                _projectID = db.Projects.Max(x => x.ProjectID);
        //                if (!IsAvailableId(_projectID))
        //                {
        //                    throw new KnownException("数据库Max值Id不合法，请检查数据库！");
        //                }
        //            }
        //            _projectID = GeneratorProjectID();
        //            return _projectID;
        //        }
        //        else
        //        {
        //            _projectID = GeneratorProjectID();
        //            return _projectID;
        //        }
        //    }
        //    set
        //    {

        //    }
        //}

        //private string SceneID
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_sceneID))
        //        {
        //            //  从数据库返回最大ID                  
        //            using (var db = new BCEnterpriseContext())
        //            {
        //                _sceneID = db.Scenes.Max(x => x.SceneID);
        //                if (!IsAvailableId(_sceneID))
        //                {
        //                    throw new KnownException("数据库Max值Id不合法，请检查数据库！");
        //                }
        //            }
        //            _sceneID = GeneratorScenceID();
        //            return _sceneID;
        //        }
        //        else
        //        {
        //            _sceneID = GeneratorScenceID();
        //            return _sceneID;
        //        }
        //    }
        //    set
        //    {

        //    }
        //}

        //private string BackUserID
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_backUserID))
        //        {
        //            //  从数据库返回最大ID                  
        //            using (var db = new BCBackContext())
        //            {
        //                _backUserID = db.BackUsers.Max(x => x.UserID);
        //                if (!IsAvailableId(_backUserID))
        //                {
        //                    throw new KnownException("数据库Max值Id不合法，请检查数据库！");
        //                }
        //            }
        //            _backUserID = GeneratorBackUserID();
        //            return _backUserID;
        //        }
        //        else
        //        {
        //            _backUserID = GeneratorBackUserID();
        //            return _backUserID;
        //        }
        //    }
        //    set
        //    {

        //    }
        //}
        //private string FrontUserID
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(_frontUserID))
        //        {
        //            //  从数据库返回最大ID                  
        //            using (var db = new BCEnterpriseContext())
        //            {
        //                _frontUserID = db.FrontUsers.Max(x => x.UserID);
        //                if (!IsAvailableId(_frontUserID))
        //                {
        //                    throw new KnownException("数据库Max值Id不合法，请检查数据库！");
        //                }
        //            }
        //            _frontUserID = GeneratorFrontUserID();
        //            return _frontUserID;
        //        }
        //        else
        //        {
        //            _frontUserID = GeneratorFrontUserID();
        //            return _frontUserID;
        //        }
        //    }
        //    set
        //    {

        //    }
        //}
        #endregion

        /// <summary>
        /// 生成企业ID
        /// </summary>
        /// <param name="professionIdentify">行业标识</param>
        /// <param name="propertyIdentify">属性标识</param>
        /// <param name="length">字符串默认指定长度为7</param>
        /// <returns>string：成功返回字符串 null：错误返回</returns>
        public virtual string GeneratorEnterpriseID(string professionIdentify, string propertyIdentify, uint length = 7)
        {
            try
            {
                char profession;
                char property;

                //  判断标识是否符合要求
                if (!string.IsNullOrEmpty(professionIdentify) && !string.IsNullOrEmpty(propertyIdentify)
                    && 1 == professionIdentify.Length && 1 == propertyIdentify.Length)
                {
                    //  转为大写
                    profession = professionIdentify.ToUpper()[0];
                    property = propertyIdentify.ToUpper()[0];
                    if (!(property >= 65 && property <= 90 && profession >= 65 && profession <= 90))
                    {
                        throw new KnownException("企业类型或企业性质不合规范！");
                    }
                }
                else
                {
                    throw new KnownException("企业类型或企业性质不合规范！");
                }

                using (var db = new BCEnterpriseContext())
                {
                    string queryStr = profession.ToString() + property.ToString();
                    string maxIdStr = db.Enterprises.Where(x => x.EnterpriseID.IndexOf(queryStr) == 0).Max(x => x.EnterpriseID);

                    //  当不存在该行业标识的企业ID，指定对应行业的起始ID，例如：CP00000
                    if (string.IsNullOrEmpty(maxIdStr))
                    {
                        var l = length <= 7 ? 7 : length;
                        byte[] byt = new byte[l];
                        for (var i = 1; i < l; i++)
                        {
                            byt[i] = 48;
                        }
                        byt[0] = (byte)profession;
                        byt[1] = (byte)property;
                        maxIdStr = AscII2String(byt);
                    }

                    //  判断企业Id是否合法，只需保证除前两个字符外都为数字即可合法
                    var numStr = Regex.Matches(maxIdStr, @"\d+")[0].Value;
                    var lastNumStr = maxIdStr.Substring(2, maxIdStr.Length - 2);
                    lastNumStr = Regex.Matches(lastNumStr, @"\d+")[0].Value;
                    if (lastNumStr.Length != numStr.Length)
                    {
                        throw new KnownException("数据库Max值Id不合法，请检查数据库！");
                    }

                    var bytArray = String2AscII(maxIdStr);

                    //  最后一个字符的ASCII码加1
                    bytArray[bytArray.Length - 1] += 1;

                    //  根据进位规则得到可用字符串对应的字节数组
                    var nextArray = GetNewByteArrayByScale(bytArray, 'Z', bytArray.Length);
                    maxIdStr = AscII2String(nextArray);
                    return maxIdStr;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 生成项目ID
        /// </summary>
        /// <param name="length">字符串长度默认5</param>
        /// <returns></returns>
        public virtual string GeneratorProjectID(uint length = 5)
        {
            try
            {
                if (string.IsNullOrEmpty(_projectID))
                {
                    using (var db = new BCEnterpriseContext())
                    {
                        _projectID = db.Projects.Max(x => x.ProjectID);
                        if (!IsAvailableId(_projectID))
                        {
                            throw new KnownException("数据库Max值Id不合法，请检查数据库！");
                        }
                    }
                    //  当数据库不存在任何ID时候，自动生成最小字符串
                    if (string.IsNullOrEmpty(_projectID))
                    {
                        length = length <= 5 ? 5 : length;
                        _projectID = GetMinString(length);
                    }

                }
                lock (_projectID)
                {
                    _projectID = GetNextStringID(_projectID);
                }
                return _projectID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 生成现场ID
        /// </summary>
        /// <param name="length">字符串长度默认5</param>
        /// <returns></returns>
        public virtual string GeneratorScenceID(uint length = 5)
        {
            try
            {
                if (string.IsNullOrEmpty(_sceneID))
                {
                    using (var db = new BCEnterpriseContext())
                    {
                        _sceneID = db.Scenes.Max(x => x.SceneID);
                        if (!IsAvailableId(_sceneID))
                        {
                            throw new KnownException("数据库Max值Id不合法，请检查数据库！");
                        }
                    }
                    //  当数据库不存在任何ID时候，自动生成最小字符串
                    if (string.IsNullOrEmpty(_sceneID))
                    {
                        length = length <= 5 ? 5 : length;
                        _sceneID = GetMinString(length);
                    }
                }
                lock (_sceneID)
                {
                    _sceneID = GetNextStringID(_sceneID);
                }              
                return _sceneID;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 生成后台用户ID
        /// </summary>
        /// <param name="length">字符串长度默认6</param>
        /// <returns></returns>
        public virtual string GeneratorBackUserID(uint length = 6)
        {
            try
            {
                if (string.IsNullOrEmpty(_backUserID))
                {
                    using (var db = new BCBackContext())
                    {
                        _backUserID = db.BackUsers.Max(x => x.UserID);
                        if (!IsAvailableId(_backUserID))
                        {
                            throw new KnownException("数据库Max值Id不合法，请检查数据库！");
                        }
                    }
                    //  当数据库不存在任何ID时候，自动生成最小字符串
                    if (string.IsNullOrEmpty(_backUserID))
                    {
                        length = length <= 6 ? 6 : length;
                        _backUserID = GetMinString(length);

                    }
                }
                lock (_backUserID)
                {
                    _backUserID = GetNextStringID(_backUserID);
                }              
                return _backUserID;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 生成前台用户ID
        /// </summary>
        /// <param name="length">字符串长度默认6</param>
        /// <returns></returns>
        public virtual string GeneratorFrontUserID(uint length = 6)
        {
            try
            {
                if (string.IsNullOrEmpty(_frontUserID))
                {
                    using (var db = new BCEnterpriseContext())
                    {
                        _frontUserID = db.FrontUsers.Max(x => x.UserID);
                        if (!IsAvailableId(_frontUserID))
                        {
                            throw new KnownException("数据库Max值Id不合法，请检查数据库！");
                        }
                    }
                    //  当数据库不存在任何ID时候，自动生成最小字符串
                    if (string.IsNullOrEmpty(_frontUserID))
                    {
                        length = length <= 6 ? 6 : length;
                        _frontUserID = GetMinString(length);
                    }
                }
                lock (_frontUserID)
                {
                    _frontUserID = GetNextStringID(_frontUserID);
                }
                return _frontUserID;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 以A开头的最小字符串生成
        /// </summary>
        /// <param name="length">指定生成字符串长度大小，默认长度为5</param>
        /// <returns></returns>
        private string GetMinString(uint length = 5)
        {
            try
            {
                var l = length;
                byte[] byt = new byte[l];
                for (var i = 1; i < l; i++)
                {
                    byt[i] = 48;
                }
                byt[0] = 65;
                var reStr = AscII2String(byt);
                return reStr;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 获得下一个可用字符串ID
        /// </summary>
        /// <param name="basicStr">当前字符串ID</param>
        /// <returns></returns>
        private string GetNextStringID(string basicStr)
        {
            try
            {
                var bytArray = String2AscII(basicStr);

                var firstAlphabet = basicStr.FirstOrDefault();

                //  最后一个字符的ASCII码加1
                bytArray[bytArray.Length - 1] += 1;
                var nextBytArray = bytArray;

                //  根据进位规则，得到新的字符串对应的字节数组
                nextBytArray = GetNewByteArrayByScale(bytArray, firstAlphabet, bytArray.Length);

                //  得到新的字符串的字母部分,需包含Z的进位，Z的下一位是 [ 
                var alpStr = Regex.Matches(AscII2String(nextBytArray), @"[A-\[]+")[0].Value;

                //  得到最后一个字母
                char lastAlphabet = alpStr.LastOrDefault();

                //  当字符串中最后一个字母比第一个字母大时，需要将原来第一个字母加一后指定为新的第一个字母
                if (firstAlphabet <= bytArray[bytArray.Length - 1])
                {
                    var l = bytArray.Length;
                    byte[] byt = new byte[l];
                    for (var i = 1; i < l; i++)
                    {
                        byt[i] = 48;
                    }
                    byt[0] = Convert.ToByte(firstAlphabet + 1);
                    nextBytArray = byt;
                    var reStr = AscII2String(nextBytArray);
                    return reStr;
                }

                //  当字母部分的最后一个字母比第一个字母大时，需要调整这个字母减一，并将下一位置为‘A’
                if (firstAlphabet < lastAlphabet)
                {
                    bytArray[alpStr.Length - 1] -= 1;
                    bytArray[alpStr.Length] = 65;
                    nextBytArray = bytArray;
                    var reStr = AscII2String(nextBytArray);
                    return reStr;
                }
                else
                {
                    var reStr = AscII2String(nextBytArray);
                    return reStr;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 字符串对应字节数组经过进位过程所得到新的对应字节数组
        /// </summary>
        /// <param name="buffer">当前字符串对应的字节数组</param>
        /// <param name="alphabetScale">首字母,以首字母来判断字母部分的进制，例如：A 代表11进制</param>
        /// <param name="flag">buffer.Lenth即可</param>
        /// <returns>byte[]</returns>
        private byte[] GetNewByteArrayByScale(byte[] buffer, char alphabetScale, int flag)
        {
            try
            {
                int[] scaleArray;
                byte[] reBuffer = buffer;

                if (1 == IsScale(buffer[flag - 1], alphabetScale, out scaleArray))
                {
                    flag -= 1;
                    buffer[flag] = Convert.ToByte((buffer[flag] - scaleArray[0]) % scaleArray[1] + scaleArray[0]);
                    buffer[flag - 1] = Convert.ToByte(buffer[flag - 1] + 1);
                    reBuffer = buffer;
                    if (1 == IsScale(reBuffer[flag - 1], alphabetScale, out scaleArray))
                    {
                        reBuffer = GetNewByteArrayByScale(reBuffer, alphabetScale, flag);
                    }
                    return reBuffer;
                }
                else
                {
                    return reBuffer;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 判断是否有进位
        /// </summary>
        /// <param name="asciiNumber">字符串中某个字符</param>
        /// <param name="alphabetScale">首字母</param>
        /// <param name="scaleArray">输出进制信息：scaleArray[0]为基准数，scaleArray[1]为进制</param>
        /// <returns>2：超出进制范围，1：进位，0：不进位，null：错误</returns>
        private int? IsScale(byte asciiNumber, char alphabetScale, out int[] scaleArray)
        {
            try
            {
                scaleArray = new[] { 48, 10 };

                if (!string.IsNullOrEmpty(asciiNumber.ToString()))
                {
                    if (asciiNumber >= 48 && asciiNumber == 58)
                    {
                        scaleArray[0] = 48;
                        scaleArray[1] = 10;
                    }
                    else
                    {
                        if (asciiNumber >= 65 && asciiNumber <= (byte)alphabetScale)
                        {
                            scaleArray[0] = 65;
                            scaleArray[1] = ((byte)alphabetScale - 65) % 26 + 11;
                        }
                        else
                        {
                            //  超出了字母最大的进制   例如：AA999加1之后为AB000，这里的B就超出了A最大范围
                            return 2;
                        }
                    }
                    if ((asciiNumber - scaleArray[0]) / scaleArray[1] > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    //  判断失败，请检查参数输入是否正确
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// strin转为对应的ASCII数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        private byte[] String2AscII(string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }
        /// <summary>
        /// AscII数组转为string
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private string AscII2String(byte[] buffer)
        {
            return Encoding.ASCII.GetString(buffer);
        }

        public bool IsAvailableId(string s)
        {
            try
            {
                byte[] bytArr;

                //  数据库返回的数据为空，说明不存在，需要新建Id，故可认为这是合法。
                if (string.IsNullOrEmpty(s)) return true;

                //  首字符不为字母不合法
                if (!(s.First() >= 65 && s.First() <= 90)) return false;

                var alpStr = Regex.Matches(s, @"[A-Z]+")[0].Value;

                //  若只有字母的判定过程。
                if (alpStr.Length == s.Length)
                {
                    var len = alpStr.Length;
                    bytArr = String2AscII(alpStr);
                    //  由Id生成规则判定，字母部分前一个字符的ASCII码不能小于后一个字符
                    for (int i = 0; i < len - 1; i++)
                    {
                        if (bytArr[i] < bytArr[i + 1])
                        {
                            return false;
                        }
                    }
                }

                //  以下是存在数字部分的判定
                else
                {
                    var numStr = Regex.Matches(s, @"\d+")[0].Value;

                    //  字母个数为零或者存在字母，数字意外的未知字符，不合法
                    if (alpStr.Length == 0 || s.Length != (alpStr.Length + numStr.Length)) return false;

                    bytArr = String2AscII(alpStr);
                    var l = bytArr.Length;

                    //  由Id生成规则判定，字母部分前一个字符的ASCII码不能小于后一个字符
                    for (int i = 0; i < l - 1; i++)
                    {
                        if (bytArr[i] < bytArr[i + 1])
                        {
                            return false;
                        }
                    }

                    //  以数字部分的长度，倒序截取该长度的字符串来判定是否仅是最后几位是数字。
                    var lastNumStr = s.Substring(alpStr.Length, numStr.Length);
                    lastNumStr = Regex.Matches(lastNumStr, @"\d+")[0].Value;
                    if (lastNumStr.Length != numStr.Length) return false;
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }

}
