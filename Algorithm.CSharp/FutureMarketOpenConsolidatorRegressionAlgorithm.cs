/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Generic;
using QuantConnect.Interfaces;
using QuantConnect.Data;
using QuantConnect.Data.Consolidators;

namespace QuantConnect.Algorithm.CSharp
{
    /// <summary>
    /// Regression algorithm using a consolidator to check GetNextMarketClose() and GetNextMarketOpen()
    /// are returning the correct market close and open times
    /// </summary>
    public class FutureMarketOpenConsolidatorRegressionAlgorithm : QCAlgorithm, IRegressionAlgorithmDefinition
    {
        private Symbol _es;
        private static List<DateTime> _expectedOpens = new List<DateTime>(){
            new DateTime(2013, 10, 07, 16, 30, 0),
            new DateTime(2013, 10, 08, 16, 30, 0),
            new DateTime(2013, 10, 09, 16, 30, 0),
            new DateTime(2013, 10, 10, 16, 30, 0),
            new DateTime(2013, 10, 11, 16, 30, 0),
            new DateTime(2013, 10, 14, 16, 30, 0)
        };
        private static List<DateTime> _expectedCloses = new List<DateTime>(){
            new DateTime(2013, 10, 07, 17, 00, 0),
            new DateTime(2013, 10, 08, 17, 00, 0),
            new DateTime(2013, 10, 09, 17, 00, 0),
            new DateTime(2013, 10, 10, 17, 00, 0),
            new DateTime(2013, 10, 11, 17, 00, 0),
            new DateTime(2013, 10, 14, 17, 00, 0)
        };
        private Queue<DateTime> _expectedOpensQueue = new Queue<DateTime>(_expectedOpens);
        private Queue<DateTime> _expectedClosesQueue = new Queue<DateTime>(_expectedCloses);

        public override void Initialize()
        {
            SetStartDate(2013, 10, 06);
            SetEndDate(2013, 10, 14);

            var es = AddSecurity(SecurityType.Future, "ES");

            Consolidate<BaseData>(es.Symbol, time =>
            {
                var date = time;
                if (time >= new DateTime(2013, 10, 21))
                {
                    date = Time;
                }

                var start = es.Exchange.Hours.GetNextMarketOpen(date, false);
                var end = es.Exchange.Hours.GetNextMarketClose(start, false);
                var period = (end - start);
                return new CalendarInfo(start, period);
            }, bar => Assert(bar));
        }

        public void Assert(BaseData bar)
        {
            var open = _expectedOpensQueue.Dequeue();
            var close = _expectedClosesQueue.Dequeue();

            if (open != bar.Time || close != bar.EndTime)
            {
                throw new Exception($"Bar span was expected to be from {open} to {close}. " +
                    $"\n But was from {bar.Time} to {bar.EndTime}.");
            }

            Logging.Log.Debug($"Consolidator Event span. Start {bar.Time} End : {bar.EndTime}");
        }

        public bool CanRunLocally { get; } = true;

        /// <summary>
        /// This is used by the regression test system to indicate which languages this algorithm is written in.
        /// </summary>
        public Language[] Languages { get; } = { Language.CSharp };

        /// <summary>
        /// This is used by the regression test system to indicate what the expected statistics are from running the algorithm
        /// </summary>
        public Dictionary<string, string> ExpectedStatistics => new Dictionary<string, string>
        {
            {"Total Trades", "0"},
            {"Average Win", "0%"},
            {"Average Loss", "0%"},
            {"Compounding Annual Return", "0%"},
            {"Drawdown", "0%"},
            {"Expectancy", "0"},
            {"Net Profit", "0%"},
            {"Sharpe Ratio", "0"},
            {"Probabilistic Sharpe Ratio", "0%"},
            {"Loss Rate", "0%"},
            {"Win Rate", "0%"},
            {"Profit-Loss Ratio", "0"},
            {"Alpha", "0"},
            {"Beta", "0"},
            {"Annual Standard Deviation", "0"},
            {"Annual Variance", "0"},
            {"Information Ratio", "-3.108"},
            {"Tracking Error", "0.163"},
            {"Treynor Ratio", "0"},
            {"Total Fees", "$0.00"},
            {"Estimated Strategy Capacity", "$0"},
            {"Lowest Capacity Asset", ""},
            {"Fitness Score", "0"},
            {"Kelly Criterion Estimate", "0"},
            {"Kelly Criterion Probability Value", "0"},
            {"Sortino Ratio", "79228162514264337593543950335"},
            {"Return Over Maximum Drawdown", "79228162514264337593543950335"},
            {"Portfolio Turnover", "0"},
            {"Total Insights Generated", "0"},
            {"Total Insights Closed", "0"},
            {"Total Insights Analysis Completed", "0"},
            {"Long Insight Count", "0"},
            {"Short Insight Count", "0"},
            {"Long/Short Ratio", "100%"},
            {"Estimated Monthly Alpha Value", "$0"},
            {"Total Accumulated Estimated Alpha Value", "$0"},
            {"Mean Population Estimated Insight Value", "$0"},
            {"Mean Population Direction", "0%"},
            {"Mean Population Magnitude", "0%"},
            {"Rolling Averaged Population Direction", "0%"},
            {"Rolling Averaged Population Magnitude", "0%"},
            {"OrderListHash", "d41d8cd98f00b204e9800998ecf8427e"}
        };
    }
}