﻿namespace Serenity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Text;
    using Dictionary = System.Collections.Generic.Dictionary<string, object>;


    public static class EntitySqlHelper
    {
       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       /// </remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetFirst(this SqlQuery query, IDbConnection connection)
       {
           using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       /// </remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetFirst(this SqlQuery query, IDbConnection connection, IEntity row, Dictionary param)
       {
           using (IDataReader reader = SqlHelper.ExecuteReader(connection, query, param))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader, new IEntity[] { row });
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetSingle(this SqlQuery query, IDbConnection connection)
       {
           using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   
                   if (reader.Read())
                       throw new InvalidOperationException("Query returned more than one result!");

                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kaydı yükler.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection)</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool GetSingle(this SqlQuery query, IDbConnection connection, Row row, Dictionary param)
       {
           using (IDataReader reader = SqlHelper.ExecuteReader(connection, query, param))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader, new Row[] { row });

                   if (reader.Read())
                       throw new InvalidOperationException("Query returned more than one result!");

                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kayıt için parametresiz bir callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi her satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="callBack">
       ///   Her kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool ForFirst(this SqlQuery query, IDbConnection connection,
           Action callBack)
       {
           using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   callBack();
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   varsa sorgunun döndürdüğü ilk kayıt için <see cref="IDataReader"/> parametresi alan bir 
       ///   callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForFirst(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi ilk satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="callBack">
       ///   İlk kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   Eğer en azından bir sonuç alındıysa <c>true</c></returns>
       public static bool ForFirst(this SqlQuery query, IDbConnection connection,
           ReaderCallBack callBack)
       {
           using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
           {
               if (reader.Read())
               {
                   query.GetFromReader(reader);
                   callBack(reader);
                   return true;
               }
               else
                   return false;
           }
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   sorgunun döndürdüğü her bir kayıt için parametresiz bir callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForEach(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi her satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="callBack">
       ///   Her kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   query.CountRecords true ise toplam kayıt sayısı, değilse 0.</returns>
       public static int ForEach(this SqlQuery query, IDbConnection connection,
           Action callBack)
       {
           int count = 0;

           if (query.Dialect().MultipleResultsets())
           {
               using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
               {
                   while (reader.Read())
                   {
                       query.GetFromReader(reader);
                       callBack();
                   }

                   if (query.CountRecords && reader.NextResult() && reader.Read())
                       return Convert.ToInt32(reader.GetValue(0));
               }
           }
           else
           {
               string[] queries = query.ToString().Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
               if (queries.Length > 1)
                   count = Convert.ToInt32(SqlHelper.ExecuteScalar(connection, queries[1], query.Params));

               using (IDataReader reader = SqlHelper.ExecuteReader(connection, queries[0], query.Params))
               {
                   while (reader.Read())
                   {
                       query.GetFromReader(reader);
                       callBack();
                   }
               }
           }

           return count;
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   sorgunun döndürdüğü her bir kayıt için <see cref="IDataReader"/> parametresi alan bir 
       ///   callback fonksiyonunu çağırır.</summary>
       /// <remarks>
       ///   <p>Bu bir extension metodu olduğundan direk <c>query.ForEach(connection, delegate() {...})</c> 
       ///   şeklinde de çalıştırılabilir.</p>
       ///   <p><c>query.GetFromReader(reader)</c> işlemi her satır için callback çağrılmadan 
       ///   önce çalıştırılır.</p>
       ///   <p>Eğer <see cref="SqlQuery.CacheTimeOut(int)"/> ile sorgu için saniye cinsinden bir önbellekleme 
       ///   süresi belirlenmişse bu değer kullanılır.</p></remarks>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="query">
       ///   Sorguyu içeren <see cref="SqlQuery"/> nesnesi.</param>
       /// <param name="callBack">
       ///   Her kayıt için çağrılacak olan callback fonksiyonu.</param>
       /// <returns>
       ///   query.CountRecords true ise toplam kayıt sayısı, değilse 0.</returns>
       public static int ForEach(this SqlQuery query, IDbConnection connection,
           ReaderCallBack callBack)
       {
           int count = 0;

           if (query.Dialect().MultipleResultsets())
           {
               using (IDataReader reader = SqlHelper.ExecuteReader(connection, query))
               {
                   while (reader.Read())
                   {
                       query.GetFromReader(reader);
                       callBack(reader);
                   }

                   if (query.CountRecords && reader.NextResult() && reader.Read())
                       return Convert.ToInt32(reader.GetValue(0));
               }
           }
           else
           {
               string[] queries = query.ToString().Split(new string[] { "\n---\n" }, StringSplitOptions.RemoveEmptyEntries);
               if (queries.Length > 1)
                   count = Convert.ToInt32(SqlHelper.ExecuteScalar(connection, queries[1], query.Params));

               using (IDataReader reader = SqlHelper.ExecuteReader(connection, queries[0], query.Params))
               {
                   while (reader.Read())
                   {
                       query.GetFromReader(reader);
                       callBack(reader);
                   }
               }
           }

           return count;
       }

       /// <summary>
       ///   <see cref="SqlQuery"/> nesnesinin içerdiği sorguyu bağlantı üzerinde çalıştırır ve
       ///   sonuçları <typeparamref name="TRow"/> tipinde row'lardan oluşan bir liste halinde döndürür.
       /// </summary>
       ///   <typeparamref name="TRow"/><see cref="Row"/>'dan türemiş bir row sınıfı.
       /// <param name="query">
       ///   Çalıştırılacak sorguyu içeren <see cref="SqlQuery"/> nesnesi</param>
       /// <param name="connection">
       ///   Sorgunun çalıştırılacağı bağlantı. Gerekirse otomatik olarak açılır.</param>
       /// <param name="loaderRow">
       ///   <paramref name="SqlQuery"/> sorgusunun döndürdüğü sonuçların GetFromReader ile içine yazılacağı
       ///   <typeparamref name="TRow"/> tipindeki nesne. Her satırın alan değerleri öncelikle bu row'a yüklenir ve 
       ///   kopyası çıkarılıp sonuç listesine eklenir.</param>
       /// <returns>
       ///   Sorgudan dönen kayıtların <typeparamref name="TRow"/> tipinde bir listesi</returns>
       public static List<TRow> List<TRow>(this SqlQuery query,
           IDbConnection connection, TRow loaderRow = null) where TRow : Row
       {
           var list = new List<TRow>();
           ForEach(query, connection, delegate()
           {
               list.Add(loaderRow.Clone());
           });
           return list;
       }

       public static void GetFromReader(this SqlQuery query, IDataReader reader)
       {
           GetFromReader(query, reader, query.IntoRows);
       }

       public static void GetFromReader(this SqlQuery query, IDataReader reader, IList<IEntity> into)
       {
           int index = 0;
           foreach (var info in query.GetColumns())
           {
               if (info.IntoField as Field != null && info.IntoRowIndex != -1)
               {
                   var row = into[info.IntoRowIndex];
                   ((Field)info.IntoField).GetFromReader(reader, index, (Row)row);
               }
               else if (info.IntoRowIndex != -1)
               {
                   var row = (Row)(into[info.IntoRowIndex]);
                   var name = reader.GetName(index);
                   var field = row.FindField(name) ?? row.FindFieldByPropertyName(name);
                   if (field != null)
                   {
                       //info.IntoField = field;
                       field.GetFromReader(reader, index, row);
                   }
                   else
                   {
                       if (reader.IsDBNull(index))
                           row.SetDictionaryData(name, null);
                       else
                       {
                           var value = reader.GetValue(index);
                           row.SetDictionaryData(name, value);
                       }
                   }
               }

               index++;
           }
       }
   }
}