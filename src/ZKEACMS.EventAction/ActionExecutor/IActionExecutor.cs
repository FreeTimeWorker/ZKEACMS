﻿/* http://www.zkea.net/ 
 * Copyright (c) ZKEASOFT. All rights reserved. 
 * http://www.zkea.net/licenses */

using Easy.RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZKEACMS.EventAction.ActionExecutor
{
    public interface IActionExecutor
    {
        ServiceResult Execute(object entity, Dictionary<string, string> args);
    }
}
