using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace CargoConversionTool
{
    public class IATAPeriodHelper
    {
        /// <summary>
        /// Returns a List of the previous, current and future IATA period.
        /// this should cover all relevant IATA periods also in case there is a holday.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetIATAPeriods(bool currentPeriod, int NumPrevPeriods, int NumFutPeriods)
        {
            List<string> iataPeriods = new List<string>();
            string actualPeriod = CalculateIATAPeriod(DateTime.Today);
            string tmpPeriod = actualPeriod;
            if (currentPeriod) iataPeriods.Add(tmpPeriod);
            for (int i = 1; i <= NumPrevPeriods; i++)
            {
                tmpPeriod = GetPreviousPeriod(tmpPeriod);
                iataPeriods.Insert(0, tmpPeriod);
            }
            tmpPeriod = actualPeriod;
            for (int i = 1; i <= NumFutPeriods; i++)
            {
                tmpPeriod = GetNextPeriod(tmpPeriod);
                iataPeriods.Insert(iataPeriods.Count, tmpPeriod);
            }
            return iataPeriods;
        }

        public static string GetIATAPeriodForXML(string iATAPeriod)
        {
            string[] components = iATAPeriod.Split(new char[] { '-' });
            if (components.Count() == 3)
            {
                string dateComponent = string.Format("{0:MM}", DateTime.Parse(iATAPeriod));
                return components[0] + dateComponent + components[2];
            }
            return "";
        }


        /// <summary>
        /// Calculates the IATA period based on a given Date
        /// </summary>
        /// <param name="baseDate"></param>
        /// <returns></returns>
        private static string CalculateIATAPeriod(DateTime baseDate)
        {
            string IATAYear;
            string IATAMonth;
            string IATAPeriod;

            //Calculate the Year
            DateTime upperBoundDate = new DateTime(baseDate.Year, 1, 8);
            if (baseDate < upperBoundDate)
                IATAYear = string.Format("{0:yy}", new DateTime(baseDate.Year - 1, baseDate.Month, baseDate.Day));
            else
                IATAYear = string.Format("{0:yy}", baseDate);

            //Calculate the Month
            upperBoundDate = new DateTime(baseDate.Year, baseDate.Month, 8);
            if (baseDate < upperBoundDate)
                IATAMonth = string.Format("{0:MMM}", new DateTime(baseDate.Year, baseDate.Month - 1, baseDate.Day));
            else
                IATAMonth = string.Format("{0:MMM}", baseDate);

            //Calculate the Period

            if (7 < baseDate.Day && baseDate.Day < 15)
                IATAPeriod = "01";
            else if (14 < baseDate.Day && baseDate.Day < 22)
                IATAPeriod = "02";
            else if (21 < baseDate.Day && baseDate.Day < 28)
                IATAPeriod = "03";
            else
                IATAPeriod = "04";
            return IATAYear + "-" + IATAMonth + "-" + IATAPeriod;
        }

        private static string GetNextPeriod(string iataPeriod)
        {
            string[] iataPeriodComponents = iataPeriod.Split(new char[] { '-' });
            if (iataPeriodComponents.Count() == 3)
            {
                int iataCycle = Convert.ToInt32(iataPeriodComponents[2]);
                if (1 <= iataCycle && iataCycle <= 3)
                {
                    iataPeriodComponents[2] = "0" + (iataCycle + 1).ToString();
                }
                else if (iataPeriodComponents[1] == string.Format("{0:MMM}", new DateTime(2000, 12, 1)))
                {
                    DateTime tempDate = new DateTime(Convert.ToInt32(iataPeriodComponents[0]) + 1, 1, 1);
                    iataPeriodComponents[1] = string.Format("{0:MMM}", tempDate);
                    iataPeriodComponents[0] = string.Format("{0:yy}", tempDate);
                    iataPeriodComponents[2] = "01";
                }
                else
                {
                    DateTime tempDate = DateTime.ParseExact(iataPeriodComponents[1], "MMM", CultureInfo.CurrentCulture);
                    tempDate = tempDate.AddMonths(1);
                    iataPeriodComponents[1] = string.Format("{0:MMM}", tempDate);
                    iataPeriodComponents[2] = "01";
                }
                return iataPeriodComponents[0] + "-" + iataPeriodComponents[1] + "-" + iataPeriodComponents[2];
            }
            return "";
        }

        private static string GetPreviousPeriod(string iataPeriod)
        {
            string[] iataPeriodComponents = iataPeriod.Split(new char[] { '-' });
            if (iataPeriodComponents.Count() == 3)
            {
                int iataCycle = Convert.ToInt32(iataPeriodComponents[2]);
                //If Period is between 2- 4, just return period -1
                if (2 <= iataCycle && iataCycle <= 4)
                {
                    iataPeriodComponents[2] = "0" + (iataCycle - 1).ToString();
                }
                //if Period = 1 and it is Jan, return Period 4 in Dec of Previous year
                else if (iataPeriodComponents[1] == string.Format("{0:MMM}", new DateTime(2000, 1, 1)))
                {
                    DateTime tempDate = new DateTime(Convert.ToInt32(iataPeriodComponents[0]) - 1, 12, 1);
                    iataPeriodComponents[1] = string.Format("{0:MMM}", tempDate);
                    iataPeriodComponents[0] = string.Format("{0:yy}", tempDate);
                    iataPeriodComponents[2] = "04";
                }
                //for all other months, return month -1 and period 4
                else
                {
                    DateTime tempDate = DateTime.ParseExact(iataPeriodComponents[1], "MMM", CultureInfo.CurrentCulture);
                    tempDate = tempDate.AddMonths(-1);
                    iataPeriodComponents[1] = string.Format("{0:MMM}", tempDate);
                    iataPeriodComponents[2] = "04";
                }
                return iataPeriodComponents[0] + "-" + iataPeriodComponents[1] + "-" + iataPeriodComponents[2];
            }
            return "";
        }
    }
}
