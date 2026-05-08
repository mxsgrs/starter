using FluentResults;
using Network.Domain.Aggregates.UserAggregate;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Aggregates.UserAggregate;

public class SecurityNoteBuilder : IEntityModelBuilder<SecurityNote>
{
    private Guid _userId = Guid.NewGuid();
    private string _reason = "Suspicious activity";

    public SecurityNoteBuilder WithUserId(Guid v) { _userId = v; return this; }
    public SecurityNoteBuilder WithReason(string v) { _reason = v; return this; }

    public Result<SecurityNote> BuildResult() => SecurityNote.Create(_userId, _reason);

    public SecurityNote Build() => SecurityNote.Create(_userId, _reason).Value;
}
