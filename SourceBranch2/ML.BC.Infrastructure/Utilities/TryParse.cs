using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ML.BC.Infrastructure
{
    /// <summary>
    /// 数据转型
    /// </summary>
    public class TryParse
    {
        /// <summary>
        ///object to int 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int intTryParse(object obj, params int[] val)
        {
            int result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = int.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///object to int64 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Int64 int64TryParse(object obj, params Int64[] val)
        {
            Int64 result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = int.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///object to decimal 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal decimalTryParse(object obj, params decimal[] val)
        {
            decimal result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = decimal.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///object to decimal 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool boolTryParse(object obj, params bool[] val)
        {
            bool result = false;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = bool.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///object to decimal 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static char charTryParse(object obj, params char[] val)
        {
            char result = '0';
            if (val != null && val.Length > 0)
            {   
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = char.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }

        /// <summary>
        /// object to string 默认值: val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string stringTryParse(object obj, params string[] val)
        {
            string result = "";
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = obj.ToString();
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///  object to double 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static double doubleTryParse(object obj, params double[] val)
        {
            double result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = double.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///  object to float 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float floatTryParse(object obj, params float[] val)
        {
            float result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = float.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///  object to byte 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static byte byteTryParse(object obj, params byte[] val)
        {
            byte result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = byte.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }
        /// <summary>
        ///  object to int16 默认值:val
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static Int16 int16TryParse(object obj, params Int16[] val)
        {
            Int16 result = 0;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }
            try
            {
                if (obj != null)
                result = Int16.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;
        }

        /// <summary>
        /// object to DateTime
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val">默认值 val</param>
        /// <returns>DateTime</returns>
        public static DateTime DateTimeTryParse(object obj, params DateTime[] val)
        {
            DateTime result = DateTime.MinValue;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }

            try
            {
                if (obj != null)
                result = DateTime.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;

        }
        /// <summary>
        /// object to ToString
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string objectToString(object obj, params object[] val)
        {
            string result = "";
            if (val != null && val.Length > 0)
            {
                result = val[0].ToString();
            }

            try
            {
                if(obj!=null)
                result = obj.ToString();
            }
            catch (Exception)
            { }

            return result;

        }
         public static Guid objectToGuid(object obj, params Guid[] val)
        {
            Guid result = Guid.Empty;
            if (val != null && val.Length > 0)
            {
                result = val[0];
            }

            try
            {
                if (obj != null)
                    result = Guid.Parse(obj.ToString());
            }
            catch (Exception)
            { }

            return result;

        }

    }
}
