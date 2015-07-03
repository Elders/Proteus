using System;
using System.Runtime.Serialization;
using System.Text;

namespace Elders.Cronus.DomainModeling
{
    [DataContract(Name = "6b2e2276-5b1a-4236-8644-ed64b3a98098")]
    public class EntityGuidId<TAggregateRootId> : EntityId<TAggregateRootId> where TAggregateRootId : IAggregateRootId
    {
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        protected EntityGuidId() { }

        public EntityGuidId(Guid idBase, TAggregateRootId rootId, string entityName)
            : base(rootId, entityName)
        {
            if (idBase == default(Guid)) throw new ArgumentException("Default guid value is not allowed.", "idBase");
            Id = idBase;
            var entityBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes(EntityName + "@"), Id.ToByteArray());
            var rootBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes("@@"), AggregateRootId.RawId);
            base.RawId = ByteArrayHelper.Combine(entityBytes, rootBytes);
        }

        public EntityGuidId(EntityGuidId<TAggregateRootId> idBase, string entityId)
            : base(idBase.RootId, entityId)
        {
            if (!IsValid(idBase)) throw new ArgumentException("Default guid value is not allowed.", "idBase");
            Id = idBase.Id;
            var entityBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes(EntityName + "@"), Id.ToByteArray());
            var rootBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes("@@"), AggregateRootId.RawId);
            base.RawId = ByteArrayHelper.Combine(entityBytes, rootBytes);
        }

        public static bool IsValid(EntityGuidId<TAggregateRootId> entityId)
        {
            return (!ReferenceEquals(null, entityId)) && entityId.Id != default(Guid);
        }

        public override string ToString()
        {
            return EntityName + "@" + Id.ToString() + "@@" + AggregateRootId.ToString() + "||" + base.ToString();
        }
    }

    [DataContract(Name = "5e1e63da-8709-40d0-8bd5-0864f26c1ca6")]
    public class EntityGuidId : EntityId
    {
        [DataMember(Order = 1)]
        public Guid Id { get; private set; }

        protected EntityGuidId() { }

        public EntityGuidId(Guid idBase, IAggregateRootId rootId, string entityName)
            : base(rootId, entityName)
        {
            if (idBase == default(Guid)) throw new ArgumentException("Default guid value is not allowed.", "idBase");
            Id = idBase;
            var entityBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes(EntityName + "@"), Id.ToByteArray());
            var rootBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes("@@"), AggregateRootId.RawId);
            base.RawId = ByteArrayHelper.Combine(entityBytes, rootBytes);
        }

        public EntityGuidId(EntityGuidId idBase, string entityId)
            : base(idBase.AggregateRootId, entityId)
        {
            if (!IsValid(idBase)) throw new ArgumentException("Default guid value is not allowed.", "idBase");
            Id = idBase.Id;
            var entityBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes(EntityName + "@"), Id.ToByteArray());
            var rootBytes = ByteArrayHelper.Combine(UTF8Encoding.UTF8.GetBytes("@@"), AggregateRootId.RawId);
            base.RawId = ByteArrayHelper.Combine(entityBytes, rootBytes);
        }

        public static bool IsValid(EntityGuidId entityId)
        {
            return (!ReferenceEquals(null, entityId)) && entityId.Id != default(Guid);
        }

        public override string ToString()
        {
            return EntityName + "@" + Id.ToString() + "@@" + AggregateRootId.ToString() + "||" + base.ToString();
        }
    }
}