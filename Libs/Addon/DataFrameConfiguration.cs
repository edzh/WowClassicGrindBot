﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Libs
{
    public class DataFrameConfiguration
    {
        private readonly IColorReader colorReader;

        public DataFrameConfiguration(IColorReader colorReader)
        {
            this.colorReader = colorReader;
        }

        private const string ConfigurationFilename = "config.json";

        public static bool ConfigurationExists()
        {
            return File.Exists(ConfigurationFilename);
        }

        public static List<DataFrame> LoadConfiguration()
        {
            return JsonConvert.DeserializeObject<List<DataFrame>>(File.ReadAllText(ConfigurationFilename));
        }

        public static void SaveConfiguration(List<DataFrame> dataFrames)
        {
            string output = JsonConvert.SerializeObject(dataFrames);

            File.WriteAllText(ConfigurationFilename, output);
        }

        public List<DataFrame> CreateConfiguration(Bitmap bmp)
        {
            var dataFrames = new List<DataFrame>() { new DataFrame(new Point(1, 1), 0) };
            for (int dataframe = 1; dataframe < 400; dataframe++)
            {
                var point = GetFramePoint(bmp, dataframe, dataFrames.Last().point.X);
                if (point == null)
                {
                    break;
                }
                dataFrames.Add(new DataFrame(point.Value, dataframe));
            }

            return dataFrames;
        }

        private Point? GetFramePoint(Bitmap bmp, int dataframe, int startX)
        {
            for (int x = startX; x < bmp.Width; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    var point = new Point(x, y);
                    if (colorReader.GetColorAt(point, bmp).B == dataframe)
                    {
                        return point;
                    }
                }
            }

            return null;
        }
    }
}