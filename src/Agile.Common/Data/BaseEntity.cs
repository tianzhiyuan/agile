using System;

namespace Agile.Common.Data
{
    [Serializable]
    public abstract class BaseEntity
    {
        public int? Id { get; set; }
        public int? CreatorId { get; set; }
        public int? LastModifierId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

		private static bool IsTransient(BaseEntity obj)
		{
			return obj != null && Equals(obj.Id, default(int));
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as BaseEntity);
		}
		public virtual bool Equals(BaseEntity other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			if (!IsTransient(this) &&
				!IsTransient(other) &&
				Id == other.Id)
			{
				var otherType = other.GetType();
				var thisType = GetType();
				return thisType.IsAssignableFrom(otherType) ||
						otherType.IsAssignableFrom(thisType);
			}

			return false;
		}

		public override int GetHashCode()
		{
			return Equals(Id, default(int)) ? base.GetHashCode() : Id.GetHashCode();
		}

		public static bool operator ==(BaseEntity x, BaseEntity y)
		{
			return Equals(x, y);
		}

		public static bool operator !=(BaseEntity x, BaseEntity y)
		{
			return !(x == y);
		}
    }
    
}
