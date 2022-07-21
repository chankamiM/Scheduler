using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler.Db_Models
{
    public class Files
    {
		public int rid;
		public string Uid ;
		public string Name;
		public string RegisterDT ;
		public string progress;
		public int fixstatus = -1;
		public string fixedPath;
		public string fixedDT;
		public string fixingDT;
		public string slicingDT ;
		public string slicedDT;
		public int slicedStatus;
		public string slicedPath;
		public int printStatus;
		public string Descriptions;
    }

    public class SlicerProfileFormat
    {
		public int rid;
		public string uid;
		public string created_by;
		public string creationDT;
		public string modified_by;
		public string modifiedDT;
		public string data;
	}

	public class FixedFileInfo
	{
		public string uid;
		public string filepathname;
		public int DBIndex;
	}

	public class SlicedFileInfo : FixedFileInfo
	{ }
}
