using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace ML.BC.Infrastructure.Aop
{
    public class AopProxy<T> : RealProxy
    {
        private readonly T _decorated;
        log4net.ILog _ilog = log4net.LogManager.GetLogger("LOG");
        public AopProxy(T decorated)
            : base(typeof(T))
        {
            _decorated = decorated;
        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            var methodInfo = methodCall.MethodBase as MethodInfo;

            try
            {
                if (methodCall != null)
                {
                    string operargStr = "";

                    //是方法调用
                    if (methodCall.InArgs != null && methodCall.InArgCount > 0)
                    {

                        //operargStr=    Serializer.ToJson(methodCall.InArgs);
                        List<string> arglist = new List<string>();
                        int i = 0;
                        foreach (object argobj in methodCall.InArgs)
                        {

                            string _argName = methodCall.GetInArgName(i);
                            object _argValue = methodCall.GetInArg(i);
                            string _value = "";
                            var dic = methodCall.Properties;
                            if (_argValue != null)
                            {
                                Type argtype = _argValue.GetType();
                                if (argtype.IsClass && argtype.IsSerializable)
                                {
                                    //参数是类需要序列化
                                    _value = Serializer.ToJson(_argValue);
                                }
                                else
                                {
                                    _value = _argValue.ToString();
                                }
                            }
                            else
                            {
                                _value = "";
                            }
                            arglist.Add(_argName + ":" + _value);
                            i++;
                        }
                        operargStr = Serializer.ToJson(arglist);
                        _ilog.InfoFormat("Start Call: MethodName:{0},MethodArg:{1},CallTime:{2}", methodCall.MethodName, operargStr, DateTime.Now.ToFullStr());
                    }
                }
            }
            catch { }

            try
            {
                object[] objs = methodCall.Args;
                var result = methodInfo.Invoke(_decorated, objs);

                string resutStr = "";
                try
                {
                    resutStr = Serializer.ToJson(result);
                }
                catch { }

                _ilog.InfoFormat("End Call: MethodName:{0},Result:{1}", methodCall.MethodName, resutStr);
                var retMsg = new ReturnMessage(result, objs, methodCall.ArgCount, methodCall.LogicalCallContext, methodCall);
                return retMsg;
            }
            catch (Exception ex)
            {
                _ilog.InfoFormat("Call failed: MethodName:{0},Result:{1}", methodCall.MethodName, ex.GetAllMessages());
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw ex;
                }
            }
        }
    }
}
