using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Petroineosfeedservice
{
    public sealed class CsvExtractorMap : CsvHelper.Configuration.ClassMap<CsvVM>
    {
        public CsvExtractorMap()
        {
            Map(m => m.LocalTime).Index(0).Name("Local Time");
            Map(m => m.Volume).Index(1).Name("Volume");
        }
    }
}
