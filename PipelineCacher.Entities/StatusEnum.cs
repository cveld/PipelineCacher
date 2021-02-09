namespace PipelineCacher.Entities
{
    public enum StatusEnum
    {
        Undefined,
        NotRun,
        Validated,
        RunSuccessfully,
        RunSuccessfullyWithIssues,
        RunFailed,
        RunAbandoned,
        RunCanceled
    }
}