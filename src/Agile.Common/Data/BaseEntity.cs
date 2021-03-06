﻿using System;

namespace Agile.Common.Data
{
	/// <summary>
	/// represents base entity
	/// </summary>
    [Serializable]
    public abstract class BaseEntity
    {
		/// <summary>
		/// key field of a entity
		/// </summary>
        public int? Id { get; set; }
		/// <summary>
		/// entity create timestamp
		/// </summary>
        public DateTime? CreatedAt { get; set; }
		/// <summary>
		/// entity last modify timestamp
		/// </summary>
        public DateTime? LastModifiedAt { get; set; }

		private static bool IsTransient(BaseEntity obj)
		{
		    return obj != null && obj.Id == null;
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
        
    }
    
}
