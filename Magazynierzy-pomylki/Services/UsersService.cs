using System;
using System.Collections.Generic;
using System.IO;
using Magazynierzy_pomylki.Models;
using Newtonsoft.Json;
using Spire.Xls;

namespace Magazynierzy_pomylki.Services
{
    public class UsersService
    {
        public string ProjectPath { get; set; }
        public UsersService()
        {
            ProjectPath = AppDomain.CurrentDomain.BaseDirectory;
        }

        public List<string> GetUsers()
        {
            List<string> users = new List<string>();

            if (File.Exists($"{ProjectPath}/Magazynierzy.json"))
            {
                using (StreamReader r = new StreamReader($"{ProjectPath}/Magazynierzy.json"))
                {
                    string json = r.ReadToEnd();
                    users = JsonConvert.DeserializeObject<List<string>>(json);
                }
            }

            return users;
        }

        public List<OperatorModel> GetOperators()
        {
            List<OperatorModel> operators = new List<OperatorModel>();

            if (File.Exists($"{ProjectPath}/Użytkownicy.xlsx"))
            {
                Workbook wb = new Workbook();
                wb.OpenPassword = "Tajne_2022";
                wb.LoadFromFile($"{ProjectPath}/Użytkownicy.xlsx");
                Worksheet sheet = wb.Worksheets[0];
                for (int i = 0; i < sheet.Rows.Length; i++)
                {
                    if (i == 0)
                        continue;

                    var row = sheet.Rows[i];

                    var tempOperator = new OperatorModel();
                    tempOperator.Name = row?.CellList[0]?.Value;
                    tempOperator.Password = row?.CellList[1]?.Value;

                    operators.Add(tempOperator);
                }

            }

            return operators;
        }
    }
}
