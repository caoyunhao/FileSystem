using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_System.FileSystem.MyIterator {
    public interface Iterator {

        bool HasNext();

        object Next();

        object Remove();

    }
}
