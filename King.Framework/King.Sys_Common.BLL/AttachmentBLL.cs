using System.Data.Common;

namespace King.Sys_Common.BLL
{
    using King.Framework.Common;
    using System;
    using System.Data;
    using System.Drawing;
    using System.IO;

    public class AttachmentBLL
    {
        public static bool CheckHigthAndWidth(string first, string second, byte[] fileData)
        {
            if (first.IsNullOrEmpty() || second.IsNullOrEmpty())
            {
                return true;
            }
            string[] strArray = first.Split(new char[] { '*' });
            string[] strArray2 = second.Split(new char[] { '*' });
            using (Stream stream = new MemoryStream(fileData))
            {
                Image image = Image.FromStream(stream);
                int width = image.Width;
                int height = image.Height;
                return ((((width >= strArray[0].ToInt()) && (width <= strArray2[0].ToInt())) && (height >= strArray[1].ToInt())) && (height <= strArray2[1].ToInt()));
            }
        }

        public void GetAttPath(string attId, ref string Path, ref double Size)
        {
            try
            {
                attId = attId.TrimEnd(new char[] { ',' });
                using (BizDataContext context = new BizDataContext(true))
                {
                    string sql = string.Format("select FilePath,FileSize from t_attachment where  ATTACHMENT_ID = {0}", attId);
                    DataTable table = context.ExecuteDataTable(sql, new DbParameter[0]);
                    if (table.Rows.Count == 1)
                    {
                        Path = table.Rows[0]["FilePath"].ToString();
                        Size = Convert.ToDouble(table.Rows[0]["FileSize"]);
                    }
                }
            }
            catch
            {
            }
        }

        private int GetTableMaxValue(string tableName, string fieldName, int defaultValue, string sqlWhere, BizDataContext context)
        {
            int result = 0;
            string sql = string.Format("SELECT MAX({0}) maxValue FROM {1} WHERE 1=1 ", fieldName, tableName);
            if (!string.IsNullOrEmpty(sqlWhere))
            {
                sql = sql + sqlWhere;
            }
            object obj2 = context.ExecuteScalar(sql, new DbParameter[0]);
            if (((obj2 != null) && (obj2 != DBNull.Value)) && int.TryParse(obj2.ToString(), out result))
            {
                return result;
            }
            return defaultValue;
        }

        public void UpdateAtt(string EntityType, int TableKay, string attIdList)
        {
            try
            {
                attIdList = attIdList.TrimEnd(new char[] { ',' });
                using (BizDataContext context = new BizDataContext(true))
                {
                    string str;
                    int num = this.GetTableMaxValue("T_Attachment", "TableVersion", 1, "", context) + 1;
                    if (!string.IsNullOrEmpty(attIdList))
                    {
                        str = string.Format("update t_attachment set OWNEROBJECTID='{0}',OWNERENTITYTYPE='{1}',TableVersion={3}\r\n            where ATTACHMENT_ID in ({2})", new object[] { TableKay, EntityType, attIdList, num });
                        context.ExecuteNonQuery(str, new DbParameter[0]);
                        str = string.Format("update t_attachment set state=9999,TableVersion={0} \r\n                                            where OWNEROBJECTID='{1}' and OWNERENTITYTYPE='{2}' and ATTACHMENT_ID not in ({3})", new object[] { num, TableKay, EntityType, attIdList });
                        context.ExecuteNonQuery(str, new DbParameter[0]);
                    }
                    else
                    {
                        str = string.Format("update t_attachment set state=9999,TableVersion={0} \r\n                                            where OWNEROBJECTID='{1}' and OWNERENTITYTYPE='{2}' ", num, TableKay, EntityType);
                        context.ExecuteNonQuery(str, new DbParameter[0]);
                    }
                }
            }
            catch
            {
            }
        }
    }
}

