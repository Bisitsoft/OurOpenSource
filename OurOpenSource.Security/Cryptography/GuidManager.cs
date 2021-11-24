using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OurOpenSource.Security.Cryptography
{
	/// <summary>
	/// GUID管理器。
	/// GUID manager.
	/// </summary>
	public class GuidManager
	{
		private SortedSet<Guid> guids = new SortedSet<Guid>();
		/// <summary>
		/// GUID管理器中所有的GUID。
		/// All GUIDs in GUID Manager.
		/// </summary>
		public Guid[] Guids { get { return guids.ToArray(); } }

		/// <summary>
		/// 获取一个真正唯一的Guid。
		/// Get a real unique GUID.
		/// </summary>
		/// <returns>
		/// 一个真正唯一的Guid。
		/// A real unique GUID.
		/// </returns>
		public Guid GetOne()
		{
			Guid r;
			do
			{
				r = Guid.NewGuid();
			} while (guids.Contains(r));
			return r;
		}

		/// <summary>
		/// 从GUID管理器中删除一个GUID。
		/// Delete a GUID from GUID manager.
		/// </summary>
		/// <param name="guid">
		/// 你想要删除的GUID。
		/// The GUID which you want to delete.
		/// </param>
		public void Remove(Guid guid)
		{
			guids.Remove(guid);
		}

		/// <summary>
		/// 判断GUID管理器中是否包含指定的GUID。
		/// Whether the specified GUID is included in the guid manager.
		/// </summary>
		/// <param name="guid">
		/// 一个你想要的判断是否存在与GUID管理器中的GUID。
		/// A guid which you want to make sure is it is contained in GUID manager.
		/// </param>
		/// <returns>
		/// GUID管理器中是否包含指定的GUID。
		/// Whether the specified GUID is included in the guid manager.
		/// </returns>
		public bool Contains(Guid guid)
		{
			return guids.Contains(guid);
		}

		/// <summary>
		/// 获取当前GUID管理器中GUID的数量。
		/// Get the count of GUID in this GUID manager.
		/// </summary>
		public int Count { get { return guids.Count; } }
	}
}
