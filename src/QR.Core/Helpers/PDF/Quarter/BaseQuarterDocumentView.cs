using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QR.Core.Helpers.PDF;

public abstract class BaseQuarterDocumentView : BaseDocumentView
{
    public override void OnInitialized()
    {
        this.GridColumns = 2;
        this.GridRows = 2;
    }
}
