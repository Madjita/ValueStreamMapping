using System;
using System.Collections.Generic;
using auntification.Models;
using DiplomReactNetCore.DAL.Models.DataBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;


namespace diplom2.MyExel
{
    public class Exel
    {
        IFormFile _file;


        List<string> Columns = new List<string>
        {
            "A","B","C","D","E","F","G",
        };

 

        public Exel([FromForm] IFormFile file)
        {
            _file = file;

            //string end = "";
            // If you use EPPlus in a noncommercial context
            // according to the Polyform Noncommercial license:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }



        public Dictionary<int, User> GetRowDataUsers()
        {
            if (_file == null)
                return null;

            var list = new Dictionary<int, User>();
            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {
                ExcelWorksheet _sheet = _package.Workbook.Worksheets[0];
               
                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;
                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                    if (row == 1)
                        continue;

                    var user = new User
                    {
                        Name = "",
                        Email = "",
                        Password = ""
                    };

                    for (int col = start.Column; col <= end.Column; col++)
                    { // ... Cell by cell...
                        object cellValue = _sheet.Cells[row, col].Text; // This got me the actual value I needed.
                        string item = cellValue.ToString();


                        switch (col)
                        {
                            //Name
                            case 2:
                                if (item != "" && row != 1)
                                {
                                    user.Name = item;
                                }
                                break;

                            //Почта
                            case 7:
                                if (item != "" && row != 1)
                                {
                                    user.Email = item;
                                }
                                break;

                            //Пароль
                            case 8:
                                if (item != "" && row != 1)
                                {
                                    //user.Password = BCrypt.Net.BCrypt.HashPassword(item);
                                }
                                break;
                        }
                    }

                    if (user.Name != "")
                    {
                        list.Add(row, user);
                    }
                }


                _package.Dispose();
            }

            return list;
        }


        public Dictionary<int, Productions> GetRowDataProductions()
        {
            if (_file == null)
                return null;

            var list = new Dictionary<int, Productions>();
            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {
                ExcelWorksheet _sheet = _package.Workbook.Worksheets["Production"];

                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;
                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                    if (row == 1)
                        continue;

                    var productions = new Productions
                    {
                        Name = "",
                    };

                    for (int col = start.Column; col <= end.Column; col++)
                    { // ... Cell by cell...
                        object cellValue = _sheet.Cells[row, col].Text; // This got me the actual value I needed.
                        string item = cellValue.ToString();


                        switch (col)
                        {
                            //Name
                            case 2:
                                if (item != "" && row != 1)
                                {
                                    productions.Name = item;
                                }
                                break;
                        }
                    }

                    if (productions.Name != "")
                    {
                        list.Add(row, productions);
                    }
                }

                _package.Dispose();
            }

            return list;
        }


        public Dictionary<int,BufferVSM> GetRowDataBufferVSM()
        {
            if (_file == null)
                return null;

            var list = new Dictionary<int, BufferVSM>();

            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {
                ExcelWorksheet _sheet = _package.Workbook.Worksheets[2];

                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;
                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                    if (row == 1)
                        continue;

                    var bufferSVSM = new BufferVSM
                    {
                        Name = "",
                        Type = "",
                        MinHold = 0,
                        Max = 0,
                        Value = 0,
                        ValueDefault = 0,
                        ReplenishmentSec = 0,
                        ReplenishmentCount = 0
                    };

                    for (int col = start.Column; col <= end.Column; col++)
                    { // ... Cell by cell...
                        object cellValue = _sheet.Cells[row, col].Text; // This got me the actual value I needed.
                        string item = cellValue.ToString();


                        switch (col)
                        {
                            //Name
                            case 2:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.Name = item;
                                }
                                break;


                            //Type
                            case 3:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.Type = item;
                                }
                                break;

                            //MinHold
                            case 4:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.MinHold = int.Parse(item);
                                }
                                break;

                            //Max
                            case 5:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.Max = int.Parse(item);
                                }
                                break;

                            //Value
                            case 6:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.Value = int.Parse(item);
                                }
                                break;

                            //ValueDefault
                            case 7:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.ValueDefault = int.Parse(item);
                                }
                                break;

                            //ReplenishmentSec
                            case 8:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.ReplenishmentSec = int.Parse(item);
                                }
                                break;

                            //ReplenishmentCount
                            case 9:
                                if (item != "" && row != 1)
                                {
                                    bufferSVSM.ReplenishmentCount = int.Parse(item);
                                }
                                break;
                        }
                    }

                    if (bufferSVSM.Name != "")
                    {
                        list.Add(row, bufferSVSM);
                    }
                }


                _package.Dispose();
            }

            return list;
        }

        public Dictionary<int, EtapVSM> GetRowDataEtapVSM()
        {
            if (_file == null)
                return null;

            var list = new Dictionary<int, EtapVSM>();

            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {
                ExcelWorksheet _sheet = _package.Workbook.Worksheets[3];

                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;
                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                    if (row == 1)
                        continue;

                    var etapVSM = new EtapVSM
                    {
                        Name = "",
                        Description = "",
                        DefaultTimeCircle = 0,
                        DefaultTimePreporation = 0,
                        DefaultAvailability = 0,
                        Parallel = false,
                    };

                    for (int col = start.Column; col <= end.Column; col++)
                    { // ... Cell by cell...
                        object cellValue = _sheet.Cells[row, col].Text; // This got me the actual value I needed.
                        string item = cellValue.ToString();


                        switch (col)
                        {
                            //Name
                            case 2:
                                if (item != "" && row != 1)
                                {
                                    etapVSM.Name = item;
                                }
                                break;


                            //Description
                            case 3:
                                if (item != "" && row != 1)
                                {
                                    etapVSM.Description = item;
                                }
                                break;

                            //DefaultTimeCircle
                            case 4:
                                if (item != "" && row != 1)
                                {
                                    etapVSM.DefaultTimeCircle = int.Parse(item);
                                }
                                break;

                            //DefaultTimePreporation
                            case 5:
                                if (item != "" && row != 1)
                                {
                                    etapVSM.DefaultTimePreporation = int.Parse(item);
                                }
                                break;

                            //Value
                            case 6:
                                if (item != "" && row != 1)
                                {
                                    etapVSM.DefaultAvailability = int.Parse(item);
                                }
                                break;

                            //ValueDefault
                            case 7:
                                if (item != "" && row != 1)
                                {
                                    etapVSM.Parallel = int.Parse(item) > 0 ? true : false ;
                                }
                                break;
                        }
                    }

                    if (etapVSM.Name != "")
                    {
                        list.Add(row, etapVSM);
                    }
                }


                _package.Dispose();
            }

            return list;
        }


        public Dictionary<int, JObject> GetRowDataEtapSections()
        {
            if (_file == null)
                return null;

            var list = new Dictionary<int, JObject>();

            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {
                ExcelWorksheet _sheet = _package.Workbook.Worksheets[4];

                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;
                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                    if (row == 1)
                        continue;

                    string etap = "";
                    string worker_name = "";

                    for (int col = start.Column; col <= end.Column; col++)
                    { // ... Cell by cell...
                        object cellValue = _sheet.Cells[row, col].Text; // This got me the actual value I needed.
                        string item = cellValue.ToString();


                        switch (col)
                        {
                            //Etap
                            case 2:
                                if (item != "" && row != 1)
                                {
                                    etap = item;
                                }
                                break;


                            //Worker name
                            case 3:
                                if (item != "" && row != 1)
                                {
                                    worker_name = item;
                                }
                                break;
                        }
                    }

                    if (etap != "" && worker_name != "")
                    {
                        var obj = new JObject();
                        obj.Add(new JProperty("etap", etap));
                        obj.Add(new JProperty("worker", worker_name));
                        list.Add(row, obj);
                    }
                }


                _package.Dispose();
            }

            return list;
        }



        public Dictionary<int, JObject> GetRowDataCardVSM()
        {
            if (_file == null)
                return null;

            var list = new Dictionary<int, JObject>();

            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {
                ExcelWorksheet _sheet = _package.Workbook.Worksheets[1];

                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;


                string production = "";

                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                    if (row == 1)
                        continue;

                    string bufer = "";
                    string etap = "";
                    string sections = "";
                   

                    for (int col = start.Column; col <= end.Column; col++)
                    { // ... Cell by cell...
                        object cellValue = _sheet.Cells[row, col].Text; // This got me the actual value I needed.
                        string item = cellValue.ToString();


                        switch (col)
                        {

                            //Production
                            case 1:
                                if (item != "" && row != 1)
                                {
                                    production = item;
                                }
                                break;

                            //Buffer
                            case 2:
                                if (item != "" && row != 1)
                                {
                                    bufer = item;
                                }
                                break;


                            //Etap
                            case 3:
                                if (item != "" && row != 1)
                                {
                                    etap = item;
                                }
                                break;

                            //Sections
                            case 4:
                                if (item != "" && row != 1)
                                {
                                    sections = item;
                                }
                                break;
                        }
                    }

                    if (production != "" && bufer != "" && etap != "" && sections != "")
                    {
                        var obj = new JObject();
                        obj.Add(new JProperty("production", production));
                        obj.Add(new JProperty("bufer", bufer));
                        obj.Add(new JProperty("etap", etap));
                        obj.Add(new JProperty("sections", sections));
                        list.Add(row, obj);
                    }
                }


                _package.Dispose();
            }

            return list;
        }


        ///Для крюгера
        public void Krg()
        {

          /*  if (_file == null)
                return;

            var list = new Dictionary<int, JObject>();

            using (ExcelPackage _package = new ExcelPackage(_file.OpenReadStream()))
            {

                ExcelWorksheet _sheet = _package.Workbook.Worksheets[1];


                var start = _sheet.Dimension.Start;
                var end = _sheet.Dimension.End;


                for (int row = start.Row; row <= end.Row; row++)
                { // Row by row...

                }
            }
          */

          
        }
    }
}
