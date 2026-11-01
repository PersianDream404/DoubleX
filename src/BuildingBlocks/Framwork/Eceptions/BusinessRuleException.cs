using System;
using System.Collections.Generic;
using System.Text;

namespace Framwork.Eceptions;

public sealed class BusinessRuleException(string message)
    : Exception(message);