using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Elders.Cronus.DomainModeling;
using Machine.Specifications;

namespace Elders.Proteus.Tests.custom_cases
{


    [Subject(typeof(Serializer))]
    public class Whem_EntityId
    {

        static Guid WorkGuid = new Guid("eeceebfd-9f2f-4c0a-8242-6d7df1470d13");
        static Guid JobId = new Guid("debd3009-120b-45d9-84ba-e2216f597de0");
        Establish context = () =>
        {
            ser = new WorkId(WorkGuid, new JobId(JobId));
            serializer = new Serializer(new GuidTypeIdentifier(typeof(WorkId).Assembly, typeof(Whem_EntityId).Assembly));
            serializer2 = new Serializer(new GuidTypeIdentifier(typeof(WorkId).Assembly, typeof(Whem_EntityId).Assembly));
            serStream = new MemoryStream();
            serializer.SerializeWithHeaders(serStream, ser);
            serStream.Position = 0;
        };
        Because of_deserialization = () => { deser = (WorkId)serializer.DeserializeWithHeaders(serStream); };

        It should_not_be_null = () => deser.ShouldNotBeNull();


        static Exception ex;
        static WorkId ser;
        static WorkId deser;
        static Stream serStream;
        static Serializer serializer;
        static Serializer serializer2;
    }

    [DataContract(Name = "33908fe3-89d8-458f-975f-4a1e273c2134")]
    public class WorkId : EntityGuidId<JobId>
    {
        protected WorkId() { }
        public WorkId(Guid id, JobId jobId) : base(id, jobId, "Work") { }
    }


    [DataContract(Name = "470532ba-fe38-4dd4-b825-26c81d75a64e")]
    public class JobId : GuidId
    {
        protected JobId() { }
        public JobId(Guid id) : base(id, "Job") { }
    }
}
