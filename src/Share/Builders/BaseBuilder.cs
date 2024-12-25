using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Share.Builders;
public class BaseBuilder
{
    public WebInfo WebInfo { get; init; }

    public BaseBuilder(WebInfo webInfo)
    {
        WebInfo = webInfo;
    }
}
