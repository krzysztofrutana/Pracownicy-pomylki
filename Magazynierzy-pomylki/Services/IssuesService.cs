using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Text;
using Magazynierzy_pomylki.Database;
using Magazynierzy_pomylki.Helpers;
using Magazynierzy_pomylki.Models;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Magazynierzy_pomylki.Services
{
    public class IssuesService
    {
        public void AddNewIssue(AddIssueModel model)
        {
            using var connection = SqLiteContext.CreateConnection();
            string query = $@"INSERT INTO Issues (UserName, Change, Date, Description, CreatedBy)
                   VALUES ('{model.UserName}', {model.Change}, '{model.Date.Date.ToString(ConstHelper.DATE_FORMAT)}', '{model.Description}', '{model.CreatedBy}');";
            var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }

        public List<IssueModel> GetIssuesForDate(DateTime dateFrom, string warehouseman, string statusFilter, string change)
        {
            List<IssueModel> issues = new List<IssueModel>();
            using var connection = SqLiteContext.CreateConnection();
            StringBuilder query = new StringBuilder();
            query.Append($"SELECT * FROM Issues WHERE Date = '{dateFrom.Date.ToString(ConstHelper.DATE_FORMAT)}' ");
            if (!string.IsNullOrWhiteSpace(warehouseman))
            {
                query.Append($"AND UserName = '{warehouseman}' ");
            }
            if (!string.IsNullOrWhiteSpace(statusFilter) && !statusFilter.Contains(ConstHelper.EMPTY_LABEL_STATUS))
            {
                if (statusFilter.Contains(ConstHelper.WITHOUT_EXPLAIN_NEED_FILTER))
                {
                    query.Append($" AND IsDriverExplainNeed = 0 AND ExplainedAtDriver = 0 ");
                }
                else if (statusFilter.Contains(ConstHelper.TO_EXPLAIN_FILTER))
                {
                    query.Append($" AND IsDriverExplainNeed = 1 ");
                }
                else if (statusFilter.Contains(ConstHelper.EXPLAINED_FILTER))
                {
                    query.Append($" AND ExplainedAtDriver = 1 ");
                }
            }
            if (!string.IsNullOrWhiteSpace(change) && !change.Contains(ConstHelper.EMPTY_LABEL_STATUS))
            {
                if (change.Contains(ConstHelper.FIRST_CHANGE_FILTER))
                {
                    query.Append($" AND Change = 1 ");
                }
                else if (change.Contains(ConstHelper.SECOND_CHANGE_FILTER))
                {
                    query.Append($" AND Change = 2 ");
                }
            }

            query.Append("ORDER BY Id ASC;");

            var command = new SQLiteCommand(query.ToString(), connection);
            using SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                IssueModel model = PrepareModel(reader);

                issues.Add(model);
            }

            return issues;
        }


        public List<IssueModel> GetIssuesFromTo(DateTime dateFrom, DateTime dateTo)
        {
            List<IssueModel> issues = new List<IssueModel>();
            using var connection = SqLiteContext.CreateConnection();

            string query = $@"SELECT * FROM Issues WHERE Date >='{dateFrom.Date.ToString(ConstHelper.DATE_FORMAT)}' AND Date <='{dateTo.Date.ToString(ConstHelper.DATE_FORMAT)}';";
            var command = new SQLiteCommand(query, connection);
            using SQLiteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                IssueModel model = PrepareModel(reader);

                issues.Add(model);
            }

            return issues;
        }

        private static IssueModel PrepareModel(SQLiteDataReader reader)
        {
            var id = reader.GetInt32(0);

            var userName = reader.GetString(1);
            var changeAsNumber = reader.GetInt32(2);
            var dateString = reader.GetString(3);
            var date = DateTime.ParseExact(dateString, ConstHelper.DATE_FORMAT, CultureInfo.InvariantCulture);
            var description = reader.GetString(4);

            var isDriverExplainNeedInt = reader.GetInt32(5);
            bool isDriverExplainNeed = isDriverExplainNeedInt == 0 ? false : true;

            var explainedAtDriverInt = reader.GetInt32(6);
            bool explainedAtDriver = explainedAtDriverInt == 0 ? false : true;

            string driverExplain = String.Empty;
            if (!reader.IsDBNull(7))
            {
                driverExplain = reader.GetString(7);
            }

            var createdBy = reader.GetString(8);

            var model = new IssueModel()
            {
                Id = id,
                UserName = userName,
                ChangeAsNumber = changeAsNumber,
                Change = changeAsNumber == 1 ? "Dzienna" : "Nocna",
                IssueDate = date,
                Description = description,
                IsDriverExplainNeed = isDriverExplainNeed,
                ExplainedAtDriver = explainedAtDriver,
                DriverExplain = driverExplain,
                CreatedBy = createdBy
            };
            return model;
        }

        public void ExportToExcel(string filePath, List<IssueModel> issues)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet1 = workbook.CreateSheet("Pomyłki");

                var rowIndex = 0;
                IRow rowHeader = sheet1.CreateRow(rowIndex);
                rowHeader.CreateCell(0).SetCellValue("Utworzone przez");
                rowHeader.CreateCell(1).SetCellValue("Imię i nazwisko");
                rowHeader.CreateCell(2).SetCellValue("Data");
                rowHeader.CreateCell(3).SetCellValue("Zmiana");
                rowHeader.CreateCell(4).SetCellValue("Opis błędu");
                rowHeader.CreateCell(5).SetCellValue("Liczba");
                rowHeader.CreateCell(6).SetCellValue("Potrzebne wyjaśnienie");
                rowHeader.CreateCell(7).SetCellValue("Wyjaśnienie");
                rowIndex++;
                foreach (var item in issues)
                {
                    IRow row = sheet1.CreateRow(rowIndex);
                    row.CreateCell(0).SetCellValue(item.CreatedBy);
                    row.CreateCell(1).SetCellValue(item.UserName);
                    row.CreateCell(2).SetCellValue(item.IssueDate.ToString("dd.MM.yyyy"));
                    row.CreateCell(3).SetCellValue(item.Change);
                    row.CreateCell(4).SetCellValue(item.Description);
                    row.CreateCell(5).SetCellValue(1);
                    row.CreateCell(6).SetCellValue(item.WithoutrDriverExplain ? "TAK" : "NIE");
                    row.CreateCell(7).SetCellValue(item.DriverExplain);

                    rowIndex++;
                }

                workbook.Write(fs);
            }
        }

        public void DeleteIssue(int id)
        {
            using var connection = SqLiteContext.CreateConnection();
            string query = $@"DELETE FROM Issues WHERE Id = {id};";
            var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }

        public void UpdateDriverExplainNeeded(int id, bool isDriverExplainNeed)
        {
            using var connection = SqLiteContext.CreateConnection();
            string query = string.Empty;
            if (isDriverExplainNeed)
            {
                query = $@"Update Issues SET IsDriverExplainNeed = 1 WHERE Id = {id}";
            }
            else
            {
                query = $@"Update Issues SET IsDriverExplainNeed = 0 WHERE Id = {id}";

            }

            var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();
        }

        public void UpdateDriverExplain(int id, string driverExplain)
        {
            using var connection = SqLiteContext.CreateConnection();
            string query = $@"Update Issues SET DriverExplain = '{driverExplain}' WHERE Id = {id}";

            var command = new SQLiteCommand(query, connection);
            command.ExecuteNonQuery();

            string query2 = $@"Update Issues SET ExplainedAtDriver = 1 WHERE Id = {id}";

            var command2 = new SQLiteCommand(query2, connection);
            command2.ExecuteNonQuery();
        }
    }
}
