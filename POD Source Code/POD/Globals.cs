using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace POD
{
    public delegate void StepEventHandler(object sender, StepArgs e);
    public delegate void FinishEventHandler(object sender, FinishArgs e);
    public delegate void AnalysisErrorHandler(object sender, ErrorArgs e);
    public delegate void GetProjectInfoHandler(object sender, GetProjectInfoArgs e);

    

    public class ClipboardMetafileHelper
    {
	    [DllImport("user32.dll")]
	    static extern bool OpenClipboard(IntPtr hWndNewOwner);
	    [DllImport("user32.dll")]
	    static extern bool EmptyClipboard();
	    [DllImport("user32.dll")]
	    static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);
	    [DllImport("user32.dll")]
	    static extern bool CloseClipboard();
	    [DllImport("gdi32.dll")]
	    static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
	    [DllImport("gdi32.dll")]
	    static extern bool DeleteEnhMetaFile(IntPtr hemf);
	
	    // Metafile mf is set to a state that is not valid inside this function.
	    static public bool PutEnhMetafileOnClipboard( IntPtr hWnd, Metafile mf )
	    {
		    bool bResult = false;
		    IntPtr hEMF, hEMF2;
		    hEMF = mf.GetHenhmetafile(); // invalidates mf
		    if( ! hEMF.Equals( new IntPtr(0) ) )
		    {
			    hEMF2 = CopyEnhMetaFile( hEMF, new IntPtr(0) );
			    if( ! hEMF2.Equals( new IntPtr(0) ) )
			    {
				    if( OpenClipboard( hWnd ) )
				    {
					    if( EmptyClipboard() )
					    {
						    IntPtr hRes = SetClipboardData( 14 /*CF_ENHMETAFILE*/, hEMF2 );
						    bResult = hRes.Equals( hEMF2 );
						    CloseClipboard();
					    }
				    }
			    }
			    DeleteEnhMetaFile( hEMF );
		    }
		    return bResult;
	    }
    }

    public enum AxisKind
    {
        X,
        Y
    }

    public static class Globals
    {
        public delegate double InvertAxisFunction(double x);

        public const int DefaultLabelCount = 10;
        //public static int AnnoyingBug = 0;
        public static int GetLabelIntervalBasedOnChartSize(Control chart, AxisKind kind)
        {
            if (chart != null)
            {
                var scale = CalculateScreenScaling(chart);

                int width = Convert.ToInt32(chart.Width / scale); //Convert.ToInt32(chart.Width - ((scale - 1) * chart.Width));
                int height = Convert.ToInt32(chart.Height / scale);//Convert.ToInt32(chart.Height - ((scale - 1) * chart.Height));

                switch (kind)
                {
                    case AxisKind.X:
                    case AxisKind.Y:
                        return Convert.ToInt32(width / 100) + 1;
                    default:
                        break;
                }
            }

            return DefaultLabelCount;
        }

        public const string UndefinedProjectName = "<undefined>";

        public const string ReportDockLabel = "Report Viewer";
        public const string ProjectDockLabel = "Project Manager";
        public const string ProgressDockLabel = "Analysis Progress";
        public const string SnashotDockLabel = "Snapshot Manager";
        public const string HandbookDockLabel = "1823A Help";
        public const string QuickDockLabel = "Quick Help";
        public const string WizardDockLabel = "Analysis";

        public const string SkillTutorialLabel = "Tutorial";
        public const string SkillNormalLabel = "LogisticRegression";
        public const string SkillBeginnerLabel = "Training";
        public const string SkillExpertLabel = "Advanced";

        public const string NotApplicable = "Not Applicable";

        public const int ActionBarColor = 0x00C0C0FF;

        public const string DefaultQuickHelpFile = "Example Quick Help.rtf";

        public const int ChartRangeBufferPercentage = 25;

        public static int StdWidth(Control control)
        {
            var scale = 1.0;

            if (control != null)
            {
                using (var g = control.CreateGraphics())
                {
                    scale = g.DpiX / 96.0;
                }
            }

            return Convert.ToInt32(70 * scale);
        }

        public static int StdHeight(Control control)
        {
            var scale = CalculateScreenScaling(control);

            return Convert.ToInt32(59 * scale);
        }

        private static double CalculateScreenScaling(Control control)
        {
            var scale = 1.0;

            if (control != null)
            {
                using (var g = control.CreateGraphics())
                {
                    scale = g.DpiX / 96.0;
                }
            }
            return scale;
        }

        public static Color AlphaOverWhiteToOpaque(Color color)
        {
            var useColor = Color.FromArgb(
                (int)((color.R) * (color.A / 255.0) + 255 - color.A),
                (int)((color.G) * (color.A / 255.0) + 255 - color.A),
                (int)((color.B) * (color.A / 255.0) + 255 - color.A));

            return useColor;
        }

        public static string CleanColumnName(string myName)
        {
            // Replace invalid characters with empty strings. 
            try
            {
                return Regex.Replace(myName, @"([!@$~()#\\/=><+/*%&|^'""[\]])+\s*", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,  
            // we should return Empty. 
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        //Dynamic Icon Hotspot Code taken from: http://stackoverflow.com/questions/550918/change-cursor-hotspot-in-winforms-net
        //By Nick (http://stackoverflow.com/users/1490/nick), Feb-15-2009
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        /// <summary>
        /// Create a cursor from a bitmap without resizing and with the specified
        /// hot spot
        /// </summary>
        public static Cursor CreateCursorNoResize(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            try
            {
                IntPtr ptr = bmp.GetHicon();
                IconInfo tmp = new IconInfo();
                GetIconInfo(ptr, ref tmp);
                tmp.xHotspot = xHotSpot;
                tmp.yHotspot = yHotSpot;
                tmp.fIcon = false;
                ptr = CreateIconIndirect(ref tmp);
                return new Cursor(ptr);
            }
            catch
            {
                MessageBox.Show("Problem creating cursor.");

                return Cursors.Default;
            }
        }



        public static string GetLongVersion(AnalysisDataTypeEnum analysisDataTypeEnum)
        {
            switch(analysisDataTypeEnum)
            {
                case AnalysisDataTypeEnum.AHat:
                    return "aHat vs a";
                case AnalysisDataTypeEnum.HitMiss:
                    return "Pass/Fail";
                case AnalysisDataTypeEnum.None:
                    return "None";
                default:
                    return "Undefined";
                    
            }
        }

        public static string PODv4Folder
        {
            get
            {
                return CreateMissingFolder(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\UDRI\\PODv4\\");
            }
        }

        private static string CreateMissingFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            return folder;
        }

        public static string PODv4ImageFolder
        {
            get
            {
                return CreateMissingFolder(PODv4Folder + "Images\\");
            }
        }

        public static string PODv4LogFolder
        {
            get
            {
                return CreateMissingFolder(PODv4Folder + "Logs\\");
            }
        }

        public static string PODv4DataFolder
        {
            get
            {
                return CreateMissingFolder(PODv4Folder + "Data\\");
            }
        }

        public static string PODv4MRUFile
        {
            get
            {
                return PODv4DataFolder + "mru.bin";
            }
        }

        public static string PODv4ProjectFile
        {
            get
            {
                return PODv4DataFolder + "Project.bin";
            }
        }

        public static string PODv4OperatorFile
        {
            get
            {
                return PODv4DataFolder + "Operator.bin";
            }
        }

        public static string PODv4InstrumentFile
        {
            get
            {
                return PODv4DataFolder + "Instrument.bin";
            }
        }

        public static string PODv4SpecimenFile
        {
            get
            {
                return PODv4DataFolder + "Specimen.bin";
            }
        }

        public static string PODv4HelpViewFile
        {
            get
            {
                return PODv4DataFolder + "HelpView.bin";
            }
        }

        public static string PODv4ParentFile
        {
            get
            {
                return PODv4DataFolder + "Parent.bin";
            }
        }

        public static string PODv4CustomerCompanyFile
        {
            get
            {
                return PODv4DataFolder + "Customer.bin";
            }
        }

        public static string PODv4NotesFile
        {
            get
            {
                return PODv4DataFolder + "Notes.bin";
            }
        }

        public static string PODv4CustomerNameFile
        {
            get
            {
                return PODv4DataFolder + "CustomerName.bin";
            }
        }

        public static string PODv4AnalystCompanyFile
        {
            get
            {
                return PODv4DataFolder + "Analyst.bin";
            }
        }

        public static string PODv4AnalystNameFile
        {
            get
            {
                return PODv4DataFolder + "AnalystName.bin";
            }
        }

        public static string PODv4LogFile
        {
            get
            {
                return PODv4LogFolder + "log.txt";
            }
        }

        public static string PODv4CommentsFile
        {
            get
            {
                return PODv4DataFolder + "RemoveComments.bin";
            }
        }

        public static void DeleteMRUList(string value, string fileName)
        {
            StreamReader sr = null;

            if (!File.Exists(fileName))
            {
                var writer = new StreamWriter(fileName);
                writer.Write("");
                writer.Close();
            }


            try
            {
                sr = new StreamReader(fileName);
                var line = "";
                var lines = new List<string>();
                var linesSplitted = new List<string>();

                while (line != null)
                {
                    line = sr.ReadLine();

                    if (line != null)
                    {
                        lines.Add(line);
                    }
                }
                                
                while (lines.Contains(value))
                {
                    int index = lines.IndexOf(value);
                    lines.RemoveAt(index);
                }

                sr.Close();

                StreamWriter sw = new StreamWriter(fileName, false);
                
                foreach (string newline in lines)
                {
                    sw.WriteLine(newline);
                }

                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Updating MRU List for" + fileName);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        public static void UpdateMRUListMultiLine(string value, string fileName, int maxLines = 8)
        {
            StreamReader sr = null;

            if (!File.Exists(fileName))
            {
                var writer = new StreamWriter(fileName);
                writer.Write("");
                writer.Close();
            }


            try
            {
                sr = new StreamReader(fileName);
                var line = "";
                var lines = new List<string>();
                var linesSplitted = new List<string>();

                line = sr.ReadToEnd();

                while (line.Length > 3)
                {
                    var newLine = SplitString("|", line);

                    linesSplitted.Add(newLine + "|");

                    line = line.Substring(newLine.Length+1);
                }

                while (linesSplitted.Contains(value))
                {
                    int index = linesSplitted.IndexOf(value);
                    linesSplitted.RemoveAt(index);
                }

                linesSplitted.Insert(0, value);

                sr.Close();

                StreamWriter sw = new StreamWriter(fileName, false);

                var writeIndex = 0;

                foreach (string newline in linesSplitted)
                {
                    if (writeIndex < maxLines)
                        sw.WriteLine(newline);
                    writeIndex++;
                }

                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Updating MRU List for" + fileName);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }
        }

        public static void UpdateMRUList(string value, string fileName, bool useSplitted = false, string splitCharacter = "|", int maxLines = 8)
        {
            StreamReader sr = null;

            if (!File.Exists(fileName))
            {
                var writer = new StreamWriter(fileName);
                writer.Write("");
                writer.Close();
            }
                

            try
            {
                sr = new StreamReader(fileName);
                var line = "";
                var lines = new List<string>();
                var linesSplitted = new List<string>();

                while (line != null)
                {
                    line = sr.ReadLine();

                    if (line != null)
                    {
                        lines.Add(line);
                        if(useSplitted)
                        {
                            linesSplitted.Add(SplitString(splitCharacter, line));
                        }

                    }
                }

                if (useSplitted)
                {
                    //remove values that match the first string of your set rather than the whole string
                    var splitted = SplitString(splitCharacter, value);

                    while (linesSplitted.Contains(splitted))
                    {
                        int index = linesSplitted.IndexOf(splitted);
                        lines.RemoveAt(index);
                        linesSplitted.RemoveAt(index);
                    }
                }
                else
                {
                    while (lines.Contains(value))
                    {
                        int index = lines.IndexOf(value);
                        lines.RemoveAt(index);
                    }
                }

                lines.Insert(0, value);

                sr.Close();

                StreamWriter sw = new StreamWriter(fileName, false);

                var writeIndex = 0;

                foreach (string newline in lines)
                {
                    if (writeIndex < maxLines)
                        sw.WriteLine(newline);
                    writeIndex++;
                }

                sw.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error Updating MRU List for" + fileName);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

        }

        private static string SplitString(string splitCharacter, string line)
        {
            if (line == null)
                return "";

            var index = line.IndexOf(splitCharacter);

            if (index >= 1)
                return line.Substring(0, index);
            else
                return "";
        }

        public static List<string> GetMRUListMultiLine(string fileName)
        {
            var lines = new List<string>();
            StreamReader sr = null;

            if (!File.Exists(fileName))
            {
                var writer = new StreamWriter(fileName);
                writer.Write("");
                writer.Close();
            }

            try
            {
                sr = new StreamReader(fileName);
                var line = "";


                line = sr.ReadToEnd();

                while (line.Length > 3)
                {
                    var newLine = SplitString("|", line);

                    lines.Add(newLine);

                    line = line.Substring(newLine.Length + 1);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error Loading MRU List for " + fileName);

                //give them an empty line to work with
                lines.Clear();
            }
            finally
            {
                if (sr != null)
                    sr.Close();


            }

            return lines;
        }

        public static List<string> GetMRUList(string fileName)
        {
            var lines = new List<string>();
            StreamReader sr = null;

            if (!File.Exists(fileName))
            {
                var writer = new StreamWriter(fileName);
                writer.Write("");
                writer.Close();
            }

            try
            {
                sr = new StreamReader(fileName);
                var line = "";

                while (line != null)
                {
                    line = sr.ReadLine();

                    if (line != null)
                        lines.Add(line);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error Loading MRU List for " + fileName);

                //give them an empty line to work with
                lines.Clear();
            }
            finally
            {
                if (sr != null)
                    sr.Close();

                
            }

            return lines;
        }

        public static List<string> GetMRUListWithoutEmpties(string fileName, bool useSplitted = false, string splitCharacter = "|")
        {
            var lines = new List<string>();
            StreamReader sr = null;

            if (!File.Exists(fileName))
            {
                var writer = new StreamWriter(fileName);
                writer.Write("");
                writer.Close();
            }

            try
            {
                sr = new StreamReader(fileName);
                var line = "";

                while (line != null)
                {
                    line = sr.ReadLine();

                    if (useSplitted)
                    {
                        var splitted = SplitString(splitCharacter, line);

                        if (splitted != null && splitted != "")
                            lines.Add(line);
                    }
                    else
                    {
                        if (line != null && line != "")
                            lines.Add(line);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error Loading non-empty MRU List for " + fileName);

                //give them an empty line to work with
                lines.Clear();
            }
            finally
            {
                if (sr != null)
                    sr.Close();


            }

            return lines;
        }

        public static string FileTimeStamp
        {
            get
            {
                return " (" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ")";
            }
        }




        public static string RandomImageFileName
        {
            get
            {
                if(!Directory.Exists(Globals.PODv4ImageFolder))
                    Directory.CreateDirectory(Globals.PODv4ImageFolder);

                return Globals.PODv4ImageFolder + Globals.FileTimeStamp + ".png";
            }
        }

        public static string NormalityChart { get; set; }

        public static void CleanUpRandomImageFiles()
        {
            if(!Directory.Exists(Globals.PODv4ImageFolder))
                Directory.CreateDirectory(Globals.PODv4ImageFolder);

            var files = Directory.GetFiles(Globals.PODv4ImageFolder);

            foreach(var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    //check here why it failed and ask user to retry if the file is in use.
                }
            }
        }

        public static string SplitIntoLines(string p)
        {
            var myCharArray = p.Trim().ToCharArray();
            int newLineCounter = 0;
            for (int i = 0; i < myCharArray.Length; i++)
            {
                if (myCharArray[i] == ' ' && newLineCounter > 40)
                {
                    myCharArray[i] = '\n';
                    newLineCounter = 0;
                    continue;
                }

                newLineCounter += 1;
            }
            return new string(myCharArray);
        }
    }

    /// <summary>
    /// Decides where the navigation bar should be placed
    /// </summary>
    public enum ControlOrganize
    {
        NaviBottom,
        NaviTop
    }

    public static class LinearityChartLabels
    {
        public const string FlawEstimate = "Flaw Estimate";
        public const string FlawResidual = "Flaw Residual";
        public const string ModelCompare = "Model Compare";
        public const string Uncensored = "Uncensored";
        public const string Original = "Original";
        public const string LeftCensor = "Left Censor";
        public const string RightCensor = "Right Censor";
        public const string CompleteCensored = "Complete Censored";
        public const string PartialCensored = "Partial Censored";
        public const string LegendTitle = "Fitted Data";
    }

    public enum ChartDataType
    {
        Input,
        TransformedInput,
        ResidualOutput,
        TransformedResidualOutput
    }

    public enum TestRating
    {
        P1,
        P05to1,
        P025to05,
        P01to025,
        P005to01,
        P005,
        Undefined
    }

    public enum ChartPartType
    {
        LinearFit,
        POD,
        CensorLeft,
        CensorRight,
        CrackMin,
        CrackMax,
        A50,
        A90,
        A9095,
        Decision,
        Undefined
    }

    public static class FontHelper
    {
        static public int MeasureDisplayStringWidth(Graphics graphics, string text,
                                            Font font)
        {
            System.Drawing.StringFormat format  = new System.Drawing.StringFormat ();
            System.Drawing.RectangleF rect    = new System.Drawing.RectangleF(0, 0, 1000, 1000);
            System.Drawing.CharacterRange[] ranges  =  { new System.Drawing.CharacterRange(0, text.Length) };
            System.Drawing.Region[] regions = new System.Drawing.Region[1];

            format.SetMeasurableCharacterRanges (ranges);

            regions = graphics.MeasureCharacterRanges (text, font, rect, format);
            rect    = regions[0].GetBounds (graphics);

            return (int)(rect.Right + 1.0f);
        }
    }

    public static class TestRatingLabels
    {
        public const string P1 = "P > 0.1";
        public const string P05to1 = "0.05 < P <= 0.1";
        public const string P025to05 = ".025 < P <= 0.05";
        public const string P01to025 = "0.01 < P <= .025";
        public const string P005to01 = ".005 < P <= 0.01";
        public const string P005 = "P <= .005";
        public const string Undefined = "Undefined";

        private static ReadOnlyCollection<string> Labels
        {
            get
            {
                return new ReadOnlyCollection<string>(new [] { P1, P05to1, P025to05, P01to025, P005to01, P005 });
            }
        }

        private static ReadOnlyCollection<TestRating> Values
        {
            get
            {
                return new ReadOnlyCollection<TestRating>(new[] { TestRating.P1, TestRating.P05to1, TestRating.P025to05, 
                                                                  TestRating.P01to025, TestRating.P005to01, TestRating.P005 });
            }
        }

        public static TestRating ValueFromLabel(string myRating)
        {
            var label = TestRating.Undefined;
            var values = Labels;

            var index = values.IndexOf(myRating);

            if (index >= 0)
                label = Values[index];

            return label;
        }
    }

    public static class Compute
    {
        public static void MinMax(DataTable myTable, DataColumn column, ref double myMin, ref double myMax)
        {
            var value = 0.0;

            foreach (DataRow row in myTable.Rows)
            {
                if (double.TryParse(row[column].ToString(), out value) == true)
                {
                    if (value < myMin)
                        myMin = value;
                    else if (value > myMax)
                        myMax = value;
                }

            }
        }

        public static void MinMax(DataTable myTable, DataColumn column, ref double myMin, ref double myMax, double xMin, double xMax, DataColumn xCol)
        {
            var value = 0.0;
            var xValue = 0.0;

            foreach (DataRow row in myTable.Rows)
            {
                var yParse = double.TryParse(row[column].ToString(), out value);
                var xParse = double.TryParse(row[xCol].ToString(), out xValue);

                if (xParse && yParse)
                {
                    if (xValue >= xMin && xValue <= xMax)
                    {
                        if (value < myMin)
                            myMin = value;
                        else if (value > myMax)
                            myMax = value;
                    }
                }

            }
        }

        public static void SanityCheck(ref double myMin, ref double myMax)
        {
            if (myMax == InitMaxValue)
                myMax = 1.0;

            if (myMin == InitMinValue)
                myMin = 0.0;
        }

        public static double InitMinValue
        {
            get
            {
                return double.MaxValue;
            }
        }

        public static double InitMaxValue
        {
            get
            {
                return double.MinValue;
            }
        }

        
    }

    public static class PODRegressionLabels
    {        
        public const string a50Line = "a50 Line";
        public const string a90Line = "a90 Line";
        public const string a9095Line = "a9095 Line";
        public const string BestFitLine = "Fit Line";

    }

    public static class PODChartLabels
    {
        public const string POD = "POD90";
        public const string POD9095 = "POD9095";
        public const string a50Area = "a50Area";
        public const string a90Area = "a90Area";
        public const string a9095Area = "a9095Area";
        public const string a50Horizontal = "a50Horizontal";
        public const string a90Horizontal = "a90Horizontal";
        public const string a50Line = "a50Line";
        public const string a90Line = "a90Line";
        public const string a9095Line = "a9095Line";
        public const string POD_All = "POD90_All";
        public const string POD9095_All = "POD9095_All";

    }

    public static class PODThresholdChartLabels
    {
        public const string POD90 = "POD90";
        public const string POD9095 = "POD9095";
        public const string POD90_All = "POD90_All";
        public const string POD9095_All = "POD9095_All";
        public const string a50Area = "a50Area";
        public const string a90Area = "a90Area";
        public const string a9095Area = "a9095Area";
        public const string ThresholdLine = "Threshold";
        public const string a50Line = "a50Line";
        public const string a90Line = "a90Line";
        public const string a9095Line = "a9095Line";
    }

    public static class NormalityChart
    {
        public const string NormalityHistogram = "NormalityHistogram";
        public const string NormalCurve = "NormalCurveOverlay";
    }
    public static class ChartColors
    {
        public static Color a50Color = Color.DarkSlateGray;
        public static Color a90Color = Color.RoyalBlue;
        public static Color a9095Color = Color.DarkOrange;
        public static Color POD90Color = a90Color;
        public static Color POD9095Color = a9095Color;
        public static Color ThresholdColor = a50Color;
        public static Color aMinColor = Color.Gray;
        public static Color aMaxColor = Color.Gray;
        public static Color LeftCensorColor = Color.Firebrick;
        public static Color RightCensorColor = Color.Firebrick;
        public static Color HorizontalColor = Color.Gray;
        public static Color FitColor = Color.Green;
        public static Color FitStdErrorColor = Color.LightGreen;
        public static Color UncensoredPoints = Color.MediumBlue;
        public static Color TestUnknownColor = Color.Yellow;
        public static Color TestFailColor = Color.Red;
        public static Color TestPassColor = Color.Green;
        public static Color CensoredPoints = LeftCensorColor;
        public static Color SemiCensoredPoints = Color.MediumPurple;

        public static int LineAlpha = 255;
        public static int AreaAlpha = 0;
        public static int BoundaryAreaAlpha = 16;
        public static int ControlBackColorAlpha = 40;
        public static int ModelCompareAlpha = 128;

        public static Color ResponseBarsColor = Color.RoyalBlue;
        public static Color NormalityCurveColor = Color.FromArgb(255, 130 , 2, 41);
        
    }

    public enum PODObjectTypeEnum
    {
        Project,
        Analysis,
        None
    }

    public enum AnalysisTypeEnum
    {
        Quick,
        Full,
        None,
        Undefined
    }

    public enum AnalysisDataTypeEnum
    {
        AHat,
        HitMiss,
        None,
        Undefined
    }

    public enum SkillLevelEnum
    {
        Tutorial,
        Training,
        Normal,
        Advanced,
        Quick,
        None
    }
        

    public enum WizardEnum
    {
        ProjectTutorial,
        ProjectTraining,
        ProjectNormal,
        ProjectQuick,
        AHatTutorial,
        AHatBeginner,
        AHatIntermediate,
        AHatExpert,
        HitMissTutorial,
        HitMissBeginner,
        HitMissIntermediate,
        HitMissExpert,
        HitMissSolveAllAnalyses,
        QuickAHatTutorial,
        QuickAHatBeginner,
        QuickHitMissTutorial,
        QuickHitMissBeginner,
        None,        
    }

    public enum TransformTypeEnum
    {
        Log,
        Linear,
        Inverse,        
        Exponetial,
        BoxCox,
        Custom,
        None
    }
    public enum ConfidenceIntervalTypeEnum
    {
        StandardWald,
        ModifiedWald,
        LR,
        MLR
    }

    public enum SamplingTypeEnum
    {
        SimpleRandomSampling,
        RankedSetSampling
    }
    public enum HitMissRegressionType
    {
        LogisticRegression,
        FirthLogisticRegression
    }
    public enum RCalculationType
    {
        Full,
        Transform,
        ThresholdChange
    }
    public static class RangeNames
    {
        public static string SpecID = "Specimen ID";
        public static string MetaData = "Meta Data";
        public static string FlawSize = "Flaw Size";
        public static string Response = "Response";
    }
    public static class PrintingToConsole
    {
        //This method is for debugging purpose
        public static void printDT(DataTable data)
        {
            //Console.WriteLine();
            Debug.WriteLine('\n');
            Dictionary<string, int> colWidths = new Dictionary<string, int>();

            foreach (DataColumn col in data.Columns)
            {
                //Console.Write(col.ColumnName);
                Debug.Write(col.ColumnName);
                var maxLabelSize = data.Rows.OfType<DataRow>()
                        .Select(m => (m.Field<object>(col.ColumnName)?.ToString() ?? "").Length)
                        .OrderByDescending(m => m).FirstOrDefault();

                colWidths.Add(col.ColumnName, maxLabelSize);
                for (int i = 0; i < maxLabelSize - col.ColumnName.Length + 10; i++) Debug.Write(" ");
            }

            //Console.WriteLine();
            Debug.WriteLine('\n');
            int rowCounter = 0;
            int limit = 200;
            foreach (DataRow dataRow in data.Rows)
            {
                for (int j = 0; j < dataRow.ItemArray.Length; j++)
                {
                    //Console.Write(dataRow.ItemArray[j]);
                    Debug.Write((dataRow.ItemArray[j]).ToString());
                    for (int i = 0; i < colWidths[data.Columns[j].ColumnName] - dataRow.ItemArray[j].ToString().Length + 10; i++) Debug.Write(" ");
                }
                //Console.WriteLine();
                Debug.WriteLine('\n');
                rowCounter = rowCounter + 1;
                if (rowCounter >= limit)
                {
                    break;
                }
            }
            Debug.WriteLine('\n');
        }
    }

    public class ExtColProperty
    {
        public static string Unit = "Unit";
        public static string Max = "Maximum";
        public static string Min = "Minimum";
        public static string Thresh = "Threshold";
        public static string MaxPrev = "Previous Maximum";
        public static string MinPrev = "Previous Minimum";
        public static string ThreshPrev = "Previous Threshold";
        public static string Original = "Original";
        public static string NewName = "NewName";

        public static string UnitDefault = "";
        public static double MaxDefault = 0.0;
        public static double MinDefault = 0.0;
        public static double ThreshDefault = 0.0;
        public static string OriginalDefault(DataColumn column)
        {
            return column.ColumnName;
        }

        public static string GetDefaultValue(string colType)
        {
            if (colType == ExtColProperty.Unit)
                return ExtColProperty.UnitDefault;
            else if (colType == ExtColProperty.Max)
                return ExtColProperty.MaxDefault.ToString();
            else if (colType == ExtColProperty.Min)
                return ExtColProperty.MinDefault.ToString();
            else if (colType == ExtColProperty.Thresh)
                return ExtColProperty.ThreshDefault.ToString();
            else
                return "";
        }
        
    }
}
