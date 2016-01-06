using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Messaging;
using ML.BC.EnterpriseData.Model;
using ML.BC.EnterpriseData.Model.Extend;
using ML.BC.Infrastructure.Exceptions;
using ML.BC.Infrastructure.MsmqHelper;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoDB.Driver.GridFS;

namespace ML.BC.EnterpriseData.MongoDb
{
    public class MongoDbProvider<T> : IMongoDbProvider<T, string> where T : MongoDBEntity
    {
        private readonly MongoDatabase _database;
        private readonly string _collectionName = typeof(T).Name;
        private readonly MongoServer _server;
        private readonly MongoCollection<T> _collection;
        public MongoCollection<T> Collection
        {
            get { return _collection; }
        }

        public MongoDbProvider()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MongoBCEnterpriseDB"].ConnectionString;
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    MongoClient client = new MongoClient(connectionString);
                    MongoUrl mongourl = MongoUrl.Create(connectionString);
                    _server = client.GetServer();
                    _database = _server.GetDatabase(mongourl.DatabaseName);
                    _collection = _database.GetCollection<T>(_collectionName);
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                //  连接字符串错误写入log
            }
        }
        /// <summary>
        /// 实例化MongonDBProvider{T}
        /// </summary>
        /// <param name="connectionString">MongoDB连接字符串：mongodb://localhost/database_name</param>
        public MongoDbProvider(string connectionString)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    MongoClient client = new MongoClient(connectionString);
                    MongoUrl mongourl = MongoUrl.Create(connectionString);
                    _server = client.GetServer();
                    _database = _server.GetDatabase(mongourl.DatabaseName);
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                //  连接字符串错误写入log
            }
        }
        /// <summary>
        /// 配置连接池数据连接数据库
        /// </summary>
        /// <param name="maxConnectPoolSize">最大连接数</param>
        /// <param name="minConnectPoolSize">最小链接数</param>
        /// <param name="hostName">主机名</param>
        /// <param name="port">端口</param>
        /// <param name="database">数据库名</param>
        public MongoDbProvider(int maxConnectPoolSize, int minConnectPoolSize, string hostName, Int32 port, string database)
        {
            if (string.IsNullOrEmpty(hostName) && string.IsNullOrEmpty(database))
            {
                //  输出错误：主机名或数据库名为空
            }
            else
            {
                try
                {
                    MongoServerAddress ipaddress = new MongoServerAddress(hostName, port);

                    //  初始化连接数据
                    MongoClientSettings settingsClient = new MongoClientSettings
                    {
                        Server = ipaddress,
                        MaxConnectionPoolSize = maxConnectPoolSize,
                        MinConnectionPoolSize = minConnectPoolSize,
                        ConnectionMode = 0
                    };

                    // MongoUrl url=newMongoUrl(ConnectionString);  
                    MongoClient client = new MongoClient(settingsClient);
                    _server = client.GetServer();
                    _database = _server.GetDatabase(database);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        /// <summary>
        /// 保存或更新对象
        /// </summary>
        /// <param name="mObject">从MongoDBEntity派生的对象</param>
        /// <returns></returns>
        public virtual T Save(T mObject)
        {
            if (null != mObject)
            {
                try
                {
                    var re = _database.GetCollection<T>(_collectionName).Save(mObject);
                    HasMongoError(re);
                    return mObject;
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("保存对象为空！无法保存");
            }

        }
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="mObject">从MongoDBEntity派生的对象</param>
        /// <returns></returns>
        public virtual T Insert(T mObject)
        {
            if (null != mObject)
            {
                try
                {
                    var re = _database.GetCollection<T>(_collectionName).Insert(mObject);
                    HasMongoError(re);
                    if (CanSend(_collectionName))
                        Send2Msmq(mObject.Id, OperationEnum.Added);
                    return mObject;
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("保存对象为空!无法保存");
            }

        }

        #region GridFS存储

        /// <summary>
        /// 字节数组方式保存文件
        /// </summary>
        /// <param name="fileData">数据内容</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileCollectionName"></param>
        public virtual void SaveFileByByteArray(byte[] fileData, string fileName, string fileCollectionName)
        {
            try
            {
                if (0 == fileData.Length || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileCollectionName))
                    throw new KnownException("缺失必需数据，无法保存数据！");
                MongoGridFSSettings fsSetting = new MongoGridFSSettings() { Root = fileCollectionName };

                //MongoGridFS fs = new MongoGridFS(_database, fsSetting);   //  将被遗弃的方法
                MongoGridFS fs = new MongoGridFS(_server, _database.Name, fsSetting);
                MongoGridFSCreateOptions option = new MongoGridFSCreateOptions { UploadDate = DateTime.Now };
                using (MongoGridFSStream gfs = fs.Create(fileName, option))
                {
                    gfs.Write(fileData, 0, fileData.Length);
                    gfs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //  设定查询集合的名称
        }

        /// <summary>
        /// 文件流的方式保存
        /// </summary>
        /// <param name="dataStream">数据流</param>
        /// <param name="fileName">保存到MongoDB中的名称</param>
        /// <param name="fileCollectionName">文件集合名称</param>
        /// <returns></returns>
        public virtual MongoGridFSFileInfo SaveFileByStream(Stream dataStream, string fileName, string fileCollectionName)
        {
            try
            {
                if (0 == dataStream.Length || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileCollectionName))
                    throw new KnownException("缺失必需数据，无法保存数据！");
                MongoGridFSSettings fsSetting = new MongoGridFSSettings() { Root = fileCollectionName };
                MongoGridFS fs = new MongoGridFS(_server, _database.Name, fsSetting);
                return fs.Upload(dataStream, fileName);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 通过文件名删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        public void DeleteFileByName(string fileName, string pictureDBName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(pictureDBName))
                    throw new KnownException("缺失必需数据，无法删除数据！");
                MongoGridFSSettings fsSetting = new MongoGridFSSettings() { Root = pictureDBName };
                MongoGridFS fs = new MongoGridFS(_server, _database.Name, fsSetting);
                fs.Delete(fileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileDBName">文件数据库名</param>
        /// <returns>Stream</returns>
        public virtual byte[] GetFileAsStream(string fileName, string fileDBName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName)) throw new KnownException("名称为空，无法获取！");
                MongoGridFSSettings fsSetting = new MongoGridFSSettings() { Root = fileDBName };
                MongoGridFS fs = new MongoGridFS(_server, _database.Name, fsSetting);
                var gfInfo = fs.FindOne(fileName);
                using (var stream = gfInfo.OpenRead())
                {
                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    return bytes;
                }
            }
            catch (Exception )
            {
               
                return new byte[0];
            }
        }

        #endregion

        #region MongoDB日志存储
        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="logObject">日志对象</param>
        /// <returns></returns>
        public virtual T WriteLog(T logObject)
        {
            try
            {
                if (null == logObject) throw new KnownException("对象为空，不能写入！");
                var re = Save(logObject);
                return re;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// 获得日志记录
        /// </summary>
        /// <param name="logObjectId">日志对象Id</param>
        /// <returns></returns>
        public virtual T GetLog(ObjectId logObjectId)
        {
            return GetById(logObjectId.ToString());
        }

        #endregion
        /// <summary>
        /// 通过ID获得对象
        /// </summary>
        /// <param name="id">对象_id</param>
        /// <returns></returns>
        public virtual T GetById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    //_database.GetCollection<T>(_collectionName).FindOneById((BsonValue)id);
                    return _database.GetCollection<T>(_collectionName).FindOne(Query.EQ("_id", new ObjectId(id)));
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("id为空，无法查询！");
            }
        }
        /// <summary>
        /// 获得满足条件的第一个对象
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <returns></returns>
        public virtual T GetByCondition(Expression<Func<T, bool>> condition)
        {
            if (null != condition)
            {
                try
                {
                    return _database.GetCollection<T>(_collectionName).AsQueryable().Where(condition).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("条件为空不能查询！");
            }
        }
        /// <summary>
        /// 获得所有对象
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll()
        {
            try
            {
                return _database.GetCollection<T>(_collectionName).FindAll();
            }
            catch (Exception ex)
            {
                //  输出错误到log      
                throw ex;
            }
        }

        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="mObject">从MongoDBEntity派生的对象</param>
        /// <returns></returns>
        public virtual T Update(T mObject)
        {
            try
            {
                if (null != mObject && null != GetById(mObject.Id))
                {
                    var re = _database.GetCollection<T>(_collectionName).Save(mObject);
                    HasMongoError(re);
                    if (CanSend(_collectionName))
                        Send2Msmq(mObject.Id, OperationEnum.Modified);
                    return re.UpdatedExisting ? mObject : null;
                }
                else
                {
                    throw new KnownException("对象为空不能更新！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获得满足条件的所有对象
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <returns></returns>
        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> condition)
        {
            if (null != condition)
            {
                try
                {
                    return _database.GetCollection<T>(_collectionName).AsQueryable().Where(condition);
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("条件为空不能查询！");
            }
        }
        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="id">MongoDB该条记录的id</param>
        public virtual bool Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var re = _database.GetCollection<T>(_collectionName).Remove(Query.EQ("_id", new ObjectId(id)));
                    HasMongoError(re);
                    //if (CanSend(_collectionName))
                    //    Send2Msmq(id, OperationEnum.Deleted);
                    return re.DocumentsAffected == 1;
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("id为空不能查询！");
            }
        }

        public virtual bool Remove()
        {
            try
            {
                var re = _database.GetCollection<T>(_collectionName).RemoveAll();
                HasMongoError(re);
                return re.DocumentsAffected > 0;
            }
            catch (Exception ex)
            {
                //  输出错误到log
                throw ex;
            }
        }
        /// <summary>
        /// 删除满足条件的所有对象
        /// </summary>
        /// <param name="condition">条件表达式</param>
        public virtual bool Delete(Expression<Func<T, bool>> condition)
        {
            try
            {
                if (null == condition) throw new KnownException("查询表达式为空！");
                var del = GetAll(condition).ToList();
                if (del.Count <= 0) return false;
                if (del.Any(t => false == Delete(t.Id)))
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                //  输出错误到log
                throw ex;
            }
            return false;
        }

        /// <summary>
        /// 返回符合条件的记录数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <returns></returns>
        public virtual long Count(Expression<Func<T, bool>> condition)
        {
            if (null != condition)
            {
                try
                {
                    return _database.GetCollection<T>(_collectionName).AsQueryable().Where(condition).LongCount();
                }
                catch (Exception ex)
                {
                    //  输出错误到log
                    throw ex;
                }
            }
            else
            {
                throw new KnownException("条件为空不能查询！");
            }
        }
        /// <summary>
        /// 返回所有记录数
        /// </summary>
        /// <returns></returns>
        public virtual long Count()
        {
            try
            {
                return _database.GetCollection<T>(_collectionName).Count();
            }
            catch (Exception ex)
            {
                //  输出错误到log
                throw ex;
            }
        }

        private bool Send2Msmq(string id, OperationEnum operation)
        {
            try
            {
                var msg = new MessageItem
                {
                    EntityName = _collectionName,
                    Operation = operation,
                    Type = TypeName2TypeEnum(_collectionName),
                    ChangeTime = DBTimeHelper.DBNowTime(),
                    Data = new List<CustomKeyValue> { new CustomKeyValue() { Key = "Id", Value = id } }
                };

                using (var msmq = ML.BC.Infrastructure.Ioc.GetService<IMsmqProvider>())
                {
                    msmq.Send<List<MessageItem>>(new Message(new List<MessageItem> { msg }));
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static TypeEnum TypeName2TypeEnum(string name)
        {
            TypeEnum re = TypeEnum.SceneData;
            switch (name)
            {
                case "SceneItem": re = TypeEnum.SceneData; break;
            }
            return re;
        }

        private static bool CanSend(string name)
        {
            switch (name)
            {
                case "SceneItem": return true;
                default: return false;
            }
        }

        private static void HasMongoError(WriteConcernResult result)
        {
            if (result == null) throw new ArgumentNullException("result");
            if (result.HasLastErrorMessage)
                throw new KnownException(result.LastErrorMessage);
        }
    }
}
