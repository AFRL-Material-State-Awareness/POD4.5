using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POD.Controls
{
    public enum DefinitionMode
    {
        None,
        ID,
        MetaData,
        Flaw,
        Response,
        ClearAll,
        ClearOne
    }

    public enum CellStatus
    {
        Valid,
        Invalid,
        InvalidWholeRow,
    }

    public enum RowStatus
    {
        Valid,
        Invalid
    }

    public class CellTag
    {
        public CellStatus Status;
        public string AutoValue;

        public CellTag(CellStatus myStatus)
        {
            Status = myStatus;
        }
    }

    public class RowTag
    {
        public RowStatus Status;

        public RowTag(RowStatus myStatus)
        {
            Status = myStatus;
        }
    }

    public class ColTag
    {
        public DefinitionMode Mode;

        public ColTag(DefinitionMode myMode)
        {
            Mode = myMode;
        }
    }

}
