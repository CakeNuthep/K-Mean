using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using CsvHelper;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace test
{
    public partial class Form1 : Form
    {

        //DataKMean origin1 = new DataKMean();
        //DataKMean origin2 = new DataKMean();
        //DataKMean origin3 = new DataKMean();
        const double errorAccept = 0;
        const int K = 3;
        List<DataKMean> listOrigin = new List<DataKMean>();
        List<DataKMean> listOldOrigin = new List<DataKMean>();
        List<DataKMean> listData = new List<DataKMean>();
        List<int> listGroup = new List<int>();
        double distanceError = 100;

        public SeriesCollection seriesCollection { get; set; }
        public Form1()
        {
            InitializeComponent();
            
            readCsv();
            var r = new Random();
            
            //foreach (var series in SeriesCollection)
            //{
            //    for (var i = 0; i < 20; i++)
            //    {
            //        series.Values.Add(new ObservablePoint(r.NextDouble() * 10, r.NextDouble() * 10));
            //    }
            //}

            //cartesianChart1.Series = SeriesCollection;
            //cartesianChart1.LegendLocation = LegendLocation.Bottom;
        }

      

        private void button1_Click_1(object sender, EventArgs e)
        {
            #region initital
            if (seriesCollection !=null && seriesCollection.Chart != null)
            {
                seriesCollection.Clear();
            }
            listOrigin.Clear();
            listOldOrigin.Clear();
            listGroup.Clear();
            listData.Clear();

            readCsv();
            for (int indexData = 0; indexData < listData.Count; indexData++)
            {
                listGroup.Add(0);
            }

            for (int i=0;i<K;i++)
            {
                
                int randIndex = randomValue(listData.Count);
                DataKMean origin = new DataKMean(listData[randIndex].V1,listData[randIndex].V2);
                listOrigin.Add(origin);
                listOldOrigin.Add(origin);
            }
            //initialGraph(listData,listOrigin);


            #endregion

            do
            {
                for (int indexData = 0; indexData < listData.Count; indexData++)
                {
                    double minDistance = -1;
                    for (int indexOrigin = 0; indexOrigin < listOrigin.Count; indexOrigin++)
                    {
                        double distance = calDistance(listData[indexData], listOrigin[indexOrigin]);
                        if (indexOrigin == 0)
                        {
                            minDistance = distance;
                            listGroup[indexData] = (indexOrigin + 1);
                        }
                        else if (distance < minDistance)
                        {
                            minDistance = distance;
                            listGroup[indexData] = (indexOrigin + 1);
                        }
                    }
                }

                for (int indexOrigin = 0; indexOrigin < listOrigin.Count; indexOrigin++)
                {
                    listOldOrigin[indexOrigin] = new DataKMean(listOrigin[indexOrigin].V1,
                        listOrigin[indexOrigin].V2);
                }

                for (int indexOrigin = 0; indexOrigin < listOrigin.Count; indexOrigin++)
                {
                    double sumV1 = 0, sumV2 = 0;
                    double count = 0;
                    for (int indexData = 0; indexData < listData.Count; indexData++)
                    {
                        if (listGroup[indexData] == (indexOrigin+1))
                        {
                            count++;
                            sumV1 += listData[indexData].V1;
                            sumV2 += listData[indexData].V2;
                        }
                    }
                    double avgV1 = sumV1 / count;
                    double avgV2 = sumV2 / count;
                    if (count != 0)
                    {
                        listOrigin[indexOrigin] = new DataKMean(avgV1, avgV2);
                    }
                    else
                    {
                        listOrigin[indexOrigin] = new DataKMean(listData[indexOrigin].V1,listOrigin[indexOrigin].V2);
                    }
                }
                double maxError = 0;
                for (int indexOrigin = 0; indexOrigin < listOrigin.Count; indexOrigin++)
                {
                    distanceError = calDistance(listOldOrigin[indexOrigin], listOrigin[indexOrigin]);
                    
                    if (maxError < distanceError)
                    {
                        maxError = distanceError;
                    }
                }
                distanceError = maxError;
                
                

            } while (distanceError > errorAccept);
            //var r = new Random();

            //foreach (var values in SeriesCollection.Select(x => x.Values))
            //{
            //    for (var i = 0; i < 20; i++)
            //    {
            //        ((ObservablePoint)values[i]).X = r.NextDouble() * 10;
            //        ((ObservablePoint)values[i]).Y = r.NextDouble() * 10;
            //    }
            //}
            //moveGraphOriginal(listOrigin);
            initialGraph(listData,listOrigin);
        }

        private void moveGraphOriginal(List<DataKMean> listOrigin)
        {
            //foreach (var values in SeriesCollection.Select(x => x.Values))
            {
                for (int i = 0; i < listOrigin.Count; i++)
                {
                    ((ObservablePoint)seriesCollection[3].Values[i]).X = listOrigin[i].V1;
                    ((ObservablePoint)seriesCollection[3].Values[i]).Y = listOrigin[i].V2;
                }
            }
            


        }

        private void initialGraph(List<DataKMean> listData, List<DataKMean> listOrigin)
        {
            seriesCollection = new SeriesCollection
            {
                 new ScatterSeries
                {
                    Title = "Origin",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Square,
                    StrokeThickness = 2
                },
                new ScatterSeries
                {
                    Title = "Group A",
                    Values = new ChartValues<ObservablePoint>(),
                    StrokeThickness = 2
                },
                new ScatterSeries
                {
                    Title = "Group B",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Diamond
                },
                new ScatterSeries
                {
                    Title = "Group C",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = DefaultGeometries.Triangle,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                }
            };
            //foreach (var series in SeriesCollection)
            {
                for (int indexListData = 0;indexListData < listData.Count;indexListData++)
                {
                    seriesCollection[listGroup[indexListData]-1].Values.Add(
                        new ObservablePoint(listData[indexListData].V1, listData[indexListData].V2));
                }
            }

            {
                for (int indexListGroup = 0; indexListGroup < listOrigin.Count; indexListGroup++)
                {
                    seriesCollection[3].Values.Add(
                        new ObservablePoint(listOrigin[indexListGroup].V1, listOrigin[indexListGroup].V2));
                }
            }

            cartesianChart1.Series = seriesCollection;
            cartesianChart1.LegendLocation = LegendLocation.Bottom;
        }

        public int randomValue(int maxIndex)
        {
            Random rand = new Random();
            int index = rand.Next(maxIndex);
            return index;
        }

        private double calDistance(DataKMean data1, DataKMean data2)
        {
            double diffY = data1.V2 - data2.V2;
            double diffX = data1.V1 - data2.V1;
            double distance = (Math.Sqrt((diffX * diffX) + (diffY * diffY)));
            return distance;
        }

        private void readCsv()
        {
            using (TextReader reader = File.OpenText(@"D:\LearningC#\page\LiveChart\test\test\Data\data.csv"))
            {
                CsvReader csv = new CsvReader(reader);
                csv.Configuration.Delimiter = ",";
                csv.Configuration.MissingFieldFound = null;
                while (csv.Read())
                {
                    DataKMean Record = csv.GetRecord<DataKMean>();
                    listData.Add(Record);
                }
            }
        }
    }
}
