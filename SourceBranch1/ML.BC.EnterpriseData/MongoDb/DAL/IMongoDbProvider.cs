using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ML.BC.EnterpriseData.MongoDb
{
    public interface IMongoDbProvider<T, in TId> where T : MongoDBEntity
    {
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="mObject">从MongoDBEntity派生的对象</param>
        /// <returns></returns>
        T Save(T mObject);
        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="mObject">从MongoDBEntity派生的对象</param>
        /// <returns></returns>
        T Insert(T mObject);
        /// <summary>
        /// 通过ID获得对象
        /// </summary>
        /// <param name="id">对象_id</param>
        /// <returns></returns>
        T GetById(TId id);
        /// <summary>
        /// 获得满足条件的第一个对象
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <returns></returns>
        T GetByCondition(Expression<Func<T, bool>> condition);
        /// <summary>
        /// 获得满足条件的所有对象
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <returns></returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> condition);
        /// <summary>
        /// 获得所有对象
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetAll();
        /// <summary>
        /// 修改对象
        /// </summary>
        /// <param name="mObject">从MongoDBEntity派生的对象</param>
        /// <returns></returns>
        T Update(T mObject);
        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="id">MongoDB该条记录的id</param>
        bool Delete(string id);
        /// <summary>
        /// 删除满足条件的所有对象
        /// </summary>
        /// <param name="condition">条件表达式</param>
        bool Delete(Expression<Func<T, bool>> condition);

        bool Remove();

        /// <summary>
        /// 通过文件名删除文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        void DeleteFileByName(string fileName, string pictureDBName);
        /// <summary>
        /// 返回符合条件的记录数
        /// </summary>
        /// <param name="condition">条件表达式</param>
        /// <returns></returns>
        long Count(Expression<Func<T, bool>> condition);
        /// <summary>
        /// 返回所有记录数
        /// </summary>
        /// <returns></returns>
        long Count();

        /// <summary>
        /// 字节数组方式保存文件
        /// </summary>
        /// <param name="fileData">数据内容</param>
        /// <param name="fileName">文件名</param>
        /// <param name="fileCollectionName"></param>
        void SaveFileByByteArray(byte[] fileData, string fileName, string fileCollectionName);

        /// <summary>
        /// 文件流的方式保存
        /// </summary>
        /// <param name="dataStream">数据流</param>
        /// <param name="fileName">保存到MongoDB中的名称</param>
        /// <param name="fileCollectionName">文件集合名称</param>
        /// <returns></returns>
        MongoGridFSFileInfo SaveFileByStream(Stream dataStream, string fileName, string fileCollectionName);

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="fileDBName">文件数据库名</param>
        /// <returns>Stream</returns>
        byte[] GetFileAsStream(string fileName, string fileDBName);

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="logObject">日志对象</param>
        /// <returns></returns>
        T WriteLog(T logObject);

        /// <summary>
        /// 获得日志记录
        /// </summary>
        /// <param name="logObjectId">日志对象Id</param>
        /// <returns></returns>
        T GetLog(ObjectId logObjectId);
    }
}
