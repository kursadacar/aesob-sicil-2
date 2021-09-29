﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTD.Core.Helpers
{
    public static class DateHelper
    {
        public static DateTime GetDateTimeCultural(string dateString)
        {
            DateTime result = DateTime.Now;
            try
            {
                result = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
            }
            catch
            {
                try
                {
                    result = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                }
                catch
                {
                    try
                    {
                        result = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                    }
                    catch
                    {
                        try
                        {
                            result = DateTime.ParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            try
                            {
                                result = DateTime.ParseExact(dateString, "MM.dd.yyyy", CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                try
                                {
                                    result = DateTime.ParseExact(dateString, "MM\\dd\\yyyy", CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    try
                                    {
                                        result = DateTime.ParseExact(dateString, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            result = DateTime.ParseExact(dateString, "dd\\MM\\yyyy", CultureInfo.InvariantCulture);
                                        }
                                        catch
                                        {
                                            try
                                            {
                                                result = DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                            }
                                            catch
                                            {
                                                return DateTime.Now;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
