using System;
using System.Collections.Generic;
using System.Linq;

namespace HashCode
{
	class Picture
	{



		public int Id { get; set; }

		public Orientation Orientation { get; set; }

		public HashSet<string> Tags { get; set; }

		private List<string> _tagList;

		public List<string> TagList
		{
			get
			{
				if (_tagList is null)
				{
					_tagList = Tags.ToList();
				}
				return _tagList;
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is Picture p)
			{
				return p.Id == Id;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Id;
		}

		private Dictionary<Picture, int> _score = new Dictionary<Picture, int>();

		public void Add(Picture pic, Func<Picture, Picture, int> score)
		{
			if (NextPicture.Add(pic))
			{
				_score[pic] = score(pic, this);
			}
		}

		public HashSet<Picture> NextPicture { get; } = new HashSet<Picture>();
	}
}