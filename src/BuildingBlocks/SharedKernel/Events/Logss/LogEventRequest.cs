using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Events.Logs;

public sealed record LogEventRequest(
string Message

) : IEvent;