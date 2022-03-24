using System;

namespace IdpCloud.Sdk.Model.Identity.Request.SoftwareDetail
{
    public class ModifyRequest
    {
        public Guid SoftwareId { get; set; }

        public string StagingPath { get; set; }

        public string DevelopPath { get; set; }

        public string ProductionPath { get; set; }

        public ExecutionType ExecutionType { get; set; }

        public ProgrammingLanguage ProgrammingLanguage { get; set; }

        public LifeState LifeState{ get; set; }

        public byte TierCount { get; set; }

        public byte LayerCount { get; set; }
    }
}
