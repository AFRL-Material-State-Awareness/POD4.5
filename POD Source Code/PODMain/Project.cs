using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POD.Analyze;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using POD.Data;
using POD.ExcelData;
using SpreadsheetLight;

namespace POD
{
    

    /// <summary>
    /// Holds global information about the project. Does not contain analyses.
    /// </summary>
    [Serializable]
    public class Project : WizardSource
    {
        #region Fields

        /// <summary>
        /// Properties of the project.
        /// </summary>
        ProjectProperties _properties;
        /// <summary>
        /// Tables that are used to feed data into the analyses.
        /// </summary>
        DataSources _sources;
        SourceInfos _infos;
        [NonSerialized]
        public AnalysisListHandler NeedAnalyses;
        [NonSerialized]
        public GetProjectInfoHandler NeedOpenAnalysis;
        #endregion

        #region Contrsutors

        /// <summary>
        /// Create a new project with no analysis type defined.
        /// </summary>
        public Project()
        {
            _sources = new DataSources();
            _properties = new ProjectProperties();
            _infos = new SourceInfos();

            AnalysisType = AnalysisTypeEnum.None;
            AnalysisDataType = AnalysisDataTypeEnum.None;
            ObjectType = PODObjectTypeEnum.Project;
        }

        /*/// <summary>
        /// Deserizliation constructor.
        /// </summary>
        /// <param name="myInfo"></param>
        /// <param name="myContext"></param>
        public Project(SerializationInfo myInfo, StreamingContext myContext)
        {
            _sources = (DataSources)myInfo.GetValue("DataSources", typeof(DataSources));
            _properties = (ProjectProperties)myInfo.GetValue("ProjectProperties", typeof(ProjectProperties));

            AnalysisType = (AnalysisTypeEnum)myInfo.GetValue("AnalysisType", typeof(AnalysisTypeEnum));
            AnalysisDataType = (AnalysisDataTypeEnum)myInfo.GetValue("AnalysisDataType", typeof(AnalysisDataTypeEnum));
            ObjectType = (PODObjectTypeEnum)myInfo.GetValue("PODObjectType", typeof(PODObjectTypeEnum));
        }*/

        #endregion

        #region Properties

        /// <summary>
        /// Get/set the name of the project.
        /// </summary>
        public override string Name
        {
            get
            {
                return _properties.Name;
            }

            set
            {
                _properties.Name = value;
            }
        }

        /// <summary>
        /// Get/set the projects properties.
        /// </summary>
        public ProjectProperties Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        /// <summary>
        /// Get a list of the project's data sources.
        /// </summary>
        public DataSources Sources
        {
            get
            {
                return _sources;
            }
        }

        /// <summary>
        /// Get a list of information about each source.
        /// </summary>
        public SourceInfos Infos
        {
            get
            {
                return _infos;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds new source from table
        /// </summary>
        /// <param name="myTable"></param>
        /// <param name="specID"></param>
        /// <param name="metaData"></param>
        /// <param name="flawSize"></param>
        /// <param name="response"></param>        
        public void AddSource(DataTable myTable, TableRange specID, TableRange metaData, TableRange flawSize, TableRange response, string sourceName)
        {
            if (_infos == null)
                _infos = new SourceInfos();

            var newSource = new DataSource(myTable, specID, metaData, flawSize, response);
            newSource.SourceName = sourceName;
            _sources.Add(newSource);

            if(!_infos.ContainsOriginal(sourceName))
            {
                _infos.Add(new SourceInfo(sourceName, sourceName, newSource));
            }
            else
            {
                var info = _infos.GetFromOriginalName(sourceName);

                info.UpdateDataSource(newSource);

                //force info to be same location as source
                _infos.Remove(info);
                _infos.Add(info);
            }
        }

        public void WriteToExcel(ExcelExport myWriter)
        {
            List<string> names = myWriter.Workbook.GetSheetNames(false);

            if(names.Contains("Project") == false)
            {
                myWriter.Workbook.AddWorksheet("Project");
            }

            myWriter.RemoveDefaultSheet();

            _properties.WriteToExcel(myWriter);            
        }

        public void WriteDataToExcel(ExcelExport myWriter)
        {
            _sources.WriteDataToExcel(myWriter);
        }

        public void WriteDataPropertiesToExcel(ExcelExport myWriter)
        {
            _sources.WriteDataPropertiesToExcel(myWriter);
        }

        /*/// <summary>
        /// Serializing the object.
        /// </summary>
        /// <param name="myInfo"></param>
        /// <param name="myContext"></param>
        public void GetObjectData(SerializationInfo myInfo, StreamingContext myContext)
        {
            myInfo.AddValue("DataSources", _sources);
            myInfo.AddValue("ProjectProperties", _properties);
            myInfo.AddValue("AnalysisType", AnalysisType);
            myInfo.AddValue("AnalysisDataType", AnalysisDataType);
            myInfo.AddValue("PODObjectType", ObjectType);
        }*/

        #endregion



        public void RequestAnalyses(ref AnalysisList currentAnalyses)
        {
            if (NeedAnalyses != null)
            {
                NeedAnalyses.Invoke(this, new AnalysisListArg(ref currentAnalyses));
            }
        }

        public override string GenerateFileName()
        {
            string name = Properties.Name;

            if (Properties.Parent.Length > 0)
                name += " - " + Properties.Parent;

            if (Properties.Analyst.Name.Length > 0)
                name += " - " + Properties.Analyst.Name;

            return name;
        }


        public void OpenAnalysis(string analysisName)
        {
            if(NeedOpenAnalysis != null)
            {
                NeedOpenAnalysis.Invoke(this, new GetProjectInfoArgs(analysisName));
            }
        }
    }
}
