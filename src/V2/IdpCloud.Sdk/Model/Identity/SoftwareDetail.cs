using System;

namespace IdpCloud.Sdk.Model.Identity
{
    public class SoftwareDetail
    {
        public Guid SoftwareDetailId { get; set; }

        public string StagingPath { get; set; }

        public string DevelopPath { get; set; }

        public string ProductionPath { get; set; }

        public ExecutionType ExecutionType { get; set; }

        public ProgrammingLanguage ProgrammingLanguage { get; set; }

        public LifeState LifeState { get; set; }

        public byte TierCount { get; set; }

        public byte LayerCount { get; set; }

        public Guid SoftwareId { get; set; }

        public Software Software { get; set; }
    }

    public enum ExecutionType : byte
    {
        WebApplication = 0,
        ClickOnce = 1,
        ShellExecute = 2,
        WebService = 3,
        WindowsService = 4,
    }

    public enum ProgrammingLanguage : byte
    {
        CSharp = 0,
        C = 1,
        CPP = 2,
        Python = 3,
        Go = 4,
        JavaScript = 5,
        HTML = 6,
        Perl = 7,
        Java = 8,
        TSQL = 9
    }

    public enum LifeState : byte
    {
        Planning = 0,
        Requirements = 1,
        Design = 2,
        DevelopAndBuild = 3,
        Testing = 4,
        Deployment = 5,
        Maintenace = 6,
        ReleasedAndLive = 7
    }
}
